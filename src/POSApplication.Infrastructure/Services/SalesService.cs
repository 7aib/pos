using POSApplication.Common.Enums;
using POSApplication.Core.DTOs;
using POSApplication.Core.Interfaces;
using POSApplication.Core.Entities;
using POSApplication.Data.Interfaces;

namespace POSApplication.Infrastructure.Services;

/// <summary>
/// Sales transaction service implementation
/// </summary>
public class SalesService : ISalesService
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly IInventoryService _inventoryService;
    private readonly IRepository<Customer> _customerRepository;

    private readonly ICurrentUserService _currentUserService;

    public SalesService(
        ISaleRepository saleRepository,
        IProductRepository productRepository,
        IInventoryService inventoryService,
        IRepository<Customer> customerRepository,
        ICurrentUserService currentUserService)
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
        _inventoryService = inventoryService;
        _customerRepository = customerRepository;
        _currentUserService = currentUserService;
    }

    public async Task<SaleDto> CreateSaleAsync(SaleDto saleDto)
    {
        // Validation
        if (saleDto.SaleItems == null || !saleDto.SaleItems.Any())
            throw new ArgumentException("Sale must have at least one item");

        if (saleDto.Payments == null || !saleDto.Payments.Any())
            throw new ArgumentException("Sale must have at least one payment");

        // Calculate totals
        var (subtotal, taxAmount, total) = await CalculateSaleTotalsAsync(
            saleDto.SaleItems.Select(si => new CartItemDto
            {
                ProductID = si.ProductID,
                Quantity = si.Quantity,
                UnitPrice = si.UnitPrice,
                TaxRate = si.TaxRate,
                DiscountAmount = si.DiscountAmount
            }).ToList(),
            saleDto.DiscountAmount
        );

        // Validate payment amount
        var totalPaid = saleDto.Payments.Sum(p => p.Amount);
        if (totalPaid < total && saleDto.PaymentStatus != PaymentStatus.Credit)
            throw new ArgumentException("Payment amount is less than total amount");

        // Check stock availability for all items
        foreach (var item in saleDto.SaleItems)
        {
            var hasStock = await _inventoryService.CheckStockAvailabilityAsync(item.ProductID, item.Quantity);
            if (!hasStock)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductID);
                throw new InvalidOperationException($"Insufficient stock for product: {product?.ProductName}");
            }
        }

        // Generate sale number
        var saleNumber = await _saleRepository.GenerateSaleNumberAsync();

        // Create sale entity
        var sale = new Sale
        {
            SaleNumber = saleNumber,
            CustomerID = saleDto.CustomerID,
            SaleDate = DateTime.Now,
            Subtotal = subtotal,
            TaxAmount = taxAmount,
            DiscountAmount = saleDto.DiscountAmount,
            TotalAmount = total,
            AmountPaid = totalPaid,
            ChangeGiven = totalPaid > total ? totalPaid - total : 0,
            Status = TransactionStatus.Completed,
            PaymentStatus = saleDto.PaymentStatus,
            CashierID = _currentUserService.UserId ?? 1, // Fallback or throw exception
            Notes = saleDto.Notes,
            CreatedAt = DateTime.Now
        };

        // Add sale items
        foreach (var itemDto in saleDto.SaleItems)
        {
            sale.SaleItems.Add(new SaleItem
            {
                ProductID = itemDto.ProductID,
                Quantity = itemDto.Quantity,
                UnitPrice = itemDto.UnitPrice,
                TaxRate = itemDto.TaxRate,
                DiscountAmount = itemDto.DiscountAmount,
                LineTotal = itemDto.LineTotal
            });
        }

        // Add payments
        foreach (var paymentDto in saleDto.Payments)
        {
            sale.Payments.Add(new Payment
            {
                PaymentMethod = paymentDto.PaymentMethod,
                Amount = paymentDto.Amount,
                CardType = paymentDto.CardType,
                CardLastFourDigits = paymentDto.CardLastFourDigits,
                TransactionReference = paymentDto.TransactionReference,
                PaymentDate = DateTime.Now,
                ProcessedBy = paymentDto.ProcessedBy
            });
        }

        // Save sale
        await _saleRepository.AddAsync(sale);
        await _saleRepository.SaveChangesAsync();

        // Deduct inventory
        foreach (var item in saleDto.SaleItems)
        {
            await _inventoryService.DeductStockAsync(item.ProductID, (int)item.Quantity, sale.SaleID);
        }

        // Award loyalty points (1 point per dollar spent)
        if (saleDto.CustomerID.HasValue)
        {
            var customer = await _customerRepository.GetByIdAsync(saleDto.CustomerID.Value);
            if (customer != null)
            {
                var pointsEarned = (int)Math.Floor(total);
                if (pointsEarned > 0)
                {
                    customer.LoyaltyPoints += pointsEarned;
                    await _customerRepository.UpdateAsync(customer);
                    await _customerRepository.SaveChangesAsync();
                }
            }
        }

        // Return DTO
        var createdSale = await _saleRepository.GetSaleWithDetailsAsync(sale.SaleID);
        return MapToDto(createdSale!);
    }

    public async Task<SaleDto?> GetSaleByIdAsync(int saleId)
    {
        var sale = await _saleRepository.GetSaleWithDetailsAsync(saleId);
        return sale != null ? MapToDto(sale) : null;
    }

    public async Task<bool> VoidSaleAsync(int saleId, string reason)
    {
        var sale = await _saleRepository.GetSaleWithDetailsAsync(saleId);
        if (sale == null)
            return false;

        if (sale.Status == TransactionStatus.Voided)
            throw new InvalidOperationException("Sale is already voided");

        // Update sale status
        sale.Status = TransactionStatus.Voided;
        sale.VoidedBy = _currentUserService.UserId;
        sale.VoidedAt = DateTime.Now;
        sale.VoidReason = reason;

        await _saleRepository.UpdateAsync(sale);
        await _saleRepository.SaveChangesAsync();

        // Restore inventory
        foreach (var item in sale.SaleItems)
        {
            await _inventoryService.DeductStockAsync(item.ProductID, -(int)item.Quantity, saleId);
        }

        return true;
    }

    public Task<(decimal Subtotal, decimal TaxAmount, decimal Total)> CalculateSaleTotalsAsync(
        List<CartItemDto> items, decimal discountAmount)
    {
        decimal subtotal = 0;
        decimal totalTax = 0;

        foreach (var item in items)
        {
            var itemSubtotal = item.Quantity * item.UnitPrice;
            var itemTax = itemSubtotal * (item.TaxRate / 100);
            
            subtotal += itemSubtotal;
            totalTax += itemTax;
        }

        var total = subtotal + totalTax - discountAmount;
        return Task.FromResult((subtotal, totalTax, total));
    }

    private SaleDto MapToDto(Sale sale)
    {
        return new SaleDto
        {
            SaleID = sale.SaleID,
            SaleNumber = sale.SaleNumber,
            CustomerID = sale.CustomerID,
            CustomerName = sale.Customer != null 
                ? $"{sale.Customer.FirstName} {sale.Customer.LastName}".Trim() 
                : null,
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
                SaleID = si.SaleID,
                ProductID = si.ProductID,
                ProductName = si.Product?.ProductName ?? "",
                Quantity = si.Quantity,
                UnitPrice = si.UnitPrice,
                TaxRate = si.TaxRate,
                DiscountAmount = si.DiscountAmount,
                LineTotal = si.LineTotal
            }).ToList(),
            Payments = sale.Payments.Select(p => new PaymentDto
            {
                PaymentID = p.PaymentID,
                SaleID = p.SaleID,
                PaymentMethod = p.PaymentMethod,
                Amount = p.Amount,
                CardType = p.CardType,
                CardLastFourDigits = p.CardLastFourDigits,
                TransactionReference = p.TransactionReference,
                PaymentDate = p.PaymentDate,
                ProcessedBy = p.ProcessedBy
            }).ToList()
        };
    }
}
