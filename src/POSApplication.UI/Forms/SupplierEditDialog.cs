using POSApplication.Core.DTOs;
using POSApplication.Core.Interfaces;
using POSApplication.UI.Theme;

namespace POSApplication.UI.Forms;

public class SupplierEditDialog : Form
{
    private readonly ISupplierService _supplierService;
    private readonly SupplierDto? _existing;
    private TextBox _txtName;
    private TextBox _txtContact;
    private TextBox _txtEmail;
    private TextBox _txtPhone;
    private TextBox _txtAddress;

    public SupplierEditDialog(ISupplierService supplierService)
    {
        _supplierService = supplierService;
        InitializeComponent();
        this.Text = "Add Supplier";
    }

    public SupplierEditDialog(ISupplierService supplierService, SupplierDto existing)
    {
        _supplierService = supplierService;
        _existing = existing;
        InitializeComponent();
        this.Text = "Edit Supplier";
        LoadData();
    }

    private void InitializeComponent()
    {
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.StartPosition = FormStartPosition.CenterParent;
        this.Size = new Size(450, 380);
        this.BackColor = AppTheme.BackgroundColor;

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(20), RowCount = 6, ColumnCount = 2 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        layout.Controls.Add(new Label { Text = "Supplier Name: *", Font = AppTheme.BodyFont, Anchor = AnchorStyles.Left }, 0, 0);
        _txtName = new TextBox { Width = 260, Font = AppTheme.BodyFont };
        layout.Controls.Add(_txtName, 1, 0);

        layout.Controls.Add(new Label { Text = "Contact Person:", Font = AppTheme.BodyFont, Anchor = AnchorStyles.Left }, 0, 1);
        _txtContact = new TextBox { Width = 260, Font = AppTheme.BodyFont };
        layout.Controls.Add(_txtContact, 1, 1);

        layout.Controls.Add(new Label { Text = "Email:", Font = AppTheme.BodyFont, Anchor = AnchorStyles.Left }, 0, 2);
        _txtEmail = new TextBox { Width = 260, Font = AppTheme.BodyFont };
        layout.Controls.Add(_txtEmail, 1, 2);

        layout.Controls.Add(new Label { Text = "Phone:", Font = AppTheme.BodyFont, Anchor = AnchorStyles.Left }, 0, 3);
        _txtPhone = new TextBox { Width = 260, Font = AppTheme.BodyFont };
        layout.Controls.Add(_txtPhone, 1, 3);

        layout.Controls.Add(new Label { Text = "Address:", Font = AppTheme.BodyFont, Anchor = AnchorStyles.Left }, 0, 4);
        _txtAddress = new TextBox { Width = 260, Height = 60, Multiline = true, Font = AppTheme.BodyFont };
        layout.Controls.Add(_txtAddress, 1, 4);

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
        _txtName.Text = _existing.SupplierName;
        _txtContact.Text = _existing.ContactPerson ?? string.Empty;
        _txtEmail.Text = _existing.Email ?? string.Empty;
        _txtPhone.Text = _existing.Phone ?? string.Empty;
        _txtAddress.Text = _existing.Address ?? string.Empty;
    }

    private async void BtnSave_Click(object? sender, EventArgs e)
    {
        var name = _txtName.Text.Trim();
        if (string.IsNullOrWhiteSpace(name))
        {
            MessageBox.Show("Supplier name is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var dto = new SupplierDto
            {
                SupplierID = _existing?.SupplierID ?? 0,
                SupplierName = name,
                ContactPerson = _txtContact.Text.Trim(),
                Email = _txtEmail.Text.Trim(),
                Phone = _txtPhone.Text.Trim(),
                Address = _txtAddress.Text.Trim(),
                IsActive = true
            };

            if (_existing != null)
                await _supplierService.UpdateSupplierAsync(dto);
            else
                await _supplierService.CreateSupplierAsync(dto);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
