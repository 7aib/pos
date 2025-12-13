using System.Text;
using POSApplication.Core.DTOs;
using POSApplication.Infrastructure.Interfaces;

namespace POSApplication.Infrastructure.Services;

/// <summary>
/// Thermal printer service implementation using ESC/POS commands
/// </summary>
public class ThermalPrinterService : IPrinterService
{
    private const string DefaultPrinterName = ""; // Will use default printer if empty

    // ESC/POS Commands
    private static readonly byte[] ESC_INIT = { 0x1B, 0x40 }; // Initialize printer
    private static readonly byte[] ESC_ALIGN_CENTER = { 0x1B, 0x61, 0x01 }; // Center alignment
    private static readonly byte[] ESC_ALIGN_LEFT = { 0x1B, 0x61, 0x00 }; // Left alignment
    private static readonly byte[] ESC_BOLD_ON = { 0x1B, 0x45, 0x01 }; // Bold ON
    private static readonly byte[] ESC_BOLD_OFF = { 0x1B, 0x45, 0x00 }; // Bold OFF
    private static readonly byte[] ESC_CUT = { 0x1D, 0x56, 0x00 }; // Cut paper

    public async Task<bool> PrintReceiptAsync(SaleDto sale)
    {
        try
        {
            var receipt = GenerateReceiptContent(sale);
            // In a real implementation, this would send to the printer
            // For now, we'll just save to a file for testing
            var fileName = $"receipt_{sale.SaleNumber}_{DateTime.Now:yyyyMMddHHmmss}.txt";
            await File.WriteAllTextAsync(fileName, receipt);
            
            return true;
        }
        catch (Exception ex)
        {
            // Log error
            Console.WriteLine($"Print error: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> PrintDuplicateReceiptAsync(int saleId)
    {
        // In a real implementation, would fetch sale from database and print
        await Task.CompletedTask;
        return true;
    }

    public async Task<bool> TestPrinterAsync()
    {
        try
        {
            var testReceipt = GenerateTestReceipt();
            var fileName = $"test_receipt_{DateTime.Now:yyyyMMddHHmmss}.txt";
            await File.WriteAllTextAsync(fileName, testReceipt);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<List<string>> GetAvailablePrintersAsync()
    {
        await Task.CompletedTask;
        // In a real implementation, would enumerate system printers
        return new List<string> { "Default Printer" };
    }

    private string GenerateReceiptContent(SaleDto sale)
    {
        var sb = new StringBuilder();
        
        // Store header
        sb.AppendLine("================================");
        sb.AppendLine("         MY STORE");
        sb.AppendLine("     123 Main Street");
        sb.AppendLine("   Phone: (555) 123-4567");
        sb.AppendLine("================================");
        sb.AppendLine();
        
        // Transaction info
        sb.AppendLine($"Sale #: {sale.SaleNumber}");
        sb.AppendLine($"Date: {sale.SaleDate:yyyy-MM-dd HH:mm:ss}");
        sb.AppendLine($"Cashier: {sale.CashierName ?? "Unknown"}");
        if (!string.IsNullOrWhiteSpace(sale.CustomerName))
            sb.AppendLine($"Customer: {sale.CustomerName}");
        sb.AppendLine("================================");
        sb.AppendLine();
        
        // Items
        sb.AppendLine("Items:");
        sb.AppendLine("--------------------------------");
        foreach (var item in sale.SaleItems)
        {
            sb.AppendLine($"{item.ProductName}");
            sb.AppendLine($"  {item.Quantity} x {item.UnitPrice:C} = {item.LineTotal:C}");
        }
        sb.AppendLine("--------------------------------");
        sb.AppendLine();
        
        // Totals
        sb.AppendLine($"Subtotal:        {sale.Subtotal,12:C}");
        sb.AppendLine($"Tax:             {sale.TaxAmount,12:C}");
        if (sale.DiscountAmount > 0)
            sb.AppendLine($"Discount:        {sale.DiscountAmount,12:C}");
        sb.AppendLine($"TOTAL:           {sale.TotalAmount,12:C}");
        sb.AppendLine();
        
        // Payment
        sb.AppendLine("Payment:");
        foreach (var payment in sale.Payments)
        {
            sb.AppendLine($"  {payment.PaymentMethod}: {payment.Amount:C}");
        }
        
        if (sale.ChangeGiven > 0)
        {
            sb.AppendLine($"Change:          {sale.ChangeGiven,12:C}");
        }
        sb.AppendLine();
        
        // Footer
        sb.AppendLine("================================");
        sb.AppendLine("    Thank you for your");
        sb.AppendLine("        business!");
        sb.AppendLine("================================");
        sb.AppendLine();
        sb.AppendLine();
        
        return sb.ToString();
    }

    private string GenerateTestReceipt()
    {
        return @"
================================
        PRINTER TEST
================================
If you can read this clearly,
  your printer is working!
================================
";
    }
}
