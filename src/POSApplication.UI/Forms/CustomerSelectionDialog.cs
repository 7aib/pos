using POSApplication.Core.Entities;
using POSApplication.Data.Interfaces;
using POSApplication.UI.Theme;
using System.Data;
using Microsoft.Extensions.DependencyInjection;

namespace POSApplication.UI.Forms;

public partial class CustomerSelectionDialog : Form
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IServiceProvider _serviceProvider;
    public Customer? SelectedCustomer { get; private set; }

    public CustomerSelectionDialog(ICustomerRepository customerRepository, IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _customerRepository = customerRepository;
        _serviceProvider = serviceProvider;
        ApplyTheme();
    }

    private void btnAddCustomer_Click(object? sender, EventArgs e) 
    {
        using var scope = _serviceProvider.CreateScope();
        var editDialog = scope.ServiceProvider.GetRequiredService<CustomerEditDialog>();
        
        if (editDialog.ShowDialog() == DialogResult.OK)
        {
             // Clear search and reload
             txtSearch.Text = ""; 
             PerformSearch().Wait(); // Or async void
        }
    }

    private void ApplyTheme()
    {
        this.BackColor = AppTheme.BackgroundColor;
        
        AppTheme.ApplyButtonTheme(btnSearch, AppTheme.PrimaryColor);
        AppTheme.ApplyButtonTheme(btnAddCustomer, AppTheme.PrimaryColor);
        AppTheme.ApplyButtonTheme(btnSelect, AppTheme.SuccessColor);
        AppTheme.ApplySecondaryButtonTheme(btnCancel);

        dgvCustomers.BackgroundColor = Color.White;
        dgvCustomers.BorderStyle = BorderStyle.None;
    }

    private async void btnSearch_Click(object sender, EventArgs e)
    {
        await PerformSearch();
    }

    private async void txtSearch_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            await PerformSearch();
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
    }

    private async Task PerformSearch()
    {
        var searchTerm = txtSearch.Text.Trim();
        var customers = await _customerRepository.SearchCustomersAsync(searchTerm);

        dgvCustomers.DataSource = customers.Select(c => new 
        {
            c.CustomerID,
            c.FirstName,
            c.LastName,
            c.Phone,
            c.Email,
            CreditLimit = c.CreditLimit
        }).ToList();
    }

    private void btnSelect_Click(object sender, EventArgs e)
    {
        SelectCustomer();
    }

    private void dgvCustomers_DoubleClick(object sender, EventArgs e)
    {
        SelectCustomer();
    }

    private async void SelectCustomer()
    {
        if (dgvCustomers.SelectedRows.Count > 0)
        {
            var customerId = (int)dgvCustomers.SelectedRows[0].Cells["CustomerID"].Value;
            // Get full customer object details with credit account? 
            // SearchCustomersAsync might not include CreditAccount.
            // But we can fetch it when used in checkout. 
            // For now, assume this is enough or fetch here.
            
            // Let's rely on caller to refetch if needed, or pass full object from here.
            // Getting fresh from repo is safer.
            SelectedCustomer = await _customerRepository.GetCustomerWithCreditAccountAsync(customerId);
            
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        else
        {
            MessageBox.Show("Please select a customer.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
