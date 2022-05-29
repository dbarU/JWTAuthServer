using JWTAuthServer.Models;

namespace JWTAuthServer.Services.TokenGenerator
{
    public interface ITokenGenerator
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
    }
}
