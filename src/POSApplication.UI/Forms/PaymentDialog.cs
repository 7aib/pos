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
    
    private NumericUpDown _numCashReceived;
    private NumericUpDown _numCreditAmount;
    
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
        CalculateValues();

        if (_customer != null && _customer.CreditAccount != null)
        {
            var available = _customer.CreditAccount.CreditLimit - _customer.CreditAccount.CurrentBalance;
            _lblCreditLimit.Text = $"Available Credit: {available:C2}";
            _numCreditAmount.Enabled = true;
            _numCreditAmount.Maximum = Math.Min(_totalAmount, available); // Can't pay more than total or available
        }
        else
        {
            _numCreditAmount.Enabled = false;
            _lblCreditLimit.Text = "Credit not available";
        }
    }

    private void CalculateValues()
    {
        var credit = _numCreditAmount.Value;
        var cash = _numCashReceived.Value;

        var amountCoveredByCredit = credit;
        var remainingAfterCredit = Math.Max(0, _totalAmount - amountCoveredByCredit);
        
        // If cash is less than remaining, we still owe money
        var amountDue = Math.Max(0, remainingAfterCredit - cash);
        
        // If cash + credit > total, we have change
        // Change is basically (Cash + Credit) - Total, but strictly from Cash part ideally
        var totalPaid = credit + cash;
        var change = Math.Max(0, totalPaid - _totalAmount);

        _lblBalance.Text = $"{amountDue:C2}";
        _lblChange.Text = $"{change:C2}";
        Change = change;

        // Visual feedback
        if (amountDue <= 0)
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
        _numCreditAmount = new NumericUpDown
        {
            DecimalPlaces = 2,
            Maximum = 1000000,
            Font = AppTheme.BodyFont,
            Width = 150,
            Anchor = AnchorStyles.Right
        };
        _numCreditAmount.ValueChanged += (s, e) => CalculateValues();
        layout.Controls.Add(_numCreditAmount, 1, 1);

        // Credit Limit Hint
        _lblCreditLimit = new Label { Text = "", Font = new Font("Segoe UI", 8), ForeColor = Color.Gray, AutoSize = true, Anchor = AnchorStyles.Top | AnchorStyles.Right };
        layout.Controls.Add(_lblCreditLimit, 1, 2);

        // 3. Cash Received
        layout.Controls.Add(new Label { Text = "Cash Received:", Font = AppTheme.HeaderFont, AutoSize = true, Anchor = AnchorStyles.Left }, 0, 3);
        _numCashReceived = new NumericUpDown
        {
            DecimalPlaces = 2,
            Maximum = 1000000,
            Font = AppTheme.HeaderFont,
            Width = 150,
            Anchor = AnchorStyles.Right
        };
        _numCashReceived.ValueChanged += (s, e) => CalculateValues();
        // Auto-select text on focus for quick entry
        _numCashReceived.Enter += (s, e) => _numCashReceived.Select(0, _numCashReceived.Text.Length);
        layout.Controls.Add(_numCashReceived, 1, 3);

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
        this.ActiveControl = _numCashReceived;
    }

    private void BtnComplete_Click(object? sender, EventArgs e)
    {
        CalculateValues(); // Ensure latest calculation
        
        // Construct Payment DTOs based on inputs
        _payments.Clear();

        var creditAmount = _numCreditAmount.Value;
        var cashReceived = _numCashReceived.Value;
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

        // 2. Add Cash Payment (Amount is Total - Credit, or just the Cash Received logic?)
        // Usually, the "Amount Paid" in records is what was needed to cover the bill. 
        // The "Change" is recorded separately or implied.
        // We will record the exact portion of the bill covered by cash.
        // If Bill $100, Credit $20, Cash Tendered $100 -> Change $20. 
        // We should record Cash Payment of $80? Or $100 and Change $20?
        // Typically POS systems record the Tendered amount in a "Tendered" field, but Payment record usually sums to Total.
        // For simple ledgers, we record the payment amount that contributes to the Sale Total.
        
        var remainingAfterCredit = Math.Max(0, _totalAmount - creditAmount);
        
        if (remainingAfterCredit > 0)
        {
            // We use cash to cover the rest
            _payments.Add(new PaymentDto
            {
                PaymentMethod = PaymentMethod.Cash,
                Amount = remainingAfterCredit, // We record what was *applied* to the sale, not just handed over
                // We might want to store "Tendered" somewhere, but for now this aligns with previous logic
                PaymentDate = DateTime.Now,
                ProcessedBy = 1
            });
        }
        
        // If they paid pure cash $100 for $90 bill, we record Payment $90, Change $10. 
        // ISalesService.CreateSaleAsync expects Payments sum to align with Total? 
        // Or if Payments > Total, it creates Change.
        // Let's stick to recording the VALID Payment amounts that sum to Total.
        
        this.DialogResult = DialogResult.OK;
    }
}
