namespace POSApplication.Core.DTOs;

/// <summary>
/// Shopping cart item DTO with calculated totals
/// </summary>
public class CartItemDto
{
    public int ProductID { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxRate { get; set; }
    public decimal DiscountAmount { get; set; }
    
    // Calculated properties
    public decimal Subtotal => Quantity * UnitPrice;
    public decimal TaxAmount => Subtotal * (TaxRate / 100);
    public decimal LineTotal => Subtotal + TaxAmount - DiscountAmount;
}
