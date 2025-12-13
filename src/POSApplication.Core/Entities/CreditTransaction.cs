using POSApplication.Common.Enums;

namespace POSApplication.Core.Entities;

public class CreditTransaction
{
    public int CreditTransactionID { get; set; }
    public int CreditAccountID { get; set; }
    public CreditTransactionType TransactionType { get; set; }
    public decimal Amount { get; set; }
    public decimal BalanceAfter { get; set; }
    public int? SaleID { get; set; }
    public string? PaymentMethod { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public int CreatedBy { get; set; }
    
    // Navigation properties
    public CreditAccount CreditAccount { get; set; } = null!;
    public Sale? Sale { get; set; }
    public User CreatedByUser { get; set; } = null!;
}
