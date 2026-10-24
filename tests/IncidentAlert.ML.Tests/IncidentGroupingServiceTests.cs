using IncidentAlert_ML.Model;
using IncidentAlert_ML.Service.Implementation;

namespace IncidentAlert.ML.Tests;

public class IncidentGroupingServiceTests
{
    private readonly IncidentGroupingService _service = new();

    private static List<SimpleIncident> Incidents(params string[] texts) =>
        texts.Select((text, i) => new SimpleIncident { Id = i + 1, Title = $"#{i + 1}", Text = text }).ToList();

    [Fact]
    public async Task NoIncidents_GiveNoGroups()
    {
        var groups = await _service.GroupIncidentsByText([]);

        Assert.Empty(groups);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(4)]
    public async Task FewerIncidentsThanClusters_StillWork(int count)
    {
        var incidents = Incidents(Enumerable.Range(1, count).Select(i => $"Report number {i} about a fire").ToArray());

        var groups = await _service.GroupIncidentsByText(incidents);

        Assert.InRange(groups.Count, 1, count);
        Assert.Equal(incidents.Select(i => i.Id).Order(), groups.SelectMany(g => g.Incidents).Select(i => i.Id).Order());
    }

    [Fact]
    public async Task EveryIncidentEndsUpInExactlyOneGroup()
    {
        var incidents = Incidents(
            "Fire in the city center near the main square",
            "Large fire broke out in the city center",
            "Smoke and fire near the main square",
            "Heavy flooding on the main road",
            "Flooding closed the road to the bridge",
            "River flooding on the main road",
            "Bank robbery downtown",
            "Armed robbery at the local bank",
            "Car accident on the highway",
            "Multiple vehicle accident on the highway",
            "Gas leak in a residential building",
            "Gas leak reported in the building");

        var groups = await _service.GroupIncidentsByText(incidents);

        Assert.InRange(groups.Count, 1, IncidentGroupingService.MaxClusters);
        Assert.Equal(incidents.Count, groups.Sum(g => g.Incidents.Count));
        Assert.Equal(incidents.Count, groups.SelectMany(g => g.Incidents).Select(i => i.Id).Distinct().Count());
        Assert.All(groups, g => Assert.StartsWith("Cluster ", g.GroupKey));
    }

    [Fact]
    public async Task IdenticalTextsShareAGroup()
    {
        var incidents = Incidents(
            "Bridge closed due to flooding",
            "Bridge closed due to flooding",
            "Robbery at local bank downtown",
            "Forest fire near national park",
            "Highway accident with several cars",
            "Gas leak in residential building");

        var groups = await _service.GroupIncidentsByText(incidents);

        var groupOf = groups.SelectMany(g => g.Incidents.Select(i => (i.Id, g.GroupKey))).ToDictionary(x => x.Id, x => x.GroupKey);
        Assert.Equal(groupOf[1], groupOf[2]);
    }
}
