using POSApplication.Core.DTOs;

namespace POSApplication.Core.Interfaces;

public interface ISupplierService
{
    Task<IEnumerable<SupplierDto>> GetAllSuppliersAsync();
    Task<SupplierDto?> GetSupplierByIdAsync(int id);
    Task<IEnumerable<SupplierDto>> SearchSuppliersAsync(string searchTerm);
    Task<SupplierDto> CreateSupplierAsync(SupplierDto supplierDto);
    Task<SupplierDto> UpdateSupplierAsync(SupplierDto supplierDto);
    Task<bool> DeleteSupplierAsync(int id);
}
