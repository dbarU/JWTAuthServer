using JWTAuthServer.Models;

namespace JWTAuthServer.Services.UserRepository
{
    /// <summary> 
    /// 
    /// </summary>
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByUserNameAsync(string userName);
        Task<User> CreateUserAsync(User user);
        Task<User> GetUserByIdAsync(Guid userId);
    }
}