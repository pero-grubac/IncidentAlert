namespace IncidentAlert_Management.Models.Dto
{
    /// <summary>An account as an administrator sees it. Never carries a password.</summary>
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public RoleEnum Role { get; set; }
        public string RoleName => Role.ToString();
        public bool SignsInWithGoogle { get; set; }
    }
}
