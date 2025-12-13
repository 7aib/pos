using POSApplication.Core.DTOs;
using POSApplication.Core.Interfaces;

namespace POSApplication.UI.Forms;

/// <summary>
/// Product Management Form - CRUD operations for products
/// </summary>
public partial class ProductManagementForm : Form
{
    private readonly IProductService _productService;
    private DataGridView _gridProducts;
    private TextBox _txtSearch;
    private Button _btnAdd;
    private Button _btnEdit;
    private Button _btnDelete;
    private Button _btnRefresh;
    private Label _lblStatus;
    private List<ProductDto> _products = new();

    public ProductManagementForm(IProductService productService)
    {
        _productService = productService;
        InitializeComponent();
        LoadProducts();
    }

    private void InitializeComponent()
    {
        this.Text = "Product Management";
        this.Size = new Size(1000, 600);
        this.StartPosition = FormStartPosition.CenterScreen;

        // Search panel
        var panelSearch = new Panel
        {
            Dock = DockStyle.Top,
            Height = 60,
            Padding = new Padding(10)
        };

        var lblSearch = new Label
        {
            Text = "Search:",
            Location = new Point(10, 18),
            Width = 60
        };

        _txtSearch = new TextBox
        {
            Location = new Point(75, 15),
            Width = 300
        };
        _txtSearch.TextChanged += TxtSearch_TextChanged;

        _btnRefresh = new Button
        {
            Text = "Refresh",
            Location = new Point(400, 13),
            Width = 80
        };
        _btnRefresh.Click += BtnRefresh_Click;

        panelSearch.Controls.AddRange(new Control[] { lblSearch, _txtSearch, _btnRefresh });

        // Button panel
        var panelButtons = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 60,
            Padding = new Padding(10)
        };

        _btnAdd = new Button
        {
            Text = "Add Product",
            Location = new Point(10, 15),
            Width = 100,
            Height = 35
        };
        _btnAdd.Click += BtnAdd_Click;

        _btnEdit = new Button
        {
            Text = "Edit",
            Location = new Point(120, 15),
            Width = 100,
            Height = 35
        };
        _btnEdit.Click += BtnEdit_Click;

        _btnDelete = new Button
        {
            Text = "Delete",
            Location = new Point(230, 15),
            Width = 100,
            Height = 35
        };
        _btnDelete.Click += BtnDelete_Click;

        _lblStatus = new Label
        {
            Text = "Ready",
            Location = new Point(750, 20),
            Width = 200,
            TextAlign = ContentAlignment.MiddleRight
        };

        panelButtons.Controls.AddRange(new Control[] { _btnAdd, _btnEdit, _btnDelete, _lblStatus });

        // DataGridView
        _gridProducts = new DataGridView
        {
            Dock = DockStyle.Fill,
            AutoGenerateColumns = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            RowHeadersVisible = false
        };

        // Configure columns
        _gridProducts.Columns.AddRange(new DataGridViewColumn[]
        {
            new DataGridViewTextBoxColumn { Name = "ProductID", HeaderText = "ID", DataPropertyName = "ProductID", Width = 50 },
            new DataGridViewTextBoxColumn { Name = "SKU", HeaderText = "SKU", DataPropertyName = "SKU", Width = 100 },
            new DataGridViewTextBoxColumn { Name = "Barcode", HeaderText = "Barcode", DataPropertyName = "Barcode", Width = 120 },
            new DataGridViewTextBoxColumn { Name = "ProductName", HeaderText = "Product Name", DataPropertyName = "ProductName", Width = 250 },
            new DataGridViewTextBoxColumn { Name = "SellPrice", HeaderText = "Price", DataPropertyName = "SellPrice", Width = 80, DefaultCellStyle = new DataGridViewCellStyle { Format = "C2" } },
            new DataGridViewTextBoxColumn { Name = "CurrentStock", HeaderText = "Stock", DataPropertyName = "CurrentStock", Width = 80 },
            new DataGridViewTextBoxColumn { Name = "MinStockLevel", HeaderText = "Min Stock", DataPropertyName = "MinStockLevel", Width = 80 },
            new DataGridViewCheckBoxColumn { Name = "IsActive", HeaderText = "Active", DataPropertyName = "IsActive", Width = 60 }
        });

        _gridProducts.CellFormatting += GridProducts_CellFormatting;
        _gridProducts.DoubleClick += GridProducts_DoubleClick;

        // Add controls to form
        this.Controls.Add(_gridProducts);
        this.Controls.Add(panelSearch);
        this.Controls.Add(panelButtons);
    }

    private async void LoadProducts()
    {
        try
        {
            _lblStatus.Text = "Loading products...";
            _products = (await _productService.GetAllProductsAsync()).ToList();
            _gridProducts.DataSource = _products;
            _lblStatus.Text = $"Total products: {_products.Count}";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading products: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            _lblStatus.Text = "Error loading products";
        }
    }

    private async void TxtSearch_TextChanged(object? sender, EventArgs e)
    {
        var searchTerm = _txtSearch.Text.Trim();
        
        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            _gridProducts.DataSource = _products;
            _lblStatus.Text = $"Total products: {_products.Count}";
            return;
        }

        try
        {
            var results = await _productService.SearchProductsAsync(searchTerm);
            _gridProducts.DataSource = results.ToList();
            _lblStatus.Text = $"Found {results.Count()} products";
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error searching products: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnRefresh_Click(object? sender, EventArgs e)
    {
        _txtSearch.Clear();
        LoadProducts();
    }

    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        var dialog = new ProductEditDialog(_productService);
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            LoadProducts();
        }
    }

    private void BtnEdit_Click(object? sender, EventArgs e)
    {
        var selectedProduct = GetSelectedProduct();
        if (selectedProduct == null)
        {
            MessageBox.Show("Please select a product to edit.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var dialog = new ProductEditDialog(_productService, selectedProduct);
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            LoadProducts();
        }
    }

    private async void BtnDelete_Click(object? sender, EventArgs e)
    {
        var selectedProduct = GetSelectedProduct();
        if (selectedProduct == null)
        {
            MessageBox.Show("Please select a product to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var result = MessageBox.Show(
            $"Are you sure you want to delete '{selectedProduct.ProductName}'?\n\nThis will mark the product as inactive.",
            "Confirm Delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            try
            {
                await _productService.DeleteProductAsync(selectedProduct.ProductID);
                MessageBox.Show("Product deleted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadProducts();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting product: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    private void GridProducts_DoubleClick(object? sender, EventArgs e)
    {
        BtnEdit_Click(sender, e);
    }

    private void GridProducts_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (_gridProducts.Columns[e.ColumnIndex].Name == "CurrentStock")
        {
            if (e.RowIndex >= 0 && e.RowIndex < _products.Count)
            {
                var product = _products[e.RowIndex];
                // Highlight low stock items
                if (product.CurrentStock <= product.MinStockLevel)
                {
                    e.CellStyle.BackColor = Color.LightCoral;
                    e.CellStyle.ForeColor = Color.DarkRed;
                }
            }
        }
    }

    private ProductDto? GetSelectedProduct()
    {
        if (_gridProducts.SelectedRows.Count > 0)
        {
            return _gridProducts.SelectedRows[0].DataBoundItem as ProductDto;
        }
        return null;
    }
}
