using System;
using System.Drawing;
using System.Windows.Forms;
using POSApplication.Core.DTOs;
using POSApplication.Core.Interfaces;

namespace POSApplication.UI.Forms;

public class CustomerEditDialog : Form
{
    private readonly ICustomerService _customerService;
    private CustomerDto? _currentCustomer;
    private ErrorProvider _errorProvider;

    private TextBox _txtFirstName;
    private TextBox _txtLastName;
    private TextBox _txtEmail;
    private TextBox _txtPhone;
    private TextBox _txtAddress;
    private TextBox _txtCity;
    private TextBox _txtState;
    private TextBox _txtZipCode;
    private TextBox _txtNotes;
    
    public CustomerEditDialog(ICustomerService customerService)
    {
        _customerService = customerService;
        InitializeComponent();
    }

    public void SetCustomer(CustomerDto customer)
    {
        _currentCustomer = customer;
        Text = "Edit Customer";
        
        _txtFirstName.Text = customer.FirstName;
        _txtLastName.Text = customer.LastName;
        _txtEmail.Text = customer.Email;
        _txtPhone.Text = customer.Phone;
        _txtAddress.Text = customer.Address;
        _txtCity.Text = customer.City;
        _txtState.Text = customer.State;
        _txtZipCode.Text = customer.ZipCode;
        _txtNotes.Text = customer.Notes;
    }

    private void InitializeComponent()
    {
        this.Text = "Add Customer";
        this.Size = new Size(500, 550);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point);

        _errorProvider = new ErrorProvider(this);
        _errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;

        var mainPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20),
            RowCount = 10,
            ColumnCount = 2,
            AutoSize = true
        };

        // Helper to add rows
        void AddRow(string label, Control control, int row)
        {
            mainPanel.Controls.Add(new Label { Text = label, AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Left }, 0, row);
            control.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            control.Width = 300;
            mainPanel.Controls.Add(control, 1, row);
        }

        _txtFirstName = new TextBox();
        _txtFirstName.Validating += TxtFirstName_Validating;
        AddRow("First Name *:", _txtFirstName, 0);

        _txtLastName = new TextBox();
        AddRow("Last Name:", _txtLastName, 1);

        _txtPhone = new TextBox();
        _txtPhone.Validating += TxtPhone_Validating;
        AddRow("Phone:", _txtPhone, 2);

        _txtEmail = new TextBox();
        _txtEmail.Validating += TxtEmail_Validating;
        AddRow("Email:", _txtEmail, 3);

        _txtAddress = new TextBox();
        AddRow("Address:", _txtAddress, 4);

        _txtCity = new TextBox();
        AddRow("City:", _txtCity, 5);

        _txtState = new TextBox();
        AddRow("State:", _txtState, 6);

        _txtZipCode = new TextBox();
        AddRow("Zip Code:", _txtZipCode, 7);

        _txtNotes = new TextBox { Multiline = true, Height = 60 };
        AddRow("Notes:", _txtNotes, 8);

        // Buttons
        var btnPanel = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.RightToLeft,
            Dock = DockStyle.Bottom,
            Height = 50,
            Padding = new Padding(0, 10, 20, 0)
        };

        var btnCancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Height = 30 };
        var btnSave = new Button { Text = "Save", Height = 30, BackColor = Color.FromArgb(0, 122, 204), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        btnSave.Click += BtnSave_Click;

        btnPanel.Controls.Add(btnCancel);
        btnPanel.Controls.Add(btnSave);

        this.Controls.Add(mainPanel);
        this.Controls.Add(btnPanel);
        
        this.AcceptButton = btnSave;
        this.CancelButton = btnCancel;
    }

    private async void BtnSave_Click(object? sender, EventArgs e)
    {
        if (!ValidateChildren(ValidationConstraints.Enabled))
        {
            MessageBox.Show("Please correct the errors before saving.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // Manual check for specific complex validations if not covered by Validating events
        // But we will add Validating events next.

        try
        {
            var customer = _currentCustomer ?? new CustomerDto();
            
            customer.FirstName = _txtFirstName.Text.Trim();
            customer.LastName = _txtLastName.Text?.Trim();
            customer.Phone = _txtPhone.Text?.Trim();
            customer.Email = _txtEmail.Text?.Trim();
            customer.Address = _txtAddress.Text?.Trim();
            customer.City = _txtCity.Text?.Trim();
            customer.State = _txtState.Text?.Trim();
            customer.ZipCode = _txtZipCode.Text?.Trim();
            customer.Notes = _txtNotes.Text?.Trim();
            customer.IsActive = true;

            if (customer.CustomerID > 0)
            {
                await _customerService.UpdateCustomerAsync(customer);
            }
            else
            {
                await _customerService.CreateCustomerAsync(customer);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving customer: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void TxtFirstName_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_txtFirstName.Text))
        {
            e.Cancel = true;
            _errorProvider.SetError(_txtFirstName, "First Name is required.");
        }
        else
        {
            e.Cancel = false;
            _errorProvider.SetError(_txtFirstName, "");
        }
    }

    private void TxtEmail_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
        if (!string.IsNullOrWhiteSpace(_txtEmail.Text))
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(_txtEmail.Text);
                if (addr.Address != _txtEmail.Text) throw new Exception();
                _errorProvider.SetError(_txtEmail, "");
            }
            catch
            {
                e.Cancel = true;
                _errorProvider.SetError(_txtEmail, "Invalid email format.");
            }
        }
        else
        {
            _errorProvider.SetError(_txtEmail, "");
        }
    }

    private void TxtPhone_Validating(object sender, System.ComponentModel.CancelEventArgs e)
    {
         if (!string.IsNullOrWhiteSpace(_txtPhone.Text))
        {
            var digitCount = _txtPhone.Text.Count(char.IsDigit);
            if (digitCount < 11)
            {
                e.Cancel = true;
                _errorProvider.SetError(_txtPhone, "Phone must have at least 11 digits.");
            }
            else
            {
                _errorProvider.SetError(_txtPhone, "");
            }
        }
        else
        {
            _errorProvider.SetError(_txtPhone, "");
        }
    }
}
