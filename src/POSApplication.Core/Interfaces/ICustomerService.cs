using POSApplication.Core.DTOs;

namespace POSApplication.Core.Interfaces;

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
    Task<CustomerDto?> GetCustomerByIdAsync(int id);
    Task<CustomerDto?> GetCustomerByPhoneAsync(string phone);
    Task<IEnumerable<CustomerDto>> SearchCustomersAsync(string searchTerm);
    Task<CustomerDto> CreateCustomerAsync(CustomerDto customerDto);
    Task<CustomerDto> UpdateCustomerAsync(CustomerDto customerDto);
    Task<bool> DeleteCustomerAsync(int id);
}
