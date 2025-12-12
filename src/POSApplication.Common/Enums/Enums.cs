namespace POSApplication.Common.Enums;

public enum UserRole
{
    Cashier,
    Manager,
    Admin
}

public enum PaymentMethod
{
    Cash,
    Card,
    CreditAccount,
    Wallet,
    Other
}

public enum TransactionStatus
{
    Completed,
    Voided,
    Suspended,
    Returned
}

public enum PaymentStatus
{
    Paid,
    Partial,
    Credit
}

public enum CreditTransactionType
{
    Charge,
    Payment,
    Interest,
    Fee,
    Adjustment
}

public enum StockAdjustmentType
{
    StockIn,
    StockOut,
    Correction,
    Damage,
    Theft
}

public enum RefundMethod
{
    Cash,
    Card,
    CreditAccount,
    StoreCredit
}

public enum SyncStatus
{
    Pending,
    Synced,
    Failed
}

public enum SyncOperation
{
    Insert,
    Update,
    Delete
}
