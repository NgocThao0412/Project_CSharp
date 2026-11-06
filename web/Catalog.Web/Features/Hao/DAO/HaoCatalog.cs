using System.Collections.Generic;
using System.Linq;

namespace Catalog.Web.Features.Hao;

public interface IHaoCatalog
{
    HaoProduct? Get(int productId);
    IReadOnlyList<HaoProduct> All();
}

public sealed class HaoDemoCatalog : IHaoCatalog
{
    private readonly List<HaoProduct> _products = new()
    {
        new(101, "CPU Intel Core i5 (mẫu)", 3950000, 10),
        new(102, "RAM DDR4 16GB (mẫu)", 890000, 15),
        new(103, "SSD NVMe 1TB (mẫu)", 1490000, 8),
        new(104, "Nguồn 650W (mẫu)", 1190000, 4)
    };
    public HaoProduct? Get(int productId) => _products.FirstOrDefault(p => p.ProductId == productId);
    public IReadOnlyList<HaoProduct> All() => _products;
}
