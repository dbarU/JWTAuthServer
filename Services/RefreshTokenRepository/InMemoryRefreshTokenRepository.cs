using JWTAuthServer.Models;
namespace JWTAuthServer.Services.RefreshTokenRepository
{
    public class InMemoryRefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly List<RefreshToken> _refreshTokens = new List<RefreshToken>();
        public Task CreateRefreshTokenAsync(RefreshToken refreshToken)
        {
            refreshToken.Id = Guid.NewGuid();
            _refreshTokens.Add(refreshToken);
            return Task.CompletedTask;
        }

        public Task DeleteAllByUserIdAsync(Guid userId)
        {
            _refreshTokens.RemoveAll(x => x.UserId == userId);
            return Task.CompletedTask;

        }

        public Task DeleteRefreshTokenAsync(Guid id)
        {
            _refreshTokens.RemoveAll(x => x.Id == id);
            return Task.CompletedTask;
        }

        public Task<RefreshToken> GetRefreshTokenAsync(string token)
        {
            return Task.FromResult(_refreshTokens
                .FirstOrDefault(t => t.Token == token));
        }
    }
}
