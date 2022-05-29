using System.IdentityModel.Tokens.Jwt;
using System.Text;
using JWTAuthServer.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace JWTAuthServer.Services.TokenValidator
{
    public class RefreshTokenValidator
    {
        private readonly AuthenticationConfiguration _configuration;

        public RefreshTokenValidator(AuthenticationConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool Validate(string refreshToken)
        {
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            var tokenValidationParameters = new TokenValidationParameters()
            {
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration.RefreshSecret)
                ),
                ValidIssuer = _configuration.Issuer,
                ValidAudience = _configuration.Audience,
                ValidateIssuerSigningKey = true,
                ValidateAudience = true,
                ValidateIssuer = true,
                ClockSkew = TimeSpan.Zero
            };
            try
            {
                handler.ValidateToken(refreshToken, tokenValidationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
