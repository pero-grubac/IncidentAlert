using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using IncidentAlert_Management.Models;

namespace IncidentAlert.Management.Tests;

public class AuthenticationTests(ManagementApiFactory factory) : IClassFixture<ManagementApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Login_ReturnsAJwtCarryingTheModeratorRole()
    {
        var user = await factory.CreateUser(RoleEnum.MODERATOR);

        var response = await _client.PostAsJsonAsync("/Login",
            new { username = user.UserName, password = ManagementApiFactory.Password });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var token = new JwtSecurityTokenHandler().ReadJwtToken(await response.Content.ReadAsStringAsync());
        Assert.Equal("MODERATOR", token.Claims.Single(c => c.Type == ClaimTypes.Role).Value);
        Assert.Equal(user.UserName, token.Subject);
        Assert.InRange(token.ValidTo, DateTime.UtcNow.AddHours(7), DateTime.UtcNow.AddHours(9));
    }

    [Fact]
    public async Task Login_WithAWrongPasswordIsABadRequest()
    {
        var user = await factory.CreateUser(RoleEnum.MODERATOR);

        var response = await _client.PostAsJsonAsync("/Login", new { username = user.UserName, password = "Wrong!123" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Invalid username or password", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Login_ForAnUnknownUserLooksLikeAWrongPassword()
    {
        var response = await _client.PostAsJsonAsync("/Login", new { username = "nobody-here", password = "Wrong!123" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("Invalid username or password", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Login_TellsAPendingAccountToWaitForApproval()
    {
        var user = await factory.CreateUser(RoleEnum.NOTITLE);

        var response = await _client.PostAsJsonAsync("/Login",
            new { username = user.UserName, password = ManagementApiFactory.Password });

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Contains("waiting for an administrator", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Register_CreatesAnAccountThatCannotModerateYet()
    {
        var username = $"newmod{Guid.NewGuid():N}"[..20];

        var register = await _client.PostAsJsonAsync("/Register", new { username, email = "new@example.com", password = "Str0ng!Pass" });
        var login = await _client.PostAsJsonAsync("/Login", new { username, password = "Str0ng!Pass" });

        Assert.Equal(HttpStatusCode.OK, register.StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, login.StatusCode);
        Assert.Equal(RoleEnum.NOTITLE, factory.Query(db => db.Users.Single(u => u.UserName == username).Role));
    }

    [Theory]
    [InlineData("/Incident/getAll")]
    [InlineData("/Category")]
    [InlineData("/Status")]
    [InlineData("/users")]
    public async Task ProtectedEndpoints_RequireASignedInUser(string url)
    {
        var response = await _client.GetAsync(url);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ModerationEndpoints_AreOpenToModeratorsAndAdministrators()
    {
        Assert.Equal(HttpStatusCode.OK, (await factory.CreateClientWithRole("MODERATOR").GetAsync("/Incident/getAll")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await factory.CreateClientWithRole("ADMIN").GetAsync("/Incident/getAll")).StatusCode);
    }

    [Fact]
    public async Task ModerationEndpoints_AreForbiddenForAPendingAccount()
    {
        var response = await factory.CreateClientWithRole("NOTITLE").GetAsync("/Incident/getAll");

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task ReportsCannotBeInjectedIntoTheQueueAnonymously()
    {
        using var form = new MultipartFormDataContent { { new StringContent("Fake"), "title" } };

        var response = await _client.PostAsync("/Incident", form);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Status_ListsEveryStatusExceptAccepted()
    {
        var moderator = await factory.CreateClientAs(RoleEnum.MODERATOR);

        var statuses = await moderator.GetFromJsonAsync<List<StatusItem>>("/Status");

        Assert.Equal(["PENDING", "REJECTED"], statuses!.Select(s => s.Name).Order());
    }

    private sealed record StatusItem(int Id, string Name);
}
