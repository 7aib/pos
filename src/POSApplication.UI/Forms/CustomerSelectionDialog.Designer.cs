namespace POSApplication.UI.Forms;

partial class CustomerSelectionDialog
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
        this.txtSearch = new System.Windows.Forms.TextBox();
        this.btnSearch = new System.Windows.Forms.Button();
        this.dgvCustomers = new System.Windows.Forms.DataGridView();
        this.btnSelect = new System.Windows.Forms.Button();
        this.btnCancel = new System.Windows.Forms.Button();
        this.btnAddCustomer = new System.Windows.Forms.Button();
        ((System.ComponentModel.ISupportInitialize)(this.dgvCustomers)).BeginInit();
        this.SuspendLayout();
        // 
        // txtSearch
        // 
        this.txtSearch.Location = new System.Drawing.Point(12, 12);
        this.txtSearch.Name = "txtSearch";
        this.txtSearch.PlaceholderText = "Search by Name, Phone or Email";
        this.txtSearch.Size = new System.Drawing.Size(300, 23);
        this.txtSearch.TabIndex = 0;
        this.txtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown);
        // 
        // btnSearch
        // 
        this.btnSearch.Location = new System.Drawing.Point(318, 11);
        this.btnSearch.Name = "btnSearch";
        this.btnSearch.Size = new System.Drawing.Size(75, 25);
        this.btnSearch.TabIndex = 1;
        this.btnSearch.Text = "Search";
        this.btnSearch.UseVisualStyleBackColor = true;
        this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
        // 
        // dgvCustomers
        // 
        this.dgvCustomers.AllowUserToAddRows = false;
        this.dgvCustomers.AllowUserToDeleteRows = false;
        this.dgvCustomers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.dgvCustomers.Location = new System.Drawing.Point(12, 50);
        this.dgvCustomers.MultiSelect = false;
        this.dgvCustomers.Name = "dgvCustomers";
        this.dgvCustomers.ReadOnly = true;
        this.dgvCustomers.RowTemplate.Height = 25;
        this.dgvCustomers.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        this.dgvCustomers.Size = new System.Drawing.Size(560, 250);
        this.dgvCustomers.TabIndex = 2;
        this.dgvCustomers.DoubleClick += new System.EventHandler(this.dgvCustomers_DoubleClick);
        // 
        // btnSelect
        // 
        this.btnSelect.DialogResult = System.Windows.Forms.DialogResult.OK;
        this.btnSelect.Location = new System.Drawing.Point(416, 315);
        this.btnSelect.Name = "btnSelect";
        this.btnSelect.Size = new System.Drawing.Size(75, 30);
        this.btnSelect.TabIndex = 3;
        this.btnSelect.Text = "Select";
        this.btnSelect.UseVisualStyleBackColor = true;
        this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
        // 
        // btnCancel
        // 
        this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        this.btnCancel.Location = new System.Drawing.Point(497, 315);
        this.btnCancel.Name = "btnCancel";
        this.btnCancel.Size = new System.Drawing.Size(75, 30);
        this.btnCancel.TabIndex = 4;
        this.btnCancel.Text = "Cancel";
        this.btnCancel.UseVisualStyleBackColor = true;
        // 
        // btnAddCustomer
        // 
        this.btnAddCustomer.Location = new System.Drawing.Point(12, 315);
        this.btnAddCustomer.Name = "btnAddCustomer";
        this.btnAddCustomer.Size = new System.Drawing.Size(120, 30);
        this.btnAddCustomer.TabIndex = 5;
        this.btnAddCustomer.Text = "New Customer";
        this.btnAddCustomer.UseVisualStyleBackColor = true;
        this.btnAddCustomer.Click += new System.EventHandler(this.btnAddCustomer_Click);
        // 
        // CustomerSelectionDialog
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(584, 361);
        this.Controls.Add(this.btnAddCustomer);
        this.Controls.Add(this.btnCancel);
        this.Controls.Add(this.btnSelect);
        this.Controls.Add(this.dgvCustomers);
        this.Controls.Add(this.btnSearch);
        this.Controls.Add(this.txtSearch);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "CustomerSelectionDialog";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        this.Text = "Select Customer";
        ((System.ComponentModel.ISupportInitialize)(this.dgvCustomers)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private System.Windows.Forms.TextBox txtSearch;
    private System.Windows.Forms.Button btnSearch;
    private System.Windows.Forms.DataGridView dgvCustomers;
    private System.Windows.Forms.Button btnSelect;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnAddCustomer;
}
