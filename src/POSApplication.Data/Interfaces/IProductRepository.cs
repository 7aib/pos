using POSApplication.Data.Entities;

namespace POSApplication.Data.Interfaces;

/// <summary>
/// Product-specific repository interface
/// </summary>
public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetByBarcodeAsync(string barcode);
    Task<Product?> GetBySKUAsync(string sku);
    Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
    Task<IEnumerable<Product>> GetLowStockProductsAsync();
    Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
    Task<Product?> GetProductWithDetailsAsync(int productId);
}
