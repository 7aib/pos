namespace POSApplication.Data.Entities;

public class Product
{
    public int ProductID { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? CategoryID { get; set; }
    public decimal? CostPrice { get; set; }
    public decimal SellPrice { get; set; }
    public decimal TaxRate { get; set; } = 0;
    public int CurrentStock { get; set; } = 0;
    public int MinStockLevel { get; set; } = 0;
    public int? MaxStockLevel { get; set; }
    public int? ReorderPoint { get; set; }
    public string? UnitOfMeasure { get; set; }
    public int? SupplierID { get; set; }
    public string? ImagePath { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    
    // Navigation properties
    public Category? Category { get; set; }
    public Supplier? Supplier { get; set; }
    public ICollection<SaleItem> SaleItems { get; set; } = new List<SaleItem>();
    public ICollection<StockAdjustment> StockAdjustments { get; set; } = new List<StockAdjustment>();
}
