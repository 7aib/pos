using Microsoft.Extensions.DependencyInjection;
using POSApplication.Data.Context;
using POSApplication.Data.Seed;
using POSApplication.UI.Configuration;

namespace POSApplication.UI;

static class Program
{
    public static IServiceProvider ServiceProvider { get; private set; } = null!;

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        // Configure dependency injection
        ServiceProvider = DependencyInjection.ConfigureServices();

        // Initialize database and seed data
        InitializeDatabase();

        // Run the application
        Application.Run(new MainForm(ServiceProvider));
    }

    private static void InitializeDatabase()
    {
        try
        {
            using var scope = ServiceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<POSDbContext>();
            
            // Seed initial data
            SeedData.SeedAsync(dbContext).Wait();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error initializing database: {ex.Message}", 
                "Database Error", 
                MessageBoxButtons.OK, 
                MessageBoxIcon.Error);
        }
    }
}