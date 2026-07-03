using POSApplication.Common.Enums;
using POSApplication.Core.DTOs;
using POSApplication.Core.Interfaces;
using POSApplication.Infrastructure.Services;
using POSApplication.UI.Theme;

namespace POSApplication.UI.Forms;

public class ReturnForm : Form
{
    private readonly IReturnService _returnService;
    private readonly int _currentUserId;
    private TextBox _txtSaleNumber;
    private Button _btnSearch;
    private DataGridView _gridItems;
    private Label _lblSaleInfo;
    private Label _lblTotalRefund;
    private ComboBox _cmbRefundMethod;
    private TextBox _txtNotes;
    private Button _btnProcessReturn;
    private Button _btnCancel;
    private SaleDto? _currentSale;
    private List<ReturnItemDto> _returnItems = new();

    public ReturnForm(IReturnService returnService, int currentUserId)
    {
        _returnService = returnService;
        _currentUserId = currentUserId;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = "Process Return";
        this.Size = new Size(800, 600);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = AppTheme.BackgroundColor;

        // Search panel
        var panelSearch = new Panel { Dock = DockStyle.Top, Height = 60, BackColor = AppTheme.PanelColor, Padding = new Padding(10) };

        var lblSearch = new Label { Text = "Sale Number:", Location = new Point(10, 18), Width = 100, Font = AppTheme.BodyFont };
        _txtSaleNumber = new TextBox { Location = new Point(120, 15), Width = 200, Font = AppTheme.BodyFont };
        _txtSaleNumber.KeyPress += (s, e) => { if (e.KeyChar == (char)Keys.Return) BtnSearch_Click(s, e); };

        _btnSearch = new Button { Text = "Search", Location = new Point(340, 13), Width = 100, Height = 30 };
        AppTheme.ApplyButtonTheme(_btnSearch, AppTheme.PrimaryColor);
        _btnSearch.Click += BtnSearch_Click;

        _lblSaleInfo = new Label { Text = "", Location = new Point(460, 18), Width = 300, Font = AppTheme.BodyFont };

        panelSearch.Controls.AddRange(new Control[] { lblSearch, _txtSaleNumber, _btnSearch, _lblSaleInfo });

        // Items grid
        _gridItems = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoGenerateColumns = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            RowHeadersVisible = false,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None
        };

