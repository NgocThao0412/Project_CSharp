using Catalog.Web.Features.Hao;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Web.Controllers;

[Route("gio-hang")]
public sealed class HaoCartController : Controller
{
    private readonly HaoCart _cart;
    private readonly HaoCartBuilder _builder;
    private readonly IHaoCatalog _catalog;
    public HaoCartController(HaoCart cart, HaoCartBuilder builder, IHaoCatalog catalog)
    {
        _cart = cart;
        _builder = builder;
        _catalog = catalog;
    }

    [HttpGet("")]
    public IActionResult Index() => View(_builder.Build(_cart.Read()));

    [HttpPost("them")]
    [ValidateAntiForgeryToken]
    public IActionResult Add(int productId, int quantity = 1, string? returnUrl = null)
    {
        var product = _catalog.Get(productId);
        if (product is null || !product.IsOnSale || quantity is < 1 or > 99)
            TempData["HaoError"] = "Sản phẩm hoặc số lượng không hợp lệ.";
        else if (!_cart.Add(productId, quantity))
            TempData["HaoError"] = "Không thể thêm sản phẩm (tối đa 30 dòng, 99 sản phẩm/dòng).";
        else
            TempData["HaoMessage"] = "Đã thêm sản phẩm vào giỏ hàng.";
        return !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? LocalRedirect(returnUrl) : RedirectToAction(nameof(Index));
    }

    [HttpPost("cap-nhat")]
    [ValidateAntiForgeryToken]
    public IActionResult Update(int productId, int quantity)
    {
        if (quantity > 0 && _catalog.Get(productId) is null)
            TempData["HaoError"] = "Không tìm thấy sản phẩm.";
        else if (!_cart.SetQuantity(productId, quantity))
            TempData["HaoError"] = "Số lượng phải trong khoảng 0–99.";
        else
            TempData["HaoMessage"] = "Đã cập nhật giỏ hàng.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("xoa")]
    [ValidateAntiForgeryToken]
    public IActionResult Remove(int productId)
    {
        _cart.SetQuantity(productId, 0);
        return RedirectToAction(nameof(Index));
    }
}
