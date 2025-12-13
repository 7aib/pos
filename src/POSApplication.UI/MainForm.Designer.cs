namespace POSApplication.UI
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            exitToolStripMenuItem = new ToolStripMenuItem();
            salesToolStripMenuItem = new ToolStripMenuItem();
            inventoryToolStripMenuItem = new ToolStripMenuItem();
            productsToolStripMenuItem = new ToolStripMenuItem();
            reportsToolStripMenuItem = new ToolStripMenuItem();
            reportsToolStripMenuItem = new ToolStripMenuItem();
            settingsToolStripMenuItem = new ToolStripMenuItem();
            manageUsersToolStripMenuItem = new ToolStripMenuItem();
            changePasswordToolStripMenuItem = new ToolStripMenuItem();
            logoutToolStripMenuItem = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            toolStripStatusUser = new ToolStripStatusLabel();
            labelWelcome = new Label();
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, salesToolStripMenuItem, inventoryToolStripMenuItem, reportsToolStripMenuItem, settingsToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1024, 24);
            menuStrip1.TabIndex = 0;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { changePasswordToolStripMenuItem, logoutToolStripMenuItem, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // changePasswordToolStripMenuItem
            // 
            changePasswordToolStripMenuItem.Name = "changePasswordToolStripMenuItem";
            changePasswordToolStripMenuItem.Size = new Size(180, 22);
            changePasswordToolStripMenuItem.Text = "Change Password";
            changePasswordToolStripMenuItem.Click += changePasswordToolStripMenuItem_Click;
            // 
            // logoutToolStripMenuItem
            // 
            logoutToolStripMenuItem.Name = "logoutToolStripMenuItem";
            logoutToolStripMenuItem.Size = new Size(180, 22);
            logoutToolStripMenuItem.Text = "Logout";
            logoutToolStripMenuItem.Click += logoutToolStripMenuItem_Click;
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(180, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // salesToolStripMenuItem
            // 
            salesToolStripMenuItem.Name = "salesToolStripMenuItem";
            salesToolStripMenuItem.Size = new Size(45, 20);
            salesToolStripMenuItem.Text = "Sales";
            salesToolStripMenuItem.Click += salesToolStripMenuItem_Click;
            // 
            // inventoryToolStripMenuItem
            // 
            inventoryToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { productsToolStripMenuItem });
            inventoryToolStripMenuItem.Name = "inventoryToolStripMenuItem";
            inventoryToolStripMenuItem.Size = new Size(69, 20);
            inventoryToolStripMenuItem.Text = "Inventory";
            // 
            // productsToolStripMenuItem
            // 
            productsToolStripMenuItem.Name = "productsToolStripMenuItem";
            productsToolStripMenuItem.Size = new Size(122, 22);
            productsToolStripMenuItem.Text = "Products";
            productsToolStripMenuItem.Click += productsToolStripMenuItem_Click;
            // 
            // reportsToolStripMenuItem
            // 
            reportsToolStripMenuItem.Name = "reportsToolStripMenuItem";
            reportsToolStripMenuItem.Size = new Size(59, 20);
            reportsToolStripMenuItem.Name = "reportsToolStripMenuItem";
            reportsToolStripMenuItem.Size = new Size(59, 20);
            reportsToolStripMenuItem.Text = "Reports";
            reportsToolStripMenuItem.Click += reportsToolStripMenuItem_Click;
            // 
            // settingsToolStripMenuItem
            // 
            settingsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { manageUsersToolStripMenuItem });
            settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            settingsToolStripMenuItem.Size = new Size(61, 20);
            settingsToolStripMenuItem.Text = "Settings";
            // 
            // manageUsersToolStripMenuItem
            // 
            manageUsersToolStripMenuItem.Name = "manageUsersToolStripMenuItem";
            manageUsersToolStripMenuItem.Size = new Size(180, 22);
            manageUsersToolStripMenuItem.Text = "Manage Users";
            manageUsersToolStripMenuItem.Click += manageUsersToolStripMenuItem_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1, toolStripStatusUser });
            statusStrip1.Location = new Point(0, 744);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(1024, 24);
            statusStrip1.TabIndex = 1;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(42, 19);
            toolStripStatusLabel1.Text = "Ready";
            // 
            // toolStripStatusUser
            // 
            toolStripStatusUser.Name = "toolStripStatusUser";
            toolStripStatusUser.Size = new Size(967, 19);
            toolStripStatusUser.Spring = true;
            toolStripStatusUser.Text = "User: Admin";
            toolStripStatusUser.TextAlign = ContentAlignment.MiddleRight;
            // 
            // labelWelcome
            // 
            labelWelcome.Anchor = AnchorStyles.None;
            labelWelcome.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            labelWelcome.Location = new Point(12, 300);
            labelWelcome.Name = "labelWelcome";
            labelWelcome.Size = new Size(1000, 100);
            labelWelcome.TabIndex = 2;
            labelWelcome.Text = "Welcome to POS Application\r\nPhase 2 - Backend Complete";
            labelWelcome.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1024, 768);
            Controls.Add(labelWelcome);
            Controls.Add(statusStrip1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "POS Application";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem changePasswordToolStripMenuItem;
        private ToolStripMenuItem logoutToolStripMenuItem;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem salesToolStripMenuItem;
        private ToolStripMenuItem inventoryToolStripMenuItem;
        private ToolStripMenuItem productsToolStripMenuItem;
        private ToolStripMenuItem reportsToolStripMenuItem;
        private ToolStripMenuItem settingsToolStripMenuItem;
        private ToolStripMenuItem manageUsersToolStripMenuItem;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private ToolStripStatusLabel toolStripStatusUser;
        private Label labelWelcome;
    }
}
