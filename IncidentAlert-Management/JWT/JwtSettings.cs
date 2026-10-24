namespace IncidentAlert_Management.JWT
{
    public class JwtSettings
    {
        public string Secret { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpiryMinutes { get; set; }

        // The key in appsettings.Development.json; a real deployment must set its own.
        public const string DevelopmentSecret = "dev-only-signing-key-change-me-0123456789abcdef";

        /// <summary>Stops the service at startup instead of signing tokens with a weak or public key.</summary>
        public static void Validate(JwtSettings settings, IHostEnvironment environment)
        {
            if (string.IsNullOrWhiteSpace(settings.Secret) || System.Text.Encoding.UTF8.GetByteCount(settings.Secret) < 32)
                throw new InvalidOperationException("JwtSettings:Secret must be set and at least 32 bytes long (JwtSettings__Secret / JWT_SECRET).");
            if (environment.IsProduction() && settings.Secret == DevelopmentSecret)
                throw new InvalidOperationException("JwtSettings:Secret still has the development value. Set JWT_SECRET for production.");
        }
    }
}
