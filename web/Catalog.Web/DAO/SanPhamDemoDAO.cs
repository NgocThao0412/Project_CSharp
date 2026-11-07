using Catalog.Web.Models;

namespace Catalog.Web.Services;

// Dữ liệu mẫu để chạy giao diện khi CHƯA kết nối SQL Server, không phải dữ liệu bán hàng thực tế.
public sealed class DemoProductRepository : IProductRepository
{
    private static readonly IReadOnlyList<CatalogProduct> Data = new List<CatalogProduct>
    {
        new(1, "CPU Intel Core i5", "CPU", "Intel", 4290000m, 12, null, "Bộ xử lý dành cho học tập và làm việc."),
        new(2, "CPU AMD Ryzen 5", "CPU", "AMD", 3990000m, 7, null, "Bộ xử lý đa nhiệm cho máy tính cá nhân."),
        new(3, "RAM DDR4 16GB", "RAM", "Kingston", 1090000m, 21, null, "Bộ nhớ phù hợp nhiều cấu hình."),
        new(4, "RAM DDR5 32GB", "RAM", "Corsair", 2490000m, 4, null, "Bộ nhớ dung lượng cao."),
        new(5, "SSD NVMe 1TB", "Ổ cứng", "Samsung", 2090000m, 9, null, "Ổ lưu trữ tốc độ cao."),
        new(6, "SSD SATA 512GB", "Ổ cứng", "Crucial", 890000m, 0, null, "Ổ lưu trữ phổ thông."),
        new(7, "Mainboard B760", "Bo mạch chủ", "ASUS", 3290000m, 6, null, "Bo mạch chủ dành cho desktop."),
        new(8, "Card đồ họa RTX 4060", "Card đồ họa", "MSI", 7990000m, 3, null, "Card đồ họa chơi game và thiết kế."),
        new(9, "Nguồn máy tính 650W", "Nguồn", "Cooler Master", 1390000m, 17, null, "Nguồn máy tính công suất 650 W."),
        new(10, "Vỏ case Mid Tower", "Vỏ máy", "DeepCool", 990000m, 11, null, "Vỏ máy tính thiết kế thông thoáng.")
    };

    public Task<IReadOnlyList<CatalogProduct>> FeaturedAsync(int count, CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<CatalogProduct>>(Data.Take(Math.Clamp(count, 1, 20)).ToList());

    public Task<IReadOnlyList<string>> CategoriesAsync(CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<string>>(Data.Select(x => x.Category).Distinct().OrderBy(x => x).ToList());

    public Task<PagedCatalog> SearchAsync(CatalogQuery query, CancellationToken ct)
    {
        var result = Data.AsEnumerable();
        if (query.SafeKeyword.Length > 0)
            result = result.Where(x => x.Name.Contains(query.SafeKeyword, StringComparison.OrdinalIgnoreCase) ||
                x.Category.Contains(query.SafeKeyword, StringComparison.OrdinalIgnoreCase) ||
                x.Brand.Contains(query.SafeKeyword, StringComparison.OrdinalIgnoreCase));
        if (query.SafeCategory.Length > 0)
            result = result.Where(x => string.Equals(x.Category, query.SafeCategory, StringComparison.OrdinalIgnoreCase));
        var items = result.OrderBy(x => x.Id).ToList();
        return Task.FromResult(new PagedCatalog(
            items.Skip((query.SafePage - 1) * CatalogQuery.PageSize).Take(CatalogQuery.PageSize).ToList(),
            items.Count, query.SafePage, CatalogQuery.PageSize, query.SafeKeyword, query.SafeCategory));
    }

    public Task<CatalogProduct?> GetAsync(int id, CancellationToken ct) =>
        Task.FromResult(Data.FirstOrDefault(x => x.Id == id));
}
