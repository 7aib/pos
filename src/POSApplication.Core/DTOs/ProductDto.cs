namespace POSApplication.Core.DTOs;

/// <summary>
/// Product Data Transfer Object
/// </summary>
public class ProductDto
{
    public int ProductID { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? CategoryID { get; set; }
    public string? CategoryName { get; set; }
    public decimal? CostPrice { get; set; }
    public decimal SellPrice { get; set; }
    public decimal TaxRate { get; set; }
    public int CurrentStock { get; set; }
    public int MinStockLevel { get; set; }
    public int? MaxStockLevel { get; set; }
    public int? ReorderPoint { get; set; }
    public string? UnitOfMeasure { get; set; }
    public int? SupplierID { get; set; }
    public string? SupplierName { get; set; }
    public string? ImagePath { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public bool IsActive { get; set; } = true;
}
