using POSApplication.Core.DTOs;

namespace POSApplication.UI.Services;

/// <summary>
/// In-memory shopping cart manager
/// </summary>
public class CartManager
{
    private readonly List<CartItemDto> _items = new();

    public IReadOnlyList<CartItemDto> Items => _items.AsReadOnly();

    public event EventHandler? CartChanged;

    public void AddItem(ProductDto product, decimal quantity = 1)
    {
        // Check if product already in cart
        var existingItem = _items.FirstOrDefault(i => i.ProductID == product.ProductID);
        
        if (existingItem != null)
        {
            // Update quantity
            existingItem.Quantity += quantity;
        }
        else
        {
            // Add new item
            _items.Add(new CartItemDto
            {
                ProductID = product.ProductID,
                ProductName = product.ProductName,
                SKU = product.SKU,
                Quantity = quantity,
                UnitPrice = product.SellPrice,
                TaxRate = product.TaxRate,
                DiscountAmount = 0
            });
        }

        OnCartChanged();
    }

    public void UpdateQuantity(int productId, decimal quantity)
    {
        var item = _items.FirstOrDefault(i => i.ProductID == productId);
        if (item != null)
        {
            if (quantity <= 0)
            {
                _items.Remove(item);
            }
            else
            {
                item.Quantity = quantity;
            }
            OnCartChanged();
        }
    }

    public void RemoveItem(int productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductID == productId);
        if (item != null)
        {
            _items.Remove(item);
            OnCartChanged();
        }
    }

    public void Clear()
    {
        _items.Clear();
        OnCartChanged();
    }

    public (decimal Subtotal, decimal TaxAmount, decimal Total) CalculateTotals()
    {
        decimal subtotal = _items.Sum(i => i.Subtotal);
        decimal taxAmount = _items.Sum(i => i.TaxAmount);
        decimal total = _items.Sum(i => i.LineTotal);

        return (subtotal, taxAmount, total);
    }

    public bool HasItems() => _items.Any();

    public int ItemCount => _items.Count;

    private void OnCartChanged()
    {
        CartChanged?.Invoke(this, EventArgs.Empty);
    }
}
