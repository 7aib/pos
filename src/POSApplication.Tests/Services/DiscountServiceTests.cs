using Moq;
using POSApplication.Common.Enums;
using POSApplication.Core.DTOs;
using POSApplication.Core.Entities;
using POSApplication.Core.Interfaces;
using POSApplication.Data.Interfaces;
using POSApplication.Infrastructure.Services;

namespace POSApplication.Tests.Services;

public class DiscountServiceTests
{
    private readonly Mock<IRepository<Discount>> _discountRepoMock;
    private readonly DiscountService _service;

    public DiscountServiceTests()
    {
        _discountRepoMock = new Mock<IRepository<Discount>>();
        _service = new DiscountService(_discountRepoMock.Object);
    }

    [Fact]
    public async Task CalculateDiscount_Percentage_ReturnsCorrectAmount()
    {
        var discount = new Discount
        {
            DiscountID = 1,
            Code = "SAVE10",
            Type = DiscountType.Percentage,
            Value = 10,
            IsActive = true,
            StartDate = DateTime.Now.AddDays(-1),
            EndDate = DateTime.Now.AddDays(1)
        };
        _discountRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Discount, bool>>>()))
            .ReturnsAsync(new List<Discount> { discount });

        var result = await _service.CalculateDiscountAsync("SAVE10", 100);

        Assert.Equal(10, result);
    }

    [Fact]
    public async Task CalculateDiscount_FixedAmount_ReturnsCorrectAmount()
    {
        var discount = new Discount
        {
            DiscountID = 2,
            Code = "SAVE5",
            Type = DiscountType.FixedAmount,
            Value = 5,
            IsActive = true,
            StartDate = DateTime.Now.AddDays(-1),
            EndDate = DateTime.Now.AddDays(1)
        };
        _discountRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Discount, bool>>>()))
            .ReturnsAsync(new List<Discount> { discount });

        var result = await _service.CalculateDiscountAsync("SAVE5", 100);

        Assert.Equal(5, result);
    }

    [Fact]
    public async Task CalculateDiscount_ExpiredCode_ReturnsZero()
    {
        var discount = new Discount
        {
            DiscountID = 3,
            Code = "OLD",
            Type = DiscountType.Percentage,
            Value = 20,
            IsActive = true,
            StartDate = DateTime.Now.AddDays(-10),
            EndDate = DateTime.Now.AddDays(-1)
        };
        _discountRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Discount, bool>>>()))
            .ReturnsAsync(new List<Discount>());

        var result = await _service.CalculateDiscountAsync("OLD", 100);

        Assert.Equal(0, result);
    }

    [Fact]
    public async Task CalculateDiscount_MaxDiscountApplied()
    {
        var discount = new Discount
        {
            DiscountID = 4,
            Code = "MAX20",
            Type = DiscountType.Percentage,
            Value = 50,
            MaxDiscountAmount = 20,
            IsActive = true,
            StartDate = DateTime.Now.AddDays(-1),
            EndDate = DateTime.Now.AddDays(1)
        };
        _discountRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Discount, bool>>>()))
            .ReturnsAsync(new List<Discount> { discount });

        var result = await _service.CalculateDiscountAsync("MAX20", 100);

        Assert.Equal(20, result);
    }

    [Fact]
    public async Task CreateDiscount_EmptyCode_ThrowsArgumentException()
    {
        var dto = new DiscountDto { Code = "", Value = 10 };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateDiscountAsync(dto));
    }

    [Fact]
    public async Task CreateDiscount_DuplicateCode_ThrowsInvalidOperationException()
    {
        var dto = new DiscountDto { Code = "SAVE10", Value = 10 };
        _discountRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Discount, bool>>>()))
            .ReturnsAsync(new List<Discount> { new Discount { Code = "SAVE10" } });

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateDiscountAsync(dto));
    }
}
