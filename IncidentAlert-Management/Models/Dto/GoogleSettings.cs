namespace IncidentAlert_Management.Models.Dto
{
    public class GoogleSettings
    {
        /// <summary>OAuth client ID. Google sign-in is off while it is empty.</summary>
        public string ClientId { get; set; } = string.Empty;

        /// <summary>Only Google accounts of this domain may sign in.</summary>
        public string AllowedDomain { get; set; } = "student.etf.unibl.org";
    }
}
