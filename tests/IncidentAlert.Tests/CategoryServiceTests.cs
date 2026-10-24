using IncidentAlert.Data;
using IncidentAlert.Exceptions;
using IncidentAlert.Models.Dto;
using IncidentAlert.Repositories.Implementation;
using IncidentAlert.Services.Implementation;

namespace IncidentAlert.Tests;

public class CategoryServiceTests
{
    [Fact]
    public async Task Add_RejectsADuplicateName()
    {
        var options = TestData.NewDatabase();
        TestData.Seed(options, db => db.Categories.AddRange(TestData.Categories("Fire")));
        using var context = new DataContext(options);
        var service = new CategoryService(new CategoryRepository(context));

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.Add(new CategoryDto { Name = "Fire" }));
    }

    [Fact]
    public async Task Delete_ThrowsForAMissingCategory()
    {
        using var context = new DataContext(TestData.NewDatabase());
        var service = new CategoryService(new CategoryRepository(context));

        await Assert.ThrowsAsync<EntityDoesNotExistException>(() => service.Delete(7));
    }

    [Fact]
    public async Task GetAll_ReturnsEveryCategory()
    {
        var options = TestData.NewDatabase();
        TestData.Seed(options, db => db.Categories.AddRange(TestData.Categories("Fire", "Flood")));
        using var context = new DataContext(options);
        var service = new CategoryService(new CategoryRepository(context));

        var categories = await service.GetAll();

        Assert.Equal(["Fire", "Flood"], categories.Select(c => c.Name).Order());
    }
}
