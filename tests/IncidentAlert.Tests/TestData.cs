using IncidentAlert.Data;
using IncidentAlert.Models;
using Microsoft.EntityFrameworkCore;

namespace IncidentAlert.Tests;

internal static class TestData
{
    /// <summary>A fresh, isolated in-memory database. Contexts created with the same name share data.</summary>
    public static DbContextOptions<DataContext> NewDatabase() =>
        new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase($"incident-alert-{Guid.NewGuid()}")
            .Options;

    public static void Seed(DbContextOptions<DataContext> options, Action<DataContext> seed)
    {
        using var context = new DataContext(options);
        seed(context);
        context.SaveChanges();
    }

    public static Category[] Categories(params string[] names) =>
        names.Select(name => new Category { Name = name }).ToArray();

    public static Location BanjaLuka() => new() { Name = "Banja Luka", Latitude = 44.7722, Longitude = 17.1910 };
}
