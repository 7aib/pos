using POSApplication.Common.Enums;
using POSApplication.Core.DTOs;
using POSApplication.Core.Interfaces;
using POSApplication.Core.Entities;
using POSApplication.UI.Theme;

namespace POSApplication.UI.Forms;

/// <summary>
/// Payment processing dialog
/// </summary>
public partial class PaymentDialog : Form
{
    private readonly IPaymentService _paymentService;
    private readonly ICreditService _creditService;
    private readonly decimal _totalAmount;
    private readonly Customer? _customer;
    private readonly List<PaymentDto> _payments = new();

    public List<PaymentDto> Payments => _payments;
    public decimal TotalPaid => _payments.Sum(p => p.Amount);
    public decimal Change { get; private set; }

    private ComboBox _cmbPaymentMethod;
    private NumericUpDown _numAmount;
    private TextBox _txtCardType;
    private TextBox _txtCardDigits;
    private Label _lblTotalAmount;
    private Label _lblAmountDue;
    private Label _lblChange;
    private Button _btnAddPayment;
    private Button _btnComplete;
    private DataGridView _gridPayments;

    public PaymentDialog(
        IPaymentService paymentService, 
        ICreditService creditService, 
        decimal totalAmount, 
        Customer? customer)
    {
        _paymentService = paymentService;
        _creditService = creditService;
        _totalAmount = totalAmount;
        _customer = customer;
        InitializeComponent();
        UpdateAmounts();
    }

    private void InitializeComponent()
    {
        this.Text = "Process Payment";
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.StartPosition = FormStartPosition.CenterParent;
        this.Size = new Size(500, 550);

        // Amount display panel
        var panelAmounts = new Panel
        {

            Location = new Point(10, 10),
            Size = new Size(460, 100),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = AppTheme.PanelColor
        };

        _lblTotalAmount = new Label
        {
            Text = "Total Amount: $0.00",
            Location = new Point(10, 10),
            Width = 200,
            Font = new Font("Segoe UI", 12, FontStyle.Bold)
        };

        _lblAmountDue = new Label
        {
            Text = "Amount Due: $0.00",
            Location = new Point(10, 40),
            Width = 200,
            Font = new Font("Segoe UI", 10)
        };

        _lblChange = new Label
        {
            Text = "Change: $0.00",
            Location = new Point(10, 70),
            Width = 200,
            Font = new Font("Segoe UI", 10),
            ForeColor = Color.Green
        };

        panelAmounts.Controls.AddRange(new Control[] { _lblTotalAmount, _lblAmountDue, _lblChange });

        // Payment method panel
        var lblPaymentMethod = new Label
        {
            Text = "Payment Method:",
            Location = new Point(20, 130),
            Width = 120
        };

        _cmbPaymentMethod = new ComboBox
        {
            Location = new Point(150, 128),
            Width = 150,
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        _cmbPaymentMethod.Items.AddRange(new object[] { "Cash", "Card", "Credit Account" });
        _cmbPaymentMethod.SelectedIndex = 0;
        _cmbPaymentMethod.SelectedIndexChanged += CmbPaymentMethod_SelectedIndexChanged;

        var lblAmount = new Label
        {
            Text = "Amount:",
            Location = new Point(20, 165),
            Width = 120
        };

        _numAmount = new NumericUpDown
        {
            Location = new Point(150, 163),
            Width = 150,
            DecimalPlaces = 2,
            Maximum = 1000000,
            Value = 0
        };

        var lblCardType = new Label
        {
            Text = "Card Type:",
            Location = new Point(20, 200),
            Width = 120,
            Name = "lblCardType",
            Visible = false
        };

        _txtCardType = new TextBox
        {
            Location = new Point(150, 198),
            Width = 150,
            Name = "txtCardType",
            Visible = false
        };

        var lblCardDigits = new Label
        {
            Text = "Last 4 Digits:",
            Location = new Point(20, 235),
            Width = 120,
            Name = "lblCardDigits",
            Visible = false
        };

        _txtCardDigits = new TextBox
        {
            Location = new Point(150, 233),
            Width = 150,
            MaxLength = 4,
            Name = "txtCardDigits",
            Visible = false
        };

        _btnAddPayment = new Button
        {
            Text = "Add Payment",
            Location = new Point(150, 270),
            Width = 150,
            Height = 35
        };
        AppTheme.ApplyButtonTheme(_btnAddPayment, AppTheme.PrimaryColor);
        _btnAddPayment.Click += BtnAddPayment_Click;

        // Payments grid
        _gridPayments = new DataGridView
        {
            Location = new Point(20, 310),
            Size = new Size(450, 120),
            AutoGenerateColumns = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            RowHeadersVisible = false
        };

        _gridPayments.Columns.AddRange(new DataGridViewColumn[]
        {
            new DataGridViewTextBoxColumn { HeaderText = "Method", DataPropertyName = "PaymentMethod", Width = 150 },
            new DataGridViewTextBoxColumn { HeaderText = "Amount", DataPropertyName = "Amount", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } },
            new DataGridViewTextBoxColumn { HeaderText = "Card Type", DataPropertyName = "CardType", Width = 100 },
            new DataGridViewTextBoxColumn { HeaderText = "Last 4", DataPropertyName = "CardLastFourDigits", Width = 90 }
        });

        // Complete button
        _btnComplete = new Button
        {
            Text = "Complete Payment",
            Location = new Point(300, 460),
            Width = 170,
            Height = 45,
            DialogResult = DialogResult.OK,
            Enabled = false
        };
        AppTheme.ApplyButtonTheme(_btnComplete, AppTheme.SuccessColor);
        _btnComplete.Click += BtnComplete_Click;

        var btnCancel = new Button
        {
            Text = "Cancel",
            Location = new Point(130, 460),
            Width = 150,
            Height = 45,
            DialogResult = DialogResult.Cancel
        };
        AppTheme.ApplySecondaryButtonTheme(btnCancel);

        // Add controls
        this.Controls.AddRange(new Control[]
        {
            panelAmounts,
            lblPaymentMethod, _cmbPaymentMethod,
            lblAmount, _numAmount,
            lblCardType, _txtCardType,
            lblCardDigits, _txtCardDigits,
            _btnAddPayment,
            _gridPayments,
            _btnComplete, btnCancel
        });

        this.AcceptButton = _btnComplete;
        this.CancelButton = btnCancel;
    }

