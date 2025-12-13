namespace POSApplication.UI.Forms;

partial class CustomerPaymentForm
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
        this.lblCustomerName = new System.Windows.Forms.Label();
        this.lblCurrentBalance = new System.Windows.Forms.Label();
        this.label1 = new System.Windows.Forms.Label();
        this.label2 = new System.Windows.Forms.Label();
        this.label3 = new System.Windows.Forms.Label();
        this.numAmount = new System.Windows.Forms.NumericUpDown();
        this.cmbPaymentMethod = new System.Windows.Forms.ComboBox();
        this.txtReference = new System.Windows.Forms.TextBox();
        this.btnPay = new System.Windows.Forms.Button();
        this.btnCancel = new System.Windows.Forms.Button();
        ((System.ComponentModel.ISupportInitialize)(this.numAmount)).BeginInit();
        this.SuspendLayout();
        // 
        // lblCustomerName
        // 
        this.lblCustomerName.AutoSize = true;
        this.lblCustomerName.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        this.lblCustomerName.Location = new System.Drawing.Point(20, 20);
        this.lblCustomerName.Name = "lblCustomerName";
        this.lblCustomerName.Size = new System.Drawing.Size(140, 21);
        this.lblCustomerName.TabIndex = 0;
        this.lblCustomerName.Text = "Customer Name";
        // 
        // lblCurrentBalance
        // 
        this.lblCurrentBalance.AutoSize = true;
        this.lblCurrentBalance.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.lblCurrentBalance.Location = new System.Drawing.Point(20, 50);
        this.lblCurrentBalance.Name = "lblCurrentBalance";
        this.lblCurrentBalance.Size = new System.Drawing.Size(144, 19);
        this.lblCurrentBalance.TabIndex = 1;
        this.lblCurrentBalance.Text = "Current Balance: $0.00";
        // 
        // label1
        // 
        this.label1.AutoSize = true;
        this.label1.Location = new System.Drawing.Point(20, 90);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(103, 15);
        this.label1.TabIndex = 2;
        this.label1.Text = "Payment Amount:";
        // 
        // label2
        // 
        this.label2.AutoSize = true;
        this.label2.Location = new System.Drawing.Point(20, 130);
        this.label2.Name = "label2";
        this.label2.Size = new System.Drawing.Size(102, 15);
        this.label2.TabIndex = 4;
        this.label2.Text = "Payment Method:";
        // 
        // label3
        // 
        this.label3.AutoSize = true;
        this.label3.Location = new System.Drawing.Point(20, 170);
        this.label3.Name = "label3";
        this.label3.Size = new System.Drawing.Size(62, 15);
        this.label3.TabIndex = 6;
        this.label3.Text = "Reference:";
        // 
        // numAmount
        // 
        this.numAmount.DecimalPlaces = 2;
        this.numAmount.Location = new System.Drawing.Point(140, 88);
        this.numAmount.Maximum = new decimal(new int[] {
        1000000,
        0,
        0,
        0});
        this.numAmount.Name = "numAmount";
        this.numAmount.Size = new System.Drawing.Size(150, 23);
        this.numAmount.TabIndex = 3;
        // 
        // cmbPaymentMethod
        // 
        this.cmbPaymentMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.cmbPaymentMethod.FormattingEnabled = true;
        this.cmbPaymentMethod.Items.AddRange(new object[] {
        "Cash",
        "Card",
        "Other"});
        this.cmbPaymentMethod.Location = new System.Drawing.Point(140, 127);
        this.cmbPaymentMethod.Name = "cmbPaymentMethod";
        this.cmbPaymentMethod.Size = new System.Drawing.Size(150, 23);
        this.cmbPaymentMethod.TabIndex = 5;
        // 
        // txtReference
        // 
        this.txtReference.Location = new System.Drawing.Point(140, 167);
        this.txtReference.Name = "txtReference";
        this.txtReference.Size = new System.Drawing.Size(150, 23);
        this.txtReference.TabIndex = 7;
        // 
        // btnPay
        // 
        this.btnPay.Location = new System.Drawing.Point(130, 220);
        this.btnPay.Name = "btnPay";
        this.btnPay.Size = new System.Drawing.Size(75, 30);
        this.btnPay.TabIndex = 8;
        this.btnPay.Text = "Pay";
        this.btnPay.UseVisualStyleBackColor = true;
        this.btnPay.Click += new System.EventHandler(this.btnPay_Click);
        // 
        // btnCancel
        // 
        this.btnCancel.Location = new System.Drawing.Point(215, 220);
        this.btnCancel.Name = "btnCancel";
        this.btnCancel.Size = new System.Drawing.Size(75, 30);
        this.btnCancel.TabIndex = 9;
        this.btnCancel.Text = "Cancel";
        this.btnCancel.UseVisualStyleBackColor = true;
        this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
        // 
        // CustomerPaymentForm
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(334, 281);
        this.Controls.Add(this.btnCancel);
        this.Controls.Add(this.btnPay);
        this.Controls.Add(this.txtReference);
        this.Controls.Add(this.cmbPaymentMethod);
        this.Controls.Add(this.numAmount);
        this.Controls.Add(this.label3);
        this.Controls.Add(this.label2);
        this.Controls.Add(this.label1);
        this.Controls.Add(this.lblCurrentBalance);
        this.Controls.Add(this.lblCustomerName);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "CustomerPaymentForm";
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        this.Text = "Receive Payment";
        ((System.ComponentModel.ISupportInitialize)(this.numAmount)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    private System.Windows.Forms.Label lblCustomerName;
    private System.Windows.Forms.Label lblCurrentBalance;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.NumericUpDown numAmount;
    private System.Windows.Forms.ComboBox cmbPaymentMethod;
    private System.Windows.Forms.TextBox txtReference;
    private System.Windows.Forms.Button btnPay;
    private System.Windows.Forms.Button btnCancel;
}
