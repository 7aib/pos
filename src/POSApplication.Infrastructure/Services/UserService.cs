using Microsoft.EntityFrameworkCore;
using POSApplication.Core.Interfaces;
using POSApplication.Data.Context;
using POSApplication.Core.Entities;
using System.Security.Cryptography;
using System.Text;

namespace POSApplication.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly POSDbContext _context;

    public UserService(POSDbContext context)
    {
        _context = context;
    }

    public async Task<User?> AuthenticateAsync(string username, string password)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null || !user.IsActive) return null;

        if (VerifyPassword(password, user.PasswordHash))
        {
            user.LastLogin = DateTime.Now;
            await _context.SaveChangesAsync();
            return user;
        }

        return null;
    }

    public async Task<User> CreateUserAsync(User user, string password)
    {
        if (await _context.Users.AnyAsync(u => u.Username == user.Username))
            throw new Exception("Username already exists");

        user.PasswordHash = HashPassword(password);
        user.CreatedAt = DateTime.Now;
        
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task UpdateUserAsync(User user)
    {
        var existing = await _context.Users.FindAsync(user.UserID);
        if (existing == null) throw new Exception("User not found");

        existing.FullName = user.FullName;
        existing.Role = user.Role;
        existing.IsActive = user.IsActive;
        existing.Email = user.Email;
        existing.Phone = user.Phone;
        // Password is updated separately
        
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        if (!VerifyPassword(oldPassword, user.PasswordHash)) return false;

        user.PasswordHash = HashPassword(newPassword);
        await _context.SaveChangesAsync();
        return true;
    }

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private bool VerifyPassword(string password, string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch
        {
            // Fallback for legacy hashes if needed, or just fail
            return false;
        }
    }
}
