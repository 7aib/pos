namespace POSApplication.UI.Forms;

partial class UserEditDialog
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.lblUsername = new System.Windows.Forms.Label();
        this.txtUsername = new System.Windows.Forms.TextBox();
        this.lblFullName = new System.Windows.Forms.Label();
        this.txtFullName = new System.Windows.Forms.TextBox();
        this.lblEmail = new System.Windows.Forms.Label();
        this.txtEmail = new System.Windows.Forms.TextBox();
        this.lblPhone = new System.Windows.Forms.Label();
        this.txtPhone = new System.Windows.Forms.TextBox();
        this.lblRole = new System.Windows.Forms.Label();
        this.cmbRole = new System.Windows.Forms.ComboBox();
        this.chkIsActive = new System.Windows.Forms.CheckBox();
        this.btnSave = new System.Windows.Forms.Button();
        this.btnCancel = new System.Windows.Forms.Button();
        this.SuspendLayout();
        // 
        // lblUsername
        // 
        this.lblUsername.AutoSize = true;
        this.lblUsername.Location = new System.Drawing.Point(20, 20);
        this.lblUsername.Name = "lblUsername";
        this.lblUsername.Size = new System.Drawing.Size(63, 15);
        this.lblUsername.TabIndex = 0;
        this.lblUsername.Text = "Username:";
        // 
        // txtUsername
        // 
        this.txtUsername.Location = new System.Drawing.Point(100, 17);
        this.txtUsername.Name = "txtUsername";
        this.txtUsername.ReadOnly = true;
        this.txtUsername.Size = new System.Drawing.Size(200, 23);
        this.txtUsername.TabIndex = 1;
        // 
        // lblFullName
        // 
        this.lblFullName.AutoSize = true;
        this.lblFullName.Location = new System.Drawing.Point(20, 60);
        this.lblFullName.Name = "lblFullName";
        this.lblFullName.Size = new System.Drawing.Size(64, 15);
        this.lblFullName.TabIndex = 2;
        this.lblFullName.Text = "Full Name:";
        // 
        // txtFullName
        // 
        this.txtFullName.Location = new System.Drawing.Point(100, 57);
        this.txtFullName.Name = "txtFullName";
        this.txtFullName.Size = new System.Drawing.Size(200, 23);
        this.txtFullName.TabIndex = 3;
        // 
        // lblEmail
        // 
        this.lblEmail.AutoSize = true;
        this.lblEmail.Location = new System.Drawing.Point(20, 100);
        this.lblEmail.Name = "lblEmail";
        this.lblEmail.Size = new System.Drawing.Size(39, 15);
        this.lblEmail.TabIndex = 4;
        this.lblEmail.Text = "Email:";
        // 
        // txtEmail
        // 
        this.txtEmail.Location = new System.Drawing.Point(100, 97);
        this.txtEmail.Name = "txtEmail";
        this.txtEmail.Size = new System.Drawing.Size(200, 23);
        this.txtEmail.TabIndex = 5;
        // 
        // lblPhone
        // 
        this.lblPhone.AutoSize = true;
        this.lblPhone.Location = new System.Drawing.Point(20, 140);
        this.lblPhone.Name = "lblPhone";
        this.lblPhone.Size = new System.Drawing.Size(44, 15);
        this.lblPhone.TabIndex = 6;
        this.lblPhone.Text = "Phone:";
        // 
        // txtPhone
        // 
        this.txtPhone.Location = new System.Drawing.Point(100, 137);
        this.txtPhone.Name = "txtPhone";
        this.txtPhone.Size = new System.Drawing.Size(200, 23);
        this.txtPhone.TabIndex = 7;
        // 
        // lblRole
        // 
        this.lblRole.AutoSize = true;
        this.lblRole.Location = new System.Drawing.Point(20, 180);
        this.lblRole.Name = "lblRole";
        this.lblRole.Size = new System.Drawing.Size(33, 15);
        this.lblRole.TabIndex = 8;
        this.lblRole.Text = "Role:";
        // 
        // cmbRole
        // 
        this.cmbRole.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.cmbRole.FormattingEnabled = true;
        this.cmbRole.Location = new System.Drawing.Point(100, 177);
        this.cmbRole.Name = "cmbRole";
        this.cmbRole.Size = new System.Drawing.Size(200, 23);
        this.cmbRole.TabIndex = 9;
        // 
        // chkIsActive
        // 
        this.chkIsActive.AutoSize = true;
        this.chkIsActive.Location = new System.Drawing.Point(100, 220);
        this.chkIsActive.Name = "chkIsActive";
        this.chkIsActive.Size = new System.Drawing.Size(70, 19);
        this.chkIsActive.TabIndex = 10;
        this.chkIsActive.Text = "Is Active";
        this.chkIsActive.UseVisualStyleBackColor = true;
        // 
        // btnSave
        // 
        this.btnSave.Location = new System.Drawing.Point(130, 260);
        this.btnSave.Name = "btnSave";
        this.btnSave.Size = new System.Drawing.Size(75, 23);
        this.btnSave.TabIndex = 11;
        this.btnSave.Text = "Save";
        this.btnSave.UseVisualStyleBackColor = true;
        this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
        // 
        // btnCancel
        // 
        this.btnCancel.Location = new System.Drawing.Point(225, 260);
        this.btnCancel.Name = "btnCancel";
        this.btnCancel.Size = new System.Drawing.Size(75, 23);
        this.btnCancel.TabIndex = 12;
        this.btnCancel.Text = "Cancel";
        this.btnCancel.UseVisualStyleBackColor = true;
        this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
        // 
        // UserEditDialog
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(334, 311);
        this.Controls.Add(this.btnCancel);
        this.Controls.Add(this.btnSave);
        this.Controls.Add(this.chkIsActive);
        this.Controls.Add(this.cmbRole);
        this.Controls.Add(this.lblRole);
        this.Controls.Add(this.txtPhone);
        this.Controls.Add(this.lblPhone);
        this.Controls.Add(this.txtEmail);
        this.Controls.Add(this.lblEmail);
        this.Controls.Add(this.txtFullName);
        this.Controls.Add(this.lblFullName);
        this.Controls.Add(this.txtUsername);
        this.Controls.Add(this.lblUsername);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "UserEditDialog";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        this.Text = "Edit User";
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private System.Windows.Forms.Label lblUsername;
    private System.Windows.Forms.TextBox txtUsername;
    private System.Windows.Forms.Label lblFullName;
    private System.Windows.Forms.TextBox txtFullName;
    private System.Windows.Forms.Label lblEmail;
    private System.Windows.Forms.TextBox txtEmail;
    private System.Windows.Forms.Label lblPhone;
    private System.Windows.Forms.TextBox txtPhone;
    private System.Windows.Forms.Label lblRole;
    private System.Windows.Forms.ComboBox cmbRole;
    private System.Windows.Forms.CheckBox chkIsActive;
    private System.Windows.Forms.Button btnSave;
    private System.Windows.Forms.Button btnCancel;
}
