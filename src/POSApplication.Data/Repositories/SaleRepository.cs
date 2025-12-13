using Microsoft.EntityFrameworkCore;
using POSApplication.Data.Context;
using POSApplication.Core.Entities;
using POSApplication.Data.Interfaces;

namespace POSApplication.Data.Repositories;

/// <summary>
/// Sale repository implementation
/// </summary>
public class SaleRepository : Repository<Sale>, ISaleRepository
{
    public SaleRepository(POSDbContext context) : base(context)
    {
    }

    public async Task<Sale?> GetSaleWithDetailsAsync(int saleId)
    {
        return await _dbSet
            .Include(s => s.SaleItems)
                .ThenInclude(si => si.Product)
            .Include(s => s.Payments)
            .Include(s => s.Customer)
            .Include(s => s.Cashier)
            .FirstOrDefaultAsync(s => s.SaleID == saleId);
    }

    public async Task<IEnumerable<Sale>> GetSalesByCashierAsync(int cashierId, DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Include(s => s.SaleItems)
            .Include(s => s.Payments)
            .Where(s => s.CashierID == cashierId && 
                       s.SaleDate >= startDate && 
                       s.SaleDate <= endDate)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<Sale>> GetDailySalesAsync(DateTime date)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        return await _dbSet
            .Include(s => s.SaleItems)
            .Include(s => s.Payments)
            .Include(s => s.Cashier)
            .Where(s => s.SaleDate >= startOfDay && s.SaleDate < endOfDay)
            .OrderByDescending(s => s.SaleDate)
            .ToListAsync();
    }

    public async Task<string> GenerateSaleNumberAsync()
    {
        var today = DateTime.Now.Date;
        var tomorrow = today.AddDays(1);
        
        var todaysSalesCount = await _dbSet
            .Where(s => s.SaleDate >= today && s.SaleDate < tomorrow)
            .CountAsync();

        var saleNumber = $"SAL-{DateTime.Now:yyyyMMdd}-{(todaysSalesCount + 1):D4}";
        return saleNumber;
    }
}
