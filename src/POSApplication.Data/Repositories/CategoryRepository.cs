using Microsoft.EntityFrameworkCore;
using POSApplication.Data.Context;
using POSApplication.Core.Entities;
using POSApplication.Data.Interfaces;

namespace POSApplication.Data.Repositories;

/// <summary>
/// Category repository implementation
/// </summary>
public class CategoryRepository : Repository<Category>, ICategoryRepository
{
    public CategoryRepository(POSDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
    {
        return await _dbSet
            .Where(c => c.IsActive)
            .OrderBy(c => c.CategoryName)
            .ToListAsync();
    }
}
