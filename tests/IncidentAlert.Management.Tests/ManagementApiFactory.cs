using System.Net.Http.Headers;
using System.Net.Http.Json;
using IncidentAlert_Management.Data;
using IncidentAlert_Management.Models;
using IncidentAlert_Management.Services;
using MassTransit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace IncidentAlert.Management.Tests;

/// <summary>Hosts the real Management API with an in-memory database and an in-memory message bus.</summary>
public sealed class ManagementApiFactory : WebApplicationFactory<Program>
{
    public const string Password = "Str0ng!Pass";

    private readonly string _database = $"incident-alert-management-{Guid.NewGuid()}";

    /// <summary>Google ID tokens the fake Google accepts, by token string.</summary>
    public FakeGoogle Google { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Anything but Development, so startup does not run PostgreSQL migrations or the demo seed.
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            // EF Core 9+ also keeps the provider configuration (Npgsql) in a separate registration.
            services.RemoveAll<DbContextOptions<DataContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<DataContext>>();
            services.AddDbContext<DataContext>(o => o.UseInMemoryDatabase(_database));
            services.AddMassTransitTestHarness();
            services.RemoveAll<IGoogleTokenValidator>();
            services.AddSingleton<IGoogleTokenValidator>(Google);
        });
    }

    public async Task<ApplicationUser> CreateUser(RoleEnum role, string? username = null, string? email = null)
    {
        using var scope = Services.CreateScope();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var user = new ApplicationUser
        {
            UserName = username ?? $"user{Guid.NewGuid():N}",
            Email = email ?? "someone@example.com",
            Role = role,
        };
        var result = await users.CreateAsync(user, Password);
        Assert.True(result.Succeeded, string.Join(", ", result.Errors.Select(e => e.Description)));
        return user;
    }

    /// <summary>A client that is signed in as a freshly created user with the given role.</summary>
    public async Task<HttpClient> CreateClientAs(RoleEnum role)
    {
        var user = await CreateUser(role);
        var client = CreateClient();
        var login = await client.PostAsJsonAsync("/Login", new { username = user.UserName, password = Password });
        login.EnsureSuccessStatusCode();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", await login.Content.ReadAsStringAsync());
        return client;
    }

    /// <summary>A client carrying a token for any role, signed like the Management API signs its own.</summary>
    public HttpClient CreateClientWithRole(string role)
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestJwt.Token(role));
        return client;
    }

    public void Seed(Action<DataContext> seed)
    {
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<DataContext>();
        seed(context);
        context.SaveChanges();
    }

    public T Query<T>(Func<DataContext, T> query)
    {
        using var scope = Services.CreateScope();
        return query(scope.ServiceProvider.GetRequiredService<DataContext>());
    }
}

public sealed class FakeGoogle : IGoogleTokenValidator
{
    private readonly Dictionary<string, GoogleIdentity> _tokens = [];

    /// <summary>Makes a token valid and returns it.</summary>
    public string Issue(string email, bool verified = true, string? subject = null, string name = "Ana Student")
    {
        var token = $"google-token-{Guid.NewGuid():N}";
        _tokens[token] = new GoogleIdentity(subject ?? Guid.NewGuid().ToString("N"), email, verified, name);
        return token;
    }

    public Task<GoogleIdentity?> Validate(string idToken) => Task.FromResult(_tokens.GetValueOrDefault(idToken));
}
