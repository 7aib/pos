using POSApplication.Common.Enums;
using POSApplication.Core.DTOs;
using POSApplication.Core.Interfaces;
using POSApplication.Core.Entities;
using POSApplication.UI.Theme;

namespace POSApplication.UI.Forms;

/// <summary>
/// Simplified Payment processing dialog
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

    // UI Controls
    private Label _lblTotal;
    private Label _lblBalance;
    private Label _lblChange;
    
    private TextBox _txtCashReceived;
    private TextBox _txtCreditAmount;
    
    private Button _btnComplete;
    private Label _lblCreditLimit; // Show available credit

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
        InitializeValues();
    }

    private void InitializeValues()
    {
        _lblTotal.Text = $"{_totalAmount:C2}";
        
        if (_customer != null && _customer.CreditAccount != null)
        {
            var available = _customer.CreditAccount.CreditLimit - _customer.CreditAccount.CurrentBalance;
            _lblCreditLimit.Text = $"Available Credit: {available:C2}";
            _txtCreditAmount.Enabled = true;
            _txtCreditAmount.PlaceholderText = $"Max: {Math.Min(_totalAmount, available):F2}"; 
        }
        else
        {
            _txtCreditAmount.Enabled = false;
            _lblCreditLimit.Text = "Credit not available";
        }
        
        CalculateValues();
    }

    private void CalculateValues()
    {
        decimal credit = 0;
        decimal cash = 0;

        decimal.TryParse(_txtCreditAmount.Text, out credit);
        decimal.TryParse(_txtCashReceived.Text, out cash);

        // Validations
        bool isValid = true;
        
        if (_customer != null && _customer.CreditAccount != null) 
        {
             var available = _customer.CreditAccount.CreditLimit - _customer.CreditAccount.CurrentBalance;
             if (credit > available) 
             {
                 _lblCreditLimit.ForeColor = AppTheme.DangerColor;
                 isValid = false;
             }
             else
             {
                 _lblCreditLimit.ForeColor = Color.Gray;
             }
        }
        
        if (credit > _totalAmount)
        {
             // Usually can't pay more credit than total
             isValid = false; // Or just warn? Let's say invalid.
        }

        var amountCoveredByCredit = credit;
        var remainingAfterCredit = Math.Max(0, _totalAmount - amountCoveredByCredit);
        
        // If cash is less than remaining, we still owe money
        var amountDue = Math.Max(0, remainingAfterCredit - cash);
        
        // Change logic
        var totalPaid = credit + cash;
        var change = Math.Max(0, totalPaid - _totalAmount);

        _lblBalance.Text = $"{amountDue:C2}";
        _lblChange.Text = $"{change:C2}";
        Change = change;

        // Visual feedback
        if (amountDue <= 0 && isValid)
        {
            _lblBalance.ForeColor = AppTheme.SuccessColor;
            _lblChange.ForeColor = AppTheme.SuccessColor;
            _btnComplete.Enabled = true;
        }
        else
        {
            _lblBalance.ForeColor = AppTheme.DangerColor;
            _lblChange.ForeColor = AppTheme.TextColor;
            _btnComplete.Enabled = false;
        }
    }

    private void InitializeComponent()
    {
        this.Text = "Checkout Payment";
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.StartPosition = FormStartPosition.CenterParent;
        this.Size = new Size(500, 500);
        this.BackColor = AppTheme.BackgroundColor;

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(30),
            RowCount = 8,
            ColumnCount = 2,
            AutoSize = true
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));

        // 1. Total Amount
        layout.Controls.Add(new Label { Text = "Total Amount:", Font = AppTheme.HeaderFont, AutoSize = true, Anchor = AnchorStyles.Left }, 0, 0);
        _lblTotal = new Label { Text = "$0.00", Font = AppTheme.HeaderFont, AutoSize = true, Anchor = AnchorStyles.Right, ForeColor = AppTheme.PrimaryColor };
        layout.Controls.Add(_lblTotal, 1, 0);

        // 2. Credit Amount
        layout.Controls.Add(new Label { Text = "Credit Account:", Font = AppTheme.BodyFont, AutoSize = true, Anchor = AnchorStyles.Left | AnchorStyles.Left }, 0, 1);
        _txtCreditAmount = new TextBox
        {
            Font = AppTheme.BodyFont,
            Width = 150,
            Anchor = AnchorStyles.Right,
            MaxLength = 10
        };
        _txtCreditAmount.TextChanged += (s, e) => CalculateValues();
        _txtCreditAmount.KeyPress += NumericOnly_KeyPress;
        layout.Controls.Add(_txtCreditAmount, 1, 1);

        // Credit Limit Hint
        _lblCreditLimit = new Label { Text = "", Font = new Font("Segoe UI", 8), ForeColor = Color.Gray, AutoSize = true, Anchor = AnchorStyles.Top | AnchorStyles.Right };
        layout.Controls.Add(_lblCreditLimit, 1, 2);

        // 3. Cash Received
        layout.Controls.Add(new Label { Text = "Cash Received:", Font = AppTheme.HeaderFont, AutoSize = true, Anchor = AnchorStyles.Left }, 0, 3);
        _txtCashReceived = new TextBox
        {
            Font = AppTheme.HeaderFont,
            Width = 150,
            Anchor = AnchorStyles.Right,
            MaxLength = 10
        };
        _txtCashReceived.TextChanged += (s, e) => CalculateValues();
        _txtCashReceived.KeyPress += NumericOnly_KeyPress;
        layout.Controls.Add(_txtCashReceived, 1, 3);

        // Spacer
        layout.Controls.Add(new Panel { Height = 20 }, 0, 4);

        // 4. Remaining Balance (Amount Due)
        layout.Controls.Add(new Label { Text = "Remaining Due:", Font = AppTheme.BodyFont, AutoSize = true, Anchor = AnchorStyles.Left }, 0, 5);
        _lblBalance = new Label { Text = "$0.00", Font = AppTheme.BodyFont, AutoSize = true, Anchor = AnchorStyles.Right };
        layout.Controls.Add(_lblBalance, 1, 5);

        // 5. Change
        layout.Controls.Add(new Label { Text = "Change:", Font = AppTheme.HeaderFont, AutoSize = true, Anchor = AnchorStyles.Left }, 0, 6);
        _lblChange = new Label { Text = "$0.00", Font = AppTheme.HeaderFont, AutoSize = true, Anchor = AnchorStyles.Right };
        layout.Controls.Add(_lblChange, 1, 6);

        // Buttons Panel
        var btnPanel = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.RightToLeft,
            Dock = DockStyle.Bottom,
            Height = 60,
            Padding = new Padding(0, 10, 30, 0)
        };

        _btnComplete = new Button 
        { 
            Text = "Complete Sale", 
            Height = 40, 
            Width = 150,
            Enabled = false 
        };
        AppTheme.ApplyButtonTheme(_btnComplete, AppTheme.SuccessColor);
        _btnComplete.Click += BtnComplete_Click;

        var btnCancel = new Button { Text = "Cancel", Height = 40, Width = 100 };
        AppTheme.ApplySecondaryButtonTheme(btnCancel);
        btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

        btnPanel.Controls.Add(_btnComplete);
        btnPanel.Controls.Add(btnCancel);

        this.Controls.Add(layout);
        this.Controls.Add(btnPanel);

        this.AcceptButton = _btnComplete;
        this.CancelButton = btnCancel;
        
        // Focus Cash field by default
        this.ActiveControl = _txtCashReceived;
    }

    private void NumericOnly_KeyPress(object? sender, KeyPressEventArgs e)
    {
        // Allow control keys, digits, and one decimal point
        if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar != '.'))
        {
            e.Handled = true;
        }

        // only allow one decimal point
        if ((e.KeyChar == '.') && ((sender as TextBox)!.Text.IndexOf('.') > -1))
        {
            e.Handled = true;
        }
    }

    private void BtnComplete_Click(object? sender, EventArgs e)
    {
        CalculateValues(); 
        
        _payments.Clear();

        decimal creditAmount = 0;
        decimal cashReceived = 0;
        decimal.TryParse(_txtCreditAmount.Text, out creditAmount);
        decimal.TryParse(_txtCashReceived.Text, out cashReceived);

        var grossTotalPaid = creditAmount + cashReceived;

        // 1. Add Credit Payment if any
        if (creditAmount > 0)
        {
            _payments.Add(new PaymentDto
            {
                PaymentMethod = PaymentMethod.CreditAccount,
                Amount = creditAmount,
                PaymentDate = DateTime.Now,
                ProcessedBy = 1 // Todo: UserID
            });
        }

        var remainingAfterCredit = Math.Max(0, _totalAmount - creditAmount);
        
        if (cashReceived > 0)
        {
            _payments.Add(new PaymentDto
            {
                PaymentMethod = PaymentMethod.Cash,
                Amount = cashReceived,
                PaymentDate = DateTime.Now,
                ProcessedBy = 1
            });
        }
        
        this.DialogResult = DialogResult.OK;
    }
}
