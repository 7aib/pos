using Microsoft.EntityFrameworkCore;
using POSApplication.Data.Context;
using POSApplication.Data.Entities;
using POSApplication.Data.Interfaces;

namespace POSApplication.Data.Repositories;

/// <summary>
/// Customer repository implementation
/// </summary>
public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(POSDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return await _dbSet
                .Where(c => c.IsActive)
                .OrderBy(c => c.FirstName)
                .ThenBy(c => c.LastName)
                .ToListAsync();
        }

        var lowerSearchTerm = searchTerm.ToLower();
        return await _dbSet
            .Where(c => c.IsActive && (
                c.FirstName.ToLower().Contains(lowerSearchTerm) ||
                c.LastName.ToLower().Contains(lowerSearchTerm) ||
                (c.Phone != null && c.Phone.Contains(searchTerm)) ||
                (c.Email != null && c.Email.ToLower().Contains(lowerSearchTerm))
            ))
            .OrderBy(c => c.FirstName)
            .ThenBy(c => c.LastName)
            .ToListAsync();
    }

    public async Task<Customer?> GetCustomerWithCreditAccountAsync(int customerId)
    {
        return await _dbSet
            .Include(c => c.CreditAccount)
            .FirstOrDefaultAsync(c => c.CustomerID == customerId);
    }
}
