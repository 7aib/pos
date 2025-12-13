using POSApplication.Core.DTOs;
using POSApplication.Core.Interfaces;

namespace POSApplication.UI.Forms;

/// <summary>
/// Dialog for creating and editing products
/// </summary>
public partial class ProductEditDialog : Form
{
    private readonly IProductService _productService;
    private readonly ProductDto? _existingProduct;
    private bool _isEditMode;

    public ProductDto? Product { get; private set; }

    // Constructor for Add mode
    public ProductEditDialog(IProductService productService)
    {
        _productService = productService;
        _isEditMode = false;
        InitializeComponent();
        this.Text = "Add Product";
    }

    // Constructor for Edit mode
    public ProductEditDialog(IProductService productService, ProductDto product)
    {
        _productService = productService;
        _existingProduct = product;
        _isEditMode = true;
        InitializeComponent();
        this.Text = "Edit Product";
        LoadProductData();
    }

    private void InitializeComponent()
    {
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.StartPosition = FormStartPosition.CenterParent;
        this.Size = new Size(500, 600);

        // Create controls
        var lblSKU = new Label { Text = "SKU:", Location = new Point(20, 20), Width = 120 };
        var txtSKU = new TextBox { Name = "txtSKU", Location = new Point(150, 18), Width = 300 };

        var lblBarcode = new Label { Text = "Barcode:", Location = new Point(20, 55), Width = 120 };
        var txtBarcode = new TextBox { Name = "txtBarcode", Location = new Point(150, 53), Width = 300 };

        var lblProductName = new Label { Text = "Product Name: *", Location = new Point(20, 90), Width = 120 };
        var txtProductName = new TextBox { Name = "txtProductName", Location = new Point(150, 88), Width = 300 };

        var lblDescription = new Label { Text = "Description:", Location = new Point(20, 125), Width = 120 };
        var txtDescription = new TextBox { Name = "txtDescription", Location = new Point(150, 123), Width = 300, Height = 60, Multiline = true };

        var lblCostPrice = new Label { Text = "Cost Price:", Location = new Point(20, 195), Width = 120 };
        var txtCostPrice = new NumericUpDown { Name = "txtCostPrice", Location = new Point(150, 193), Width = 140, DecimalPlaces = 2, Maximum = 1000000 };

        var lblSellPrice = new Label { Text = "Sell Price: *", Location = new Point(20, 230), Width = 120 };
        var txtSellPrice = new NumericUpDown { Name = "txtSellPrice", Location = new Point(150, 228), Width = 140, DecimalPlaces = 2, Maximum = 1000000 };

        var lblTaxRate = new Label { Text = "Tax Rate (%):", Location = new Point(20, 265), Width = 120 };
        var txtTaxRate = new NumericUpDown { Name = "txtTaxRate", Location = new Point(150, 263), Width = 140, DecimalPlaces = 2, Maximum = 100 };

        var lblCurrentStock = new Label { Text = "Current Stock:", Location = new Point(20, 300), Width = 120 };
        var txtCurrentStock = new NumericUpDown { Name = "txtCurrentStock", Location = new Point(150, 298), Width = 140, Maximum = 1000000 };

        var lblMinStock = new Label { Text = "Min Stock Level:", Location = new Point(20, 335), Width = 120 };
        var txtMinStock = new NumericUpDown { Name = "txtMinStock", Location = new Point(150, 333), Width = 140, Maximum = 1000000 };

        var lblReorderPoint = new Label { Text = "Reorder Point:", Location = new Point(20, 370), Width = 120 };
        var txtReorderPoint = new NumericUpDown { Name = "txtReorderPoint", Location = new Point(150, 368), Width = 140, Maximum = 1000000 };

        var lblUnitOfMeasure = new Label { Text = "Unit of Measure:", Location = new Point(20, 405), Width = 120 };
        var txtUnitOfMeasure = new TextBox { Name = "txtUnitOfMeasure", Location = new Point(150, 403), Width = 140 };

        var chkIsActive = new CheckBox { Name = "chkIsActive", Text = "Active", Location = new Point(150, 438), Checked = true };

        // Buttons
        var btnSave = new Button 
        { 
            Name = "btnSave",
            Text = "Save", 
            Location = new Point(250, 510), 
            Width = 90, 
            Height = 35,
            DialogResult = DialogResult.OK
        };
        btnSave.Click += BtnSave_Click;

        var btnCancel = new Button 
        { 
            Name = "btnCancel",
            Text = "Cancel", 
            Location = new Point(350, 510), 
            Width = 90, 
            Height = 35,
            DialogResult = DialogResult.Cancel
        };

        // Add controls to form
        this.Controls.AddRange(new Control[]
        {
            lblSKU, txtSKU,
            lblBarcode, txtBarcode,
            lblProductName, txtProductName,
            lblDescription, txtDescription,
            lblCostPrice, txtCostPrice,
            lblSellPrice, txtSellPrice,
            lblTaxRate, txtTaxRate,
            lblCurrentStock, txtCurrentStock,
            lblMinStock, txtMinStock,
            lblReorderPoint, txtReorderPoint,
            lblUnitOfMeasure, txtUnitOfMeasure,
            chkIsActive,
            btnSave, btnCancel
        });

        this.AcceptButton = btnSave;
        this.CancelButton = btnCancel;
    }

