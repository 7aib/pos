using POSApplication.Common.Enums;
using POSApplication.Core.DTOs;
using POSApplication.Core.Entities;
using POSApplication.Core.Interfaces;
using POSApplication.Infrastructure.Interfaces;
using POSApplication.Data.Interfaces;
using POSApplication.UI.Services;
using POSApplication.UI.Theme;

namespace POSApplication.UI.Forms;

/// <summary>
/// Main POS Checkout Form for sales processing
/// </summary>
public partial class POSCheckoutForm : Form
{
    private readonly IProductService _productService;
    private readonly ISalesService _salesService;
    private readonly IPaymentService _paymentService;
    private readonly ICreditService _creditService;
    private readonly IPrinterService _printerService;
    private readonly IInventoryService _inventoryService;
    private readonly ICustomerRepository _customerRepository;
    private readonly CartManager _cartManager;

    private Customer? _selectedCustomer;
    private Label _lblCustomerName;
    private Button _btnSelectCustomer;
    private Button _btnPayBalance;

    private TextBox _txtSearch;
    private TextBox _txtBarcode;
    private DataGridView _gridSearchResults;
    private DataGridView _gridCart;
    private NumericUpDown _numQuantity;
    private Button _btnAddToCart;
    private Label _lblSubtotal;
    private Label _lblTax;
    private Label _lblTotal;
    private Button _btnCompleteSale;
    private Button _btnClearCart;
    private List<ProductDto> _searchResults = new();

    public POSCheckoutForm(
        IProductService productService,
        ISalesService salesService,
        IPaymentService paymentService,
        ICreditService creditService,
        IPrinterService printerService,
        IInventoryService inventoryService,
        ICustomerRepository customerRepository)
    {
        _productService = productService;
        _salesService = salesService;
        _paymentService = paymentService;
        _creditService = creditService;
        _printerService = printerService;
        _inventoryService = inventoryService;
        _customerRepository = customerRepository;
        _cartManager = new CartManager();
        _cartManager.CartChanged += CartManager_CartChanged;

        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = "POS Checkout";
        this.Size = new Size(1300, 800);
        this.StartPosition = FormStartPosition.CenterScreen;

        // ===== LEFT PANEL: Product Search =====
        var panelLeft = new Panel
        {
            Dock = DockStyle.Left,
            Width = 500,
            Padding = new Padding(15),
            BackColor = AppTheme.PanelColor
        };

        var lblSearch = new Label
        {
            Text = "Search Product (Name/SKU):",
            Location = new Point(10, 10),
            Width = 200
        };

        _txtSearch = new TextBox
        {
            Location = new Point(10, 35),
            Width = 320
        };
        _txtSearch.TextChanged += TxtSearch_TextChanged;

        var lblBarcode = new Label
        {
            Text = "Barcode Scanner:",
            Location = new Point(10, 70),
            Width = 200
        };

        _txtBarcode = new TextBox
        {
            Location = new Point(10, 95),
            Width = 320
        };
        _txtBarcode.KeyPress += TxtBarcode_KeyPress;

        // Search results grid
        var lblSearchResults = new Label
        {
            Text = "Search Results:",
            Location = new Point(10, 130),
            Width = 150
        };

        _gridSearchResults = new DataGridView
        {
            Location = new Point(10, 155),
            Size = new Size(470, 300),
            AutoGenerateColumns = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            RowHeadersVisible = false
        };

        _gridSearchResults.Columns.AddRange(new DataGridViewColumn[]
        {
            new DataGridViewTextBoxColumn { HeaderText = "SKU", DataPropertyName = "SKU", Width = 100 },
            new DataGridViewTextBoxColumn { HeaderText = "Product", DataPropertyName = "ProductName", Width = 200 },
            new DataGridViewTextBoxColumn { HeaderText = "Price", DataPropertyName = "SellPrice", Width = 80, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } },
            new DataGridViewTextBoxColumn { HeaderText = "Stock", DataPropertyName = "CurrentStock", Width = 80 }
        });

        var lblQuantity = new Label
        {
            Text = "Quantity:",
            Location = new Point(10, 470),
            Width = 80
        };

        _numQuantity = new NumericUpDown
        {
            Location = new Point(100, 468),
            Width = 100,
            Minimum = 1,
            Maximum = 1000,
            Value = 1
        };

        _btnAddToCart = new Button
        {
            Text = "Add to Cart",
            Location = new Point(220, 465),
            Width = 140,
            Height = 35
        };
        AppTheme.ApplyButtonTheme(_btnAddToCart, AppTheme.PrimaryColor);
        _btnAddToCart.Click += BtnAddToCart_Click;

