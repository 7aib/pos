using POSApplication.Core.Entities;
using POSApplication.Common.Enums;
using System;
using System.Windows.Forms;
using System.Linq;

namespace POSApplication.UI.Forms;

public partial class UserEditDialog : Form
{
    public User User { get; private set; }

    public UserEditDialog(User user)
    {
        InitializeComponent();
        User = user;
        LoadData();
    }

    private void LoadData()
    {
        txtUsername.Text = User.Username;
        txtFullName.Text = User.FullName;
        txtEmail.Text = User.Email;
        txtPhone.Text = User.Phone;
        chkIsActive.Checked = User.IsActive;

        cmbRole.DataSource = Enum.GetValues(typeof(UserRole));
        cmbRole.SelectedItem = User.Role;
    }

    private void btnSave_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtFullName.Text))
        {
            MessageBox.Show("Full Name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        User.FullName = txtFullName.Text.Trim();
        User.Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim();
        User.Phone = string.IsNullOrWhiteSpace(txtPhone.Text) ? null : txtPhone.Text.Trim();
        User.IsActive = chkIsActive.Checked;
        
        if (cmbRole.SelectedItem is UserRole role)
        {
            User.Role = role;
        }

        this.DialogResult = DialogResult.OK;
        this.Close();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        this.DialogResult = DialogResult.Cancel;
        this.Close();
    }
}
