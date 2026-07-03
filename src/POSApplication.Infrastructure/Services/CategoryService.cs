using POSApplication.Core.DTOs;
using POSApplication.Core.Entities;
using POSApplication.Core.Interfaces;
using POSApplication.Data.Interfaces;

namespace POSApplication.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IRepository<Product> _productRepository;

    public CategoryService(ICategoryRepository categoryRepository, IRepository<Product> productRepository)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
    {
        var categories = await _categoryRepository.GetAllAsync();
        return categories.OrderByDescending(c => c.IsActive).ThenBy(c => c.CategoryName).Select(MapToDto);
    }

    public async Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync()
    {
        var categories = await _categoryRepository.GetActiveCategoriesAsync();
        return categories.Select(MapToDto);
    }

    public async Task<CategoryDto?> GetCategoryByIdAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        return category != null ? MapToDto(category) : null;
    }

    public async Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto)
    {
        if (string.IsNullOrWhiteSpace(categoryDto.CategoryName))
            throw new ArgumentException("Category name is required");

        var existing = await _categoryRepository.FindAsync(c => c.CategoryName == categoryDto.CategoryName);
        if (existing.Any())
            throw new InvalidOperationException($"Category '{categoryDto.CategoryName}' already exists");

        var category = new Category
        {
            CategoryName = categoryDto.CategoryName,
            ParentCategoryID = categoryDto.ParentCategoryID,
            Description = categoryDto.Description,
            IsActive = categoryDto.IsActive
        };

        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();

        return MapToDto(category);
    }

    public async Task<CategoryDto> UpdateCategoryAsync(CategoryDto categoryDto)
    {
        var existing = await _categoryRepository.GetByIdAsync(categoryDto.CategoryID);
        if (existing == null)
            throw new InvalidOperationException($"Category with ID {categoryDto.CategoryID} not found");

        if (string.IsNullOrWhiteSpace(categoryDto.CategoryName))
            throw new ArgumentException("Category name is required");

        var duplicate = await _categoryRepository.FindAsync(c => c.CategoryName == categoryDto.CategoryName && c.CategoryID != categoryDto.CategoryID);
        if (duplicate.Any())
            throw new InvalidOperationException($"Category '{categoryDto.CategoryName}' already exists");

        existing.CategoryName = categoryDto.CategoryName;
        existing.ParentCategoryID = categoryDto.ParentCategoryID;
        existing.Description = categoryDto.Description;
        existing.IsActive = categoryDto.IsActive;

        await _categoryRepository.UpdateAsync(existing);
        await _categoryRepository.SaveChangesAsync();

        return MapToDto(existing);
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);
        if (category == null)
            return false;

        var products = await _productRepository.FindAsync(p => p.CategoryID == id && p.IsActive);
        if (products.Any())
            throw new InvalidOperationException("Cannot delete category with active products. Remove or reassign products first.");

        category.IsActive = false;
        await _categoryRepository.UpdateAsync(category);
        await _categoryRepository.SaveChangesAsync();

        return true;
    }

    private CategoryDto MapToDto(Category category)
    {
        return new CategoryDto
        {
            CategoryID = category.CategoryID,
            CategoryName = category.CategoryName,
            ParentCategoryID = category.ParentCategoryID,
            ParentCategoryName = category.ParentCategory?.CategoryName,
            Description = category.Description,
            IsActive = category.IsActive,
            ProductCount = category.Products?.Count(p => p.IsActive) ?? 0
        };
    }
}
