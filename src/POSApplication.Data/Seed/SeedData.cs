using Microsoft.EntityFrameworkCore;
using POSApplication.Common.Enums;
using POSApplication.Data.Context;
using POSApplication.Data.Entities;

namespace POSApplication.Data.Seed;

/// <summary>
/// Database seeding for initial data
/// </summary>
public static class SeedData
{
    public static async Task SeedAsync(POSDbContext context)
    {
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Seed Users
        if (!await context.Users.AnyAsync())
        {
            var adminUser = new User
            {
                Username = "admin",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                FullName = "Administrator",
                Email = "admin@pos.com",
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            var cashierUser = new User
            {
                Username = "cashier",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("cashier123"),
                FullName = "John Cashier",
                Email = "cashier@pos.com",
                Role = UserRole.Cashier,
                IsActive = true,
                CreatedAt = DateTime.Now
            };

            context.Users.AddRange(adminUser, cashierUser);
            await context.SaveChangesAsync();
        }

        // Seed Categories
        if (!await context.Categories.AnyAsync())
        {
            var categories = new[]
            {
                new Category { CategoryName = "Electronics", Description = "Electronic items and gadgets", IsActive = true },
                new Category { CategoryName = "Groceries", Description = "Food and grocery items", IsActive = true },
                new Category { CategoryName = "Clothing", Description = "Apparel and accessories", IsActive = true },
                new Category { CategoryName = "Home & Garden", Description = "Home improvement and garden supplies", IsActive = true },
                new Category { CategoryName = "Books", Description = "Books and magazines", IsActive = true }
            };

            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();
        }

        // Seed Suppliers
        if (!await context.Suppliers.AnyAsync())
        {
            var suppliers = new[]
            {
                new Supplier { SupplierName = "Tech Distributors Inc", ContactPerson = "Mike Johnson", Phone = "555-0101", IsActive = true, CreatedAt = DateTime.Now },
                new Supplier { SupplierName = "Food Wholesalers Co", ContactPerson = "Sarah Williams", Phone = "555-0102", IsActive = true, CreatedAt = DateTime.Now },
                new Supplier { SupplierName = "Fashion Direct", ContactPerson = "Emily Brown", Phone = "555-0103", IsActive = true, CreatedAt = DateTime.Now }
            };

            context.Suppliers.AddRange(suppliers);
            await context.SaveChangesAsync();
        }

        // Seed Products
        if (!await context.Products.AnyAsync())
        {
            var electronicsCategory = await context.Categories.FirstAsync(c => c.CategoryName == "Electronics");
            var groceriesCategory = await context.Categories.FirstAsync(c => c.CategoryName == "Groceries");
            var clothingCategory = await context.Categories.FirstAsync(c => c.CategoryName == "Clothing");
            var techSupplier = await context.Suppliers.FirstAsync(s => s.SupplierName == "Tech Distributors Inc");
            var foodSupplier = await context.Suppliers.FirstAsync(s => s.SupplierName == "Food Wholesalers Co");

            var products = new[]
            {
                new Product
                {
                    SKU = "ELECT-001",
                    Barcode = "1234567890123",
                    ProductName = "Wireless Mouse",
                    Description = "Ergonomic wireless mouse with USB receiver",
                    CategoryID = electronicsCategory.CategoryID,
                    CostPrice = 15.00m,
                    SellPrice = 29.99m,
                    TaxRate = 10.0m,
                    CurrentStock = 50,
                    MinStockLevel = 10,
                    ReorderPoint = 15,
                    UnitOfMeasure = "piece",
                    SupplierID = techSupplier.SupplierID,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new Product
                {
                    SKU = "ELECT-002",
                    Barcode = "1234567890124",
                    ProductName = "USB Cable",
                    Description = "USB-C to USB-A cable, 6ft",
                    CategoryID = electronicsCategory.CategoryID,
                    CostPrice = 5.00m,
                    SellPrice = 12.99m,
                    TaxRate = 10.0m,
                    CurrentStock = 100,
                    MinStockLevel = 20,
                    ReorderPoint = 30,
                    UnitOfMeasure = "piece",
                    SupplierID = techSupplier.SupplierID,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new Product
                {
                    SKU = "GROC-001",
                    Barcode = "2234567890123",
                    ProductName = "Orange Juice",
                    Description = "Fresh orange juice, 1 liter",
                    CategoryID = groceriesCategory.CategoryID,
                    CostPrice = 2.50m,
                    SellPrice = 4.99m,
                    TaxRate = 0.0m,
                    CurrentStock = 30,
                    MinStockLevel = 10,
                    ReorderPoint = 15,
                    UnitOfMeasure = "liter",
                    SupplierID = foodSupplier.SupplierID,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new Product
                {
                    SKU = "GROC-002",
                    Barcode = "2234567890124",
                    ProductName = "Bread",
                    Description = "Whole wheat bread loaf",
                    CategoryID = groceriesCategory.CategoryID,
                    CostPrice = 1.50m,
                    SellPrice = 3.49m,
                    TaxRate = 0.0m,
                    CurrentStock = 40,
                    MinStockLevel = 15,
                    ReorderPoint = 20,
                    UnitOfMeasure = "piece",
                    SupplierID = foodSupplier.SupplierID,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new Product
                {
                    SKU = "CLOTH-001",
                    Barcode = "3234567890123",
                    ProductName = "T-Shirt",
                    Description = "Cotton t-shirt, various sizes",
                    CategoryID = clothingCategory.CategoryID,
                    CostPrice = 8.00m,
                    SellPrice = 19.99m,
                    TaxRate = 10.0m,
                    CurrentStock = 60,
                    MinStockLevel = 20,
                    ReorderPoint = 25,
                    UnitOfMeasure = "piece",
                    SupplierID = null,
                    IsActive = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            };

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }

        // Seed Walk-in Customer
        if (!await context.Customers.AnyAsync())
        {
            var walkInCustomer = new Customer
            {
                CustomerCode = "WALK-IN",
                FirstName = "Walk-in",
                LastName = "Customer",
                IsActive = true,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            context.Customers.Add(walkInCustomer);
            await context.SaveChangesAsync();
        }
    }
}
