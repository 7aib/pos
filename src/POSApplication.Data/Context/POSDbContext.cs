using Microsoft.EntityFrameworkCore;
using POSApplication.Core.Entities;

namespace POSApplication.Data.Context;

public class POSDbContext : DbContext
{
    public POSDbContext(DbContextOptions<POSDbContext> options) : base(options)
    {
    }

    // DbSets
    public DbSet<User> Users { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<SaleItem> SaleItems { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<CreditAccount> CreditAccounts { get; set; }
    public DbSet<CreditTransaction> CreditTransactions { get; set; }
    public DbSet<StockAdjustment> StockAdjustments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User entity configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserID);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(50);
            entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.HasIndex(e => e.Username).IsUnique();
            
            entity.HasOne(e => e.Creator)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Customer entity configuration
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerID);
            entity.Property(e => e.CustomerCode).HasMaxLength(20);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.State).HasMaxLength(50);
            entity.Property(e => e.ZipCode).HasMaxLength(10);
            entity.Property(e => e.CreditLimit).HasColumnType("decimal(10,2)");
            entity.Property(e => e.TotalPurchases).HasColumnType("decimal(10,2)");
            entity.HasIndex(e => e.CustomerCode).IsUnique();
            entity.HasIndex(e => e.Phone);
            entity.HasIndex(e => e.Email);
        });

        // Product entity configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductID);
            entity.Property(e => e.SKU).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Barcode).HasMaxLength(50);
            entity.Property(e => e.ProductName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.CostPrice).HasColumnType("decimal(10,2)");
            entity.Property(e => e.SellPrice).HasColumnType("decimal(10,2)");
            entity.Property(e => e.TaxRate).HasColumnType("decimal(5,2)");
            entity.Property(e => e.UnitOfMeasure).HasMaxLength(20);
            entity.Property(e => e.ImagePath).HasMaxLength(255);
            entity.HasIndex(e => e.SKU).IsUnique();
            entity.HasIndex(e => e.Barcode).IsUnique();
            
            entity.HasOne(e => e.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(e => e.CategoryID)
                .OnDelete(DeleteBehavior.SetNull);
                
            entity.HasOne(e => e.Supplier)
                .WithMany(s => s.Products)
                .HasForeignKey(e => e.SupplierID)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // Category entity configuration
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryID);
            entity.Property(e => e.CategoryName).IsRequired().HasMaxLength(100);
            
            entity.HasOne(e => e.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(e => e.ParentCategoryID)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Supplier entity configuration
        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierID);
            entity.Property(e => e.SupplierName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ContactPerson).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        // Sale entity configuration
        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.SaleID);
            entity.Property(e => e.SaleNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Subtotal).HasColumnType("decimal(10,2)");
            entity.Property(e => e.TaxAmount).HasColumnType("decimal(10,2)");
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(10,2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(10,2)");
            entity.Property(e => e.AmountPaid).HasColumnType("decimal(10,2)");
            entity.Property(e => e.ChangeGiven).HasColumnType("decimal(10,2)");
            entity.HasIndex(e => e.SaleNumber).IsUnique();
            entity.HasIndex(e => e.SaleDate);
            entity.HasIndex(e => e.CustomerID);
            entity.HasIndex(e => e.CashierID);
            
            entity.HasOne(e => e.Customer)
                .WithMany(c => c.Sales)
                .HasForeignKey(e => e.CustomerID)
                .OnDelete(DeleteBehavior.SetNull);
                
            entity.HasOne(e => e.Cashier)
                .WithMany(u => u.Sales)
                .HasForeignKey(e => e.CashierID)
                .OnDelete(DeleteBehavior.Restrict);
                
            entity.HasOne(e => e.VoidedByUser)
                .WithMany()
                .HasForeignKey(e => e.VoidedBy)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // SaleItem entity configuration
        modelBuilder.Entity<SaleItem>(entity =>
        {
            entity.HasKey(e => e.SaleItemID);
            entity.Property(e => e.Quantity).HasColumnType("decimal(10,3)");
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(10,2)");
            entity.Property(e => e.TaxRate).HasColumnType("decimal(5,2)");
            entity.Property(e => e.DiscountAmount).HasColumnType("decimal(10,2)");
            entity.Property(e => e.LineTotal).HasColumnType("decimal(10,2)");
            entity.Property(e => e.ReturnedQuantity).HasColumnType("decimal(10,3)");
            
            entity.HasOne(e => e.Sale)
                .WithMany(s => s.SaleItems)
                .HasForeignKey(e => e.SaleID)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Product)
                .WithMany(p => p.SaleItems)
                .HasForeignKey(e => e.ProductID)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Payment entity configuration
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentID);
            entity.Property(e => e.Amount).HasColumnType("decimal(10,2)");
            entity.Property(e => e.CardType).HasMaxLength(20);
            entity.Property(e => e.CardLastFourDigits).HasMaxLength(4);
            entity.Property(e => e.TransactionReference).HasMaxLength(100);
            
            entity.HasOne(e => e.Sale)
                .WithMany(s => s.Payments)
                .HasForeignKey(e => e.SaleID)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.ProcessedByUser)
                .WithMany()
                .HasForeignKey(e => e.ProcessedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // CreditAccount entity configuration
        modelBuilder.Entity<CreditAccount>(entity =>
        {
            entity.HasKey(e => e.CreditAccountID);
            entity.Property(e => e.CreditLimit).HasColumnType("decimal(10,2)");
            entity.Property(e => e.CurrentBalance).HasColumnType("decimal(10,2)");
            entity.Property(e => e.InterestRate).HasColumnType("decimal(5,2)");
            entity.Property(e => e.InterestType).HasMaxLength(20);
            
            entity.HasOne(e => e.Customer)
                .WithOne(c => c.CreditAccount)
                .HasForeignKey<CreditAccount>(e => e.CustomerID)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // CreditTransaction entity configuration
        modelBuilder.Entity<CreditTransaction>(entity =>
        {
            entity.HasKey(e => e.CreditTransactionID);
            entity.Property(e => e.Amount).HasColumnType("decimal(10,2)");
            entity.Property(e => e.BalanceAfter).HasColumnType("decimal(10,2)");
            entity.Property(e => e.PaymentMethod).HasMaxLength(20);
            entity.HasIndex(e => e.CreditAccountID);
            entity.HasIndex(e => e.CreatedAt);
            
            entity.HasOne(e => e.CreditAccount)
                .WithMany(ca => ca.CreditTransactions)
                .HasForeignKey(e => e.CreditAccountID)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.Sale)
                .WithMany()
                .HasForeignKey(e => e.SaleID)
                .OnDelete(DeleteBehavior.SetNull);
                
            entity.HasOne(e => e.CreatedByUser)
                .WithMany()
                .HasForeignKey(e => e.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // StockAdjustment entity configuration
        modelBuilder.Entity<StockAdjustment>(entity =>
        {
            entity.HasKey(e => e.AdjustmentID);
            entity.Property(e => e.Reason).HasMaxLength(200);
            
            entity.HasOne(e => e.Product)
                .WithMany(p => p.StockAdjustments)
                .HasForeignKey(e => e.ProductID)
                .OnDelete(DeleteBehavior.Cascade);
                
            entity.HasOne(e => e.AdjustedByUser)
                .WithMany()
                .HasForeignKey(e => e.AdjustedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Seed Default Admin User - Moved to SeedData.cs
        // modelBuilder.Entity<User>().HasData(...)
    }
}