    private void LoadProductData()
    {
        if (_existingProduct == null) return;

        GetControl<TextBox>("txtSKU").Text = _existingProduct.SKU;
        GetControl<TextBox>("txtBarcode").Text = _existingProduct.Barcode ?? string.Empty;
        GetControl<TextBox>("txtProductName").Text = _existingProduct.ProductName;
        GetControl<TextBox>("txtDescription").Text = _existingProduct.Description ?? string.Empty;
        GetControl<NumericUpDown>("txtCostPrice").Value = _existingProduct.CostPrice ?? 0;
        GetControl<NumericUpDown>("txtSellPrice").Value = _existingProduct.SellPrice;
        GetControl<NumericUpDown>("txtTaxRate").Value = _existingProduct.TaxRate;
        GetControl<NumericUpDown>("txtCurrentStock").Value = _existingProduct.CurrentStock;
        GetControl<NumericUpDown>("txtMinStock").Value = _existingProduct.MinStockLevel;
        GetControl<NumericUpDown>("txtReorderPoint").Value = _existingProduct.ReorderPoint ?? 0;
        GetControl<TextBox>("txtUnitOfMeasure").Text = _existingProduct.UnitOfMeasure ?? string.Empty;
        GetControl<CheckBox>("chkIsActive").Checked = _existingProduct.IsActive;
    }

    private async void BtnSave_Click(object? sender, EventArgs e)
    {
        try
        {
            // Validate inputs
            var productName = GetControl<TextBox>("txtProductName").Text.Trim();
            var sku = GetControl<TextBox>("txtSKU").Text.Trim();
            var sellPrice = GetControl<NumericUpDown>("txtSellPrice").Value;

            if (string.IsNullOrWhiteSpace(productName))
            {
                MessageBox.Show("Product name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(sku))
            {
                MessageBox.Show("SKU is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (sellPrice <= 0)
            {
                MessageBox.Show("Sell price must be greater than zero.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Create DTO
            var productDto = new ProductDto
            {
                ProductID = _existingProduct?.ProductID ?? 0,
                SKU = sku,
                Barcode = GetControl<TextBox>("txtBarcode").Text.Trim(),
                ProductName = productName,
                Description = GetControl<TextBox>("txtDescription").Text.Trim(),
                CostPrice = GetControl<NumericUpDown>("txtCostPrice").Value,
                SellPrice = sellPrice,
                TaxRate = GetControl<NumericUpDown>("txtTaxRate").Value,
                CurrentStock = (int)GetControl<NumericUpDown>("txtCurrentStock").Value,
                MinStockLevel = (int)GetControl<NumericUpDown>("txtMinStock").Value,
                ReorderPoint = (int)GetControl<NumericUpDown>("txtReorderPoint").Value,
                UnitOfMeasure = GetControl<TextBox>("txtUnitOfMeasure").Text.Trim(),
                IsActive = GetControl<CheckBox>("chkIsActive").Checked
            };

            // Save to database
            if (_isEditMode)
            {
                Product = await _productService.UpdateProductAsync(productDto);
                MessageBox.Show("Product updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                Product = await _productService.CreateProductAsync(productDto);
                MessageBox.Show("Product created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving product: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            this.DialogResult = DialogResult.None; // Keep dialog open
        }
    }

    private T GetControl<T>(string name) where T : Control
    {
        return (T)this.Controls.Find(name, false).First();
    }
}
