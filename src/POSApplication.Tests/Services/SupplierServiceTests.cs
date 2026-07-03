using Moq;
using POSApplication.Core.DTOs;
using POSApplication.Core.Entities;
using POSApplication.Core.Interfaces;
using POSApplication.Data.Interfaces;
using POSApplication.Infrastructure.Services;

namespace POSApplication.Tests.Services;

public class SupplierServiceTests
{
    private readonly Mock<IRepository<Supplier>> _supplierRepoMock;
    private readonly Mock<IRepository<Product>> _productRepoMock;
    private readonly SupplierService _service;

    public SupplierServiceTests()
    {
        _supplierRepoMock = new Mock<IRepository<Supplier>>();
        _productRepoMock = new Mock<IRepository<Product>>();
        _service = new SupplierService(_supplierRepoMock.Object, _productRepoMock.Object);
    }

    [Fact]
    public async Task CreateSupplier_ValidInput_ReturnsDto()
    {
        var dto = new SupplierDto { SupplierName = "Test Supplier", IsActive = true };
        _supplierRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Supplier, bool>>>()))
            .ReturnsAsync(new List<Supplier>());

        var result = await _service.CreateSupplierAsync(dto);

        Assert.Equal("Test Supplier", result.SupplierName);
        _supplierRepoMock.Verify(r => r.AddAsync(It.IsAny<Supplier>()), Times.Once);
    }

    [Fact]
    public async Task CreateSupplier_EmptyName_ThrowsArgumentException()
    {
        var dto = new SupplierDto { SupplierName = "" };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateSupplierAsync(dto));
    }

    [Fact]
    public async Task SearchSuppliers_MatchingTerm_ReturnsResults()
    {
        var suppliers = new List<Supplier>
        {
            new Supplier { SupplierID = 1, SupplierName = "Tech Distributors" },
            new Supplier { SupplierID = 2, SupplierName = "Food Wholesalers" }
        };
        _supplierRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Supplier, bool>>>()))
            .ReturnsAsync((System.Linq.Expressions.Expression<Func<Supplier, bool>> pred) =>
                suppliers.Where(pred.Compile()).ToList());

        var results = await _service.SearchSuppliersAsync("Tech");

        Assert.Single(results);
        Assert.Equal("Tech Distributors", results.First().SupplierName);
    }
}
