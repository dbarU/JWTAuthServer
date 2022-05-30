namespace JWTAuthServer.Configuration
{
    public record AuthenticationConfiguration
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public string Secret { get; set; } = string.Empty;
        public float ExpiresInMinutes { get; set; } = 0.0f;
        public string RefreshSecret { get; set; } = string.Empty;
        public float RefreshExpiresInMinutes { get; set; } = 0.0f;
        public string KeyVaultUrl { get; set; } = string.Empty;

    }
}
