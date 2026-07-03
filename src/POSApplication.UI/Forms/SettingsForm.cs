using Microsoft.Extensions.Configuration;
using POSApplication.UI.Theme;

namespace POSApplication.UI.Forms;

public class SettingsForm : Form
{
    private readonly IConfiguration _configuration;
    private TextBox _txtStoreName;
    private TextBox _txtAddress;
    private TextBox _txtPhone;
    private NumericUpDown _numTaxRate;

    public SettingsForm(IConfiguration configuration)
    {
        _configuration = configuration;
        InitializeComponent();
        LoadSettings();
    }

    private void InitializeComponent()
    {
        this.Text = "Store Settings";
        this.Size = new Size(450, 350);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.StartPosition = FormStartPosition.CenterParent;
        this.BackColor = AppTheme.BackgroundColor;

        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(20), RowCount = 6, ColumnCount = 2 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        layout.Controls.Add(new Label { Text = "Store Name:", Font = AppTheme.BodyFont, Anchor = AnchorStyles.Left }, 0, 0);
        _txtStoreName = new TextBox { Width = 250, Font = AppTheme.BodyFont };
        layout.Controls.Add(_txtStoreName, 1, 0);

        layout.Controls.Add(new Label { Text = "Address:", Font = AppTheme.BodyFont, Anchor = AnchorStyles.Left }, 0, 1);
        _txtAddress = new TextBox { Width = 250, Height = 50, Multiline = true, Font = AppTheme.BodyFont };
        layout.Controls.Add(_txtAddress, 1, 1);

        layout.Controls.Add(new Label { Text = "Phone:", Font = AppTheme.BodyFont, Anchor = AnchorStyles.Left }, 0, 2);
        _txtPhone = new TextBox { Width = 250, Font = AppTheme.BodyFont };
        layout.Controls.Add(_txtPhone, 1, 2);

        layout.Controls.Add(new Label { Text = "Tax Rate (%):", Font = AppTheme.BodyFont, Anchor = AnchorStyles.Left }, 0, 3);
        _numTaxRate = new NumericUpDown { Width = 100, DecimalPlaces = 2, Maximum = 100, Font = AppTheme.BodyFont };
        layout.Controls.Add(_numTaxRate, 1, 3);

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

    private void LoadSettings()
    {
        _txtStoreName.Text = _configuration["StoreSettings:StoreName"] ?? "My Store";
        _txtAddress.Text = _configuration["StoreSettings:Address"] ?? "123 Main Street";
        _txtPhone.Text = _configuration["StoreSettings:Phone"] ?? "(555) 123-4567";

        var taxRate = 10.0m;
        decimal.TryParse(_configuration["StoreSettings:TaxRate"], out taxRate);
        _numTaxRate.Value = taxRate;
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        var storeName = _txtStoreName.Text.Trim();
        if (string.IsNullOrWhiteSpace(storeName))
        {
            MessageBox.Show("Store name is required.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var appSettingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
            if (!File.Exists(appSettingsPath))
                appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");

            var connectionStr = _configuration.GetConnectionString("DefaultConnection") ?? "Data Source=pos.db";
            var address = _txtAddress.Text.Trim().Replace("\r\n", " ").Replace("\n", " ");
            var phone = _txtPhone.Text.Trim();

            var json = $$"""
            {
              "ConnectionStrings": {
                "DefaultConnection": "{{connectionStr}}"
              },
              "StoreSettings": {
                "StoreName": "{{storeName}}",
                "Address": "{{address}}",
                "Phone": "{{phone}}",
                "TaxRate": {{_numTaxRate.Value}}
              },
              "Printer": {
                "DefaultPrinter": "",
                "PrinterType": "Thermal"
              }
            }
            """;

            File.WriteAllText(appSettingsPath, json);

            MessageBox.Show("Settings saved successfully! Changes will take effect after restart.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving settings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
