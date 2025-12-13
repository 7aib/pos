namespace POSApplication.Core.DTOs;

/// <summary>
/// Customer Data Transfer Object
/// </summary>
public class CustomerDto
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
    public int LoyaltyPoints { get; set; }
    public decimal CreditLimit { get; set; }
    public decimal TotalPurchases { get; set; }
    public bool IsActive { get; set; }
    public string? Notes { get; set; }
    
    // Display property
    public string FullName => $"{FirstName} {LastName}".Trim();
}
