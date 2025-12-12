using POSApplication.Common.Enums;

namespace POSApplication.Data.Entities;

public class StockAdjustment
{
    public int AdjustmentID { get; set; }
    public int ProductID { get; set; }
    public StockAdjustmentType AdjustmentType { get; set; }
    public int Quantity { get; set; } // Can be positive or negative
    public string? Reason { get; set; }
    public string? Notes { get; set; }
    public int AdjustedBy { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    // Navigation properties
    public Product Product { get; set; } = null!;
    public User AdjustedByUser { get; set; } = null!;
}