        _gridItems.Columns.AddRange(new DataGridViewColumn[]
        {
            new DataGridViewTextBoxColumn { Name = "SaleItemID", HeaderText = "ID", DataPropertyName = "SaleItemID", Width = 50, ReadOnly = true },
            new DataGridViewTextBoxColumn { Name = "ProductName", HeaderText = "Product", DataPropertyName = "ProductName", Width = 200, ReadOnly = true },
            new DataGridViewTextBoxColumn { Name = "Quantity", HeaderText = "Qty Purchased", DataPropertyName = "Quantity", Width = 100, ReadOnly = true },
            new DataGridViewTextBoxColumn { Name = "UnitPrice", HeaderText = "Unit Price", DataPropertyName = "UnitPrice", Width = 100, ReadOnly = true, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } },
            new DataGridViewTextBoxColumn { Name = "ReturnQty", HeaderText = "Return Qty", Width = 100 },
            new DataGridViewTextBoxColumn { Name = "RefundAmount", HeaderText = "Refund", Width = 100, ReadOnly = true, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } }
        });

        _gridItems.CellValueChanged += GridItems_CellValueChanged;
        _gridItems.CurrentCellDirtyStateChanged += (s, e) => { if (_gridItems.IsCurrentCellDirty) _gridItems.CommitEdit(DataGridViewDataErrorContexts.Commit); };

        // Bottom panel
        var panelBottom = new Panel { Dock = DockStyle.Bottom, Height = 120, BackColor = AppTheme.PanelColor, Padding = new Padding(10) };

        _lblTotalRefund = new Label { Text = "Total Refund: $0.00", Location = new Point(10, 10), Width = 300, Font = AppTheme.HeaderFont, ForeColor = AppTheme.DangerColor };

        var lblMethod = new Label { Text = "Refund Method:", Location = new Point(10, 50), Width = 110, Font = AppTheme.BodyFont };
        _cmbRefundMethod = new ComboBox { Location = new Point(125, 47), Width = 150, DropDownStyle = ComboBoxStyle.DropDownList };
        _cmbRefundMethod.Items.AddRange(new object[] { "Cash", "Card", "CreditAccount", "StoreCredit" });
        _cmbRefundMethod.SelectedIndex = 0;

        var lblNotes = new Label { Text = "Notes:", Location = new Point(300, 50), Width = 50, Font = AppTheme.BodyFont };
        _txtNotes = new TextBox { Location = new Point(355, 47), Width = 250, Font = AppTheme.BodyFont };

        _btnProcessReturn = new Button { Text = "Process Return", Location = new Point(10, 80), Width = 140, Height = 35 };
        AppTheme.ApplyButtonTheme(_btnProcessReturn, AppTheme.DangerColor);
        _btnProcessReturn.Click += BtnProcessReturn_Click;

        _btnCancel = new Button { Text = "Cancel", Location = new Point(160, 80), Width = 100, Height = 35 };
        AppTheme.ApplySecondaryButtonTheme(_btnCancel);
        _btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

        panelBottom.Controls.AddRange(new Control[] { _lblTotalRefund, lblMethod, _cmbRefundMethod, lblNotes, _txtNotes, _btnProcessReturn, _btnCancel });

        this.Controls.Add(_gridItems);
        this.Controls.Add(panelSearch);
        this.Controls.Add(panelBottom);
        this.CancelButton = _btnCancel;
    }

    private async void BtnSearch_Click(object? sender, EventArgs e)
    {
        var saleNumber = _txtSaleNumber.Text.Trim();
        if (string.IsNullOrWhiteSpace(saleNumber))
        {
            MessageBox.Show("Please enter a sale number.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            _currentSale = await _returnService.GetSaleByNumberAsync(saleNumber);
            if (_currentSale == null)
            {
                MessageBox.Show($"Sale '{saleNumber}' not found.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _lblSaleInfo.Text = "";
                _gridItems.DataSource = null;
                return;
            }

            _lblSaleInfo.Text = $"Sale: {_currentSale.SaleNumber} | Date: {_currentSale.SaleDate:g} | Total: {_currentSale.TotalAmount:C2}";

            var returnableItems = await _returnService.GetReturnableItemsAsync(_currentSale.SaleID);
            _returnItems = returnableItems.Select(r => new ReturnItemDto
            {
                SaleItemID = r.SaleItemID,
                ProductID = r.ProductID,
                ProductName = r.ProductName,
                OriginalQuantity = r.Quantity,
                UnitPrice = r.UnitPrice,
                ReturnQuantity = 0,
                RefundAmount = 0
            }).ToList();

            _gridItems.DataSource = _returnItems;
            UpdateTotalRefund();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error searching sale: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void GridItems_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;

        var item = _returnItems[e.RowIndex];
        var column = _gridItems.Columns[e.ColumnIndex];

        if (column?.Name == "ReturnQty")
        {
            var returnQtyStr = _gridItems.Rows[e.RowIndex].Cells[e.ColumnIndex].Value?.ToString() ?? "0";
            decimal returnQty;
            decimal.TryParse(returnQtyStr, out returnQty);

            returnQty = Math.Max(0, Math.Min(returnQty, item.OriginalQuantity));
            item.ReturnQuantity = returnQty;
            item.RefundAmount = returnQty * item.UnitPrice;

            _gridItems.Rows[e.RowIndex].Cells["RefundAmount"].Value = item.RefundAmount;
            UpdateTotalRefund();
        }
    }

    private void UpdateTotalRefund()
    {
        var total = _returnItems.Sum(r => r.RefundAmount);
        _lblTotalRefund.Text = $"Total Refund: {total:C2}";
        _btnProcessReturn.Enabled = total > 0 && _cmbRefundMethod.SelectedItem != null;
    }

    private async void BtnProcessReturn_Click(object? sender, EventArgs e)
    {
        if (_currentSale == null)
        {
            MessageBox.Show("Please search for a sale first.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var itemsToReturn = _returnItems.Where(r => r.ReturnQuantity > 0).ToList();
        if (itemsToReturn.Count == 0)
        {
            MessageBox.Show("Please enter return quantities for at least one item.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var refundMethod = Enum.Parse<RefundMethod>(_cmbRefundMethod.SelectedItem?.ToString() ?? "Cash");

        if (MessageBox.Show($"Process return of {itemsToReturn.Count} item(s) with total refund of {_returnItems.Sum(r => r.RefundAmount):C2}?", "Confirm Return", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        try
        {
            var success = await _returnService.ProcessReturnAsync(
                _currentSale.SaleID,
                itemsToReturn,
                refundMethod,
                _currentUserId,
                _txtNotes.Text.Trim());

            if (success)
            {
                MessageBox.Show("Return processed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error processing return: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
