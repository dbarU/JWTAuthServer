using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JWTAuthServer.Models.AuthDbContext;

public class AuthenticationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public AuthenticationDbContext(DbContextOptions<AuthenticationDbContext> options) : base(options)
    {

    }
    //Tables
    //remove as taken care of by identitydbcontext?
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> Tokens { get; set; }
}