using POSApplication.Common.Enums;
using POSApplication.Core.DTOs;
using POSApplication.Core.Interfaces;
using POSApplication.Infrastructure.Interfaces;
using POSApplication.UI.Services;

namespace POSApplication.UI.Forms;

/// <summary>
/// Main POS Checkout Form for sales processing
/// </summary>
public partial class POSCheckoutForm : Form
{
    private readonly IProductService _productService;
    private readonly ISalesService _salesService;
    private readonly IPaymentService _paymentService;
    private readonly IPrinterService _printerService;
    private readonly IInventoryService _inventoryService;
    private readonly CartManager _cartManager;

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
        IPrinterService printerService,
        IInventoryService inventoryService)
    {
        _productService = productService;
        _salesService = salesService;
        _paymentService = paymentService;
        _printerService = printerService;
        _inventoryService = inventoryService;
        _cartManager = new CartManager();
        _cartManager.CartChanged += CartManager_CartChanged;

        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = "POS Checkout";
        this.Size = new Size(1200, 700);
        this.StartPosition = FormStartPosition.CenterScreen;

        // ===== LEFT PANEL: Product Search =====
        var panelLeft = new Panel
        {
            Dock = DockStyle.Left,
            Width = 500,
            Padding = new Padding(10)
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
            Width = 120,
            Height = 35
        };
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
            Padding = new Padding(10)
        };

        var lblCart = new Label
        {
            Text = "Shopping Cart:",
            Location = new Point(10, 10),
            Width = 150,
            Font = new Font("Segoe UI", 12, FontStyle.Bold)
        };

        _gridCart = new DataGridView
        {
            Location = new Point(10, 40),
            Size = new Size(650, 400),
            AutoGenerateColumns = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            RowHeadersVisible = false
        };

        _gridCart.Columns.AddRange(new DataGridViewColumn[]
        {
            new DataGridViewTextBoxColumn { HeaderText = "Product", DataPropertyName = "ProductName", Width = 200 },
            new DataGridViewTextBoxColumn { HeaderText = "SKU", DataPropertyName = "SKU", Width = 100 },
            new DataGridViewTextBoxColumn { HeaderText = "Qty", DataPropertyName = "Quantity", Width = 60 },
            new DataGridViewTextBoxColumn { HeaderText = "Unit Price", DataPropertyName = "UnitPrice", Width = 90, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } },
            new DataGridViewTextBoxColumn { HeaderText = "Tax", DataPropertyName = "TaxAmount", Width = 80, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } },
            new DataGridViewTextBoxColumn { HeaderText = "Total", DataPropertyName = "LineTotal", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } }
        });

        var btnRemoveItem = new Button
        {
            Text = "Remove Item",
            Location = new Point(10, 450),
            Width = 120,
            Height = 35
        };
        btnRemoveItem.Click += BtnRemoveItem_Click;

        _btnClearCart = new Button
        {
            Text = "Clear Cart",
            Location = new Point(140, 450),
            Width = 120,
            Height = 35
        };
        _btnClearCart.Click += BtnClearCart_Click;

        // Totals panel
        var panelTotals = new Panel
        {
            Location = new Point(10, 500),
            Size = new Size(650, 120),
            BorderStyle = BorderStyle.FixedSingle
        };

        _lblSubtotal = new Label
        {
            Text = "Subtotal: $0.00",
            Location = new Point(420, 10),
            Width = 200,
            Font = new Font("Segoe UI", 11),
            TextAlign = ContentAlignment.MiddleRight
        };

        _lblTax = new Label
        {
            Text = "Tax: $0.00",
            Location = new Point(420, 40),
            Width = 200,
            Font = new Font("Segoe UI", 11),
            TextAlign = ContentAlignment.MiddleRight
        };

        _lblTotal = new Label
        {
            Text = "TOTAL: $0.00",
            Location = new Point(420, 70),
            Width = 200,
            Font = new Font("Segoe UI", 14, FontStyle.Bold),
            ForeColor = Color.DarkGreen,
            TextAlign = ContentAlignment.MiddleRight
        };

        panelTotals.Controls.AddRange(new Control[] { _lblSubtotal, _lblTax, _lblTotal });

        _btnCompleteSale = new Button
        {
            Text = "Complete Sale (F12)",
            Location = new Point(480, 630),
            Width = 180,
            Height = 50,
            Font = new Font("Segoe UI", 12, FontStyle.Bold),
            BackColor = Color.Green,
            ForeColor = Color.White
        };
        _btnCompleteSale.Click += BtnCompleteSale_Click;

        panelRight.Controls.AddRange(new Control[]
        {
            lblCart, _gridCart,
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
            var paymentDialog = new PaymentDialog(_paymentService, total);
            if (paymentDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            // Create sale DTO
            var saleDto = new SaleDto
            {
                CustomerID = null, // Walk-in customer
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
