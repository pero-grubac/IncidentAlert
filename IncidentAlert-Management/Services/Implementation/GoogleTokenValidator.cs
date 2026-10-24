using Google.Apis.Auth;
using IncidentAlert_Management.Models.Dto;

namespace IncidentAlert_Management.Services.Implementation
{
    public class GoogleTokenValidator(GoogleSettings settings, ILogger<GoogleTokenValidator> logger) : IGoogleTokenValidator
    {
        public async Task<GoogleIdentity?> Validate(string idToken)
        {
            if (string.IsNullOrWhiteSpace(settings.ClientId))
                return null;

            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken,
                    new GoogleJsonWebSignature.ValidationSettings { Audience = [settings.ClientId] });
                return new GoogleIdentity(payload.Subject, payload.Email, payload.EmailVerified, payload.Name ?? payload.Email);
            }
            catch (InvalidJwtException ex)
            {
                logger.LogWarning("Rejected a Google ID token: {Reason}", ex.Message);
                return null;
            }
        }
    }
}
