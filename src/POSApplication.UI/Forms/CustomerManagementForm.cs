using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using POSApplication.Core.DTOs;
using POSApplication.Core.Interfaces;

namespace POSApplication.UI.Forms;

public class CustomerManagementForm : Form
{
    // We don't use the injected service directly to avoid stale data issues (EF Core caching)
    // private readonly ICustomerService _customerService; 
    private readonly IServiceProvider _serviceProvider;
    private DataGridView _customerGrid;
    private TextBox _searchBox;
    private Button _btnAdd;
    private Button _btnEdit;
    private Button _btnDelete;
    private Button _btnRefresh;

    public CustomerManagementForm(ICustomerService customerService, IServiceProvider serviceProvider)
    {
        // _customerService = customerService;
        _serviceProvider = serviceProvider;
        InitializeComponent();
        LoadCustomers();
    }

    private void InitializeComponent()
    {
        this.Text = "Customer Management";
        this.Size = new Size(1000, 800);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);

        // Search Panel
        var topPanel = new Panel { Dock = DockStyle.Top, Height = 60, Padding = new Padding(15) };
        
        var lblSearch = new Label { Text = "Search:", AutoSize = true, Location = new Point(15, 20) };
        _searchBox = new TextBox { Location = new Point(90, 17), Width = 300, PlaceholderText = "Search by Name, Phone, or Email" };
        _searchBox.TextChanged += async (s, e) => await SearchCustomers(_searchBox.Text);

        _btnAdd = new Button { Text = "Add Customer", Location = new Point(490, 15), Width = 150, Height = 30, BackColor = Color.FromArgb(0, 122, 204), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        _btnAdd.Click += BtnAdd_Click;

        _btnEdit = new Button { Text = "Edit", Location = new Point(650, 15), Width = 100, Height = 30, BackColor = Color.FromArgb(240, 240, 240), FlatStyle = FlatStyle.Flat };
        _btnEdit.Click += BtnEdit_Click;

        _btnDelete = new Button { Text = "Delete", Location = new Point(760, 15), Width = 100, Height = 30, BackColor = Color.IndianRed, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        _btnDelete.Click += BtnDelete_Click;
        
        _btnRefresh = new Button { Text = "Refresh", Location = new Point(870, 15), Width = 100, Height = 30, BackColor = Color.Lavender, FlatStyle = FlatStyle.Flat };
        _btnRefresh.Click += async (s, e) => { _searchBox.Text = string.Empty; await LoadCustomers(); };

        topPanel.Controls.Add(lblSearch);
        topPanel.Controls.Add(_searchBox);
        topPanel.Controls.Add(_btnAdd);
        topPanel.Controls.Add(_btnEdit);
        topPanel.Controls.Add(_btnDelete);
        topPanel.Controls.Add(_btnRefresh);

        // Grid
        _customerGrid = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            ReadOnly = true,
            AllowUserToAddRows = false,
            RowHeadersVisible = false,
            BackgroundColor = Color.White
        };
        _customerGrid.Columns.Add("Id", "Id");
        _customerGrid.Columns["Id"].Visible = false;
        
        _customerGrid.Columns.Add("Name", "Name");
        _customerGrid.Columns.Add("Phone", "Phone");
        _customerGrid.Columns.Add("Email", "Email");
        _customerGrid.Columns.Add("LoyaltyPoints", "Loyalty Points");
        _customerGrid.Columns.Add("CreditLimit", "Credit Limit");

        this.Controls.Add(_customerGrid);
        this.Controls.Add(topPanel);
    }

    private async Task LoadCustomers()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var customerService = scope.ServiceProvider.GetRequiredService<ICustomerService>();
            var customers = await customerService.GetAllCustomersAsync();
            BindGrid(customers);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading customers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task SearchCustomers(string searchTerm)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var customerService = scope.ServiceProvider.GetRequiredService<ICustomerService>();
            
            IEnumerable<CustomerDto> customers;
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                customers = await customerService.GetAllCustomersAsync();
            }
            else
            {
                customers = await customerService.SearchCustomersAsync(searchTerm);
            }
            BindGrid(customers);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error searching customers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BindGrid(IEnumerable<CustomerDto> customers)
    {
        _customerGrid.Rows.Clear();
        foreach (var c in customers.Where(c => c.IsActive))
        {
            _customerGrid.Rows.Add(c.CustomerID, c.FullName, c.Phone, c.Email, c.LoyaltyPoints, c.CreditLimit.ToString("C"));
        }
    }

    private async void BtnAdd_Click(object? sender, EventArgs e)
    {
        using var scope = _serviceProvider.CreateScope();
        var editDialog = scope.ServiceProvider.GetRequiredService<CustomerEditDialog>();
        
        if (editDialog.ShowDialog() == DialogResult.OK)
        {
            await LoadCustomers();
        }
    }

    private async void BtnEdit_Click(object? sender, EventArgs e)
    {
        if (_customerGrid.SelectedRows.Count == 0)
        {
            MessageBox.Show("Please select a customer to edit.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var id = (int)_customerGrid.SelectedRows[0].Cells["Id"].Value;
        
        using var scope = _serviceProvider.CreateScope();
        var customerService = scope.ServiceProvider.GetRequiredService<ICustomerService>();
        var customer = await customerService.GetCustomerByIdAsync(id);

        if (customer != null)
        {
            // Note: We use the same scope for dialog to ensure context consistency if needed, 
            // OR we can resolve dialog from same scope.
            var editDialog = scope.ServiceProvider.GetRequiredService<CustomerEditDialog>();
            editDialog.SetCustomer(customer);

            if (editDialog.ShowDialog() == DialogResult.OK)
            {
                // Refresh grid after update
                await LoadCustomers();
            }
        }
    }

    private async void BtnDelete_Click(object? sender, EventArgs e)
    {
        if (_customerGrid.SelectedRows.Count == 0)
        {
            MessageBox.Show("Please select a customer to delete.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var id = (int)_customerGrid.SelectedRows[0].Cells["Id"].Value;
        var name = _customerGrid.SelectedRows[0].Cells["Name"].Value.ToString();

        if (MessageBox.Show($"Are you sure you want to delete customer '{name}'?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var customerService = scope.ServiceProvider.GetRequiredService<ICustomerService>();
                await customerService.DeleteCustomerAsync(id);
                await LoadCustomers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting customer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
