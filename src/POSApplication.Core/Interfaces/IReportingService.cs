using POSApplication.Core.DTOs;

namespace POSApplication.Core.Interfaces;

public interface IReportingService
{
    Task<DailySalesStats> GetDailySalesStatsAsync(DateTime date);
    Task<List<TopProductDto>> GetTopSellingProductsAsync(int count);
}

public class DailySalesStats
{
    public DateTime Date { get; set; }
    public decimal TotalRevenue { get; set; }
    public int TotalTransactions { get; set; }
    public decimal AverageTransactionValue { get; set; }
}

public class TopProductDto
{
    public string ProductName { get; set; } = string.Empty;
    public int QuantitySold { get; set; }
    public decimal TotalRevenue { get; set; }
}
