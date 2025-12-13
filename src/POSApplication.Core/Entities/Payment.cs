using POSApplication.Common.Enums;

namespace POSApplication.Core.Entities;

public class Payment
{
    public int PaymentID { get; set; }
    public int SaleID { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public decimal Amount { get; set; }
    public string? CardType { get; set; }
    public string? CardLastFourDigits { get; set; }
    public string? TransactionReference {get; set; }
    public DateTime PaymentDate { get; set; } = DateTime.Now;
    public int? ProcessedBy { get; set; }
    
    // Navigation properties
    public Sale Sale { get; set; } = null!;
    public User? ProcessedByUser { get; set; }
}
