using POSApplication.Core.DTOs;

namespace POSApplication.Core.Interfaces;

public interface IDiscountService
{
    Task<IEnumerable<DiscountDto>> GetAllDiscountsAsync();
    Task<DiscountDto?> GetDiscountByIdAsync(int id);
    Task<DiscountDto?> GetValidDiscountByCodeAsync(string code, decimal purchaseAmount);
    Task<DiscountDto> CreateDiscountAsync(DiscountDto discountDto);
    Task<DiscountDto> UpdateDiscountAsync(DiscountDto discountDto);
    Task<bool> DeleteDiscountAsync(int id);
    Task<Decimal> CalculateDiscountAsync(string code, decimal subtotal);
}
