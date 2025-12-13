using POSApplication.Core.Interfaces;
using POSApplication.Core.Entities;

namespace POSApplication.UI.Forms;

public partial class LoginForm : Form
{
    private readonly IUserService _userService;
    public User? AuthenticatedUser { get; private set; }

    public LoginForm(IUserService userService)
    {
        InitializeComponent();
        _userService = userService;
    }

    private async void btnLogin_Click(object sender, EventArgs e)
    {
        try
        {
            btnLogin.Enabled = false;
            var username = txtUsername.Text.Trim();
            var password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both username and password.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var user = await _userService.AuthenticateAsync(username, password);
            if (user != null)
            {
                AuthenticatedUser = user;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid username or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnLogin.Enabled = true;
        }
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        this.DialogResult = DialogResult.Cancel;
        this.Close();
    }
}
