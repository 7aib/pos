using System.Text.RegularExpressions;
using POSApplication.Infrastructure.Interfaces;

namespace POSApplication.Infrastructure.Services;

/// <summary>
/// Barcode handling service implementation
/// </summary>
public class BarcodeService : IBarcodeService
{
    // Common barcode formats
    private static readonly Regex UpcARegex = new(@"^\d{12}$"); // UPC-A: 12 digits
    private static readonly Regex Ean13Regex = new(@"^\d{13}$"); // EAN-13: 13 digits
    private static readonly Regex Ean8Regex = new(@"^\d{8}$"); // EAN-8: 8 digits
    private static readonly Regex Code128Regex = new(@"^[\x20-\x7E]+$"); // Code 128: ASCII characters

    public bool ValidateBarcode(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            return false;

        var normalized = NormalizeBarcode(barcode);

        // Check if matches any common barcode format
        return UpcARegex.IsMatch(normalized) ||
               Ean13Regex.IsMatch(normalized) ||
               Ean8Regex.IsMatch(normalized) ||
               Code128Regex.IsMatch(normalized);
    }

    public string NormalizeBarcode(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
            return string.Empty;

        // Remove whitespace and convert to uppercase
        return barcode.Trim().ToUpper();
    }
}
