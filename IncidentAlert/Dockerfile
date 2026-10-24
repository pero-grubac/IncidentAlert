# syntax=docker/dockerfile:1

# Runtime image. Visual Studio's fast-mode debugging targets this stage directly.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=5000
EXPOSE 5000

# Restore first (cached until a .csproj changes), then publish.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Contracts/Contracts.csproj", "Contracts/"]
COPY ["IncidentAlert/IncidentAlert.csproj", "IncidentAlert/"]
RUN dotnet restore "IncidentAlert/IncidentAlert.csproj"
COPY Contracts/ Contracts/
COPY IncidentAlert/ IncidentAlert/
RUN dotnet publish "IncidentAlert/IncidentAlert.csproj" -c $BUILD_CONFIGURATION -o /app/publish --no-restore /p:UseAppHost=false

FROM base AS final
COPY --from=build /app/publish .
# Uploaded incident images live here; the compose file mounts a volume on it.
RUN mkdir -p wwwroot/uploads && chown -R app:app wwwroot
USER app
ENTRYPOINT ["dotnet", "IncidentAlert.dll"]
