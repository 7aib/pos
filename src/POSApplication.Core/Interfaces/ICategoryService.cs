using POSApplication.Core.DTOs;

namespace POSApplication.Core.Interfaces;

public interface ICategoryService
{
    Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync();
    Task<IEnumerable<CategoryDto>> GetActiveCategoriesAsync();
    Task<CategoryDto?> GetCategoryByIdAsync(int id);
    Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto);
    Task<CategoryDto> UpdateCategoryAsync(CategoryDto categoryDto);
    Task<bool> DeleteCategoryAsync(int id);
}
