using POSApplication.Common.Enums;
using POSApplication.Core.DTOs;
using POSApplication.Core.Entities;
using POSApplication.Core.Interfaces;
using POSApplication.Data.Interfaces;

namespace POSApplication.Infrastructure.Services;

public interface IReturnService
{
    Task<SaleDto?> GetSaleByNumberAsync(string saleNumber);
    Task<List<SaleItemDto>> GetReturnableItemsAsync(int saleId);
    Task<bool> ProcessReturnAsync(int saleId, List<ReturnItemDto> items, RefundMethod refundMethod, int processedByUserId, string? notes = null);
}

public class ReturnItemDto
{
    public int SaleItemID { get; set; }
    public int ProductID { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal OriginalQuantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal ReturnQuantity { get; set; }
    public decimal RefundAmount { get; set; }
}

public class ReturnService : IReturnService
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly IRepository<StockAdjustment> _stockAdjustmentRepository;
    private readonly ICurrentUserService _currentUserService;

    public ReturnService(
        ISaleRepository saleRepository,
        IProductRepository productRepository,
        IRepository<StockAdjustment> stockAdjustmentRepository,
        ICurrentUserService currentUserService)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
        _stockAdjustmentRepository = stockAdjustmentRepository;
        _currentUserService = currentUserService;
    }

    public async Task<SaleDto?> GetSaleByNumberAsync(string saleNumber)
    {
        var sales = await _saleRepository.FindAsync(s => s.SaleNumber == saleNumber);
        var sale = sales.FirstOrDefault();
        if (sale == null) return null;

        var saleWithDetails = await _saleRepository.GetSaleWithDetailsAsync(sale.SaleID);
        if (saleWithDetails == null) return null;

        return MapSaleToDto(saleWithDetails);
    }

    public async Task<List<SaleItemDto>> GetReturnableItemsAsync(int saleId)
    {
        var sale = await _saleRepository.GetSaleWithDetailsAsync(saleId);
        if (sale == null) return new List<SaleItemDto>();

        return sale.SaleItems
            .Where(si => !si.IsReturned && (si.Quantity - si.ReturnedQuantity) > 0)
            .Select(si => new SaleItemDto
            {
                SaleItemID = si.SaleItemID,
                ProductID = si.ProductID,
                ProductName = si.Product?.ProductName ?? "Unknown",
                Quantity = si.Quantity - si.ReturnedQuantity,
                UnitPrice = si.UnitPrice,
                TaxRate = si.TaxRate,
                DiscountAmount = si.DiscountAmount,
                LineTotal = si.LineTotal
            }).ToList();
    }

    public async Task<bool> ProcessReturnAsync(int saleId, List<ReturnItemDto> items, RefundMethod refundMethod, int processedByUserId, string? notes = null)
    {
        var sale = await _saleRepository.GetSaleWithDetailsAsync(saleId);
        if (sale == null)
            throw new InvalidOperationException("Sale not found");

        if (sale.Status == TransactionStatus.Returned)
            throw new InvalidOperationException("This sale has already been fully returned");

        decimal totalRefund = 0;

        foreach (var item in items)
        {
            if (item.ReturnQuantity <= 0) continue;

            var saleItem = sale.SaleItems.FirstOrDefault(si => si.SaleItemID == item.SaleItemID);
            if (saleItem == null)
                throw new InvalidOperationException($"Sale item {item.SaleItemID} not found");

            var availableQty = saleItem.Quantity - saleItem.ReturnedQuantity;
            if (item.ReturnQuantity > availableQty)
                throw new InvalidOperationException($"Cannot return {item.ReturnQuantity} of {saleItem.Product?.ProductName}. Only {availableQty} available.");

            // Calculate refund for this item
            var lineRefund = item.ReturnQuantity * item.UnitPrice;
            var taxRefund = lineRefund * (saleItem.TaxRate / 100);
            var itemRefund = lineRefund + taxRefund - (saleItem.DiscountAmount * (item.ReturnQuantity / saleItem.Quantity));
            totalRefund += itemRefund;

            // Update sale item
            saleItem.ReturnedQuantity += item.ReturnQuantity;
            if (saleItem.ReturnedQuantity >= saleItem.Quantity)
            {
                saleItem.IsReturned = true;
            }

            // Restore stock
            var product = await _productRepository.GetByIdAsync(item.ProductID);
            if (product != null)
            {
                product.CurrentStock += (int)item.ReturnQuantity;
                await _productRepository.UpdateAsync(product);

                // Create stock adjustment
                var adjustment = new StockAdjustment
                {
                    ProductID = item.ProductID,
                    AdjustmentType = StockAdjustmentType.StockIn,
                    Quantity = (int)item.ReturnQuantity,
                    Reason = $"Return - Sale #{sale.SaleNumber}",
                    Notes = notes,
                    AdjustedBy = processedByUserId,
                    CreatedAt = DateTime.Now
                };
                await _stockAdjustmentRepository.AddAsync(adjustment);
            }
        }

        // Update sale status
        var returnedItemsCount = sale.SaleItems.Count(si => si.IsReturned);
        if (returnedItemsCount == sale.SaleItems.Count)
        {
            sale.Status = TransactionStatus.Returned;
        }

        sale.Notes = (sale.Notes + $" | Return processed {DateTime.Now:yyyy-MM-dd HH:mm}. Refund: {totalRefund:C2} ({refundMethod}). {notes}").Trim(' ', '|');

        await _saleRepository.SaveChangesAsync();
        return true;
    }

    private SaleDto MapSaleToDto(Sale sale)
    {
        return new SaleDto
        {
            SaleID = sale.SaleID,
            SaleNumber = sale.SaleNumber,
            CustomerID = sale.CustomerID,
            CustomerName = sale.Customer != null ? $"{sale.Customer.FirstName} {sale.Customer.LastName}" : null,
            SaleDate = sale.SaleDate,
            Subtotal = sale.Subtotal,
            TaxAmount = sale.TaxAmount,
            DiscountAmount = sale.DiscountAmount,
            TotalAmount = sale.TotalAmount,
            AmountPaid = sale.AmountPaid,
            ChangeGiven = sale.ChangeGiven,
            Status = sale.Status,
            PaymentStatus = sale.PaymentStatus,
            CashierID = sale.CashierID,
            CashierName = sale.Cashier?.FullName,
            Notes = sale.Notes,
            SaleItems = sale.SaleItems.Select(si => new SaleItemDto
            {
                SaleItemID = si.SaleItemID,
                ProductID = si.ProductID,
                ProductName = si.Product?.ProductName ?? "Unknown",
                Quantity = si.Quantity,
                UnitPrice = si.UnitPrice,
                TaxRate = si.TaxRate,
                DiscountAmount = si.DiscountAmount,
                LineTotal = si.LineTotal,
                IsReturned = si.IsReturned,
                ReturnedQuantity = si.ReturnedQuantity
            }).ToList(),
            Payments = sale.Payments.Select(p => new PaymentDto
            {
                PaymentID = p.PaymentID,
                PaymentMethod = p.PaymentMethod,
                Amount = p.Amount,
                PaymentDate = p.PaymentDate,
                ProcessedBy = p.ProcessedBy
            }).ToList()
        };
    }
}
