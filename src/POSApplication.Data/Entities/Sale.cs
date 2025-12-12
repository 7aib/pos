using POSApplication.Common.Enums;

namespace POSApplication.Data.Entities;

public class Sale
{
    public int SaleID { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public int? CustomerID { get; set; }
    public DateTime SaleDate { get; set; } = DateTime.Now;
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; } = 0;
    public decimal DiscountAmount { get; set; } = 0;
    public decimal TotalAmount { get; set; }
    public decimal AmountPaid { get; set; } = 0;
    public decimal ChangeGiven { get; set; } = 0;
    public TransactionStatus Status { get; set; } = TransactionStatus.Completed;
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Paid;
    public int CashierID { get; set; }
    public string? Notes { get; set; }
    public bool IsSynced { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public int? VoidedBy { get; set; }
    public DateTime? VoidedAt { get; set; }
    public string? VoidReason { get; set; }
    
    // Navigation properties
    public Customer? Customer { get; set; }
    public User Cashier { get; set; } = null!;
    public User? VoidedByUser { get; set; }
    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
