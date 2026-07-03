using POSApplication.Core.DTOs;
using POSApplication.Core.Entities;
using POSApplication.Core.Interfaces;
using POSApplication.Data.Interfaces;

namespace POSApplication.Infrastructure.Services;

public class SupplierService : ISupplierService
{
    private readonly IRepository<Supplier> _supplierRepository;
    private readonly IRepository<Product> _productRepository;

    public SupplierService(IRepository<Supplier> supplierRepository, IRepository<Product> productRepository)
    {
        _supplierRepository = supplierRepository;
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync()
    {
        var suppliers = await _supplierRepository.GetAllAsync();
        return suppliers.OrderByDescending(s => s.IsActive).ThenBy(s => s.SupplierName).Select(MapToDto);
    }

    public async Task<SupplierDto?> GetSupplierByIdAsync(int id)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        return supplier != null ? MapToDto(supplier) : null;
    }

    public async Task<IEnumerable<SupplierDto>> SearchSuppliersAsync(string searchTerm)
    {
        var suppliers = await _supplierRepository.FindAsync(s =>
            s.SupplierName.Contains(searchTerm) ||
            (s.ContactPerson != null && s.ContactPerson.Contains(searchTerm)) ||
            (s.Email != null && s.Email.Contains(searchTerm)) ||
            (s.Phone != null && s.Phone.Contains(searchTerm)));
        return suppliers.Select(MapToDto);
    }

    public async Task<SupplierDto> CreateSupplierAsync(SupplierDto supplierDto)
    {
        if (string.IsNullOrWhiteSpace(supplierDto.SupplierName))
            throw new ArgumentException("Supplier name is required");

        var existing = await _supplierRepository.FindAsync(s => s.SupplierName == supplierDto.SupplierName);
        if (existing.Any())
            throw new InvalidOperationException($"Supplier '{supplierDto.SupplierName}' already exists");

        var supplier = new Supplier
        {
            SupplierName = supplierDto.SupplierName,
            ContactPerson = supplierDto.ContactPerson,
            Email = supplierDto.Email,
            Phone = supplierDto.Phone,
            Address = supplierDto.Address,
            IsActive = supplierDto.IsActive,
            CreatedAt = DateTime.Now
        };

        await _supplierRepository.AddAsync(supplier);
        await _supplierRepository.SaveChangesAsync();

        return MapToDto(supplier);
    }

    public async Task<SupplierDto> UpdateSupplierAsync(SupplierDto supplierDto)
    {
        var existing = await _supplierRepository.GetByIdAsync(supplierDto.SupplierID);
        if (existing == null)
            throw new InvalidOperationException($"Supplier with ID {supplierDto.SupplierID} not found");

        if (string.IsNullOrWhiteSpace(supplierDto.SupplierName))
            throw new ArgumentException("Supplier name is required");

        var duplicate = await _supplierRepository.FindAsync(s => s.SupplierName == supplierDto.SupplierName && s.SupplierID != supplierDto.SupplierID);
        if (duplicate.Any())
            throw new InvalidOperationException($"Supplier '{supplierDto.SupplierName}' already exists");

        existing.SupplierName = supplierDto.SupplierName;
        existing.ContactPerson = supplierDto.ContactPerson;
        existing.Email = supplierDto.Email;
        existing.Phone = supplierDto.Phone;
        existing.Address = supplierDto.Address;
        existing.IsActive = supplierDto.IsActive;

        await _supplierRepository.UpdateAsync(existing);
        await _supplierRepository.SaveChangesAsync();

        return MapToDto(existing);
    }

    public async Task<bool> DeleteSupplierAsync(int id)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        if (supplier == null)
            return false;

        var products = await _productRepository.FindAsync(p => p.SupplierID == id && p.IsActive);
        if (products.Any())
            throw new InvalidOperationException("Cannot delete supplier with active products. Remove or reassign products first.");

        supplier.IsActive = false;
        await _supplierRepository.UpdateAsync(supplier);
        await _supplierRepository.SaveChangesAsync();

        return true;
    }

    private SupplierDto MapToDto(Supplier supplier)
    {
        return new SupplierDto
        {
            SupplierID = supplier.SupplierID,
            SupplierName = supplier.SupplierName,
            ContactPerson = supplier.ContactPerson,
            Email = supplier.Email,
            Phone = supplier.Phone,
            Address = supplier.Address,
            IsActive = supplier.IsActive,
            ProductCount = supplier.Products?.Count(p => p.IsActive) ?? 0
        };
    }
}
