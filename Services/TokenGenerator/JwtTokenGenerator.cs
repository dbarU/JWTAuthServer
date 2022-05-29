using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JWTAuthServer.Services.TokenGenerator
{
    public static class JwtTokenGenerator
    {
        public static string GenerateToken(
            string secretKey,
            string issuer,
            string audience,
            float expirationInMinutes,
            IEnumerable<Claim>? claims = null
        )
        {
            SecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            SigningCredentials credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            JwtSecurityToken securityToken = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(expirationInMinutes),
                credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(securityToken);
        }
    }
}
