using POSApplication.Core.DTOs;
using POSApplication.Core.Interfaces;
using POSApplication.UI.Theme;

namespace POSApplication.UI.Forms;

public class DiscountEditDialog : Form
{
    private readonly IDiscountService _discountService;
    private readonly DiscountDto? _existing;
    private TextBox _txtCode;
    private TextBox _txtDescription;
    private ComboBox _cmbType;
    private NumericUpDown _numValue;
    private NumericUpDown _numMaxDiscount;
    private NumericUpDown _numMinPurchase;
    private NumericUpDown _numUsageLimit;
    private DateTimePicker _dtpStart;
    private DateTimePicker _dtpEnd;
    private CheckBox _chkActive;

    public DiscountEditDialog(IDiscountService discountService)
    {
        _discountService = discountService;
        InitializeComponent();
        this.Text = "Add Discount";
    }

    public DiscountEditDialog(IDiscountService discountService, DiscountDto existing)
    {
        _discountService = discountService;
        _existing = existing;
        InitializeComponent();
        this.Text = "Edit Discount";
        LoadData();
    }

    private void InitializeComponent()
    {
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.StartPosition = FormStartPosition.CenterParent;
        this.Size = new Size(450, 480);
        this.BackColor = AppTheme.BackgroundColor;

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(20), RowCount = 10, ColumnCount = 2 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        layout.Controls.Add(new Label { Text = "Code: *", Font = AppTheme.BodyFont, Anchor = AnchorStyles.Left }, 0, 0);
        _txtCode = new TextBox { Width = 200, Font = AppTheme.BodyFont, CharacterCasing = CharacterCasing.Upper };
        layout.Controls.Add(_txtCode, 1, 0);

        layout.Controls.Add(new Label { Text = "Description:", Font = AppTheme.BodyFont, Anchor = AnchorStyles.Left }, 0, 1);
        _txtDescription = new TextBox { Width = 200, Font = AppTheme.BodyFont };
        layout.Controls.Add(_txtDescription, 1, 1);

        layout.Controls.Add(new Label { Text = "Type: *", Font = AppTheme.BodyFont, Anchor = AnchorStyles.Left }, 0, 2);
        _cmbType = new ComboBox { Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
        _cmbType.Items.AddRange(new object[] { "Percentage", "FixedAmount" });
        _cmbType.SelectedIndex = 0;
        layout.Controls.Add(_cmbType, 1, 2);

        layout.Controls.Add(new Label { Text = "Value: *", Font = AppTheme.BodyFont, Anchor = AnchorStyles.Left }, 0, 3);
        _numValue = new NumericUpDown { Width = 100, DecimalPlaces = 2, Maximum = 100000 };
        layout.Controls.Add(_numValue, 1, 3);

        layout.Controls.Add(new Label { Text = "Max Discount:", Font = AppTheme.BodyFont, Anchor = AnchorStyles.Left }, 0, 4);
        _numMaxDiscount = new NumericUpDown { Width = 100, DecimalPlaces = 2, Maximum = 100000 };
        layout.Controls.Add(_numMaxDiscount, 1, 4);

        layout.Controls.Add(new Label { Text = "Min Purchase:", Font = AppTheme.BodyFont, Anchor = AnchorStyles.Left }, 0, 5);
        _numMinPurchase = new NumericUpDown { Width = 100, DecimalPlaces = 2, Maximum = 100000 };
        layout.Controls.Add(_numMinPurchase, 1, 5);

        layout.Controls.Add(new Label { Text = "Usage Limit:", Font = AppTheme.BodyFont, Anchor = AnchorStyles.Left }, 0, 6);
        _numUsageLimit = new NumericUpDown { Width = 100, Maximum = 100000 };
        _numUsageLimit.Value = 0;
        layout.Controls.Add(_numUsageLimit, 1, 6);

        layout.Controls.Add(new Label { Text = "Start Date: *", Font = AppTheme.BodyFont, Anchor = AnchorStyles.Left }, 0, 7);
        _dtpStart = new DateTimePicker { Width = 200, Format = DateTimePickerFormat.Short };
        layout.Controls.Add(_dtpStart, 1, 7);

        layout.Controls.Add(new Label { Text = "End Date: *", Font = AppTheme.BodyFont, Anchor = AnchorStyles.Left }, 0, 8);
        _dtpEnd = new DateTimePicker { Width = 200, Format = DateTimePickerFormat.Short };
        layout.Controls.Add(_dtpEnd, 1, 8);

        _chkActive = new CheckBox { Text = "Active", Checked = true, Font = AppTheme.BodyFont };
        layout.Controls.Add(_chkActive, 1, 9);

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
        _txtCode.Text = _existing.Code;
        _txtDescription.Text = _existing.Description;
        _cmbType.SelectedItem = _existing.Type;
        _numValue.Value = _existing.Value;
        _numMaxDiscount.Value = _existing.MaxDiscountAmount ?? 0;
        _numMinPurchase.Value = _existing.MinPurchaseAmount ?? 0;
        _numUsageLimit.Value = _existing.UsageLimit ?? 0;
        _dtpStart.Value = _existing.StartDate;
        _dtpEnd.Value = _existing.EndDate;
        _chkActive.Checked = _existing.IsActive;
    }

    private async void BtnSave_Click(object? sender, EventArgs e)
    {
        var code = _txtCode.Text.Trim();
        if (string.IsNullOrWhiteSpace(code))
        {
            MessageBox.Show("Discount code is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_numValue.Value <= 0)
        {
            MessageBox.Show("Value must be greater than zero.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_dtpEnd.Value < _dtpStart.Value)
        {
            MessageBox.Show("End date must be after start date.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var dto = new DiscountDto
            {
                DiscountID = _existing?.DiscountID ?? 0,
                Code = code,
                Description = _txtDescription.Text.Trim(),
                Type = _cmbType.SelectedItem?.ToString() ?? "Percentage",
                Value = _numValue.Value,
                MaxDiscountAmount = _numMaxDiscount.Value > 0 ? _numMaxDiscount.Value : null,
                MinPurchaseAmount = _numMinPurchase.Value > 0 ? _numMinPurchase.Value : null,
                UsageLimit = _numUsageLimit.Value > 0 ? (int)_numUsageLimit.Value : null,
                StartDate = _dtpStart.Value,
                EndDate = _dtpEnd.Value,
                IsActive = _chkActive.Checked
            };

            if (_existing != null)
                await _discountService.UpdateDiscountAsync(dto);
            else
                await _discountService.CreateDiscountAsync(dto);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
