using POSApplication.Core.DTOs;
using POSApplication.Core.Interfaces;
using POSApplication.UI.Theme;

namespace POSApplication.UI.Forms;

public class DiscountManagementForm : Form
{
    private readonly IDiscountService _discountService;
    private DataGridView _gridDiscounts;
    private TextBox _txtSearch;
    private Button _btnAdd;
    private Button _btnEdit;
    private Button _btnDelete;
    private Button _btnRefresh;
    private Label _lblStatus;
    private List<DiscountDto> _discounts = new();

    public DiscountManagementForm(IDiscountService discountService)
    {
        _discountService = discountService;
        InitializeComponent();
        LoadDiscounts();
    }

    private void InitializeComponent()
    {
        this.Text = "Discount Management";
        this.Size = new Size(900, 500);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = AppTheme.BackgroundColor;

        var panelTop = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = AppTheme.PanelColor, Padding = new Padding(10) };

        var lblSearch = new Label { Text = "Search:", Location = new Point(10, 18), Width = 60, Font = AppTheme.BodyFont };
        _txtSearch = new TextBox { Location = new Point(75, 15), Width = 250 };
        _txtSearch.TextChanged += TxtSearch_TextChanged;

        _btnRefresh = new Button { Text = "Refresh", Location = new Point(340, 13), Width = 80, Height = 30 };
        AppTheme.ApplySecondaryButtonTheme(_btnRefresh);
        _btnRefresh.Click += (s, e) => { _txtSearch.Clear(); LoadDiscounts(); };

        panelTop.Controls.AddRange(new Control[] { lblSearch, _txtSearch, _btnRefresh });

        var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 60, BackColor = AppTheme.PanelColor, Padding = new Padding(10) };

        _btnAdd = new Button { Text = "Add Discount", Location = new Point(10, 13), Width = 120, Height = 35 };
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

        _gridDiscounts = new DataGridView
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

        _gridDiscounts.Columns.AddRange(new DataGridViewColumn[]
        {
            new DataGridViewTextBoxColumn { Name = "DiscountID", HeaderText = "ID", DataPropertyName = "DiscountID", Width = 50 },
            new DataGridViewTextBoxColumn { Name = "Code", HeaderText = "Code", DataPropertyName = "Code", Width = 120 },
            new DataGridViewTextBoxColumn { Name = "Description", HeaderText = "Description", DataPropertyName = "Description", Width = 200 },
            new DataGridViewTextBoxColumn { Name = "Type", HeaderText = "Type", DataPropertyName = "Type", Width = 100 },
            new DataGridViewTextBoxColumn { Name = "Value", HeaderText = "Value", DataPropertyName = "Value", Width = 80 },
            new DataGridViewTextBoxColumn { Name = "StartDate", HeaderText = "Start", DataPropertyName = "StartDate", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "d" } },
            new DataGridViewTextBoxColumn { Name = "EndDate", HeaderText = "End", DataPropertyName = "EndDate", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "d" } },
            new DataGridViewCheckBoxColumn { Name = "IsActive", HeaderText = "Active", DataPropertyName = "IsActive", Width = 60 }
        });

        _gridDiscounts.DoubleClick += (s, e) => BtnEdit_Click(s, e);

        this.Controls.Add(_gridDiscounts);
        this.Controls.Add(panelTop);
        this.Controls.Add(panelBottom);
    }

    private async void LoadDiscounts()
    {
        try
        {
            _lblStatus.Text = "Loading...";
            _discounts = (await _discountService.GetAllDiscountsAsync()).ToList();
            _gridDiscounts.DataSource = _discounts;
            _lblStatus.Text = $"Total: {_discounts.Count} discounts";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading discounts: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            _lblStatus.Text = "Error loading";
        }
    }

    private void TxtSearch_TextChanged(object? sender, EventArgs e)
    {
        var term = _txtSearch.Text.Trim();
        if (string.IsNullOrWhiteSpace(term))
        {
            _gridDiscounts.DataSource = _discounts;
            return;
        }

        var results = _discounts.Where(d =>
            d.Code.Contains(term, StringComparison.OrdinalIgnoreCase) ||
            d.Description.Contains(term, StringComparison.OrdinalIgnoreCase)).ToList();
        _gridDiscounts.DataSource = results;
        _lblStatus.Text = $"Found: {results.Count}";
    }

    private async void BtnAdd_Click(object? sender, EventArgs e)
    {
        var dialog = new DiscountEditDialog(_discountService);
        if (dialog.ShowDialog() == DialogResult.OK)
            LoadDiscounts();
    }

    private async void BtnEdit_Click(object? sender, EventArgs e)
    {
        if (_gridDiscounts.SelectedRows.Count == 0)
        {
            MessageBox.Show("Please select a discount to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var selected = _gridDiscounts.SelectedRows[0].DataBoundItem as DiscountDto;
        if (selected == null) return;

        var dialog = new DiscountEditDialog(_discountService, selected);
        if (dialog.ShowDialog() == DialogResult.OK)
            LoadDiscounts();
    }

    private async void BtnDelete_Click(object? sender, EventArgs e)
    {
        if (_gridDiscounts.SelectedRows.Count == 0)
        {
            MessageBox.Show("Please select a discount to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var selected = _gridDiscounts.SelectedRows[0].DataBoundItem as DiscountDto;
        if (selected == null) return;

        if (MessageBox.Show($"Delete discount '{selected.Code}'?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            try
            {
                await _discountService.DeleteDiscountAsync(selected.DiscountID);
                LoadDiscounts();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting discount: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
