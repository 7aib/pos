namespace POSApplication.Core.DTOs;

/// <summary>
/// Sale item DTO
/// </summary>
public class SaleItemDto
{
    public int SaleItemID { get; set; }
    public int SaleID { get; set; }
    public int ProductID { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxRate { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal LineTotal { get; set; }
}
