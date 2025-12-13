namespace POSApplication.Core.Interfaces;

/// <summary>
/// Inventory management service interface
/// </summary>
public interface IInventoryService
{
    Task<bool> CheckStockAvailabilityAsync(int productId, decimal quantity);
    Task DeductStockAsync(int productId, int quantity, int saleId);
}
