using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using POSApplication.Core.Interfaces;
using POSApplication.Data.Context;
using POSApplication.Data.Entities;
using POSApplication.Data.Interfaces;
using POSApplication.Data.Repositories;
using POSApplication.Infrastructure.Interfaces;
using POSApplication.Infrastructure.Services;

namespace POSApplication.UI.Configuration;

/// <summary>
/// Dependency injection configuration
/// </summary>
public static class DependencyInjection
{
    public static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // Configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        // DbContext
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<POSDbContext>(options =>
            options.UseSqlite(connectionString));

        // Repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ISaleRepository, SaleRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();

        // Services
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ISalesService, SalesService>();
        services.AddScoped<IInventoryService, InventoryService>();
        services.AddScoped<IPaymentService, PaymentService>();

        // Infrastructure Services
        services.AddSingleton<IPrinterService, ThermalPrinterService>();
        services.AddSingleton<IBarcodeService, BarcodeService>();

        return services.BuildServiceProvider();
    }
}
