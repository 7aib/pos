using POSApplication.Common.Enums;
using POSApplication.Core.DTOs;
using POSApplication.Core.Interfaces;

namespace POSApplication.UI.Forms;

/// <summary>
/// Payment processing dialog
/// </summary>
public partial class PaymentDialog : Form
{
    private readonly IPaymentService _paymentService;
    private readonly decimal _totalAmount;
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

    public PaymentDialog(IPaymentService paymentService, decimal totalAmount)
    {
        _paymentService = paymentService;
        _totalAmount = totalAmount;
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
            BorderStyle = BorderStyle.FixedSingle
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
            Width = 120,
            Height = 30
        };
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
            Width = 150,
            Height = 40,
            DialogResult = DialogResult.OK,
            Enabled = false
        };
        _btnComplete.Click += BtnComplete_Click;

        var btnCancel = new Button
        {
            Text = "Cancel",
            Location = new Point(140, 460),
            Width = 150,
            Height = 40,
            DialogResult = DialogResult.Cancel
        };

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
