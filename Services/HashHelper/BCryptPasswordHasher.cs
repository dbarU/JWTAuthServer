namespace JWTAuthServer.Services.HashHelper
{
    using BCrypt.Net;

    public class BCryptPasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Verify(password, passwordHash);
        }
    }
}
