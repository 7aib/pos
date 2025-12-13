using Microsoft.Extensions.DependencyInjection;
using POSApplication.Core.Interfaces;
using POSApplication.Infrastructure.Interfaces;
using POSApplication.UI.Forms;

namespace POSApplication.UI;

public partial class MainForm : Form
{
    private readonly IServiceProvider _serviceProvider;

    public MainForm(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        InitializeComponent();
        this.Text = "POS Application - Phase 2";
    }

    private void exitToolStripMenuItem_Click(object sender, EventArgs e)
    {
        Application.Exit();
    }

    private void salesToolStripMenuItem_Click(object sender, EventArgs e)
    {
        // Open POS Checkout form
        var productService = _serviceProvider.GetRequiredService<IProductService>();
        var salesService = _serviceProvider.GetRequiredService<ISalesService>();
        var paymentService = _serviceProvider.GetRequiredService<IPaymentService>();
        var printerService = _serviceProvider.GetRequiredService<IPrinterService>();
        var inventoryService = _serviceProvider.GetRequiredService<IInventoryService>();

        var checkoutForm = new POSCheckoutForm(
            productService, 
            salesService, 
            paymentService, 
            printerService,
            inventoryService);
        checkoutForm.ShowDialog();
    }

    private void productsToolStripMenuItem_Click(object sender, EventArgs e)
    {
        // Open Product Management form
        var productService = _serviceProvider.GetRequiredService<IProductService>();
        var productManagementForm = new ProductManagementForm(productService);
        productManagementForm.ShowDialog();
    }
}
