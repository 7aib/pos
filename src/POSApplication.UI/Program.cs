using Microsoft.Extensions.DependencyInjection;
using POSApplication.Data.Context;
using POSApplication.Data.Seed;
using POSApplication.UI.Configuration;
using POSApplication.UI.Forms;

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

        // Resolve LoginForm
        var loginForm = ServiceProvider.GetRequiredService<LoginForm>();

        if (loginForm.ShowDialog() == DialogResult.OK && loginForm.AuthenticatedUser != null)
        {
            // Run the main application
            // Note: We're resolving MainForm from DI now to support injection
            // We need to pass the user, so we can't use DI to resolve MainForm directly if it requires User in constructor
            // unless we register User as a scoped service, which is a bit tricky with Transient Forms.
            // Simpler to just manually instantiate MainForm here.
            
            var mainForm = new MainForm(ServiceProvider, loginForm.AuthenticatedUser);
            Application.Run(mainForm); 
        }
        else
        {
            Application.Exit();
        }
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
