using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Http.Json;
using IncidentAlert_Management.Models;

namespace IncidentAlert.Management.Tests;

public class GoogleSignInTests(ManagementApiFactory factory) : IClassFixture<ManagementApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    private static string StudentEmail() => $"s{Guid.NewGuid():N}"[..12] + "@student.etf.unibl.org";

    private Task<HttpResponseMessage> SignIn(string idToken) =>
        _client.PostAsJsonAsync("/Login/oauth", new { idToken });

    [Fact]
    public async Task FirstSignIn_CreatesAPendingAccount()
    {
        var email = StudentEmail();

        var response = await SignIn(factory.Google.Issue(email));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var user = factory.Query(db => db.Users.Single(u => u.Email == email));
        Assert.Equal(RoleEnum.NOTITLE, user.Role);
        Assert.False(string.IsNullOrEmpty(user.GoogleId));
    }

    [Fact]
    public async Task APendingAccount_IsToldToWait()
    {
        var email = StudentEmail();
        var subject = Guid.NewGuid().ToString("N");
        await SignIn(factory.Google.Issue(email, subject: subject));

        var again = await SignIn(factory.Google.Issue(email, subject: subject));

        Assert.Equal(HttpStatusCode.Forbidden, again.StatusCode);
    }

    [Fact]
    public async Task AnApprovedAccount_GetsAToken()
    {
        var email = StudentEmail();
        var subject = Guid.NewGuid().ToString("N");
        await SignIn(factory.Google.Issue(email, subject: subject));
        factory.Seed(db => db.Users.Single(u => u.Email == email).Role = RoleEnum.MODERATOR);

        var response = await SignIn(factory.Google.Issue(email, subject: subject));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var token = new JwtSecurityTokenHandler().ReadJwtToken(await response.Content.ReadAsStringAsync());
        Assert.Contains(token.Claims, c => c.Value == "MODERATOR");
    }

    [Fact]
    public async Task AnExistingAccount_IsLinkedByItsVerifiedEmail()
    {
        var email = StudentEmail();
        var user = await factory.CreateUser(RoleEnum.MODERATOR, email: email);

        var response = await SignIn(factory.Google.Issue(email));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.False(string.IsNullOrEmpty(factory.Query(db => db.Users.Single(u => u.Id == user.Id).GoogleId)));
    }

    [Fact]
    public async Task ATokenGoogleDidNotIssue_IsRejected()
    {
        var response = await SignIn("forged-token");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("someone@gmail.com", true)]
    [InlineData("someone@student.etf.unibl.org.evil.com", true)]
    [InlineData("someone@student.etf.unibl.org", false)]
    public async Task OnlyVerifiedStudentAddresses_MaySignIn(string email, bool verified)
    {
        var response = await SignIn(factory.Google.Issue(email, verified));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.False(factory.Query(db => db.Users.Any(u => u.Email == email)));
    }

    [Fact]
    public async Task TheOldContract_WithAClaimedEmail_NoLongerWorks()
    {
        // Before, the browser sent the email and Google id itself and the API trusted them.
        var response = await _client.PostAsJsonAsync("/Login/oauth",
            new { googleId = "1", email = StudentEmail(), username = "attacker" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
