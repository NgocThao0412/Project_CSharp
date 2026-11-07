using System.Collections.Generic;
using System.Linq;

namespace Catalog.Web.Features.BanHang;

public interface IDanhMucSanPham
{
    SanPhamBanHang? Get(int productId);
    IReadOnlyList<SanPhamBanHang> All();
}

public sealed class DanhMucSanPhamDemo : IDanhMucSanPham
{
    private readonly List<SanPhamBanHang> _products = new()
    {
        new(101, "CPU Intel Core i5 (mẫu)", 3950000, 10),
        new(102, "RAM DDR4 16GB (mẫu)", 890000, 15),
        new(103, "SSD NVMe 1TB (mẫu)", 1490000, 8),
        new(104, "Nguồn 650W (mẫu)", 1190000, 4)
    };
    public SanPhamBanHang? Get(int productId) => _products.FirstOrDefault(p => p.ProductId == productId);
    public IReadOnlyList<SanPhamBanHang> All() => _products;
}
