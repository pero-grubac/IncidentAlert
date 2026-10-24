using IncidentAlert_Management.Data;
using IncidentAlert_Management.Exceptions;
using IncidentAlert_Management.Models;
using IncidentAlert_Management.Models.Dto;
using IncidentAlert_Management.Repositories.Implementation;
using IncidentAlert_Management.Services;
using IncidentAlert_Management.Services.Implementation;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace IncidentAlert.Management.Tests;

/// <summary>The path a citizen's report takes when it arrives from the IncidentAlert service.</summary>
public class IncidentServiceTests
{
    private readonly DbContextOptions<DataContext> _options = new DbContextOptionsBuilder<DataContext>()
        .UseInMemoryDatabase($"management-service-{Guid.NewGuid()}")
        .Options;

    private readonly IImageService _images = Substitute.For<IImageService>();

    private IncidentService CreateService(DataContext context) => new(
        new IncidentRepository(context),
        new LocationRepository(context),
        new CategoryRepository(context),
        new IncidentCategoryRepository(context),
        _images,
        Substitute.For<IPublishEndpoint>());

    private void SeedCategories(params string[] names)
    {
        using var context = new DataContext(_options);
        context.Categories.AddRange(names.Select(n => new Category { Name = n }));
        context.SaveChanges();
    }

    private static IncidentDto Report(params string[] categories) => new()
    {
        Title = "Flooded road",
        Text = "Water over the main road.",
        DateTime = DateTime.UtcNow,
        Location = new LocationDto { Name = "Sarajevo", Latitude = 43.85, Longitude = 18.41 },
        Categories = categories,
    };

    [Fact]
    public async Task Add_QueuesTheReportAsPendingWithAllCategories()
    {
        SeedCategories("Flood", "Traffic");

        using (var context = new DataContext(_options))
            await CreateService(context).Add(Report("Flood", "Traffic"));

        using var verify = new DataContext(_options);
        var incident = await verify.Incidents
            .Include(i => i.IncidentCategories).ThenInclude(ic => ic.Category)
            .SingleAsync();
        Assert.Equal(StatusEnum.PENDING, incident.Status);
        Assert.Equal(["Flood", "Traffic"], incident.IncidentCategories.Select(ic => ic.Category.Name).Order());
    }

    [Fact]
    public async Task Add_StoresEveryImage()
    {
        SeedCategories("Flood");
        var report = Report("Flood");
        report.ImagesData = [new ImageData { FileName = "a.jpg", Content = [1] }, new ImageData { FileName = "b.jpg", Content = [2] }];

        using var context = new DataContext(_options);
        await CreateService(context).Add(report);

        await _images.Received(2).Add(Arg.Any<ImageData>(), Arg.Any<int>());
    }

    [Fact]
    public async Task Add_RejectsAReportWithAnUnknownCategory()
    {
        SeedCategories("Flood");
        using var context = new DataContext(_options);

        await Assert.ThrowsAsync<EntityCanNotBeCreatedException>(() => CreateService(context).Add(Report("Flood", "Volcano")));
        Assert.Empty(context.Incidents);
    }

    [Fact]
    public async Task Add_RejectsAReportWithoutCategories()
    {
        using var context = new DataContext(_options);

        await Assert.ThrowsAsync<EntityCanNotBeCreatedException>(() => CreateService(context).Add(Report()));
    }
}
