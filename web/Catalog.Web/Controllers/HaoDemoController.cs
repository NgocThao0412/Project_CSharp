using Catalog.Web.Features.Hao;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Catalog.Web.Controllers;

// Delete/disable this controller before deployment. Only for testing HAO feature in isolation.
[Route("hao-demo")]
public sealed class HaoDemoController : Controller
{
    private readonly IHaoCatalog _catalog;
    private readonly IWebHostEnvironment _environment;
    public HaoDemoController(IHaoCatalog catalog, IWebHostEnvironment environment)
    {
        _catalog = catalog;
        _environment = environment;
    }
    [HttpGet("")]
    public IActionResult Index() => _environment.IsDevelopment() ? View(_catalog.All()) : NotFound();
}
