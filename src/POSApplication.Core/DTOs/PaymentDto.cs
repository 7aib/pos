using POSApplication.Common.Enums;

namespace POSApplication.Core.DTOs;

/// <summary>
/// Payment Data Transfer Object
/// </summary>
public class PaymentDto
{
    public int PaymentID { get; set; }
    public int SaleID { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public decimal Amount { get; set; }
    public string? CardType { get; set; }
    public string? CardLastFourDigits { get; set; }
    public string? TransactionReference { get; set; }
    public DateTime PaymentDate { get; set; }
    public int? ProcessedBy { get; set; }
}
