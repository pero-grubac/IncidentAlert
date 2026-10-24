namespace IncidentAlert_Management.Models.Dto
{
    /// <summary>The ID token ("credential") the browser received from Google Identity Services.</summary>
    public class GoogleLoginDto
    {
        public string IdToken { get; set; } = string.Empty;
    }
}
