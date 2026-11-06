using Catalog.Web.Models;
using Catalog.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Web.Controllers;

// React chỉ gọi API này, backend giữ nguyên IProductRepository và truy vấn SQL Server.
[ApiController]
[Route("api/catalog")]
public sealed class CatalogApiController(IProductRepository products) : ControllerBase
{
    [HttpGet("featured")]
    public async Task<ActionResult<IReadOnlyList<CatalogProduct>>> Featured(
        [FromQuery] int count = 6, CancellationToken ct = default)
    {
        return Ok(await products.FeaturedAsync(Math.Clamp(count, 1, 20), ct));
    }

    [HttpGet("categories")]
    public async Task<ActionResult<IReadOnlyList<string>>> Categories(CancellationToken ct)
    {
        return Ok(await products.CategoriesAsync(ct));
    }

    [HttpGet("products")]
    public async Task<ActionResult<PagedCatalog>> Search(
        [FromQuery] string? q, [FromQuery] string? category,
        [FromQuery] int page = 1, CancellationToken ct = default)
    {
        return Ok(await products.SearchAsync(new CatalogQuery(q, category, page), ct));
    }

    [HttpGet("products/{id:int}")]
    public async Task<ActionResult<CatalogProduct>> Details(int id, CancellationToken ct)
    {
        if (id <= 0) return NotFound();
        var item = await products.GetAsync(id, ct);
        return item is null ? NotFound() : Ok(item);
    }
}
