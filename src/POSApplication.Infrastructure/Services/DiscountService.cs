using POSApplication.Common.Enums;
using POSApplication.Core.DTOs;
using POSApplication.Core.Entities;
using POSApplication.Core.Interfaces;
using POSApplication.Data.Interfaces;

namespace POSApplication.Infrastructure.Services;

public class DiscountService : IDiscountService
{
    private readonly IRepository<Discount> _discountRepository;

    public DiscountService(IRepository<Discount> discountRepository)
    {
        _discountRepository = discountRepository;
    }

    public async Task<IEnumerable<DiscountDto>> GetAllDiscountsAsync()
    {
        var discounts = await _discountRepository.GetAllAsync();
        return discounts.OrderByDescending(d => d.IsActive).ThenBy(d => d.Code).Select(MapToDto);
    }

    public async Task<DiscountDto?> GetDiscountByIdAsync(int id)
    {
        var discount = await _discountRepository.GetByIdAsync(id);
        return discount != null ? MapToDto(discount) : null;
    }

    public async Task<DiscountDto?> GetValidDiscountByCodeAsync(string code, decimal purchaseAmount)
    {
        var discounts = await _discountRepository.FindAsync(d =>
            d.Code == code && d.IsActive &&
            d.StartDate <= DateTime.Now && d.EndDate >= DateTime.Now);

        var discount = discounts.FirstOrDefault();
        if (discount == null) return null;

        if (discount.UsageLimit.HasValue && discount.UsageCount >= discount.UsageLimit.Value)
            return null;

        if (discount.MinPurchaseAmount.HasValue && purchaseAmount < discount.MinPurchaseAmount.Value)
            return null;

        return MapToDto(discount);
    }

    public async Task<Decimal> CalculateDiscountAsync(string code, decimal subtotal)
    {
        var discount = await GetValidDiscountByCodeAsync(code, subtotal);
        if (discount == null) return 0;

        decimal discountAmount;
        if (discount.Type == "Percentage")
        {
            discountAmount = subtotal * (discount.Value / 100);
            if (discount.MaxDiscountAmount.HasValue)
                discountAmount = Math.Min(discountAmount, discount.MaxDiscountAmount.Value);
        }
        else
        {
            discountAmount = Math.Min(discount.Value, subtotal);
        }

        return Math.Round(discountAmount, 2);
    }

    public async Task<DiscountDto> CreateDiscountAsync(DiscountDto discountDto)
    {
        if (string.IsNullOrWhiteSpace(discountDto.Code))
            throw new ArgumentException("Discount code is required");

        var existing = await _discountRepository.FindAsync(d => d.Code == discountDto.Code);
        if (existing.Any())
            throw new InvalidOperationException($"Discount code '{discountDto.Code}' already exists");

        var discount = new Discount
        {
            Code = discountDto.Code.ToUpper(),
            Description = discountDto.Description,
            Type = Enum.Parse<DiscountType>(discountDto.Type),
            Value = discountDto.Value,
            MaxDiscountAmount = discountDto.MaxDiscountAmount,
            MinPurchaseAmount = discountDto.MinPurchaseAmount,
            UsageLimit = discountDto.UsageLimit,
            StartDate = discountDto.StartDate,
            EndDate = discountDto.EndDate,
            IsActive = discountDto.IsActive,
            CreatedAt = DateTime.Now
        };

        await _discountRepository.AddAsync(discount);
        await _discountRepository.SaveChangesAsync();

        return MapToDto(discount);
    }

    public async Task<DiscountDto> UpdateDiscountAsync(DiscountDto discountDto)
    {
        var existing = await _discountRepository.GetByIdAsync(discountDto.DiscountID);
        if (existing == null)
            throw new InvalidOperationException($"Discount with ID {discountDto.DiscountID} not found");

        if (string.IsNullOrWhiteSpace(discountDto.Code))
            throw new ArgumentException("Discount code is required");

        var duplicate = await _discountRepository.FindAsync(d => d.Code == discountDto.Code && d.DiscountID != discountDto.DiscountID);
        if (duplicate.Any())
            throw new InvalidOperationException($"Discount code '{discountDto.Code}' already exists");

        existing.Code = discountDto.Code.ToUpper();
        existing.Description = discountDto.Description;
        existing.Type = Enum.Parse<DiscountType>(discountDto.Type);
        existing.Value = discountDto.Value;
        existing.MaxDiscountAmount = discountDto.MaxDiscountAmount;
        existing.MinPurchaseAmount = discountDto.MinPurchaseAmount;
        existing.UsageLimit = discountDto.UsageLimit;
        existing.StartDate = discountDto.StartDate;
        existing.EndDate = discountDto.EndDate;
        existing.IsActive = discountDto.IsActive;

        await _discountRepository.UpdateAsync(existing);
        await _discountRepository.SaveChangesAsync();

        return MapToDto(existing);
    }

    public async Task<bool> DeleteDiscountAsync(int id)
    {
        var discount = await _discountRepository.GetByIdAsync(id);
        if (discount == null) return false;

        discount.IsActive = false;
        await _discountRepository.UpdateAsync(discount);
        await _discountRepository.SaveChangesAsync();
        return true;
    }

    private DiscountDto MapToDto(Discount discount)
    {
        return new DiscountDto
        {
            DiscountID = discount.DiscountID,
            Code = discount.Code,
            Description = discount.Description,
            Type = discount.Type.ToString(),
            Value = discount.Value,
            MaxDiscountAmount = discount.MaxDiscountAmount,
            MinPurchaseAmount = discount.MinPurchaseAmount,
            UsageLimit = discount.UsageLimit,
            UsageCount = discount.UsageCount,
            StartDate = discount.StartDate,
            EndDate = discount.EndDate,
            IsActive = discount.IsActive
        };
    }
}
