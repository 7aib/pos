using POSApplication.Common.Enums;
using System.ComponentModel.DataAnnotations;

namespace POSApplication.Core.Entities;

public class User
{
    public int UserID { get; set; }
    
    [Required]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    public string PasswordHash { get; set; } = string.Empty;
    
    public string FullName { get; set; } = string.Empty;
    
    public string? Email { get; set; }
    public string? Phone { get; set; }
    
    public UserRole Role { get; set; } = UserRole.Cashier;
    
    public bool IsActive { get; set; } = true;
    
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? LastLogin { get; set; }
    
    public int? CreatedBy { get; set; }
    public User? Creator { get; set; }

    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
