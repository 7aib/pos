using POSApplication.Core.Entities;

namespace POSApplication.Core.Interfaces;

public interface IUserService
{
    Task<User?> AuthenticateAsync(string username, string password);
    Task<User> CreateUserAsync(User user, string password);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(int id);
    Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
}
