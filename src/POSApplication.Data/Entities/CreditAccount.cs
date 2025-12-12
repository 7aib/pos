namespace POSApplication.Data.Entities;

public class CreditAccount
{
    public int CreditAccountID { get; set; }
    public int CustomerID { get; set; }
    public decimal CreditLimit { get; set; }
    public decimal CurrentBalance { get; set; } = 0;
    public decimal InterestRate { get; set; } = 0;
    public string? InterestType { get; set; } // 'none', 'simple', 'compound'
    public int PaymentTermDays { get; set; } = 30;
    public DateTime? LastStatementDate { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsFrozen { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    // Navigation properties
    public Customer Customer { get; set; } = null!;
    public ICollection<CreditTransaction> CreditTransactions { get; set; } = new List<CreditTransaction>();
}
