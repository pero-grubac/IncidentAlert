# syntax=docker/dockerfile:1

# Runtime image. Visual Studio's fast-mode debugging targets this stage directly.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=5004
EXPOSE 5004

# Restore first (cached until a .csproj changes), then publish.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Contracts/Contracts.csproj", "Contracts/"]
COPY ["IncidentAlert-ML/IncidentAlert-ML.csproj", "IncidentAlert-ML/"]
RUN dotnet restore "IncidentAlert-ML/IncidentAlert-ML.csproj"
COPY Contracts/ Contracts/
COPY IncidentAlert-ML/ IncidentAlert-ML/
RUN dotnet publish "IncidentAlert-ML/IncidentAlert-ML.csproj" -c $BUILD_CONFIGURATION -o /app/publish --no-restore /p:UseAppHost=false

FROM base AS final
COPY --from=build /app/publish .
USER app
ENTRYPOINT ["dotnet", "IncidentAlert-ML.dll"]
