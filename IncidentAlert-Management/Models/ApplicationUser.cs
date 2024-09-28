using Microsoft.AspNetCore.Identity;

namespace IncidentAlert_Management.Models
{
    public class ApplicationUser : IdentityUser
    {
        public RoleEnum Role { get; set; }
        public string GoogleId { get; set; } = string.Empty;

    }
}
