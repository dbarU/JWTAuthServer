using JWTAuthServer.Models;

namespace JWTAuthServer.Services.UserRepository
{
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly IList<User> _users = new List<User>();

        public Task<User> CreateUserAsync(User user)
        {
            user.Id = Guid.NewGuid();
            _users.Add(user);
            return Task.FromResult(user);
        }

        public Task<User> GetByEmailAsync(string email)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.Email == email));
        }

        public Task<User> GetByUserNameAsync(string userName)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.UserName == userName));

        }

        public Task<User> GetUserByIdAsync(Guid userId)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.Id == userId));
        }
    }
}