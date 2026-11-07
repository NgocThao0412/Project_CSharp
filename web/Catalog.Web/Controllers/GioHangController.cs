using Catalog.Web.Features.BanHang;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Web.Controllers;

[Route("gio-hang")]
public sealed class GioHangController : Controller
{
    private readonly GioHang _cart;
    private readonly TaoGioHang _builder;
    private readonly IDanhMucSanPham _catalog;
    public GioHangController(GioHang cart, TaoGioHang builder, IDanhMucSanPham catalog)
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
            TempData["ThongBaoLoi"] = "Sản phẩm hoặc số lượng không hợp lệ.";
        else if (!_cart.Add(productId, quantity))
            TempData["ThongBaoLoi"] = "Không thể thêm sản phẩm (tối đa 30 dòng, 99 sản phẩm/dòng).";
        else
            TempData["ThongBaoThanhCong"] = "Đã thêm sản phẩm vào giỏ hàng.";
        return !string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl)
            ? LocalRedirect(returnUrl) : RedirectToAction(nameof(Index));
    }

    [HttpPost("cap-nhat")]
    [ValidateAntiForgeryToken]
    public IActionResult Update(int productId, int quantity)
    {
        if (quantity > 0 && _catalog.Get(productId) is null)
            TempData["ThongBaoLoi"] = "Không tìm thấy sản phẩm.";
        else if (!_cart.SetQuantity(productId, quantity))
            TempData["ThongBaoLoi"] = "Số lượng phải trong khoảng 0–99.";
        else
            TempData["ThongBaoThanhCong"] = "Đã cập nhật giỏ hàng.";
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
