using Microsoft.Extensions.DependencyInjection;
using POSApplication.Data.Context;
using POSApplication.Data.Seed;
using POSApplication.UI.Configuration;
using POSApplication.UI.Forms;
using POSApplication.Core.Interfaces;
using Serilog;

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
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File("logs/pos-.log", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
            .CreateLogger();

        try
        {
            Log.Information("Starting POS Application");

            ApplicationConfiguration.Initialize();

            ServiceProvider = DependencyInjection.ConfigureServices();

            InitializeDatabase();

            bool userRequestedLogout = true;
            while (userRequestedLogout)
            {
                userRequestedLogout = false;

                var loginForm = ServiceProvider.GetRequiredService<LoginForm>();

                if (loginForm.ShowDialog() == DialogResult.OK && loginForm.AuthenticatedUser != null)
                {
                    var currentUserService = ServiceProvider.GetRequiredService<ICurrentUserService>();
                    currentUserService.SetCurrentUser(loginForm.AuthenticatedUser);

                    var mainForm = new MainForm(ServiceProvider, loginForm.AuthenticatedUser);
                    Application.Run(mainForm);

                    if (mainForm.IsLogout)
                    {
                        userRequestedLogout = true;
                        currentUserService.ClearCurrentUser();
                    }
                }
                else
                {
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
            MessageBox.Show($"Fatal error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static void InitializeDatabase()
    {
        try
        {
            using var scope = ServiceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<POSDbContext>();

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
