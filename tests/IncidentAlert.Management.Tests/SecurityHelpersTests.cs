using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IncidentAlert_Management.JWT;
using IncidentAlert_Management.Models;
using IncidentAlert_Management.Services.Implementation;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;

namespace IncidentAlert.Management.Tests;

public class SecurityHelpersTests
{
    private static readonly JwtSettings Settings = new()
    {
        Secret = "a-test-signing-key-that-is-long-enough-123",
        Issuer = "tests",
        Audience = "tests",
        ExpiryMinutes = 30,
    };

    [Fact]
    public void Jwt_IsSignedAndCarriesTheRole()
    {
        var token = new JwtService(Settings).GenerateJwtToken(new ApplicationUser { UserName = "mod", Role = RoleEnum.MODERATOR });

        var principal = new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
        {
            ValidIssuer = Settings.Issuer,
            ValidAudience = Settings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Settings.Secret)),
        }, out var validated);

        Assert.True(principal.IsInRole("MODERATOR"));
        Assert.InRange(validated.ValidTo, DateTime.UtcNow.AddMinutes(29), DateTime.UtcNow.AddMinutes(31));
    }

    [Fact]
    public void Jwt_IsRejectedWithAnotherKey()
    {
        var token = new JwtService(Settings).GenerateJwtToken(new ApplicationUser { UserName = "mod", Role = RoleEnum.MODERATOR });

        Assert.ThrowsAny<SecurityTokenException>(() => new JwtSecurityTokenHandler().ValidateToken(token,
            new TokenValidationParameters
            {
                ValidIssuer = Settings.Issuer,
                ValidAudience = Settings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("another-key-that-is-also-long-enough-456")),
            }, out _));
    }

    [Fact]
    public void GeneratedPasswords_SatisfyTheIdentityPolicy()
    {
        var service = new CustomPasswordService();

        for (var i = 0; i < 200; i++)
        {
            var password = service.GenerateRandomPassword();

            Assert.True(password.Length >= 6);
            Assert.Contains(password, char.IsDigit);
            Assert.Contains(password, char.IsLower);
            Assert.Contains(password, char.IsUpper);
            Assert.Contains(password, c => !char.IsLetterOrDigit(c));
        }
    }

    private sealed class Environment(string name) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = name;
        public string ApplicationName { get; set; } = "tests";
        public string ContentRootPath { get; set; } = ".";
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }

    [Theory]
    [InlineData("")]
    [InlineData("too-short")]
    public void Startup_RefusesAMissingOrShortSecret(string secret) =>
        Assert.Throws<InvalidOperationException>(() =>
            JwtSettings.Validate(new JwtSettings { Secret = secret }, new Environment("Development")));

    [Fact]
    public void Startup_RefusesTheDevelopmentSecretInProduction()
    {
        var settings = new JwtSettings { Secret = JwtSettings.DevelopmentSecret };

        JwtSettings.Validate(settings, new Environment("Development"));
        Assert.Throws<InvalidOperationException>(() => JwtSettings.Validate(settings, new Environment("Production")));
    }
}
