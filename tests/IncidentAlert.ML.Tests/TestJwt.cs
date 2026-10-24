using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace IncidentAlert.ML.Tests;

/// <summary>The JWT settings every API under test runs with, and tokens signed with them.</summary>
public static class TestJwt
{
    public const string Secret = "test-signing-key-for-the-api-tests-0123456789";
    // Same issuer and audience as appsettings.json
    public const string Issuer = "incident-alert-management";
    public const string Audience = "incident-alert";

    // The "Testing" environment does not load appsettings.Development.json, so the secret comes from here.
    [ModuleInitializer]
    internal static void Configure() => Environment.SetEnvironmentVariable("JwtSettings__Secret", Secret);

    public static string Token(string role, string username = "tester", string secret = Secret)
    {
        var credentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(Issuer, Audience,
            [new Claim(JwtRegisteredClaimNames.Sub, username), new Claim(ClaimTypes.Role, role)],
            expires: DateTime.UtcNow.AddMinutes(10), signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
