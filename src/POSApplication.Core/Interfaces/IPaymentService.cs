using POSApplication.Core.DTOs;

namespace POSApplication.Core.Interfaces;

/// <summary>
/// Payment processing service interface
/// </summary>
public interface IPaymentService
{
    Task<bool> ValidatePaymentAsync(PaymentDto payment);
    decimal CalculateChange(decimal totalAmount, decimal amountPaid);
}
