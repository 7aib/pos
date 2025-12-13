using POSApplication.Data.Entities;

namespace POSApplication.Data.Interfaces;

/// <summary>
/// Sale-specific repository interface
/// </summary>
public interface ISaleRepository : IRepository<Sale>
{
    Task<Sale?> GetSaleWithDetailsAsync(int saleId);
    Task<IEnumerable<Sale>> GetSalesByCashierAsync(int cashierId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<Sale>> GetDailySalesAsync(DateTime date);
    Task<string> GenerateSaleNumberAsync();
}
