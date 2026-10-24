using Contracts.Incident;
using IncidentAlert.Data;
using IncidentAlert.Exceptions;
using IncidentAlert.Models;
using IncidentAlert.Models.Dto;
using IncidentAlert.Repositories.Implementation;
using IncidentAlert.Services;
using IncidentAlert.Services.Implementation;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace IncidentAlert.Tests;

public class IncidentServiceTests
{
    private readonly DbContextOptions<DataContext> _options = TestData.NewDatabase();
    private readonly IImageService _images = Substitute.For<IImageService>();
    private readonly IPublishEndpoint _publisher = Substitute.For<IPublishEndpoint>();

    private IncidentService CreateService(DataContext context) => new(
        new IncidentRepository(context),
        new LocationRepository(context),
        new CategoryRepository(context),
        new IncidentCategoryRepository(context),
        _images,
        _publisher);

    private static ApprovedIncident Approved(params string[] categories) => new()
    {
        Title = "Fire in city center",
        Text = "A fire broke out near the main square.",
        DateTime = new DateTime(2026, 5, 1, 10, 30, 0, DateTimeKind.Utc),
        Location = new LocationDto { Name = "Banja Luka", Latitude = 44.7722, Longitude = 17.1910 },
        Categories = categories,
    };

    [Fact]
    public async Task Approve_StoresIncidentWithNewLocationAndEveryCategory()
    {
        TestData.Seed(_options, db => db.Categories.AddRange(TestData.Categories("Fire", "Flood", "Crime")));

        using (var context = new DataContext(_options))
            await CreateService(context).Approve(Approved("Fire", "Flood"));

        using var verify = new DataContext(_options);
        var incident = await verify.Incidents
            .Include(i => i.Location)
            .Include(i => i.IncidentCategories).ThenInclude(ic => ic.Category)
            .SingleAsync();

        Assert.Equal("Fire in city center", incident.Title);
        Assert.Equal("Banja Luka", incident.Location!.Name);
        Assert.Equal(["Fire", "Flood"], incident.IncidentCategories.Select(ic => ic.Category.Name).Order());
    }

    [Fact]
    public async Task Approve_ReusesAnExistingLocationWithTheSameName()
    {
        TestData.Seed(_options, db =>
        {
            db.Categories.AddRange(TestData.Categories("Fire"));
            db.Locations.Add(TestData.BanjaLuka());
        });

        using (var context = new DataContext(_options))
            await CreateService(context).Approve(Approved("Fire"));

        using var verify = new DataContext(_options);
        Assert.Equal(1, await verify.Locations.CountAsync());
    }

    [Fact]
    public async Task Approve_SavesEveryAttachedImage()
    {
        TestData.Seed(_options, db => db.Categories.AddRange(TestData.Categories("Fire")));
        var incident = Approved("Fire");
        incident.ImagesData =
        [
            new ImageData { FileName = "one.jpg", Content = [1] },
            new ImageData { FileName = "two.jpg", Content = [2] },
        ];

        using var context = new DataContext(_options);
        await CreateService(context).Approve(incident);

        await _images.Received(2).Add(Arg.Any<ImageData>(), Arg.Any<int>());
    }

    [Fact]
    public async Task Approve_RejectsAnIncidentWithoutCategories()
    {
        using var context = new DataContext(_options);

        await Assert.ThrowsAsync<EntityCanNotBeCreatedException>(() => CreateService(context).Approve(Approved()));
        Assert.Empty(context.Incidents);
    }

    [Fact]
    public async Task Approve_RejectsAnUnknownCategory()
    {
        TestData.Seed(_options, db => db.Categories.AddRange(TestData.Categories("Fire")));
        using var context = new DataContext(_options);

        await Assert.ThrowsAsync<EntityCanNotBeCreatedException>(
            () => CreateService(context).Approve(Approved("Fire", "Earthquake")));
        Assert.Empty(context.Incidents);
    }

    [Fact]
    public async Task Add_PublishesTheReportForModerationWithImageBytes()
    {
        using var context = new DataContext(_options);
        var bytes = "fake-image"u8.ToArray();
        var file = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "images", "photo.jpg");
        var report = new IncidentDto
        {
            Title = "Flooded road",
            Text = "Water over the main road.",
            Categories = ["Flood"],
            Location = new LocationDto { Name = "Sarajevo", Latitude = 43.85, Longitude = 18.41 },
            Images = [file],
        };

        await CreateService(context).Add(report);

        await _publisher.Received(1).Publish(
            Arg.Is<IncidentCreateEvent>(e =>
                e.Title == "Flooded road" &&
                e.Location!.Name == "Sarajevo" &&
                e.ImagesData.Single().FileName == "photo.jpg" &&
                e.ImagesData.Single().Content.SequenceEqual(bytes)),
            Arg.Any<CancellationToken>());
        Assert.Empty(context.Incidents); // nothing is stored until a moderator approves it
    }

    [Fact]
    public async Task Update_PersistsTheChange()
    {
        var location = TestData.BanjaLuka();
        var incident = new Incident { Title = "Old title", Text = "Text", DateTime = DateTime.UtcNow, Location = location };
        TestData.Seed(_options, db => db.Incidents.Add(incident));

        using (var context = new DataContext(_options))
        {
            await CreateService(context).Update(incident.Id, new IncidentDto
            {
                Id = incident.Id,
                Title = "New title",
                Text = "Text",
                DateTime = incident.DateTime,
                Location = new LocationDto { Id = location.Id, Name = location.Name },
            });
        }

        using var verify = new DataContext(_options);
        Assert.Equal("New title", (await verify.Incidents.SingleAsync()).Title);
    }

    [Fact]
    public async Task Update_RejectsAMismatchedId()
    {
        using var context = new DataContext(_options);

        await Assert.ThrowsAsync<ArgumentException>(
            () => CreateService(context).Update(1, new IncidentDto { Id = 2 }));
    }

    [Fact]
    public async Task GetById_ThrowsForAMissingIncident()
    {
        using var context = new DataContext(_options);

        await Assert.ThrowsAsync<EntityDoesNotExistException>(() => CreateService(context).GetById(42));
    }

    [Fact]
    public async Task GetAllInDateRange_ReturnsOnlyIncidentsInsideTheRange()
    {
        var location = TestData.BanjaLuka();
        TestData.Seed(_options, db => db.Incidents.AddRange(
            new Incident { Title = "Before", Text = "-", DateTime = new DateTime(2026, 4, 30, 12, 0, 0, DateTimeKind.Utc), Location = location },
            new Incident { Title = "Inside", Text = "-", DateTime = new DateTime(2026, 5, 2, 12, 0, 0, DateTimeKind.Utc), Location = location },
            new Incident { Title = "After", Text = "-", DateTime = new DateTime(2026, 5, 6, 12, 0, 0, DateTimeKind.Utc), Location = location }));
        _images.GetImageNames(Arg.Any<int>()).Returns(new List<string>());

        using var context = new DataContext(_options);
        var result = await CreateService(context).GetAllInDateRange(
            new DateTime(2026, 5, 1, 0, 0, 0, DateTimeKind.Utc),
            new DateTime(2026, 5, 5, 0, 0, 0, DateTimeKind.Utc));

        Assert.Equal(["Inside"], result.Select(i => i.Title));
    }
}
