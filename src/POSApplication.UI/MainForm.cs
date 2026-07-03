using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using POSApplication.Core.Interfaces;
using POSApplication.Infrastructure.Interfaces;
using POSApplication.Infrastructure.Services;
using POSApplication.Core.Entities;
using POSApplication.Data.Interfaces;
using POSApplication.UI.Forms;

namespace POSApplication.UI;

public partial class MainForm : Form
{
    private readonly User _currentUser;
    private readonly IServiceProvider _serviceProvider;
    public bool IsLogout { get; private set; } = false;

    public MainForm(IServiceProvider serviceProvider, User currentUser)
    {
        _serviceProvider = serviceProvider;
        _currentUser = currentUser;
        InitializeComponent();
        this.Text = "POS Application";
        
        toolStripStatusUser.Text = $"User: {_currentUser.FullName} ({_currentUser.Role})";
        
        // Apply permissions
        if (_currentUser.Role != POSApplication.Common.Enums.UserRole.Admin)
        {
            settingsToolStripMenuItem.Visible = false;
            manageUsersToolStripMenuItem.Visible = false;
            // "Cashier can only add new customers" implies they shouldn't access full management
            // So we hide the top-level Customers menu which allows full CRUD
            customersToolStripMenuItem.Visible = false; 
        }
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Application.Exit();
    }

    private void changePasswordToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var userService = _serviceProvider.GetRequiredService<IUserService>();
        var dialog = new ChangePasswordDialog(userService, _currentUser.UserID);
        dialog.ShowDialog();
    }

    private void logoutToolStripMenuItem_Click(object sender, EventArgs e)
    {
        IsLogout = true;
        this.Close();
    }

    private void salesToolStripMenuItem_Click(object sender, EventArgs e)
    {
        // Open POS Checkout form
        var productService = _serviceProvider.GetRequiredService<IProductService>();
        var salesService = _serviceProvider.GetRequiredService<ISalesService>();
        var paymentService = _serviceProvider.GetRequiredService<IPaymentService>();
        var printerService = _serviceProvider.GetRequiredService<IPrinterService>();
        var inventoryService = _serviceProvider.GetRequiredService<IInventoryService>();
        var creditService = _serviceProvider.GetRequiredService<ICreditService>();
        var customerRepository = _serviceProvider.GetRequiredService<ICustomerRepository>();
        var discountService = _serviceProvider.GetService<IDiscountService>();

        var checkoutForm = new POSCheckoutForm(
            productService, 
            salesService, 
            paymentService,
            creditService, 
            printerService,
            inventoryService,
            customerRepository,
            _serviceProvider,
            _currentUser,
            discountService);
        checkoutForm.ShowDialog();
    }

    private void processReturnToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var returnService = _serviceProvider.GetRequiredService<IReturnService>();
        var form = new ReturnForm(returnService, _currentUser.UserID);
        form.ShowDialog();
    }

    private void customersToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var customerService = _serviceProvider.GetRequiredService<ICustomerService>();
        // Check if we need to pass IServiceProvider to the form (based on Constructor)
        var customerManagementForm = new CustomerManagementForm(customerService, _serviceProvider);
        customerManagementForm.ShowDialog();
    }

    private void productsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        // Open Product Management form
        var productService = _serviceProvider.GetRequiredService<IProductService>();
        var categoryService = _serviceProvider.GetRequiredService<ICategoryService>();
        var productManagementForm = new ProductManagementForm(productService);
        productManagementForm.ShowDialog();
    }

    private void categoriesToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var categoryService = _serviceProvider.GetRequiredService<ICategoryService>();
        var form = new CategoryManagementForm(categoryService);
        form.ShowDialog();
    }

    private void suppliersToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var supplierService = _serviceProvider.GetRequiredService<ISupplierService>();
        var form = new SupplierManagementForm(supplierService);
        form.ShowDialog();
    }

    private void reportsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var reportingService = _serviceProvider.GetRequiredService<IReportingService>();
        var reportsForm = new ReportsForm(reportingService);
        reportsForm.ShowDialog();
    }

    private void manageUsersToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var userService = _serviceProvider.GetRequiredService<IUserService>();
        var userForm = new UserManagementForm(userService);
        userForm.ShowDialog();
    }

    private void storeSettingsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var configuration = _serviceProvider.GetRequiredService<IConfiguration>();
        var settingsForm = new SettingsForm(configuration);
        settingsForm.ShowDialog();
    }

    private void discountsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        var discountService = _serviceProvider.GetRequiredService<IDiscountService>();
        var form = new DiscountManagementForm(discountService);
        form.ShowDialog();
    }
}
