using POSApplication.Common.Enums;
using POSApplication.Core.DTOs;
using POSApplication.Core.Interfaces;

namespace POSApplication.Infrastructure.Services;

/// <summary>
/// Payment processing service implementation
/// </summary>
public class PaymentService : IPaymentService
{
    public async Task<bool> ValidatePaymentAsync(PaymentDto payment)
    {
        // Basic validation
        if (payment.Amount <= 0)
            return false;

        // Payment method specific validation
        switch (payment.PaymentMethod)
        {
            case PaymentMethod.Card:
                // Card payments should have card type
                if (string.IsNullOrWhiteSpace(payment.CardType))
                    return false;
                break;

            case PaymentMethod.Cash:
                // Cash payments are always valid if amount > 0
                break;

            case PaymentMethod.CreditAccount:
                // Credit limit validation is handled in the UI (PaymentDialog) 
                // and strictly enforced in CreditService.ProcessCreditPaymentAsync.
                // PaymentService just acknowledges the method is valid.
                break;
        }

        return await Task.FromResult(true);
    }

    public decimal CalculateChange(decimal totalAmount, decimal amountPaid)
    {
        var change = amountPaid - totalAmount;
        return change > 0 ? change : 0;
    }
}
