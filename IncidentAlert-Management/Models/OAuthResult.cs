namespace IncidentAlert_Management.Models
{
    public enum OAuthResult
    {
        /// <summary>A new account was created and waits for an administrator.</summary>
        Created,
        LoggedIn,
        /// <summary>The account exists but has not been approved yet.</summary>
        Pending,
    }
}