    private void CmbPaymentMethod_SelectedIndexChanged(object? sender, EventArgs e)
    {
        var isCard = _cmbPaymentMethod.SelectedItem?.ToString() == "Card";
        
        this.Controls.Find("lblCardType", false).FirstOrDefault()!.Visible = isCard;
        _txtCardType.Visible = isCard;
        this.Controls.Find("lblCardDigits", false).FirstOrDefault()!.Visible = isCard;
        _txtCardDigits.Visible = isCard;
    }

    private async void BtnAddPayment_Click(object? sender, EventArgs e)
    {
        try
        {
            var amount = _numAmount.Value;
            if (amount <= 0)
            {
                MessageBox.Show("Please enter a valid payment amount.", "Invalid Amount", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var paymentMethod = _cmbPaymentMethod.SelectedItem?.ToString() switch
            {
                "Cash" => PaymentMethod.Cash,
                "Card" => PaymentMethod.Card,
                "Credit Account" => PaymentMethod.CreditAccount,
                _ => PaymentMethod.Cash
            };

            var payment = new PaymentDto
            {
                PaymentMethod = paymentMethod,
                Amount = amount,
                CardType = _txtCardType.Visible ? _txtCardType.Text.Trim() : null,
                CardLastFourDigits = _txtCardDigits.Visible ? _txtCardDigits.Text.Trim() : null,
                PaymentDate = DateTime.Now,
                ProcessedBy = 1 // TODO: Get actual user ID
            };

            // Validate payment
            var isValid = await _paymentService.ValidatePaymentAsync(payment);
            if (!isValid)
            {
                MessageBox.Show("Payment validation failed. Please check card details.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Credit Account specific validation
            if (paymentMethod == PaymentMethod.CreditAccount)
            {
                if (_customer == null)
                {
                    MessageBox.Show("Walk-in customers cannot pay with Credit Account.\nPlease select a customer first.", "Customer Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try 
                {
                    // Get latest balance
                    var balance = await _creditService.GetCustomerBalanceAsync(_customer.CustomerID);
                    var creditAccount = await _creditService.GetCreditAccountAsync(_customer.CustomerID);
                    var creditLimit = creditAccount?.CreditLimit ?? 0;

                    if (creditAccount == null || !creditAccount.IsActive)
                    {
                         MessageBox.Show("Customer does not have an active credit account.", "Credit Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                         return;
                    }

                    if (balance + amount > creditLimit)
                    {
                         MessageBox.Show($"Credit limit exceeded.\nLimit: {creditLimit:C2}\nCurrent Balance: {balance:C2}\nAttempted: {amount:C2}", "Credit Limit Exceeded", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                         return;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error validating credit: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }

            _payments.Add(payment);
            _gridPayments.DataSource = null;
            _gridPayments.DataSource = _payments;

            // Clear inputs
            _numAmount.Value = 0;
            _txtCardType.Clear();
            _txtCardDigits.Clear();

            UpdateAmounts();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error adding payment: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnComplete_Click(object? sender, EventArgs e)
    {
        var amountDue = _totalAmount - TotalPaid;
        
        if (amountDue > 0.01m) // Allow small rounding difference
        {
            MessageBox.Show($"Payment incomplete. Amount due: {amountDue:C2}", "Incomplete Payment", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            this.DialogResult = DialogResult.None;
            return;
        }

        Change = _paymentService.CalculateChange(_totalAmount, TotalPaid);
        this.DialogResult = DialogResult.OK;
    }

    private void UpdateAmounts()
    {
        _lblTotalAmount.Text = $"Total Amount: {_totalAmount:C2}";
        
        var amountDue = _totalAmount - TotalPaid;
        _lblAmountDue.Text = $"Amount Due: {amountDue:C2}";

        if (TotalPaid >= _totalAmount)
        {
            var change = TotalPaid - _totalAmount;
            _lblChange.Text = $"Change: {change:C2}";
            _lblChange.ForeColor = Color.Green;
            _btnComplete.Enabled = true;
        }
        else
        {
            _lblChange.Text = "Change: $0.00";
            _lblChange.ForeColor = Color.Black;
            _btnComplete.Enabled = false;
        }

        // Auto-fill next payment amount
        if (amountDue > 0)
        {
            _numAmount.Value = amountDue;
        }
    }
}
