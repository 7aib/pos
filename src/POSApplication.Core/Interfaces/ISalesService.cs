using POSApplication.Core.DTOs;

namespace POSApplication.Core.Interfaces;

/// <summary>
/// Sales transaction service interface
/// </summary>
public interface ISalesService
{
    Task<SaleDto> CreateSaleAsync(SaleDto sale);
    Task<SaleDto?> GetSaleByIdAsync(int saleId);
    Task<bool> VoidSaleAsync(int saleId, int userId, string reason);
    Task<(decimal Subtotal, decimal TaxAmount, decimal Total)> CalculateSaleTotalsAsync(
        List<CartItemDto> items, decimal discountAmount);
}
