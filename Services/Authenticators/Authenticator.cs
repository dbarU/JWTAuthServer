using JWTAuthServer.Models;
using JWTAuthServer.Models.Response;
using JWTAuthServer.Services.RefreshTokenRepository;
using JWTAuthServer.Services.TokenGenerator;

namespace JWTAuthServer.Services.Authenticators
{
    public class Authenticator
    {
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public Authenticator(
            ITokenGenerator tokenGenerator,
            IRefreshTokenRepository refreshTokenRepository
        )
        {
            _refreshTokenRepository = refreshTokenRepository;
            _tokenGenerator = tokenGenerator;
        }

        public async Task<AuthenticatedUserResponse> Authenticate(User user)
        {
            string accessToken = _tokenGenerator.GenerateAccessToken(user);
            string refreshToken = _tokenGenerator.GenerateRefreshToken();
            RefreshToken RefreshTokenDto = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id
            };
            await _refreshTokenRepository.CreateRefreshTokenAsync(RefreshTokenDto);

            return new AuthenticatedUserResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}
