using Microsoft.EntityFrameworkCore;

namespace JWTAuthServer.Models.AuthDbContext;

public class AuthenticationDbContext : DbContext
{
    public AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options) : base(options)
    {

    }
    //Tables
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> Tokens { get; set; }
}