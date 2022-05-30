using JWTAuthServer.Models;
using JWTAuthServer.Models.AuthDbContext;
using Microsoft.EntityFrameworkCore;

namespace JWTAuthServer.Services.UserRepository;

public class EfCoreDbUserRepository : IUserRepository
{
    private readonly AuthenticationDbContext _context;

    public EfCoreDbUserRepository(AuthenticationDbContext context)
    {
        _context = context;
    }

    public async Task<User> CreateUserAsync(User user)
    {
        _context.Users.Add(user);
        _ = await _context.SaveChangesAsync();
        return user;
    }

    public async Task<User> GetByEmailAsync(string email)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.EmailAddress == email);
        return user;
    }

    public async Task<User> GetByUserNameAsync(string userName)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName);
        return user;
    }

    public async Task<User> GetUserByIdAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        return user;

    }
}