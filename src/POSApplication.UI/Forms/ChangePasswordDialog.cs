using POSApplication.Core.Interfaces;

namespace POSApplication.UI.Forms;

public partial class ChangePasswordDialog : Form
{
    private readonly IUserService _userService;
    private readonly int _userId;

    public ChangePasswordDialog(IUserService userService, int userId)
    {
        InitializeComponent();
        _userService = userService;
        _userId = userId;
    }

    private async void btnChange_Click(object sender, EventArgs e)
    {
        try
        {
            var oldPassword = txtCurrentPassword.Text;
            var newPassword = txtNewPassword.Text;
            var confirmPassword = txtConfirmPassword.Text;

            if (string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
            {
                MessageBox.Show("Please fill all fields.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (newPassword != confirmPassword)
            {
                MessageBox.Show("New passwords do not match.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var success = await _userService.ChangePasswordAsync(_userId, oldPassword, newPassword);
            if (success)
            {
                MessageBox.Show("Password changed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Incorrect current password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        this.DialogResult = DialogResult.Cancel;
        this.Close();
    }
}
