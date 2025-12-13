using Microsoft.EntityFrameworkCore;
using POSApplication.Data.Context;
using POSApplication.Core.Entities;
using POSApplication.Data.Interfaces;

namespace POSApplication.Data.Repositories;

/// <summary>
/// Product repository implementation
/// </summary>
public class ProductRepository : Repository<Product>, IProductRepository
{
    public ProductRepository(POSDbContext context) : base(context)
    {
    }

    public async Task<Product?> GetByBarcodeAsync(string barcode)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.Barcode == barcode);
    }

    public async Task<Product?> GetBySKUAsync(string sku)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.SKU == sku);
    }

    public async Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await _dbSet
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Where(p => p.IsActive)
                .OrderBy(p => p.ProductName)
                .ToListAsync();
        }

        var lowerSearchTerm = searchTerm.ToLower();
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Where(p => p.IsActive && (
                p.ProductName.ToLower().Contains(lowerSearchTerm) ||
                p.SKU.ToLower().Contains(lowerSearchTerm) ||
                (p.Barcode != null && p.Barcode.ToLower().Contains(lowerSearchTerm))
            ))
            .OrderBy(p => p.ProductName)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetLowStockProductsAsync()
    {
        return await _dbSet
            .Include(p => p.Category)
            .Where(p => p.IsActive && p.CurrentStock <= p.MinStockLevel)
            .OrderBy(p => p.CurrentStock)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Where(p => p.CategoryID == categoryId && p.IsActive)
            .OrderBy(p => p.ProductName)
            .ToListAsync();
    }

    public async Task<Product?> GetProductWithDetailsAsync(int productId)
    {
        return await _dbSet
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .FirstOrDefaultAsync(p => p.ProductID == productId);
    }
}
