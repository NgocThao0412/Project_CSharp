using System.Collections.Generic;

namespace Catalog.Web.Features.BanHang;

public sealed class TaoGioHang
{
    private readonly IDanhMucSanPham _catalog;
    public TaoGioHang(IDanhMucSanPham catalog) => _catalog = catalog;
    public GioHangViewModel Build(IEnumerable<MucGioHang> entries)
    {
        var lines = new List<DongGioHang>();
        foreach (var entry in entries)
        {
            var product = _catalog.Get(entry.ProductId);
            if (entry.Quantity is < 1 or > 99) continue;
            if (product is { IsOnSale: true, PriceVnd: >= 0 })
                lines.Add(new DongGioHang(product.ProductId, product.Name, product.PriceVnd, entry.Quantity, product.ActualStock));
            else
                lines.Add(new DongGioHang(entry.ProductId, $"Sản phẩm mã {entry.ProductId} không còn bán", 0, entry.Quantity, 0, false));
        }
        return new GioHangViewModel { Lines = lines };
    }
}
