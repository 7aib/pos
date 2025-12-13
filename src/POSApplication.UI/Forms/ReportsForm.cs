using POSApplication.Core.Interfaces;

namespace POSApplication.UI.Forms;

public partial class ReportsForm : Form
{
    private readonly IReportingService _reportingService;

    public ReportsForm(IReportingService reportingService)
    {
        InitializeComponent();
        _reportingService = reportingService;
    }

    private async void ReportsForm_Load(object sender, EventArgs e)
    {
        await LoadReportAsync(DateTime.Today);
    }

    private async void btnLoadReport_Click(object sender, EventArgs e)
    {
        await LoadReportAsync(dtpDate.Value);
    }

    private async Task LoadReportAsync(DateTime date)
    {
        try
        {
            // Daily Stats
            var stats = await _reportingService.GetDailySalesStatsAsync(date);
            lblTotalRevenue.Text = $"Total Revenue: {stats.TotalRevenue:C}";
            lblTotalTransactions.Text = $"Transactions: {stats.TotalTransactions}";
            lblAvgTransaction.Text = $"Avg Transaction: {stats.AverageTransactionValue:C}";

            // Top Products
            var topProducts = await _reportingService.GetTopSellingProductsAsync(10);
            dgvTopProducts.DataSource = topProducts;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading report: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