        panelLeft.Controls.AddRange(new Control[]
        {
            lblSearch, _txtSearch,
            lblBarcode, _txtBarcode,
            lblSearchResults, _gridSearchResults,
            lblQuantity, _numQuantity, _btnAddToCart
        });

        // ===== RIGHT PANEL: Shopping Cart =====
        var panelRight = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(15),
            BackColor = AppTheme.BackgroundColor
        };

        var lblCart = new Label
        {
            Text = "Shopping Cart:",
            Location = new Point(10, 10),
            Width = 150,
            Font = AppTheme.SubHeaderFont,
            ForeColor = AppTheme.TextColor
        };

        // Customer Selection Controls
        _lblCustomerName = new Label 
        { 
            Text = "Customer: Walk-in", 
            Location = new Point(170, 10), 
            Width = 300, 
            Font = new Font("Segoe UI", 10),
            TextAlign = ContentAlignment.MiddleRight
        };

        _btnSelectCustomer = new Button 
        { 
            Text = "Select Customer", 
            Location = new Point(480, 5), 
            Width = 140, 
            Height = 35 
        };
        AppTheme.ApplySecondaryButtonTheme(_btnSelectCustomer);
        _btnSelectCustomer.Click += BtnSelectCustomer_Click;

        _btnPayBalance = new Button 
        { 
            Text = "Pay Balance", 
            Location = new Point(630, 5), 
            Width = 120, 
            Height = 35,
            Enabled = false
        };
        AppTheme.ApplySecondaryButtonTheme(_btnPayBalance); // Default styling
        _btnPayBalance.Click += BtnPayBalance_Click;

        _gridCart = new DataGridView
        {
            Location = new Point(10, 50),
            Size = new Size(700, 400),
            AutoGenerateColumns = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            RowHeadersVisible = false,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None
        };
        // TODO: Apply grid styling helper if available

        _gridCart.Columns.AddRange(new DataGridViewColumn[]
        {
            new DataGridViewTextBoxColumn { HeaderText = "Product", DataPropertyName = "ProductName", Width = 220 },
            new DataGridViewTextBoxColumn { HeaderText = "SKU", DataPropertyName = "SKU", Width = 100 },
            new DataGridViewTextBoxColumn { HeaderText = "Qty", DataPropertyName = "Quantity", Width = 60 },
            new DataGridViewTextBoxColumn { HeaderText = "Unit Price", DataPropertyName = "UnitPrice", Width = 90, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } },
            new DataGridViewTextBoxColumn { HeaderText = "Tax", DataPropertyName = "TaxAmount", Width = 80, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } },
            new DataGridViewTextBoxColumn { HeaderText = "Total", DataPropertyName = "LineTotal", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } }
        });

        var btnRemoveItem = new Button
        {
            Text = "Remove Item",
            Location = new Point(10, 460),
            Width = 140,
            Height = 35
        };
        AppTheme.ApplySecondaryButtonTheme(btnRemoveItem);
        btnRemoveItem.BackColor = Color.WhiteSmoke;
        btnRemoveItem.Click += BtnRemoveItem_Click;

        _btnClearCart = new Button
        {
            Text = "Clear Cart",
            Location = new Point(160, 460),
            Width = 140,
            Height = 35
        };
        AppTheme.ApplySecondaryButtonTheme(_btnClearCart);
        _btnClearCart.ForeColor = AppTheme.DangerColor;
        _btnClearCart.Click += BtnClearCart_Click;

        // Totals panel
        var panelTotals = new Panel
        {
            Location = new Point(10, 510),
            Size = new Size(700, 130),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = AppTheme.PanelColor
        };

        _lblSubtotal = new Label
        {
            Text = "Subtotal: $0.00",
            Location = new Point(450, 10),
            Width = 240,
            Font = AppTheme.SubHeaderFont,
            TextAlign = ContentAlignment.MiddleRight
        };

        _lblTax = new Label
        {
            Text = "Tax: $0.00",
            Location = new Point(450, 40),
            Width = 240,
            Font = AppTheme.SubHeaderFont,
            TextAlign = ContentAlignment.MiddleRight
        };

        _lblTotal = new Label
        {
            Text = "TOTAL: $0.00",
            Location = new Point(450, 75),
            Width = 240,
            Font = AppTheme.LargeFont,
            ForeColor = AppTheme.SuccessColor,
            TextAlign = ContentAlignment.MiddleRight
        };

        panelTotals.Controls.AddRange(new Control[] { _lblSubtotal, _lblTax, _lblTotal });

        _btnCompleteSale = new Button
        {
            Text = "COMPLETE SALE (F12)",
            Location = new Point(510, 650),
            Width = 200,
            Height = 55
        };
        AppTheme.ApplyButtonTheme(_btnCompleteSale, AppTheme.SuccessColor);
        _btnCompleteSale.Click += BtnCompleteSale_Click;

        panelRight.Controls.AddRange(new Control[]
        {
            lblCart, _lblCustomerName, _btnSelectCustomer, _btnPayBalance, _gridCart,
            btnRemoveItem, _btnClearCart,
            panelTotals, _btnCompleteSale
        });

        // Add panels to form
        this.Controls.Add(panelRight);
        this.Controls.Add(panelLeft);

        // Keyboard shortcuts
        this.KeyPreview = true;
        this.KeyDown += POSCheckoutForm_KeyDown;

        UpdateCartDisplay();
    }

    private async void TxtSearch_TextChanged(object? sender, EventArgs e)
    {
        var searchTerm = _txtSearch.Text.Trim();
        
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            _searchResults.Clear();
            _gridSearchResults.DataSource = null;
            return;
        }

        if (searchTerm.Length < 2) return;

        try
        {
            _searchResults = (await _productService.SearchProductsAsync(searchTerm))
                .Where(p => p.IsActive)
                .ToList();
            _gridSearchResults.DataSource = _searchResults;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error searching products: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }



    private void BtnSelectCustomer_Click(object? sender, EventArgs e)
    {
        using (var dlg = new CustomerSelectionDialog(_customerRepository))
        {
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                _selectedCustomer = dlg.SelectedCustomer;
                _lblCustomerName.Text = _selectedCustomer != null 
                    ? $"Customer: {_selectedCustomer.FirstName} {_selectedCustomer.LastName}"
                    : "Customer: Walk-in";
                    
                if (_selectedCustomer?.CreditAccount != null)
                {
                     _lblCustomerName.Text += $" (Credit: {_selectedCustomer.CreditAccount.CurrentBalance:C2} / {_selectedCustomer.CreditAccount.CreditLimit:C2})";
                }
                
                _btnPayBalance.Enabled = _selectedCustomer != null;
            }
        }
    }



    private async void BtnPayBalance_Click(object? sender, EventArgs e)
    {
        if (_selectedCustomer == null) return;

        using (var dlg = new CustomerPaymentForm(_creditService, _selectedCustomer, 1)) // TODO: UserID
        {
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                // Refresh customer balance display
                 var balance = await _creditService.GetCustomerBalanceAsync(_selectedCustomer.CustomerID);
                 // Need to refresh CreditAccount object if used elsewhere, but for label we can just update text
                 if (_selectedCustomer.CreditAccount != null) _selectedCustomer.CreditAccount.CurrentBalance = balance;
                 
                  _lblCustomerName.Text = $"Customer: {_selectedCustomer.FirstName} {_selectedCustomer.LastName}";
                 
                 // Fetch latest limit?
                 var account = await _creditService.GetCreditAccountAsync(_selectedCustomer.CustomerID);
                 if (account != null)
                 {
                     _lblCustomerName.Text += $" (Credit: {account.CurrentBalance:C2} / {account.CreditLimit:C2})";
                 }
            }
        }
    }

    private async void TxtBarcode_KeyPress(object? sender, KeyPressEventArgs e)
    {
        if (e.KeyChar == (char)Keys.Return)
        {
            e.Handled = true;
            var barcode = _txtBarcode.Text.Trim();
            
            if (string.IsNullOrWhiteSpace(barcode)) return;

            try
            {
                var product = await _productService.GetProductByBarcodeAsync(barcode);
                if (product != null && product.IsActive)
                {
                    _cartManager.AddItem(product, 1);
                    _txtBarcode.Clear();
                }
                else
                {
                    MessageBox.Show($"Product not found for barcode: {barcode}", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error processing barcode: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void BtnAddToCart_Click(object? sender, EventArgs e)
    {
        if (_gridSearchResults.SelectedRows.Count == 0)
        {
            MessageBox.Show("Please select a product from search results.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var selectedProduct = _gridSearchResults.SelectedRows[0].DataBoundItem as ProductDto;
        if (selectedProduct != null)
        {
            var quantity = _numQuantity.Value;
            
            // Check stock availability
            if (selectedProduct.CurrentStock < quantity)
            {
                MessageBox.Show($"Insufficient stock. Available: {selectedProduct.CurrentStock}", "Stock Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _cartManager.AddItem(selectedProduct, quantity);
            _numQuantity.Value = 1;
        }
    }

    private void BtnRemoveItem_Click(object? sender, EventArgs e)
    {
        if (_gridCart.SelectedRows.Count == 0)
        {
            MessageBox.Show("Please select an item to remove.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var selectedItem = _gridCart.SelectedRows[0].DataBoundItem as CartItemDto;
        if (selectedItem != null)
        {
            _cartManager.RemoveItem(selectedItem.ProductID);
        }
    }

    private void BtnClearCart_Click(object? sender, EventArgs e)
    {
        if (!_cartManager.HasItems()) return;

        var result = MessageBox.Show("Are you sure you want to clear the cart?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (result == DialogResult.Yes)
        {
            _cartManager.Clear();
        }
    }

    private async void BtnCompleteSale_Click(object? sender, EventArgs e)
    {
        if (!_cartManager.HasItems())
        {
            MessageBox.Show("Cart is empty. Please add items before completing sale.", "Empty Cart", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var (subtotal, tax, total) = _cartManager.CalculateTotals();

            // Open payment dialog
            var paymentDialog = new PaymentDialog(_paymentService, _creditService, total, _selectedCustomer);
            if (paymentDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            // Create sale DTO
            var saleDto = new SaleDto
            {
                CustomerID = _selectedCustomer?.CustomerID, 
                CashierID = 1, // TODO: Get from logged-in user
                Subtotal = subtotal,
                TaxAmount = tax,
                DiscountAmount = 0,
                TotalAmount = total,
                AmountPaid = paymentDialog.TotalPaid,
                ChangeGiven = paymentDialog.Change,
                PaymentStatus = PaymentStatus.Paid,
                SaleItems = _cartManager.Items.Select(item => new SaleItemDto
                {
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    TaxRate = item.TaxRate,
                    DiscountAmount = item.DiscountAmount,
                    LineTotal = item.LineTotal
                }).ToList(),
                Payments = paymentDialog.Payments
            };

            // Process sale
            var completedSale = await _salesService.CreateSaleAsync(saleDto);

            // Process Credit Account payments if any
            foreach (var payment in completedSale.Payments)
            {
                if (payment.PaymentMethod == PaymentMethod.CreditAccount && completedSale.CustomerID.HasValue)
                {
                   await _creditService.ProcessCreditPaymentAsync(
                       completedSale.CustomerID.Value, 
                       payment.Amount, 
                       completedSale.SaleID, 
                       completedSale.CashierID); 
                }
            }

            // Print receipt
            await _printerService.PrintReceiptAsync(completedSale);

            // Show success message
            MessageBox.Show(
                $"Sale completed successfully!\n\n" +
                $"Sale Number: {completedSale.SaleNumber}\n" +
                $"Total: {completedSale.TotalAmount:C2}\n" +
                $"Change: {completedSale.ChangeGiven:C2}\n\n" +
                $"Receipt saved to file.",
                "Sale Complete",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            // Clear cart
            _cartManager.Clear();
            _txtSearch.Clear();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error completing sale: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void CartManager_CartChanged(object? sender, EventArgs e)
    {
        UpdateCartDisplay();
    }

    private void UpdateCartDisplay()
    {
        _gridCart.DataSource = null;
        _gridCart.DataSource = _cartManager.Items.ToList();

        var (subtotal, tax, total) = _cartManager.CalculateTotals();
        _lblSubtotal.Text = $"Subtotal: {subtotal:C2}";
        _lblTax.Text = $"Tax: {tax:C2}";
        _lblTotal.Text = $"TOTAL: {total:C2}";

        _btnCompleteSale.Enabled = _cartManager.HasItems();
    }

    private void POSCheckoutForm_KeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.F12 && _cartManager.HasItems())
        {
            BtnCompleteSale_Click(sender, e);
        }
    }

    protected override void OnKeyPress(KeyPressEventArgs e)
    {
        base.OnKeyPress(e);

        // If the focused control is NOT one of our input fields, redirect to barcode scanner
        if (!(_txtSearch.Focused || _numQuantity.Focused || _txtBarcode.Focused))
        {
            // If it's a valid character, append to barcode field and focus it
            if (!char.IsControl(e.KeyChar))
            {
                _txtBarcode.Focus();
                _txtBarcode.AppendText(e.KeyChar.ToString());
                e.Handled = true;
            }
        }
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        _txtBarcode.Focus();
    }
}
