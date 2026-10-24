using IncidentAlert_Statistics.Models;
using IncidentAlert_Statistics.Service.Implementation;

namespace IncidentAlert.Statistics.Tests;

public class StatisticsServiceTests
{
    private readonly StatisticsService _service = new();

    private static IncidentDto Incident(string location, int daysAgo, params string[] categories) => new()
    {
        Title = $"{location} incident",
        DateTime = DateTime.Now.AddDays(-daysAgo),
        Location = new LocationDto { Name = location },
        Categories = categories,
    };

    private static readonly List<IncidentDto> Incidents =
    [
        Incident("Sarajevo", 400, "Fire"),
        Incident("Sarajevo", 300, "Fire"),
        Incident("Sarajevo", 200, "Flood"),
        Incident("Tuzla", 20, "Traffic"),
        Incident("Mostar", 3, "Crime"),
        Incident("Mostar", 2, "Crime", "Traffic"),
    ];

    [Fact]
    public void LocationWithMostIncidents_IsCalculatedForEveryPeriod()
    {
        var result = _service.GetLocationWithMostIncidents(Incidents);

        Assert.Equal(["Total", "Last year", "Last month", "Last week"], result.Keys);
        Assert.Equal(new LocationIncidentCount { LocationName = "Sarajevo", IncidentCount = 3 }, result["Total"]);
        Assert.Equal(new LocationIncidentCount { LocationName = "Mostar", IncidentCount = 2 }, result["Last week"]);
    }

    [Fact]
    public void LocationWithMostIncidentsPerCategory_CountsEachLocationAndCategoryPair()
    {
        var result = _service.GetLocationWithMostIncidentsPerCategory(Incidents);

        Assert.Equal(new LocationCategoryIncidentCount { LocationName = "Sarajevo", CategoryName = "Fire", IncidentCount = 2 }, result["Total"]);
        Assert.Equal(new LocationCategoryIncidentCount { LocationName = "Mostar", CategoryName = "Crime", IncidentCount = 2 }, result["Last week"]);
    }

    [Fact]
    public void NumberOfIncidentsPerCategory_CountsAnIncidentOncePerCategory()
    {
        var result = _service.GetNumberOfIncidentsPerCategory(Incidents);

        var lastMonth = result["Last month"].ToDictionary(c => c.CategoryName, c => c.IncidentCount);
        Assert.Equal(new Dictionary<string, int> { ["Traffic"] = 2, ["Crime"] = 2 }, lastMonth);
        Assert.Equal(6 + 1, result["Total"].Sum(c => c.IncidentCount));
    }

    [Fact]
    public void EmptyInput_FallsBackToUnknown()
    {
        var location = _service.GetLocationWithMostIncidents([]);
        var perCategory = _service.GetNumberOfIncidentsPerCategory([]);

        Assert.Equal(new LocationIncidentCount { LocationName = "Unknown", IncidentCount = 0 }, location["Total"]);
        Assert.Empty(perCategory["Total"]);
    }

    [Fact]
    public void IncidentsWithoutALocation_AreGroupedAsUnknown()
    {
        var result = _service.GetLocationWithMostIncidents([new IncidentDto { DateTime = DateTime.Now, Location = null }]);

        Assert.Equal("Unknown", result["Total"].LocationName);
        Assert.Equal(1, result["Total"].IncidentCount);
    }

    [Fact]
    public void LocationHeat_CountsEveryPlaceWithItsCoordinates()
    {
        var incidents = new List<IncidentDto>
        {
            new() { DateTime = DateTime.UtcNow.AddDays(-1), Location = new LocationDto { Name = "Tuzla", Latitude = 44.5, Longitude = 18.6 } },
            new() { DateTime = DateTime.UtcNow.AddDays(-2), Location = new LocationDto { Name = "Tuzla", Latitude = 44.5, Longitude = 18.6 } },
            new() { DateTime = DateTime.UtcNow.AddDays(-40), Location = new LocationDto { Name = "Mostar", Latitude = 43.3, Longitude = 17.8 } },
            new() { DateTime = DateTime.UtcNow, Location = null },
        };

        var all = _service.GetLocationHeat(incidents, since: null);
        var recent = _service.GetLocationHeat(incidents, since: DateTime.UtcNow.AddDays(-30));

        Assert.Equal(new LocationHeat { LocationName = "Tuzla", Latitude = 44.5, Longitude = 18.6, IncidentCount = 2 }, all[0]);
        Assert.Equal(["Tuzla", "Mostar"], all.Select(h => h.LocationName));
        Assert.Equal(["Tuzla"], recent.Select(h => h.LocationName));
    }

    [Fact]
    public void IncidentsPerDay_ListsEveryDayOfTheRange()
    {
        var today = new DateOnly(2026, 5, 10);
        var incidents = new List<IncidentDto>
        {
            new() { DateTime = new DateTime(2026, 5, 10, 9, 0, 0, DateTimeKind.Utc), Categories = ["Fire", "Traffic"] },
            new() { DateTime = new DateTime(2026, 5, 10, 18, 0, 0, DateTimeKind.Utc), Categories = ["Fire"] },
            new() { DateTime = new DateTime(2026, 5, 8, 12, 0, 0, DateTimeKind.Utc), Categories = ["Flood"] },
            new() { DateTime = new DateTime(2026, 4, 1, 12, 0, 0, DateTimeKind.Utc), Categories = ["Crime"] },
        };

        var days = _service.GetIncidentsPerDay(incidents, 3, today);

        Assert.Equal([new DateOnly(2026, 5, 8), new DateOnly(2026, 5, 9), new DateOnly(2026, 5, 10)], days.Select(d => d.Date));
        Assert.Equal([1, 0, 2], days.Select(d => d.Total));
        Assert.Equal(new Dictionary<string, int> { ["Fire"] = 2, ["Traffic"] = 1 }, days[2].Categories);
        Assert.Empty(days[1].Categories);
    }
}
