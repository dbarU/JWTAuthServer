namespace JWTAuthServer.Models
{
    public record RefreshToken
    {
        public Guid Id { get; set; }
        public string Token { get; init; } = string.Empty;
        
        public Guid UserId { get; init; }
    }
}
