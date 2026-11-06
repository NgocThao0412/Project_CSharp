using Microsoft.AspNetCore.Mvc;
using Catalog.Web.Services;

namespace Catalog.Web.Controllers;

public sealed class HomeController(IProductRepository products) : Controller
{
    [HttpGet("/")]
    public async Task<IActionResult> Index(CancellationToken ct) =>
        View(await products.FeaturedAsync(6, ct));

    [HttpGet("/loi")]
    public IActionResult Error() => View();
}
