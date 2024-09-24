using IncidentAlert_Management.JWT;
using IncidentAlert_Management.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IncidentAlert_Management.Services.Implementation
{
    public class JwtService(JwtSettings jwtSettings) : IJwtService
    {
        private readonly JwtSettings _jwtSettings = jwtSettings;

        public string GenerateJwtToken(ApplicationUser user)
        {
            var claims = new[]
                {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(ClaimTypes.Role, user.Role.ToString()),

                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString(), ClaimValueTypes.Integer64)
                };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                            issuer: _jwtSettings.Issuer,
                            audience: _jwtSettings.Audience,
                            claims: claims,
                            expires: DateTime.Now.AddMinutes(_jwtSettings.ExpiryMinutes),
                            signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
