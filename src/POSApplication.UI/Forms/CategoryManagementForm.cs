using POSApplication.Core.DTOs;
using POSApplication.Core.Interfaces;
using POSApplication.UI.Theme;

namespace POSApplication.UI.Forms;

public class CategoryManagementForm : Form
{
    private readonly ICategoryService _categoryService;
    private DataGridView _gridCategories;
    private TextBox _txtSearch;
    private Button _btnAdd;
    private Button _btnEdit;
    private Button _btnDelete;
    private Button _btnRefresh;
    private Label _lblStatus;
    private List<CategoryDto> _categories = new();

    public CategoryManagementForm(ICategoryService categoryService)
    {
        _categoryService = categoryService;
        InitializeComponent();
        LoadCategories();
    }

    private void InitializeComponent()
    {
        this.Text = "Category Management";
        this.Size = new Size(800, 500);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = AppTheme.BackgroundColor;

        var panelTop = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = AppTheme.PanelColor, Padding = new Padding(10) };

        var lblSearch = new Label { Text = "Search:", Location = new Point(10, 18), Width = 60, Font = AppTheme.BodyFont };
        _txtSearch = new TextBox { Location = new Point(75, 15), Width = 250 };
        _txtSearch.TextChanged += TxtSearch_TextChanged;

        _btnRefresh = new Button { Text = "Refresh", Location = new Point(340, 13), Width = 80, Height = 30 };
        AppTheme.ApplySecondaryButtonTheme(_btnRefresh);
        _btnRefresh.Click += (s, e) => { _txtSearch.Clear(); LoadCategories(); };

        panelTop.Controls.AddRange(new Control[] { lblSearch, _txtSearch, _btnRefresh });

        var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 60, BackColor = AppTheme.PanelColor, Padding = new Padding(10) };

        _btnAdd = new Button { Text = "Add Category", Location = new Point(10, 13), Width = 120, Height = 35 };
        AppTheme.ApplyButtonTheme(_btnAdd, AppTheme.PrimaryColor);
        _btnAdd.Click += BtnAdd_Click;

        _btnEdit = new Button { Text = "Edit", Location = new Point(140, 13), Width = 100, Height = 35 };
        AppTheme.ApplySecondaryButtonTheme(_btnEdit);
        _btnEdit.Click += BtnEdit_Click;

        _btnDelete = new Button { Text = "Delete", Location = new Point(250, 13), Width = 100, Height = 35 };
        AppTheme.ApplySecondaryButtonTheme(_btnDelete);
        _btnDelete.ForeColor = AppTheme.DangerColor;
        _btnDelete.Click += BtnDelete_Click;

        _lblStatus = new Label { Text = "Ready", Location = new Point(500, 18), Width = 250, TextAlign = ContentAlignment.MiddleRight, Font = AppTheme.BodyFont };
        panelBottom.Controls.AddRange(new Control[] { _btnAdd, _btnEdit, _btnDelete, _lblStatus });

        _gridCategories = new DataGridView
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

        _gridCategories.Columns.AddRange(new DataGridViewColumn[]
        {
            new DataGridViewTextBoxColumn { Name = "CategoryID", HeaderText = "ID", DataPropertyName = "CategoryID", Width = 50 },
            new DataGridViewTextBoxColumn { Name = "CategoryName", HeaderText = "Category Name", DataPropertyName = "CategoryName", Width = 200 },
            new DataGridViewTextBoxColumn { Name = "Description", HeaderText = "Description", DataPropertyName = "Description", Width = 250 },
            new DataGridViewTextBoxColumn { Name = "ProductCount", HeaderText = "Products", DataPropertyName = "ProductCount", Width = 80 },
            new DataGridViewCheckBoxColumn { Name = "IsActive", HeaderText = "Active", DataPropertyName = "IsActive", Width = 60 }
        });

        _gridCategories.DoubleClick += (s, e) => BtnEdit_Click(s, e);

        this.Controls.Add(_gridCategories);
        this.Controls.Add(panelTop);
        this.Controls.Add(panelBottom);
    }

    private async void LoadCategories()
    {
        try
        {
            _lblStatus.Text = "Loading...";
            _categories = (await _categoryService.GetAllCategoriesAsync()).ToList();
            _gridCategories.DataSource = _categories;
            _lblStatus.Text = $"Total: {_categories.Count} categories";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            _lblStatus.Text = "Error loading";
        }
    }

    private async void TxtSearch_TextChanged(object? sender, EventArgs e)
    {
        var term = _txtSearch.Text.Trim();
        if (string.IsNullOrWhiteSpace(term))
        {
            _gridCategories.DataSource = _categories;
            return;
        }

        try
        {
            var results = _categories.Where(c =>
                c.CategoryName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                (c.Description != null && c.Description.Contains(term, StringComparison.OrdinalIgnoreCase))).ToList();
            _gridCategories.DataSource = results;
            _lblStatus.Text = $"Found: {results.Count}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error searching: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void BtnAdd_Click(object? sender, EventArgs e)
    {
        var dialog = new CategoryEditDialog(_categoryService);
        if (dialog.ShowDialog() == DialogResult.OK)
            LoadCategories();
    }

    private async void BtnEdit_Click(object? sender, EventArgs e)
    {
        if (_gridCategories.SelectedRows.Count == 0)
        {
            MessageBox.Show("Please select a category to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var selected = _gridCategories.SelectedRows[0].DataBoundItem as CategoryDto;
        if (selected == null) return;

        var dialog = new CategoryEditDialog(_categoryService, selected);
        if (dialog.ShowDialog() == DialogResult.OK)
            LoadCategories();
    }

    private async void BtnDelete_Click(object? sender, EventArgs e)
    {
        if (_gridCategories.SelectedRows.Count == 0)
        {
            MessageBox.Show("Please select a category to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var selected = _gridCategories.SelectedRows[0].DataBoundItem as CategoryDto;
        if (selected == null) return;

        if (MessageBox.Show($"Delete category '{selected.CategoryName}'?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
        {
            try
            {
                await _categoryService.DeleteCategoryAsync(selected.CategoryID);
                LoadCategories();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting category: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
