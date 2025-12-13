using POSApplication.Core.DTOs;

namespace POSApplication.Infrastructure.Interfaces;

/// <summary>
/// Receipt printer service interface
/// </summary>
public interface IPrinterService
{
    Task<bool> PrintReceiptAsync(SaleDto sale);
    Task<bool> PrintDuplicateReceiptAsync(int saleId);
    Task<bool> TestPrinterAsync();
    Task<List<string>> GetAvailablePrintersAsync();
}
