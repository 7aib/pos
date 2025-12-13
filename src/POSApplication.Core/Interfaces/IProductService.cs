using POSApplication.Core.DTOs;

namespace POSApplication.Core.Interfaces;

/// <summary>
/// Product management service interface
/// </summary>
public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm);
    Task<ProductDto?> GetProductByBarcodeAsync(string barcode);
    Task<ProductDto?> GetProductBySKUAsync(string sku);
    Task<IEnumerable<ProductDto>> GetLowStockProductsAsync();
    Task<ProductDto> CreateProductAsync(ProductDto product);
    Task<ProductDto> UpdateProductAsync(ProductDto product);
    Task<bool> DeleteProductAsync(int id);
    Task UpdateStockAsync(int productId, int quantityChange, string reason);
}
