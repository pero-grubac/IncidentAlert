using System.Net;
using System.Net.Http.Json;
using IncidentAlert_Management.Models;

namespace IncidentAlert.Management.Tests;

public class AccountAdministrationTests(ManagementApiFactory factory) : IClassFixture<ManagementApiFactory>
{
    private sealed record UserItem(string Id, string Username, string Email, string RoleName, bool SignsInWithGoogle);

    [Fact]
    public async Task AnAdministrator_ApprovesAPendingAccount_WhichCanThenSignIn()
    {
        var admin = await factory.CreateClientAs(RoleEnum.ADMIN);
        var pending = await factory.CreateUser(RoleEnum.NOTITLE);

        var users = await admin.GetFromJsonAsync<List<UserItem>>("/users");
        Assert.Contains(users!, u => u.Id == pending.Id && u.RoleName == "NOTITLE");

        var approve = await admin.PutAsync($"/users/{pending.Id}/approve", null);
        var login = await factory.CreateClient().PostAsJsonAsync("/Login",
            new { username = pending.UserName, password = ManagementApiFactory.Password });

        Assert.Equal(HttpStatusCode.OK, approve.StatusCode);
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
    }

    [Fact]
    public async Task Revoking_TakesTheModeratorRoleAway()
    {
        var admin = await factory.CreateClientAs(RoleEnum.ADMIN);
        var moderator = await factory.CreateUser(RoleEnum.MODERATOR);

        var response = await admin.PutAsync($"/users/{moderator.Id}/revoke", null);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(RoleEnum.NOTITLE, factory.Query(db => db.Users.Single(u => u.Id == moderator.Id).Role));
    }

    [Fact]
    public async Task Deleting_RemovesTheAccount()
    {
        var admin = await factory.CreateClientAs(RoleEnum.ADMIN);
        var pending = await factory.CreateUser(RoleEnum.NOTITLE);

        var response = await admin.DeleteAsync($"/users/{pending.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.False(factory.Query(db => db.Users.Any(u => u.Id == pending.Id)));
    }

    [Fact]
    public async Task AnotherAdministrator_CannotBeChanged()
    {
        var admin = await factory.CreateClientAs(RoleEnum.ADMIN);
        var otherAdmin = await factory.CreateUser(RoleEnum.ADMIN);

        var response = await admin.PutAsync($"/users/{otherAdmin.Id}/revoke", null);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(RoleEnum.ADMIN, factory.Query(db => db.Users.Single(u => u.Id == otherAdmin.Id).Role));
    }

    [Fact]
    public async Task Moderators_CannotManageAccounts()
    {
        var moderator = await factory.CreateClientAs(RoleEnum.MODERATOR);
        var pending = await factory.CreateUser(RoleEnum.NOTITLE);

        Assert.Equal(HttpStatusCode.Forbidden, (await moderator.GetAsync("/users")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await moderator.PutAsync($"/users/{pending.Id}/approve", null)).StatusCode);
    }

    [Fact]
    public async Task AnUnknownAccount_IsNotFound()
    {
        var admin = await factory.CreateClientAs(RoleEnum.ADMIN);

        var response = await admin.PutAsync($"/users/{Guid.NewGuid()}/approve", null);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
