using POSApplication.Common.Enums;

namespace POSApplication.Core.DTOs;

/// <summary>
/// Sale Data Transfer Object
/// </summary>
public class SaleDto
{
    public int SaleID { get; set; }
    public string SaleNumber { get; set; } = string.Empty;
    public int? CustomerID { get; set; }
    public string? CustomerName { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AmountPaid { get; set; }
    public decimal ChangeGiven { get; set; }
    public TransactionStatus Status { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public int CashierID { get; set; }
    public string? CashierName { get; set; }
    public string? Notes { get; set; }
    
    // Related items
    public List<SaleItemDto> SaleItems { get; set; } = new();
    public List<PaymentDto> Payments { get; set; } = new();
}
