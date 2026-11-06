using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Catalog.Web.Features.Hao;

// Store ONLY product ID + quantity. Product price/name are read again on every request.
public sealed class HaoCart
{
    private const string SessionKey = "HaoCartV1";
    private readonly IHttpContextAccessor _context;

    public HaoCart(IHttpContextAccessor context) => _context = context;

    private ISession Session => _context.HttpContext?.Session
        ?? throw new InvalidOperationException("Chua kich hoat app.UseSession() trong Program.cs.");

    public List<HaoCartEntry> Read()
    {
        var json = Session.GetString(SessionKey);
        if (string.IsNullOrEmpty(json)) return new();
        try
        {
            return (JsonSerializer.Deserialize<List<HaoCartEntry>>(json) ?? new())
                .Where(x => x.ProductId > 0 && x.Quantity > 0 && x.Quantity <= 99)
                .GroupBy(x => x.ProductId)
                .Select(g => new HaoCartEntry(g.Key, Math.Min(99, g.Sum(x => x.Quantity))))
                .Take(30)
                .ToList();
        }
        catch (JsonException) { return new(); }
    }

    public bool SetQuantity(int productId, int quantity)
    {
        if (productId <= 0 || quantity is < 0 or > 99) return false;
        var cart = Read();
        var at = cart.FindIndex(x => x.ProductId == productId);
        if (quantity == 0)
        {
            if (at >= 0) cart.RemoveAt(at);
        }
        else if (at >= 0) cart[at] = new HaoCartEntry(productId, quantity);
        else if (cart.Count < 30) cart.Add(new HaoCartEntry(productId, quantity));
        else return false;
        Session.SetString(SessionKey, JsonSerializer.Serialize(cart));
        return true;
    }

    public bool Add(int productId, int quantity)
    {
        if (quantity is < 1 or > 99) return false;
        var current = Read().FirstOrDefault(x => x.ProductId == productId)?.Quantity ?? 0;
        return SetQuantity(productId, current + quantity);
    }

    public void Clear() => Session.Remove(SessionKey);
}
