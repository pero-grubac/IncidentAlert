#!/usr/bin/env bash
# End-to-end check against a running stack (docker compose up -d --build):
# a citizen reports an incident, a moderator approves it, and it shows up on the public API.
set -euo pipefail

API=${API:-http://localhost:5000}
MANAGEMENT=${MANAGEMENT:-http://localhost:5002}
ML=${ML:-http://localhost:5004}
STATISTICS=${STATISTICS:-http://localhost:5006}

wait_for() {
  local url=$1
  for _ in $(seq 1 60); do
    if curl -fsS "$url" > /dev/null 2>&1; then return 0; fi
    sleep 2
  done
  echo "✗ $url did not become healthy" >&2
  return 1
}

echo "→ Waiting for the services"
for base in "$API" "$MANAGEMENT" "$ML" "$STATISTICS"; do
  wait_for "$base/health"
  echo "  ✓ $base"
done

title="Smoke test $(date +%s)"
location="Smoke Town $RANDOM"

echo "→ Reporting \"$title\""
curl -fsS -o /dev/null "$API/Incident" \
  -F "title=$title" -F "text=Reported by the smoke test" \
  -F "dateTime=$(date -u +%Y-%m-%dT%H:%M:%SZ)" \
  -F "location.name=$location" -F "location.latitude=44.77" -F "location.longitude=17.19" \
  -F "categories=Fire" -F "categories=Traffic"

echo "→ Signing in as the demo moderator"
token=$(curl -fsS "$MANAGEMENT/Login" -H 'Content-Type: application/json' \
  -d '{"username":"moderator","password":"Admin123!"}')

echo "→ Waiting for the report to reach the moderation queue"
id=""
for _ in $(seq 1 30); do
  id=$(curl -fsS "$MANAGEMENT/Incident/getAll" -H "Authorization: Bearer $token" \
    | jq -r --arg t "$title" '.[] | select(.title == $t) | .id')
  [ -n "$id" ] && break
  sleep 1
done
[ -n "$id" ] || { echo "✗ the report never reached moderation" >&2; exit 1; }
echo "  ✓ pending as #$id"

echo "→ Approving it"
curl -fsS -o /dev/null -X PUT "$MANAGEMENT/Incident/Approve/$id" -H "Authorization: Bearer $token"

echo "→ Waiting for it on the public API"
for _ in $(seq 1 30); do
  found=$(curl -fsS -G "$API/Incident/GetAllByLocationName/$(jq -rn --arg l "$location" '$l|@uri')" \
    | jq -r --arg t "$title" '[.[] | select(.title == $t) | .categories | length] | first // 0')
  if [ "$found" = "2" ]; then
    echo "  ✓ published with both categories"
    # The analysis services only answer signed-in moderators.
    status=$(curl -s -o /dev/null -w "%{http_code}" "$STATISTICS/Statistics/NumberOfIncidentsPerCategory")
    [ "$status" = "401" ] || { echo "✗ statistics answered without a token ($status)" >&2; exit 1; }
    curl -fsS -H "Authorization: Bearer $token" "$STATISTICS/Statistics/NumberOfIncidentsPerCategory" > /dev/null && echo "  ✓ statistics (and 401 without a token)"
    curl -fsS -H "Authorization: Bearer $token" "$ML/Incident/grouped-incidents" > /dev/null && echo "  ✓ ML grouping"
    echo "Smoke test passed"
    exit 0
  fi
  sleep 1
done

echo "✗ the approved incident never appeared on the public API" >&2
exit 1
