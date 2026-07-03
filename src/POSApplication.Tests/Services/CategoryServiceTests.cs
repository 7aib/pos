using Moq;
using POSApplication.Core.DTOs;
using POSApplication.Core.Entities;
using POSApplication.Core.Interfaces;
using POSApplication.Data.Interfaces;
using POSApplication.Infrastructure.Services;

namespace POSApplication.Tests.Services;

public class CategoryServiceTests
{
    private readonly Mock<ICategoryRepository> _categoryRepoMock;
    private readonly Mock<IRepository<Product>> _productRepoMock;
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _categoryRepoMock = new Mock<ICategoryRepository>();
        _productRepoMock = new Mock<IRepository<Product>>();
        _service = new CategoryService(_categoryRepoMock.Object, _productRepoMock.Object);
    }

    [Fact]
    public async Task CreateCategory_ValidInput_ReturnsDto()
    {
        var dto = new CategoryDto { CategoryName = "Test Category", IsActive = true };
        _categoryRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Category, bool>>>()))
            .ReturnsAsync(new List<Category>());

        var result = await _service.CreateCategoryAsync(dto);

        Assert.Equal("Test Category", result.CategoryName);
        _categoryRepoMock.Verify(r => r.AddAsync(It.IsAny<Category>()), Times.Once);
    }

    [Fact]
    public async Task CreateCategory_EmptyName_ThrowsArgumentException()
    {
        var dto = new CategoryDto { CategoryName = "" };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateCategoryAsync(dto));
    }

    [Fact]
    public async Task CreateCategory_DuplicateName_ThrowsInvalidOperationException()
    {
        var dto = new CategoryDto { CategoryName = "Electronics" };
        _categoryRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Category, bool>>>()))
            .ReturnsAsync(new List<Category> { new Category { CategoryName = "Electronics" } });

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateCategoryAsync(dto));
    }

    [Fact]
    public async Task DeleteCategory_WithActiveProducts_ThrowsInvalidOperationException()
    {
        _categoryRepoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Category { CategoryID = 1, CategoryName = "Test" });
        _productRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>()))
            .ReturnsAsync(new List<Product> { new Product { ProductID = 1 } });

        await Assert.ThrowsAsync<InvalidOperationException>(() => _service.DeleteCategoryAsync(1));
    }

    [Fact]
    public async Task DeleteCategory_NoProducts_ReturnsTrue()
    {
        _categoryRepoMock.Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(new Category { CategoryID = 1, CategoryName = "Test" });
        _productRepoMock.Setup(r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>()))
            .ReturnsAsync(new List<Product>());

        var result = await _service.DeleteCategoryAsync(1);

        Assert.True(result);
    }
}
