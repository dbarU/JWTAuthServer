using JWTAuthServer.Configuration;
using JWTAuthServer.Models;
using System.Security.Claims;

namespace JWTAuthServer.Services.TokenGenerator
{
    public class AccessTokenGenerator : ITokenGenerator
    {
        private readonly AuthenticationConfiguration _configuration;

        public AccessTokenGenerator(AuthenticationConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateAccessToken(User user)
        {
            List<Claim> claims = new List<Claim>()
            {
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.EmailAddress)
            };

            return JwtTokenGenerator.GenerateToken(
                _configuration.Secret,
                _configuration.Issuer,
                _configuration.Audience,
                _configuration.ExpiresInMinutes,
                claims
            );
        }

        public string GenerateRefreshToken()
        {
            return JwtTokenGenerator.GenerateToken(
                _configuration.Secret,
                _configuration.Issuer,
                _configuration.Audience,
                _configuration.ExpiresInMinutes
            );
        }
    }
}
