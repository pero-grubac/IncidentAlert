using System.Net;
using System.Net.Http.Json;
using Contracts.Incident;
using IncidentAlert.Data;
using IncidentAlert.Models;
using IncidentAlert.Models.Dto;
using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IncidentAlert.Tests;

/// <summary>Hosts the real API with an in-memory database and an in-memory message bus.</summary>
public sealed class IncidentAlertApiFactory : WebApplicationFactory<Program>
{
    private readonly string _database = $"incident-alert-api-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Anything but Development, so startup does not run PostgreSQL migrations.
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            // EF Core 9+ also keeps the provider configuration (Npgsql) in a separate registration.
            services.RemoveAll<DbContextOptions<DataContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<DataContext>>();
            services.AddDbContext<DataContext>(o => o.UseInMemoryDatabase(_database));
            services.AddMassTransitTestHarness();
        });
    }

    public void Seed(Action<DataContext> seed)
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        seed(context);
        context.SaveChanges();
    }
}

public class ApiTests : IClassFixture<IncidentAlertApiFactory>
{
    private readonly IncidentAlertApiFactory _factory;
    private readonly HttpClient _client;

    public ApiTests(IncidentAlertApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_ReportsHealthy()
    {
        var response = await _client.GetAsync("/health");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Healthy", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task GetById_ReturnsNotFoundWithAJsonError()
    {
        var response = await _client.GetAsync("/Incident/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains("does not exists", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task GetAllByLocationName_ReturnsApprovedIncidentsWithCategories()
    {
        _factory.Seed(db =>
        {
            var fire = new Category { Name = $"Fire-{Guid.NewGuid():N}" };
            db.Incidents.Add(new Incident
            {
                Title = "Fire in Mostar",
                Text = "Smoke over the old bridge.",
                DateTime = DateTime.UtcNow,
                Location = new Location { Name = "Mostar", Latitude = 43.34, Longitude = 17.80 },
                IncidentCategories = [new IncidentCategory { Category = fire }],
            });
        });

        var incidents = await _client.GetFromJsonAsync<List<ResponseIncidentDto>>("/Incident/GetAllByLocationName/mostar");

        var incident = Assert.Single(incidents!);
        Assert.Equal("Fire in Mostar", incident.Title);
        Assert.StartsWith("Fire-", Assert.Single(incident.Categories).Name);
    }

    [Fact]
    public async Task Post_PublishesTheReportToTheModerationQueue()
    {
        var harness = _factory.Services.GetTestHarness();
        using var form = new MultipartFormDataContent
        {
            { new StringContent("Fallen tree"), "title" },
            { new StringContent("A tree blocks the road."), "text" },
            { new StringContent("2026-05-01T10:00:00Z"), "dateTime" },
            { new StringContent("Tuzla"), "location.name" },
            { new StringContent("44.53"), "location.latitude" },
            { new StringContent("18.67"), "location.longitude" },
            { new StringContent("Traffic"), "categories" },
        };

        var response = await _client.PostAsync("/Incident", form);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(await harness.Published.Any<IncidentCreateEvent>(m => m.Context.Message.Title == "Fallen tree"));
    }
}
