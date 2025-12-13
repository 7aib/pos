namespace POSApplication.UI.Forms
{
    partial class ReportsForm
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
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.btnLoadReport = new System.Windows.Forms.Button();
            this.dgvTopProducts = new System.Windows.Forms.DataGridView();
            this.lblTotalRevenue = new System.Windows.Forms.Label();
            this.lblTotalTransactions = new System.Windows.Forms.Label();
            this.lblAvgTransaction = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTopProducts)).BeginInit();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dtpDate
            // 
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDate.Location = new System.Drawing.Point(12, 70);
            this.dtpDate.Name = "dtpDate";
            this.dtpDate.Size = new System.Drawing.Size(120, 23);
            this.dtpDate.TabIndex = 0;
            // 
            // btnLoadReport
            // 
            this.btnLoadReport.Location = new System.Drawing.Point(138, 70);
            this.btnLoadReport.Name = "btnLoadReport";
            this.btnLoadReport.Size = new System.Drawing.Size(75, 23);
            this.btnLoadReport.TabIndex = 1;
            this.btnLoadReport.Text = "Load";
            this.btnLoadReport.UseVisualStyleBackColor = true;
            this.btnLoadReport.Click += new System.EventHandler(this.btnLoadReport_Click);
            // 
            // dgvTopProducts
            // 
            this.dgvTopProducts.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTopProducts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTopProducts.Location = new System.Drawing.Point(3, 19);
            this.dgvTopProducts.Name = "dgvTopProducts";
            this.dgvTopProducts.RowTemplate.Height = 25;
            this.dgvTopProducts.Size = new System.Drawing.Size(519, 288);
            this.dgvTopProducts.TabIndex = 2;
            // 
            // lblTotalRevenue
            // 
            this.lblTotalRevenue.AutoSize = true;
            this.lblTotalRevenue.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTotalRevenue.Location = new System.Drawing.Point(15, 30);
            this.lblTotalRevenue.Name = "lblTotalRevenue";
            this.lblTotalRevenue.Size = new System.Drawing.Size(134, 21);
            this.lblTotalRevenue.TabIndex = 3;
            this.lblTotalRevenue.Text = "Total Revenue: $0";
            // 
            // lblTotalTransactions
            // 
            this.lblTotalTransactions.AutoSize = true;
            this.lblTotalTransactions.Location = new System.Drawing.Point(15, 60);
            this.lblTotalTransactions.Name = "lblTotalTransactions";
            this.lblTotalTransactions.Size = new System.Drawing.Size(110, 15);
            this.lblTotalTransactions.TabIndex = 4;
            this.lblTotalTransactions.Text = "Total Transactions: 0";
            // 
            // lblAvgTransaction
            // 
            this.lblAvgTransaction.AutoSize = true;
            this.lblAvgTransaction.Location = new System.Drawing.Point(15, 80);
            this.lblAvgTransaction.Name = "lblAvgTransaction";
            this.lblAvgTransaction.Size = new System.Drawing.Size(125, 15);
            this.lblAvgTransaction.TabIndex = 5;
            this.lblAvgTransaction.Text = "Avg Transaction Val: $0";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.WhiteSmoke;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 50);
            this.panel1.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(163, 30);
            this.label1.TabIndex = 0;
            this.label1.Text = "Sales Dashboard";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lblTotalRevenue);
            this.groupBox1.Controls.Add(this.lblTotalTransactions);
            this.groupBox1.Controls.Add(this.lblAvgTransaction);
            this.groupBox1.Location = new System.Drawing.Point(12, 110);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(240, 310);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Daily Stats";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.dgvTopProducts);
            this.groupBox2.Location = new System.Drawing.Point(263, 110);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(525, 310);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Top Selling Products";
            // 
            // ReportsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.btnLoadReport);
            this.Controls.Add(this.dtpDate);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReportsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Reports";
            this.Load += new System.EventHandler(this.ReportsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTopProducts)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.Button btnLoadReport;
        private System.Windows.Forms.DataGridView dgvTopProducts;
        private System.Windows.Forms.Label lblTotalRevenue;
        private System.Windows.Forms.Label lblTotalTransactions;
        private System.Windows.Forms.Label lblAvgTransaction;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}
