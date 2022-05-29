using System;

namespace JWTAuthServer.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string EmailAddress { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
    }
}