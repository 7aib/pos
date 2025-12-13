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

        bool userRequestedLogout = true;
        while (userRequestedLogout)
        {
            userRequestedLogout = false;
            
            // Resolve LoginForm - Create a new scope or use existing? 
            // Since LoginForm is transient, we can resolve it.
            var loginForm = ServiceProvider.GetRequiredService<LoginForm>();

            if (loginForm.ShowDialog() == DialogResult.OK && loginForm.AuthenticatedUser != null)
            {
                var mainForm = new MainForm(ServiceProvider, loginForm.AuthenticatedUser);
                Application.Run(mainForm);
                
                if (mainForm.IsLogout)
                {
                    userRequestedLogout = true;
                }
            }
            else
            {
                // User closed Login form or failed login
                break; 
            }
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
