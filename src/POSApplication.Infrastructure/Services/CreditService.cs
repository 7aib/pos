using Microsoft.EntityFrameworkCore;
using POSApplication.Common.Enums;
using POSApplication.Core.Entities;
using POSApplication.Core.Interfaces;
using POSApplication.Data.Context;

namespace POSApplication.Infrastructure.Services;

public class CreditService : ICreditService
{
    private readonly POSDbContext _context;

    public CreditService(POSDbContext context)
    {
        _context = context;
    }

    public async Task<CreditAccount?> GetCreditAccountAsync(int customerId)
    {
        return await _context.CreditAccounts
            .Include(ca => ca.CreditTransactions)
            .FirstOrDefaultAsync(ca => ca.CustomerID == customerId);
    }

    public async Task<CreditAccount> GetOrCreateCreditAccountAsync(int customerId)
    {
        var account = await GetCreditAccountAsync(customerId);
        if (account == null)
        {
            account = new CreditAccount
            {
                CustomerID = customerId,
                CreditLimit = 0, // Default limit, should be set by admin
                CurrentBalance = 0,
                CreatedAt = DateTime.Now,
                IsActive = true
            };
            _context.CreditAccounts.Add(account);
            await _context.SaveChangesAsync();
        }
        return account;
    }

    public async Task<decimal> GetCustomerBalanceAsync(int customerId)
    {
        var account = await _context.CreditAccounts
            .AsNoTracking()
            .FirstOrDefaultAsync(ca => ca.CustomerID == customerId);
        return account?.CurrentBalance ?? 0;
    }

    public async Task<bool> ProcessCreditPaymentAsync(int customerId, decimal amount, int saleId, int userId)
    {
        var account = await GetOrCreateCreditAccountAsync(customerId);

        if (!account.IsActive || account.IsFrozen)
            throw new InvalidOperationException("Credit account is not active.");

        if (account.CurrentBalance + amount > account.CreditLimit)
            throw new InvalidOperationException("Credit limit exceeded.");

        account.CurrentBalance += amount;
        
        var transaction = new CreditTransaction
        {
            CreditAccountID = account.CreditAccountID,
            TransactionType = CreditTransactionType.Charge,
            Amount = amount,
            BalanceAfter = account.CurrentBalance,
            SaleID = saleId,
            CreatedAt = DateTime.Now,
            CreatedBy = userId
        };

        _context.CreditTransactions.Add(transaction);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> MakePaymentOnAccountAsync(int customerId, decimal amount, PaymentMethod method, string reference, int userId)
    {
        var account = await GetOrCreateCreditAccountAsync(customerId);

        if (amount <= 0) throw new ArgumentException("Payment amount must be positive.");

        account.CurrentBalance -= amount;

        var transaction = new CreditTransaction
        {
            CreditAccountID = account.CreditAccountID,
            TransactionType = CreditTransactionType.Payment,
            Amount = amount,
            BalanceAfter = account.CurrentBalance,
            PaymentMethod = method.ToString(),
            Notes = reference,
            CreatedAt = DateTime.Now,
            CreatedBy = userId
        };

        _context.CreditTransactions.Add(transaction);
        await _context.SaveChangesAsync();
        return true;
    }
}
