using System.Net;
using System.Net.Http.Json;
using Contracts.Category;
using Contracts.Incident;
using IncidentAlert_Management.Models;
using MassTransit.Testing;
using Microsoft.EntityFrameworkCore;

namespace IncidentAlert.Management.Tests;

public class ModerationTests(ManagementApiFactory factory) : IClassFixture<ManagementApiFactory>
{
    private int SeedPendingIncident(string title, params string[] categories)
    {
        var incident = new Incident
        {
            Title = title,
            Text = "Reported by a citizen.",
            DateTime = DateTime.UtcNow,
            Status = StatusEnum.PENDING,
            Location = new Location { Name = $"Place {Guid.NewGuid():N}", Latitude = 44.77, Longitude = 17.19 },
        };
        factory.Seed(db =>
        {
            foreach (var name in categories)
            {
                var category = db.Categories.FirstOrDefault(c => c.Name == name) ?? new Category { Name = name };
                incident.IncidentCategories.Add(new IncidentCategory { Category = category });
            }
            db.Incidents.Add(incident);
        });
        return incident.Id;
    }

    [Fact]
    public async Task Approve_PublishesTheIncidentAndRemovesItFromTheQueue()
    {
        var harness = factory.Services.GetTestHarness();
        var moderator = await factory.CreateClientAs(RoleEnum.MODERATOR);
        var id = SeedPendingIncident("Gas leak", "Fire", "Other");

        var response = await moderator.PutAsync($"/Incident/Approve/{id}", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(await harness.Published.Any<IncidentApprovedEvent>(m =>
            m.Context.Message.Title == "Gas leak" &&
            m.Context.Message.Categories.Order().SequenceEqual(new[] { "Fire", "Other" })));
        Assert.False(factory.Query(db => db.Incidents.Any(i => i.Id == id)));
    }

    [Fact]
    public async Task Approve_ReturnsNotFoundForAnUnknownIncident()
    {
        var moderator = await factory.CreateClientAs(RoleEnum.MODERATOR);

        var response = await moderator.PutAsync("/Incident/Approve/987654", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ChangeStatus_UpdatesTheIncident()
    {
        var moderator = await factory.CreateClientAs(RoleEnum.MODERATOR);
        var id = SeedPendingIncident("Broken light", "Traffic");

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/Incident/ChangeStatus/{id}")
        {
            Content = JsonContent.Create("REJECTED"),
        };
        var response = await moderator.SendAsync(request);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(StatusEnum.REJECTED, factory.Query(db => db.Incidents.AsNoTracking().Single(i => i.Id == id).Status));
    }

    [Fact]
    public async Task ChangeStatus_RejectsAnUnknownStatus()
    {
        var moderator = await factory.CreateClientAs(RoleEnum.MODERATOR);
        var id = SeedPendingIncident("Noise", "Other");

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/Incident/ChangeStatus/{id}")
        {
            Content = JsonContent.Create("ARCHIVED"),
        };
        var response = await moderator.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task ChangeStatus_CannotSkipApproval()
    {
        var moderator = await factory.CreateClientAs(RoleEnum.MODERATOR);
        var id = SeedPendingIncident("Shortcut", "Other");

        var request = new HttpRequestMessage(HttpMethod.Delete, $"/Incident/ChangeStatus/{id}")
        {
            Content = JsonContent.Create("ACCEPTED"),
        };
        var response = await moderator.SendAsync(request);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(StatusEnum.PENDING, factory.Query(db => db.Incidents.AsNoTracking().Single(i => i.Id == id).Status));
    }

    [Fact]
    public async Task Delete_RemovesTheIncident()
    {
        var moderator = await factory.CreateClientAs(RoleEnum.MODERATOR);
        var id = SeedPendingIncident("Spam report", "Other");

        var response = await moderator.DeleteAsync($"/Incident/{id}");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.False(factory.Query(db => db.Incidents.Any(i => i.Id == id)));
    }

    [Fact]
    public async Task CreateCategory_IsSharedWithTheOtherServices()
    {
        var harness = factory.Services.GetTestHarness();
        var moderator = await factory.CreateClientAs(RoleEnum.MODERATOR);
        var name = $"Storm {Guid.NewGuid():N}";

        var response = await moderator.PostAsJsonAsync("/Category", name);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(await harness.Published.Any<CategoryCreatedEvent>(m => m.Context.Message.Name == name));
    }
}
