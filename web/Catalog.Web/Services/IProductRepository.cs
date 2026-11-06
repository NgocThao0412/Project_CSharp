using Catalog.Web.Models;

namespace Catalog.Web.Services;

public interface IProductRepository
{
    Task<IReadOnlyList<CatalogProduct>> FeaturedAsync(int count, CancellationToken ct);
    Task<IReadOnlyList<string>> CategoriesAsync(CancellationToken ct);
    Task<PagedCatalog> SearchAsync(CatalogQuery query, CancellationToken ct);
    Task<CatalogProduct?> GetAsync(int id, CancellationToken ct);
}
