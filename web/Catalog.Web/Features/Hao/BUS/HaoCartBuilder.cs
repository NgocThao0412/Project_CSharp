using System.Collections.Generic;

namespace Catalog.Web.Features.Hao;

public sealed class HaoCartBuilder
{
    private readonly IHaoCatalog _catalog;
    public HaoCartBuilder(IHaoCatalog catalog) => _catalog = catalog;
    public HaoCartPageVm Build(IEnumerable<HaoCartEntry> entries)
    {
        var lines = new List<HaoCartLine>();
        foreach (var entry in entries)
        {
            var product = _catalog.Get(entry.ProductId);
            if (entry.Quantity is < 1 or > 99) continue;
            if (product is { IsOnSale: true, PriceVnd: >= 0 })
                lines.Add(new HaoCartLine(product.ProductId, product.Name, product.PriceVnd, entry.Quantity, product.ActualStock));
            else
                lines.Add(new HaoCartLine(entry.ProductId, $"Sản phẩm mã {entry.ProductId} không còn bán", 0, entry.Quantity, 0, false));
        }
        return new HaoCartPageVm { Lines = lines };
    }
}
