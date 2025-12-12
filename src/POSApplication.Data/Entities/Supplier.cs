namespace POSApplication.Data.Entities;

public class Supplier
{
    public int SupplierID { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    // Navigation properties
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
