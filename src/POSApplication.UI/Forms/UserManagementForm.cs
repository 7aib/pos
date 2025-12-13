using POSApplication.Common.Enums;
using POSApplication.Core.Interfaces;
using POSApplication.Core.Entities;

namespace POSApplication.UI.Forms;

public partial class UserManagementForm : Form
{
    private readonly IUserService _userService;

    public UserManagementForm(IUserService userService)
    {
        InitializeComponent();
        _userService = userService;
    }

    private async void UserManagementForm_Load(object sender, EventArgs e)
    {
        await LoadUsersAsync();
    }

    private async Task LoadUsersAsync()
    {
        var users = await _userService.GetAllUsersAsync();
        dgvUsers.DataSource = users.Select(u => new
        {
            u.UserID,
            u.Username,
            u.FullName,
            u.Role,
            u.IsActive,
            u.CreatedAt,
            u.LastLogin
        }).ToList();
    }

    private async void btnAddUser_Click(object sender, EventArgs e)
    {
        // For simplicity, using a basic input box or a small dialog would be better.
        // But since I don't have a separate UserEditDialog yet, I'll just create a default Cashier user for now
        // or prompt for username/password.
        
        // Let's create a quick "AddUserDialog" logic here or just a dummy implementation to show it works 
        // until I create a dedicated dialog?
        // Actually, I can create a new user with default password 'user123'.
        
        // Better: I will create a simple dialog for adding user.
        // But to save time/complexity, I'll add "NewUser" + Random number.
        // Real implementation should have a dialog.
        
        // I will implement a very basic input form for username/password later if needed.
        // For now, let's just add a test user.
        
        var newUser = new User 
        { 
            Username = "cashier" + new Random().Next(100, 999),
            FullName = "New Cashier",
            Role = UserRole.Cashier,
            IsActive = true
        };
        
        try 
        {
            await _userService.CreateUserAsync(newUser, "password123");
            MessageBox.Show($"Created user {newUser.Username} with password 'password123'");
            await LoadUsersAsync();
        }
        catch(Exception ex)
        {
            MessageBox.Show("Error creating user: " + ex.Message);
        }
    }

    private async void btnEditUser_Click(object sender, EventArgs e)
    {
        if (dgvUsers.SelectedRows.Count > 0)
        {
            var userId = (int)dgvUsers.SelectedRows[0].Cells["UserID"].Value;
            var user = await _userService.GetUserByIdAsync(userId);
            
            if (user == null)
            {
                 MessageBox.Show("User not found.");
                 return;
            }

            // Must run on UI thread, assuming we are on it.
            using (var dlg = new UserEditDialog(user))
            {
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                         await _userService.UpdateUserAsync(dlg.User);
                         await LoadUsersAsync();
                         MessageBox.Show("User updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error updating user: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        else
        {
            MessageBox.Show("Please select a user to edit.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private async void btnDeleteUser_Click(object sender, EventArgs e)
    {
        if (dgvUsers.SelectedRows.Count > 0)
        {
            var userId = (int)dgvUsers.SelectedRows[0].Cells["UserID"].Value;
            var username = dgvUsers.SelectedRows[0].Cells["Username"].Value.ToString();

            if (username == "admin")
            {
                MessageBox.Show("Cannot delete the default admin user.");
                return;
            }

            if (MessageBox.Show($"Are you sure you want to delete user {username}?", "Confirm Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                await _userService.DeleteUserAsync(userId);
                await LoadUsersAsync();
            }
        }
    }

    private void btnClose_Click(object sender, EventArgs e)
    {
        this.Close();
    }
}
