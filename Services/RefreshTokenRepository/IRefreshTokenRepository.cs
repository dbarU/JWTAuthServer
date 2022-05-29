using JWTAuthServer.Models;

namespace JWTAuthServer.Services.RefreshTokenRepository
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> GetRefreshTokenAsync(string token);
        Task CreateRefreshTokenAsync(RefreshToken refreshToken);
        Task DeleteRefreshTokenAsync(Guid id);
        Task DeleteAllByUserIdAsync(Guid userId);
    }
}
