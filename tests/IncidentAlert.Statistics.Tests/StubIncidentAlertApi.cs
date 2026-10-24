using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace IncidentAlert.Statistics.Tests;

/// <summary>Answers the Statistics service's calls to the IncidentAlert API without a network.</summary>
public sealed class StubHandler : HttpMessageHandler
{
    public HttpStatusCode Status { get; set; } = HttpStatusCode.OK;
    public object Body { get; set; } = Array.Empty<object>();
    public List<string> Requests { get; } = [];

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Requests.Add(request.RequestUri!.PathAndQuery);
        return Task.FromResult(new HttpResponseMessage(Status) { Content = JsonContent.Create(Body) });
    }
}

public sealed class StatisticsApiFactory : WebApplicationFactory<Program>
{
    public StubHandler Upstream { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
            services.AddHttpClient("IncidentAlert").ConfigurePrimaryHttpMessageHandler(() => Upstream));
    }
}
