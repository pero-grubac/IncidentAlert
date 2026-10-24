namespace IncidentAlert_Management.Services
{
    /// <summary>What a verified Google ID token says about the person signing in.</summary>
    public record GoogleIdentity(string Subject, string Email, bool EmailVerified, string Name);

    public interface IGoogleTokenValidator
    {
        /// <summary>Checks the token's signature, issuer, expiry and audience. Returns null for an invalid token.</summary>
        Task<GoogleIdentity?> Validate(string idToken);
    }
}
