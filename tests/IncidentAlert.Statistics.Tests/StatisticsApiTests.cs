using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace IncidentAlert.Statistics.Tests;

public class StatisticsApiTests(StatisticsApiFactory factory) : IClassFixture<StatisticsApiFactory>
{
    private readonly HttpClient _client = SignedIn(factory.CreateClient());

    private static HttpClient SignedIn(HttpClient client)
    {
        client.DefaultRequestHeaders.Authorization = new("Bearer", TestJwt.Token("MODERATOR"));
        return client;
    }

    private static object Incident(string location, params string[] categories) => new
    {
        id = 1,
        title = "t",
        text = "t",
        dateTime = DateTime.UtcNow,
        location = new { id = 1, name = location, latitude = 0.0, longitude = 0.0 },
        categories,
        images = Array.Empty<object>(),
    };

    [Fact]
    public async Task NumberOfIncidentsPerCategory_AggregatesTheUpstreamIncidents()
    {
        factory.Upstream.Status = HttpStatusCode.OK;
        factory.Upstream.Body = new[] { Incident("Tuzla", "Fire"), Incident("Tuzla", "Fire", "Flood") };

        var result = await _client.GetFromJsonAsync<Dictionary<string, List<JsonElement>>>("/Statistics/NumberOfIncidentsPerCategory");

        var total = result!["Total"].ToDictionary(e => e.GetProperty("categoryName").GetString()!, e => e.GetProperty("incidentCount").GetInt32());
        Assert.Equal(2, total["Fire"]);
        Assert.Equal(1, total["Flood"]);
        Assert.Contains("/Incident/getAll", factory.Upstream.Requests);
    }

    [Fact]
    public async Task LocationWithMostIncidents_ReturnsTheBusiestLocation()
    {
        factory.Upstream.Status = HttpStatusCode.OK;
        factory.Upstream.Body = new[] { Incident("Tuzla", "Fire"), Incident("Mostar", "Fire"), Incident("Mostar", "Crime") };

        var result = await _client.GetFromJsonAsync<Dictionary<string, JsonElement>>("/Statistics/LocationWithMostIncidents");

        Assert.Equal("Mostar", result!["Total"].GetProperty("locationName").GetString());
        Assert.Equal(2, result["Total"].GetProperty("incidentCount").GetInt32());
    }

    [Fact]
    public async Task LocationHeat_AndIncidentsPerDay_AreServed()
    {
        factory.Upstream.Status = HttpStatusCode.OK;
        factory.Upstream.Body = new[] { Incident("Tuzla", "Fire"), Incident("Tuzla", "Flood") };

        var heat = await _client.GetFromJsonAsync<List<JsonElement>>("/Statistics/LocationHeat");
        var perDay = await _client.GetFromJsonAsync<List<JsonElement>>("/Statistics/IncidentsPerDay?days=7");
        var clamped = await _client.GetFromJsonAsync<List<JsonElement>>("/Statistics/IncidentsPerDay?days=5000");

        Assert.Equal(2, Assert.Single(heat!).GetProperty("incidentCount").GetInt32());
        Assert.Equal(7, perDay!.Count);
        Assert.Equal(2, perDay[^1].GetProperty("total").GetInt32());
        Assert.Equal(365, clamped!.Count);
    }

    [Fact]
    public async Task UpstreamFailure_IsPassedThrough()
    {
        factory.Upstream.Status = HttpStatusCode.ServiceUnavailable;

        var response = await _client.GetAsync("/Statistics/LocationWithMostIncidentsPerCategory");

        Assert.Equal(HttpStatusCode.ServiceUnavailable, response.StatusCode);
    }

    [Fact]
    public async Task Statistics_RequireASignedInModerator()
    {
        var anonymous = factory.CreateClient();
        var citizen = factory.CreateClient();
        citizen.DefaultRequestHeaders.Authorization = new("Bearer", TestJwt.Token("NOTITLE"));
        var forged = factory.CreateClient();
        forged.DefaultRequestHeaders.Authorization = new("Bearer", TestJwt.Token("MODERATOR", secret: "someone-elses-key-that-is-long-enough-000"));

        Assert.Equal(HttpStatusCode.Unauthorized, (await anonymous.GetAsync("/Statistics/NumberOfIncidentsPerCategory")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await citizen.GetAsync("/Statistics/NumberOfIncidentsPerCategory")).StatusCode);
        Assert.Equal(HttpStatusCode.Unauthorized, (await forged.GetAsync("/Statistics/NumberOfIncidentsPerCategory")).StatusCode);
    }

    [Fact]
    public async Task Health_IsPublic()
    {
        var response = await factory.CreateClient().GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
