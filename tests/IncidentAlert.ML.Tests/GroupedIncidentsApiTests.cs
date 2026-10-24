using System.Net;
using System.Net.Http.Json;
using IncidentAlert_ML.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace IncidentAlert.ML.Tests;

public sealed class StubHandler : HttpMessageHandler
{
    public HttpStatusCode Status { get; set; } = HttpStatusCode.OK;
    public object Body { get; set; } = Array.Empty<object>();

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) =>
        Task.FromResult(new HttpResponseMessage(Status) { Content = JsonContent.Create(Body) });
}

public sealed class MlApiFactory : WebApplicationFactory<Program>
{
    public StubHandler Upstream { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
            services.AddHttpClient("IncidentAlert").ConfigurePrimaryHttpMessageHandler(() => Upstream));
    }
}

public class GroupedIncidentsApiTests(MlApiFactory factory) : IClassFixture<MlApiFactory>
{
    private readonly HttpClient _client = SignedIn(factory.CreateClient());

    private static HttpClient SignedIn(HttpClient client)
    {
        client.DefaultRequestHeaders.Authorization = new("Bearer", TestJwt.Token("MODERATOR"));
        return client;
    }

    [Fact]
    public async Task GroupedIncidents_ReturnsEveryUpstreamIncident()
    {
        factory.Upstream.Status = HttpStatusCode.OK;
        factory.Upstream.Body = new[]
        {
            new { id = 1, title = "Fire", text = "Fire in the center", dateTime = DateTime.UtcNow, location = new { id = 1, name = "Tuzla" } },
            new { id = 2, title = "Flood", text = "Flooded main road", dateTime = DateTime.UtcNow, location = new { id = 2, name = "Mostar" } },
        };

        var groups = await _client.GetFromJsonAsync<List<IncidentGroup>>("/Incident/grouped-incidents");

        Assert.Equal([1, 2], groups!.SelectMany(g => g.Incidents).Select(i => i.Id).Order());
    }

    [Fact]
    public async Task GroupedIncidents_WithNoIncidentsIsAnEmptyList()
    {
        factory.Upstream.Status = HttpStatusCode.OK;
        factory.Upstream.Body = Array.Empty<object>();

        var groups = await _client.GetFromJsonAsync<List<IncidentGroup>>("/Incident/grouped-incidents");

        Assert.Empty(groups!);
    }

    [Fact]
    public async Task GroupedIncidents_RequireASignedInModerator()
    {
        var response = await factory.CreateClient().GetAsync("/Incident/grouped-incidents");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Administrators_CanReadThemToo()
    {
        var admin = factory.CreateClient();
        admin.DefaultRequestHeaders.Authorization = new("Bearer", TestJwt.Token("ADMIN"));
        factory.Upstream.Status = HttpStatusCode.OK;
        factory.Upstream.Body = Array.Empty<object>();

        var response = await admin.GetAsync("/Incident/grouped-incidents");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpstreamFailure_IsPassedThrough()
    {
        factory.Upstream.Status = HttpStatusCode.BadGateway;

        var response = await _client.GetAsync("/Incident/grouped-incidents");

        Assert.Equal(HttpStatusCode.BadGateway, response.StatusCode);
    }
}
