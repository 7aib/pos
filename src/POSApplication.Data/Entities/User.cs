using POSApplication.Common.Enums;

namespace POSApplication.Data.Entities;

public class User
{
    public int UserID { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? LastLoginAt { get; set; }
    public int? CreatedBy { get; set; }
    
    // Navigation properties
    public User? Creator { get; set; }
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
