using POSApplication.Core.DTOs;
using POSApplication.Core.Interfaces;
using POSApplication.UI.Theme;

namespace POSApplication.UI.Forms;

public class SupplierManagementForm : Form
{
    private readonly ISupplierService _supplierService;
    private DataGridView _gridSuppliers;
    private TextBox _txtSearch;
    private Button _btnAdd;
    private Button _btnEdit;
    private Button _btnDelete;
    private Button _btnRefresh;
    private Label _lblStatus;
    private List<SupplierDto> _suppliers = new();

    public SupplierManagementForm(ISupplierService supplierService)
    {
        _supplierService = supplierService;
        InitializeComponent();
        LoadSuppliers();
    }

    private void InitializeComponent()
    {
        this.Text = "Supplier Management";
        this.Size = new Size(900, 500);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = AppTheme.BackgroundColor;

        var panelTop = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = AppTheme.PanelColor, Padding = new Padding(10) };

        var lblSearch = new Label { Text = "Search:", Location = new Point(10, 18), Width = 60, Font = AppTheme.BodyFont };
        _txtSearch = new TextBox { Location = new Point(75, 15), Width = 250 };
        _txtSearch.TextChanged += TxtSearch_TextChanged;

        _btnRefresh = new Button { Text = "Refresh", Location = new Point(340, 13), Width = 80, Height = 30 };
        AppTheme.ApplySecondaryButtonTheme(_btnRefresh);
        _btnRefresh.Click += (s, e) => { _txtSearch.Clear(); LoadSuppliers(); };

        panelTop.Controls.AddRange(new Control[] { lblSearch, _txtSearch, _btnRefresh });

        var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 60, BackColor = AppTheme.PanelColor, Padding = new Padding(10) };

        _btnAdd = new Button { Text = "Add Supplier", Location = new Point(10, 13), Width = 120, Height = 35 };
        AppTheme.ApplyButtonTheme(_btnAdd, AppTheme.PrimaryColor);
        _btnAdd.Click += BtnAdd_Click;

        _btnEdit = new Button { Text = "Edit", Location = new Point(140, 13), Width = 100, Height = 35 };
        AppTheme.ApplySecondaryButtonTheme(_btnEdit);
        _btnEdit.Click += BtnEdit_Click;

        _btnDelete = new Button { Text = "Delete", Location = new Point(250, 13), Width = 100, Height = 35 };
        AppTheme.ApplySecondaryButtonTheme(_btnDelete);
        _btnDelete.ForeColor = AppTheme.DangerColor;
        _btnDelete.Click += BtnDelete_Click;

        _lblStatus = new Label { Text = "Ready", Location = new Point(600, 18), Width = 250, TextAlign = ContentAlignment.MiddleRight, Font = AppTheme.BodyFont };
        panelBottom.Controls.AddRange(new Control[] { _btnAdd, _btnEdit, _btnDelete, _lblStatus });

        _gridSuppliers = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoGenerateColumns = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            RowHeadersVisible = false,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None
        };

        _gridSuppliers.Columns.AddRange(new DataGridViewColumn[]
        {
            new DataGridViewTextBoxColumn { Name = "SupplierID", HeaderText = "ID", DataPropertyName = "SupplierID", Width = 50 },
            new DataGridViewTextBoxColumn { Name = "SupplierName", HeaderText = "Supplier Name", DataPropertyName = "SupplierName", Width = 200 },
            new DataGridViewTextBoxColumn { Name = "ContactPerson", HeaderText = "Contact", DataPropertyName = "ContactPerson", Width = 150 },
            new DataGridViewTextBoxColumn { Name = "Email", HeaderText = "Email", DataPropertyName = "Email", Width = 180 },
            new DataGridViewTextBoxColumn { Name = "Phone", HeaderText = "Phone", DataPropertyName = "Phone", Width = 120 },
            new DataGridViewTextBoxColumn { Name = "ProductCount", HeaderText = "Products", DataPropertyName = "ProductCount", Width = 80 },
            new DataGridViewCheckBoxColumn { Name = "IsActive", HeaderText = "Active", DataPropertyName = "IsActive", Width = 60 }
        });

        _gridSuppliers.DoubleClick += (s, e) => BtnEdit_Click(s, e);

        this.Controls.Add(_gridSuppliers);
        this.Controls.Add(panelTop);
        this.Controls.Add(panelBottom);
    }

    private async void LoadSuppliers()
    {
        try
        {
            _lblStatus.Text = "Loading...";
            _suppliers = (await _supplierService.GetAllSuppliersAsync()).ToList();
            _gridSuppliers.DataSource = _suppliers;
            _lblStatus.Text = $"Total: {_suppliers.Count} suppliers";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading suppliers: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            _lblStatus.Text = "Error loading";
        }
    }

    private async void TxtSearch_TextChanged(object? sender, EventArgs e)
    {
        var term = _txtSearch.Text.Trim();
        if (string.IsNullOrWhiteSpace(term))
        {
            _gridSuppliers.DataSource = _suppliers;
            return;
        }

        try
        {
            var results = _suppliers.Where(s =>
                s.SupplierName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                (s.ContactPerson != null && s.ContactPerson.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                (s.Email != null && s.Email.Contains(term, StringComparison.OrdinalIgnoreCase)) ||
                (s.Phone != null && s.Phone.Contains(term, StringComparison.OrdinalIgnoreCase))).ToList();
            _gridSuppliers.DataSource = results;
            _lblStatus.Text = $"Found: {results.Count}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error searching: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void BtnAdd_Click(object? sender, EventArgs e)
    {
        var dialog = new SupplierEditDialog(_supplierService);
        if (dialog.ShowDialog() == DialogResult.OK)
            LoadSuppliers();
    }

    private async void BtnEdit_Click(object? sender, EventArgs e)
    {
        if (_gridSuppliers.SelectedRows.Count == 0)
        {
            MessageBox.Show("Please select a supplier to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var selected = _gridSuppliers.SelectedRows[0].DataBoundItem as SupplierDto;
        if (selected == null) return;

        var dialog = new SupplierEditDialog(_supplierService, selected);
        if (dialog.ShowDialog() == DialogResult.OK)
            LoadSuppliers();
    }

    private async void BtnDelete_Click(object? sender, EventArgs e)
    {
        if (_gridSuppliers.SelectedRows.Count == 0)
        {
            MessageBox.Show("Please select a supplier to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var selected = _gridSuppliers.SelectedRows[0].DataBoundItem as SupplierDto;
        if (selected == null) return;

        if (MessageBox.Show($"Delete supplier '{selected.SupplierName}'?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            try
            {
                await _supplierService.DeleteSupplierAsync(selected.SupplierID);
                LoadSuppliers();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting supplier: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
