using POSApplication.Common.Enums;
using POSApplication.Core.Entities;
using POSApplication.Core.Interfaces;
using POSApplication.UI.Theme;

namespace POSApplication.UI.Forms;

public partial class CustomerPaymentForm : Form
{
    private readonly ICreditService _creditService;
    private readonly Customer _customer;
    private readonly int _currentUserId;

    public CustomerPaymentForm(ICreditService creditService, Customer customer, int currentUserId)
    {
        InitializeComponent();
        _creditService = creditService;
        _customer = customer;
        _currentUserId = currentUserId;

        ApplyTheme();
        LoadData();
    }

    private void ApplyTheme()
    {
        this.BackColor = AppTheme.BackgroundColor;
        AppTheme.ApplyButtonTheme(btnPay, AppTheme.SuccessColor);
        AppTheme.ApplySecondaryButtonTheme(btnCancel);
    }

    private async void LoadData()
    {
        lblCustomerName.Text = $"{_customer.FirstName} {_customer.LastName}";
        cmbPaymentMethod.SelectedIndex = 0; // Default Cash

        try
        {
            var balance = await _creditService.GetCustomerBalanceAsync(_customer.CustomerID);
            lblCurrentBalance.Text = $"Current Balance: {balance:C2}";
            numAmount.Value = balance > 0 ? balance : 0;
            
            // If balance is 0 or negative (credit), maybe disable pay?
            // Actually, customers can prepay (negative balance).
            // But usually this form is to clear debt.
            // Let's assume debt is positive balance in this system (based on CreditService: Charge +, Payment -)
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading balance: {ex.Message}");
        }
    }

    private async void btnPay_Click(object sender, EventArgs e)
    {
        if (numAmount.Value <= 0)
        {
             MessageBox.Show("Please enter a valid amount.", "Invalid Amount", MessageBoxButtons.OK, MessageBoxIcon.Warning);
             return;
        }

        try
        {
            var method = cmbPaymentMethod.SelectedItem?.ToString() switch
            {
                "Cash" => PaymentMethod.Cash,
                "Card" => PaymentMethod.Card,
                "Other" => PaymentMethod.Other,
                _ => PaymentMethod.Cash
            };

            await _creditService.MakePaymentOnAccountAsync(
                _customer.CustomerID, 
                numAmount.Value, 
                method, 
                txtReference.Text.Trim(), 
                _currentUserId);

            MessageBox.Show("Payment recorded successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error processing payment: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        this.Close();
    }
}
