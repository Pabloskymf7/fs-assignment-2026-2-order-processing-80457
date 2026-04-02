using System;
using System.Collections.Generic;
using System.Linq;
using Shared.DTOs;

namespace CustomerPortal.Services;

public class CartService
{
    private readonly List<CartItemDto> _items = new();

    public IReadOnlyList<CartItemDto> Items => _items.AsReadOnly();

    public decimal Total => _items.Sum(i => i.Quantity * i.UnitPrice);

    public int ItemCount => _items.Sum(i => i.Quantity);

    public event Action? OnChange;

    public void AddItem(CartItemDto item)
    {
        var existing = _items.FirstOrDefault(i => i.ProductId == item.ProductId);
        if (existing is not null)
            existing.Quantity += item.Quantity;
        else
            _items.Add(item);

        OnChange?.Invoke();
    }

    public void RemoveItem(int productId)
    {
        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if (item is not null)
        {
            _items.Remove(item);
            OnChange?.Invoke();
        }
    }

    public void Clear()
    {
        _items.Clear();
        OnChange?.Invoke();
    }
}