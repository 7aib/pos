using POSApplication.Common.Enums;
using POSApplication.Core.Entities;

namespace POSApplication.Core.Interfaces;

public interface ICreditService
{
    Task<CreditAccount?> GetCreditAccountAsync(int customerId);
    Task<CreditAccount> GetOrCreateCreditAccountAsync(int customerId);
    Task<decimal> GetCustomerBalanceAsync(int customerId);
    Task<bool> ProcessCreditPaymentAsync(int customerId, decimal amount, int saleId, int userId);
    Task<bool> MakePaymentOnAccountAsync(int customerId, decimal amount, PaymentMethod method, string reference, int userId);
}
