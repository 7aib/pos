using POSApplication.Core.Entities;

namespace POSApplication.Data.Interfaces;

/// <summary>
/// Customer repository interface
/// </summary>
public interface ICustomerRepository : IRepository<Customer>
{
    Task<IEnumerable<Customer>> SearchCustomersAsync(string searchTerm);
    Task<Customer?> GetCustomerWithCreditAccountAsync(int customerId);
}
