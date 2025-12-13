using POSApplication.Core.Entities;

namespace POSApplication.Data.Interfaces;

/// <summary>
/// Category repository interface
/// </summary>
public interface ICategoryRepository : IRepository<Category>
{
    Task<IEnumerable<Category>> GetActiveCategoriesAsync();
}
