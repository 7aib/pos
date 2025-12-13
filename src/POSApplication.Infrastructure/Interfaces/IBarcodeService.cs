namespace POSApplication.Infrastructure.Interfaces;

/// <summary>
/// Barcode handling service interface
/// </summary>
public interface IBarcodeService
{
    bool ValidateBarcode(string barcode);
    string NormalizeBarcode(string barcode);
}
