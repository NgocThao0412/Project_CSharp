namespace Catalog.Web.Models;

public sealed record CatalogProduct(
    int Id,
    string Name,
    string Category,
    string Brand,
    decimal Price,
    int AvailableQuantity,
    string? ImageUrl,
    string Description);

public sealed record CatalogQuery(string? Keyword, string? Category, int Page = 1)
{
    public const int PageSize = 9;
    public int SafePage => Math.Clamp(Page, 1, 10000);
    public string SafeKeyword => (Keyword ?? "").Trim()[..Math.Min((Keyword ?? "").Trim().Length, 100)];
    public string SafeCategory => (Category ?? "").Trim()[..Math.Min((Category ?? "").Trim().Length, 100)];
}

public sealed record PagedCatalog(
    IReadOnlyList<CatalogProduct> Items,
    int Total,
    int Page,
    int PageSize,
    string Keyword,
    string Category)
{
    public int PageCount => Math.Max(1, (Total + PageSize - 1) / PageSize);
}

public sealed record CatalogListViewModel(PagedCatalog Results, IReadOnlyList<string> Categories);
