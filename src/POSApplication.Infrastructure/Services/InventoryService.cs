using POSApplication.Common.Enums;
using POSApplication.Core.Interfaces;
using POSApplication.Core.Entities;
using POSApplication.Data.Interfaces;

namespace POSApplication.Infrastructure.Services;

/// <summary>
/// Inventory management service implementation
/// </summary>
public class InventoryService : IInventoryService
{
    private readonly IProductRepository _productRepository;
    private readonly IRepository<StockAdjustment> _stockAdjustmentRepository;

    public InventoryService(
        IProductRepository productRepository,
        IRepository<StockAdjustment> stockAdjustmentRepository)
    {
        _productRepository = productRepository;
        _stockAdjustmentRepository = stockAdjustmentRepository;
    }

    public async Task<bool> CheckStockAvailabilityAsync(int productId, decimal quantity)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            return false;

        return product.CurrentStock >= quantity;
    }

    public async Task DeductStockAsync(int productId, int quantity, int saleId)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            throw new InvalidOperationException($"Product with ID {productId} not found");

        // Update stock
        product.CurrentStock -= quantity;
        product.UpdatedAt = DateTime.Now;

        // Create stock adjustment record
        var adjustment = new StockAdjustment
        {
            ProductID = productId,
            AdjustmentType = quantity > 0 ? StockAdjustmentType.StockOut : StockAdjustmentType.StockIn,
            Quantity = -quantity, // Negative for deduction
            Reason = $"Sale transaction (Sale ID: {saleId})",
            AdjustedBy = 1, // TODO: Get current user ID
            CreatedAt = DateTime.Now
        };

        await _productRepository.UpdateAsync(product);
        await _stockAdjustmentRepository.AddAsync(adjustment);
        await _productRepository.SaveChangesAsync();
    }
}
