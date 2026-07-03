namespace POSApplication.Core.DTOs;

public class CategoryDto
{
    public int CategoryID { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int? ParentCategoryID { get; set; }
    public string? ParentCategoryName { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public int ProductCount { get; set; }
}
