using POSApplication.Core.DTOs;
using POSApplication.Core.Interfaces;
using POSApplication.Core.Entities;
using POSApplication.Data.Interfaces;

namespace POSApplication.Infrastructure.Services;

/// <summary>
/// Product management service implementation
/// </summary>
public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IRepository<StockAdjustment> _stockAdjustmentRepository;

    public ProductService(
        IProductRepository productRepository,
        IRepository<StockAdjustment> stockAdjustmentRepository)
    {
        _productRepository = productRepository;
        _stockAdjustmentRepository = stockAdjustmentRepository;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _productRepository.GetAllAsync();
        return products.Select(MapToDto);
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetProductWithDetailsAsync(id);
        return product != null ? MapToDto(product) : null;
    }

    public async Task<IEnumerable<ProductDto>> SearchProductsAsync(string searchTerm)
    {
        var products = await _productRepository.SearchProductsAsync(searchTerm);
        return products.Select(MapToDto);
    }

    public async Task<ProductDto?> GetProductByBarcodeAsync(string barcode)
    {
        var product = await _productRepository.GetByBarcodeAsync(barcode);
        return product != null ? MapToDto(product) : null;
    }

    public async Task<ProductDto?> GetProductBySKUAsync(string sku)
    {
        var product = await _productRepository.GetBySKUAsync(sku);
        return product != null ? MapToDto(product) : null;
    }

    public async Task<IEnumerable<ProductDto>> GetLowStockProductsAsync()
    {
        var products = await _productRepository.GetLowStockProductsAsync();
        return products.Select(MapToDto);
    }

    public async Task<ProductDto> CreateProductAsync(ProductDto productDto)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(productDto.ProductName))
            throw new ArgumentException("Product name is required");

        if (string.IsNullOrWhiteSpace(productDto.SKU))
            throw new ArgumentException("SKU is required");

        if (productDto.SellPrice <= 0)
            throw new ArgumentException("Sell price must be greater than zero");

        // Check SKU uniqueness
        var existingProduct = await _productRepository.GetBySKUAsync(productDto.SKU);
        if (existingProduct != null)
            throw new InvalidOperationException($"Product with SKU '{productDto.SKU}' already exists");

        // Check barcode uniqueness if provided
        if (!string.IsNullOrWhiteSpace(productDto.Barcode))
        {
            var existingByBarcode = await _productRepository.GetByBarcodeAsync(productDto.Barcode);
            if (existingByBarcode != null)
                throw new InvalidOperationException($"Product with barcode '{productDto.Barcode}' already exists");
        }

        var product = MapToEntity(productDto);
        product.CreatedAt = DateTime.Now;
        product.UpdatedAt = DateTime.Now;

        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();

        return MapToDto(product);
    }

    public async Task<ProductDto> UpdateProductAsync(ProductDto productDto)
    {
        var existingProduct = await _productRepository.GetByIdAsync(productDto.ProductID);
        if (existingProduct == null)
            throw new InvalidOperationException($"Product with ID {productDto.ProductID} not found");

        // Validation
        if (string.IsNullOrWhiteSpace(productDto.ProductName))
            throw new ArgumentException("Product name is required");

        if (productDto.SellPrice <= 0)
            throw new ArgumentException("Sell price must be greater than zero");

        // Check SKU uniqueness (excluding current product)
        if (existingProduct.SKU != productDto.SKU)
        {
            var skuExists = await _productRepository.GetBySKUAsync(productDto.SKU);
            if (skuExists != null)
                throw new InvalidOperationException($"Product with SKU '{productDto.SKU}' already exists");
        }

        // Check barcode uniqueness (excluding current product)
        if (!string.IsNullOrWhiteSpace(productDto.Barcode) && existingProduct.Barcode != productDto.Barcode)
        {
            var barcodeExists = await _productRepository.GetByBarcodeAsync(productDto.Barcode);
            if (barcodeExists != null)
                throw new InvalidOperationException($"Product with barcode '{productDto.Barcode}' already exists");
        }

        // Update properties
        existingProduct.SKU = productDto.SKU;
        existingProduct.Barcode = productDto.Barcode;
        existingProduct.ProductName = productDto.ProductName;
        existingProduct.Description = productDto.Description;
        existingProduct.CategoryID = productDto.CategoryID;
        existingProduct.CostPrice = productDto.CostPrice;
        existingProduct.SellPrice = productDto.SellPrice;
        existingProduct.TaxRate = productDto.TaxRate;
        existingProduct.CurrentStock = productDto.CurrentStock;
        existingProduct.MinStockLevel = productDto.MinStockLevel;
        existingProduct.MaxStockLevel = productDto.MaxStockLevel;
        existingProduct.ReorderPoint = productDto.ReorderPoint;
        existingProduct.UnitOfMeasure = productDto.UnitOfMeasure;
        existingProduct.SupplierID = productDto.SupplierID;
        existingProduct.ImagePath = productDto.ImagePath;
        existingProduct.ExpiryDate = productDto.ExpiryDate;
        existingProduct.IsActive = productDto.IsActive;
        existingProduct.UpdatedAt = DateTime.Now;

        await _productRepository.UpdateAsync(existingProduct);
        await _productRepository.SaveChangesAsync();

        return MapToDto(existingProduct);
    }

    public async Task<bool> DeleteProductAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return false;

        // Soft delete
        product.IsActive = false;
        product.UpdatedAt = DateTime.Now;
        
        await _productRepository.UpdateAsync(product);
        await _productRepository.SaveChangesAsync();

        return true;
    }

    public async Task UpdateStockAsync(int productId, int quantityChange, string reason)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            throw new InvalidOperationException($"Product with ID {productId} not found");

        product.CurrentStock += quantityChange;
        product.UpdatedAt = DateTime.Now;

        await _productRepository.UpdateAsync(product);
        await _productRepository.SaveChangesAsync();
    }

    private ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            ProductID = product.ProductID,
            SKU = product.SKU,
            Barcode = product.Barcode,
            ProductName = product.ProductName,
            Description = product.Description,
            CategoryID = product.CategoryID,
            CategoryName = product.Category?.CategoryName,
            CostPrice = product.CostPrice,
            SellPrice = product.SellPrice,
            TaxRate = product.TaxRate,
            CurrentStock = product.CurrentStock,
            MinStockLevel = product.MinStockLevel,
            MaxStockLevel = product.MaxStockLevel,
            ReorderPoint = product.ReorderPoint,
            UnitOfMeasure = product.UnitOfMeasure,
            SupplierID = product.SupplierID,
            SupplierName = product.Supplier?.SupplierName,
            ImagePath = product.ImagePath,
            ExpiryDate = product.ExpiryDate,
            IsActive = product.IsActive
        };
    }

    private Product MapToEntity(ProductDto dto)
    {
        return new Product
        {
            ProductID = dto.ProductID,
            SKU = dto.SKU,
            Barcode = dto.Barcode,
            ProductName = dto.ProductName,
            Description = dto.Description,
            CategoryID = dto.CategoryID,
            CostPrice = dto.CostPrice,
            SellPrice = dto.SellPrice,
            TaxRate = dto.TaxRate,
            CurrentStock = dto.CurrentStock,
            MinStockLevel = dto.MinStockLevel,
            MaxStockLevel = dto.MaxStockLevel,
            ReorderPoint = dto.ReorderPoint,
            UnitOfMeasure = dto.UnitOfMeasure,
            SupplierID = dto.SupplierID,
            ImagePath = dto.ImagePath,
            ExpiryDate = dto.ExpiryDate,
            IsActive = dto.IsActive
        };
    }
}
