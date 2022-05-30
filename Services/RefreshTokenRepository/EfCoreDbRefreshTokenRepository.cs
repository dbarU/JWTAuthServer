using JWTAuthServer.Models;
using JWTAuthServer.Models.AuthDbContext;
using Microsoft.EntityFrameworkCore;

namespace JWTAuthServer.Services.RefreshTokenRepository;

public class EfCoreDbRefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AuthenticationDbContext _context;

    public EfCoreDbRefreshTokenRepository(AuthenticationDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken> GetRefreshTokenAsync(string token)
    {
        var refreshToken = await _context.Tokens.FirstOrDefaultAsync(x => x.Token == token);
        return refreshToken;
    }

    public async Task CreateRefreshTokenAsync(RefreshToken refreshToken)
    {
        _context.Tokens.Add(refreshToken);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteRefreshTokenAsync(Guid id)
    {
        var token = await _context.Tokens.FindAsync(id);
        if (token == null)
            return;

        _context.Tokens.Remove(token);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAllByUserIdAsync(Guid userId)
    {
        var tokens = await _context.Tokens
            .Where(t => t.UserId == userId)
            .ToListAsync();

        if (tokens.Count == 0)
            return;

        _context.Tokens.RemoveRange(tokens);
        await _context.SaveChangesAsync();
    }
}