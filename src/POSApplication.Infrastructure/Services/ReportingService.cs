using Microsoft.EntityFrameworkCore;
using POSApplication.Core.DTOs;
using POSApplication.Core.Interfaces;
using POSApplication.Data.Context;

namespace POSApplication.Infrastructure.Services;

public class ReportingService : IReportingService
{
    private readonly POSDbContext _context;

    public ReportingService(POSDbContext context)
    {
        _context = context;
    }

    public async Task<DailySalesStats> GetDailySalesStatsAsync(DateTime date)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1).AddTicks(-1);

        var sales = await _context.Sales
            .Where(s => s.SaleDate >= startOfDay && s.SaleDate <= endOfDay && s.VoidedBy == null)
            .ToListAsync();

        return new DailySalesStats
        {
            Date = startOfDay,
            TotalRevenue = sales.Sum(s => s.TotalAmount),
            TotalTransactions = sales.Count,
            AverageTransactionValue = sales.Any() ? sales.Average(s => s.TotalAmount) : 0
        };
    }

    public async Task<List<TopProductDto>> GetTopSellingProductsAsync(int count)
    {
        // This query might be complex for EF Core to translate fully in some cases, 
        // but for basic aggregation it should work. Grouping by ProductID.
        
        // Fetch data first to avoid SQLite decimal aggregation issues
        var saleItems = await _context.SaleItems
            .Include(si => si.Product)
            .Where(si => si.Sale.VoidedBy == null)
            .ToListAsync();

        var topProducts = saleItems
            .GroupBy(si => new { si.ProductID, si.Product.ProductName })
            .Select(g => new TopProductDto
            {
                ProductName = g.Key.ProductName,
                QuantitySold = (int)g.Sum(si => si.Quantity),
                TotalRevenue = g.Sum(si => si.LineTotal)
            })
            .OrderByDescending(tp => tp.QuantitySold)
            .Take(count)
            .ToList();

        return topProducts;
    }
}
