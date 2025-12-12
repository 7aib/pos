namespace POSApplication.Data.Entities;

public class Customer
{
    public int CustomerID { get; set; }
    public string? CustomerCode { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int LoyaltyPoints { get; set; } = 0;
    public decimal CreditLimit { get; set; } = 0;
    public decimal TotalPurchases { get; set; } = 0;
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    
    // Navigation properties
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    public CreditAccount? CreditAccount { get; set; }
}
