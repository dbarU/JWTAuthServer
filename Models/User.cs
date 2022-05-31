using Microsoft.AspNetCore.Identity;

namespace JWTAuthServer.Models
{
    public class User : IdentityUser<Guid>
    {
        //public Guid Id { get; set; }
        //public string EmailAddress { get; set; }
        //public string UserName { get; set; }
        //public string PasswordHash { get; set; }
    }
}