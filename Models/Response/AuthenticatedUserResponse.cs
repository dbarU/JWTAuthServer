using System;

namespace JWTAuthServer.Models.Response
{
    public record AuthenticatedUserResponse
    {
        public string AccessToken { get; init; } = string.Empty;
        public string RefreshToken { get; init; } = string.Empty;
    }
}