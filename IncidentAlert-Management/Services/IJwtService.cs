using IncidentAlert_Management.Models;

namespace IncidentAlert_Management.Services
{
    public interface IJwtService
    {
        string GenerateJwtToken(ApplicationUser user);
    }
}
