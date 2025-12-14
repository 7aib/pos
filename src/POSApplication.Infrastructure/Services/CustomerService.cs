using POSApplication.Core.DTOs;
using POSApplication.Core.Entities;
using POSApplication.Core.Interfaces;
using POSApplication.Data.Interfaces;

namespace POSApplication.Infrastructure.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;

    public CustomerService(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
    {
        var customers = await _customerRepository.GetAllAsync();
        return customers.Select(MapToDto);
    }

    public async Task<CustomerDto?> GetCustomerByIdAsync(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        return customer != null ? MapToDto(customer) : null;
    }

    public async Task<CustomerDto?> GetCustomerByPhoneAsync(string phone)
    {
        // This is efficient enough for now, but a dedicated repository method would be better for large datasets
        var allCustomers = await _customerRepository.GetAllAsync();
        var customer = allCustomers.FirstOrDefault(c => c.Phone == phone);
        return customer != null ? MapToDto(customer) : null;
    }

    public async Task<IEnumerable<CustomerDto>> SearchCustomersAsync(string searchTerm)
    {
        var customers = await _customerRepository.SearchCustomersAsync(searchTerm);
        return customers.Select(MapToDto);
    }

    public async Task<CustomerDto> CreateCustomerAsync(CustomerDto customerDto)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(customerDto.FirstName))
            throw new ArgumentException("First name is required");

        // Basic uniqueness check on phone if provided
        if (!string.IsNullOrWhiteSpace(customerDto.Phone))
        {
            var existing = await GetCustomerByPhoneAsync(customerDto.Phone);
            if (existing != null)
                throw new InvalidOperationException($"Customer with phone '{customerDto.Phone}' already exists");
        }

        var customer = MapToEntity(customerDto);
        customer.CreatedAt = DateTime.Now;
        customer.UpdatedAt = DateTime.Now;

        // Auto-create credit account for every new customer
        customer.CreditAccount = new CreditAccount
        {
            CreditLimit = customer.CreditLimit, // Use limit from DTO (set in UI)
            CurrentBalance = 0,
            IsActive = true,
            CreatedAt = DateTime.Now,
            PaymentTermDays = 30
        };

        await _customerRepository.AddAsync(customer);
        await _customerRepository.SaveChangesAsync();

        return MapToDto(customer);
    }

    public async Task<CustomerDto> UpdateCustomerAsync(CustomerDto customerDto)
    {
        var existingCustomer = await _customerRepository.GetByIdAsync(customerDto.CustomerID);
        if (existingCustomer == null)
            throw new InvalidOperationException($"Customer with ID {customerDto.CustomerID} not found");

        if (string.IsNullOrWhiteSpace(customerDto.FirstName))
            throw new ArgumentException("First name is required");

        // Phone uniqueness check (excluding current customer)
        if (!string.IsNullOrWhiteSpace(customerDto.Phone) && existingCustomer.Phone != customerDto.Phone)
        {
            var existing = await GetCustomerByPhoneAsync(customerDto.Phone);
            if (existing != null)
                throw new InvalidOperationException($"Customer with phone '{customerDto.Phone}' already exists");
        }

        // Update fields
        existingCustomer.FirstName = customerDto.FirstName;
        existingCustomer.LastName = customerDto.LastName;
        existingCustomer.Email = customerDto.Email;
        existingCustomer.Phone = customerDto.Phone;
        existingCustomer.Address = customerDto.Address;
        existingCustomer.City = customerDto.City;
        existingCustomer.State = customerDto.State;
        existingCustomer.ZipCode = customerDto.ZipCode;
        existingCustomer.DateOfBirth = customerDto.DateOfBirth;
        existingCustomer.Notes = customerDto.Notes;
        existingCustomer.IsActive = customerDto.IsActive;
        existingCustomer.UpdatedAt = DateTime.Now;

        await _customerRepository.UpdateAsync(existingCustomer);
        await _customerRepository.SaveChangesAsync();

        return MapToDto(existingCustomer);
    }

    public async Task<bool> DeleteCustomerAsync(int id)
    {
        var customer = await _customerRepository.GetByIdAsync(id);
        if (customer == null)
            return false;

        // Soft delete
        customer.IsActive = false;
        customer.UpdatedAt = DateTime.Now;

        await _customerRepository.UpdateAsync(customer);
        await _customerRepository.SaveChangesAsync();

        return true;
    }

    private CustomerDto MapToDto(Customer customer)
    {
        return new CustomerDto
        {
            CustomerID = customer.CustomerID,
            CustomerCode = customer.CustomerCode,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            Phone = customer.Phone,
            Address = customer.Address,
            City = customer.City,
            State = customer.State,
            ZipCode = customer.ZipCode,
            DateOfBirth = customer.DateOfBirth,
            LoyaltyPoints = customer.LoyaltyPoints,
            CreditLimit = customer.CreditLimit,
            TotalPurchases = customer.TotalPurchases,
            IsActive = customer.IsActive,
            Notes = customer.Notes
        };
    }

    private Customer MapToEntity(CustomerDto dto)
    {
        return new Customer
        {
            CustomerID = dto.CustomerID,
            CustomerCode = dto.CustomerCode,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Phone = dto.Phone,
            Address = dto.Address,
            City = dto.City,
            State = dto.State,
            ZipCode = dto.ZipCode,
            DateOfBirth = dto.DateOfBirth,
            LoyaltyPoints = dto.LoyaltyPoints,
            CreditLimit = dto.CreditLimit,
            TotalPurchases = dto.TotalPurchases,
            IsActive = dto.IsActive,
            Notes = dto.Notes
        };
    }
}
