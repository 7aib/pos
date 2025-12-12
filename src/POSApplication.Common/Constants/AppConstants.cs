namespace POSApplication.Common.Constants;

public static class AppConstants
{
    public const string AppName = "POS Application";
    public const string AppVersion = "1.0.0";
    public const string DatabaseFileName = "pos.db";
    
    // Session
    public const int DefaultSessionTimeoutMinutes = 30;
    
    // Pagination
    public const int DefaultPageSize = 50;
    public const int MaxPageSize = 500;
    
    // Validation
    public const int MinPasswordLength = 8;
    public const int MaxPasswordLength = 50;
    public const decimal MaxDiscountPercentage = 100;
    public const decimal MinCreditLimit = 0;
    public const decimal MaxCreditLimit = 1000000;
    
    // Date Formats
    public const string DateFormat = "yyyy-MM-dd";
    public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
    public const string ReceiptDateTimeFormat = "MMM dd, yyyy hh:mm tt";
    
    // Tax
    public const decimal DefaultTaxRate = 0.10m; // 10%
    
    // Sync
    public const int MaxSyncRetries = 3;
    public const int SyncIntervalMinutes = 15;
}

public static class DatabaseConstants
{
    public const string UsersTable = "Users";
    public const string CustomersTable = "Customers";
    public const string ProductsTable = "Products";
    public const string SalesTable = "Sales";
    public const string CreditAccountsTable = "CreditAccounts";
}

public static class ErrorMessages
{
    public const string InvalidCredentials = "Invalid username or password.";
    public const string UserNotFound = "User not found.";
    public const string CustomerNotFound = "Customer not found.";
    public const string ProductNotFound = "Product not found.";
    public const string InsufficientStock = "Insufficient stock available.";
    public const string InvalidPaymentAmount = "Invalid payment amount.";
    public const string CreditLimitExceeded = "Credit limit exceeded.";
    public const string UnauthorizedAccess = "You do not have permission to perform this action.";
    public const string DatabaseError = "A database error occurred. Please try again.";
}

public static class SuccessMessages
{
    public const string LoginSuccessful = "Login successful.";
    public const string SaleCompleted = "Sale completed successfully.";
    public const string ProductAdded = "Product added successfully.";
    public const string ProductUpdated = "Product updated successfully.";
    public const string CustomerAdded = "Customer added successfully.";
    public const string PaymentProcessed = "Payment processed successfully.";
}
