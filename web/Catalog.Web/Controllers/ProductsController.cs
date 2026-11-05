using Microsoft.AspNetCore.Mvc;
using Catalog.Web.Models;
using Catalog.Web.Services;

namespace Catalog.Web.Controllers;

[Route("san-pham")]
public sealed class ProductsController(IProductRepository products) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> Index([FromQuery] string? q, [FromQuery] string? category,
        [FromQuery] int page = 1, CancellationToken ct = default)
    {
        var results = await products.SearchAsync(new CatalogQuery(q, category, page), ct);
        var categories = await products.CategoriesAsync(ct);
        return View(new CatalogListViewModel(results, categories));
    }

    [HttpGet("/tim-kiem")]
    public Task<IActionResult> Search([FromQuery] string? q, [FromQuery] string? category,
        [FromQuery] int page = 1, CancellationToken ct = default) => Index(q, category, page, ct);

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        if (id <= 0) return NotFound();
        var item = await products.GetAsync(id, ct);
        return item is null ? NotFound() : View(item);
    }
}
