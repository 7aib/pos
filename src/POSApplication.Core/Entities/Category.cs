namespace POSApplication.Core.Entities;

public class Category
{
    public int CategoryID { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int? ParentCategoryID { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public Category? ParentCategory { get; set; }
    public ICollection<Category> SubCategories { get; set; } = new List<Category>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
