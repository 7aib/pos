namespace POSApplication.Data.Entities;

public class SaleItem
{
    public int SaleItemID { get; set; }
    public int SaleID { get; set; }
    public int ProductID { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TaxRate { get; set; } = 0;
    public decimal DiscountAmount { get; set; } = 0;
    public decimal LineTotal { get; set; }
    public bool IsReturned { get; set; } = false;
    public decimal ReturnedQuantity { get; set; } = 0;
    
    // Navigation properties
    public Sale Sale { get; set; } = null!;
    public Product Product { get; set; } = null!;
}
