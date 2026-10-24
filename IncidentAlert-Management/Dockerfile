# syntax=docker/dockerfile:1

# Runtime image. Visual Studio's fast-mode debugging targets this stage directly.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=5002
EXPOSE 5002

# Restore first (cached until a .csproj changes), then publish.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Contracts/Contracts.csproj", "Contracts/"]
COPY ["IncidentAlert-Management/IncidentAlert-Management.csproj", "IncidentAlert-Management/"]
RUN dotnet restore "IncidentAlert-Management/IncidentAlert-Management.csproj"
COPY Contracts/ Contracts/
COPY IncidentAlert-Management/ IncidentAlert-Management/
RUN dotnet publish "IncidentAlert-Management/IncidentAlert-Management.csproj" -c $BUILD_CONFIGURATION -o /app/publish --no-restore /p:UseAppHost=false

FROM base AS final
COPY --from=build /app/publish .
# Uploaded incident images live here; the compose file mounts a volume on it.
RUN mkdir -p wwwroot/uploads && chown -R app:app wwwroot
USER app
ENTRYPOINT ["dotnet", "IncidentAlert-Management.dll"]
