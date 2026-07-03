using POSApplication.Core.DTOs;
using POSApplication.Core.Interfaces;
using POSApplication.UI.Theme;

namespace POSApplication.UI.Forms;

public class CategoryEditDialog : Form
{
    private readonly ICategoryService _categoryService;
    private readonly CategoryDto? _existing;
    private TextBox _txtName;
    private TextBox _txtDescription;

    public CategoryEditDialog(ICategoryService categoryService)
    {
        _categoryService = categoryService;
        InitializeComponent();
        this.Text = "Add Category";
    }

    public CategoryEditDialog(ICategoryService categoryService, CategoryDto existing)
    {
        _categoryService = categoryService;
        _existing = existing;
        InitializeComponent();
        this.Text = "Edit Category";
        LoadData();
    }

    private void InitializeComponent()
    {
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.StartPosition = FormStartPosition.CenterParent;
        this.Size = new Size(400, 250);
        this.BackColor = AppTheme.BackgroundColor;

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(20), RowCount = 4, ColumnCount = 2 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        layout.Controls.Add(new Label { Text = "Category Name: *", Font = AppTheme.BodyFont, Anchor = AnchorStyles.Left }, 0, 0);
        _txtName = new TextBox { Width = 220, Font = AppTheme.BodyFont };
        layout.Controls.Add(_txtName, 1, 0);

        layout.Controls.Add(new Label { Text = "Description:", Font = AppTheme.BodyFont, Anchor = AnchorStyles.Left }, 0, 1);
        _txtDescription = new TextBox { Width = 220, Height = 60, Multiline = true, Font = AppTheme.BodyFont };
        layout.Controls.Add(_txtDescription, 1, 1);

        var btnPanel = new FlowLayoutPanel { FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Bottom, Height = 50 };

        var btnSave = new Button { Text = "Save", Width = 100, Height = 35 };
        AppTheme.ApplyButtonTheme(btnSave, AppTheme.SuccessColor);
        btnSave.Click += BtnSave_Click;

        var btnCancel = new Button { Text = "Cancel", Width = 100, Height = 35 };
        AppTheme.ApplySecondaryButtonTheme(btnCancel);
        btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

        btnPanel.Controls.Add(btnSave);
        btnPanel.Controls.Add(btnCancel);

        this.Controls.Add(layout);
        this.Controls.Add(btnPanel);
        this.AcceptButton = btnSave;
        this.CancelButton = btnCancel;
    }

    private void LoadData()
    {
        if (_existing == null) return;
        _txtName.Text = _existing.CategoryName;
        _txtDescription.Text = _existing.Description ?? string.Empty;
    }

    private async void BtnSave_Click(object? sender, EventArgs e)
    {
        var name = _txtName.Text.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            MessageBox.Show("Category name is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var dto = new CategoryDto
            {
                CategoryID = _existing?.CategoryID ?? 0,
                CategoryName = name,
                Description = _txtDescription.Text.Trim(),
                IsActive = true
            };

            if (_existing != null)
                await _categoryService.UpdateCategoryAsync(dto);
            else
                await _categoryService.CreateCategoryAsync(dto);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
