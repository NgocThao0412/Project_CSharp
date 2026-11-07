using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Catalog.Web.Features.BanHang;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Catalog.Web.Controllers;

[Route("don-hang")]
public sealed class DonHangController : Controller
{
    private readonly GioHang _cart;
    private readonly TaoGioHang _builder;
    private readonly IDonHang _orders;
    private readonly ILogger<DonHangController> _logger;

    public DonHangController(GioHang cart, TaoGioHang builder, IDonHang orders, ILogger<DonHangController> logger)
    {
        _cart = cart;
        _builder = builder;
        _orders = orders;
        _logger = logger;
    }

    [HttpGet("dat-hang")]
    public IActionResult Checkout()
    {
        var vm = _builder.Build(_cart.Read());
        if (vm.IsEmpty || vm.HasUnavailable)
        {
            if (vm.HasUnavailable) TempData["ThongBaoLoi"] = "Có sản phẩm ngừng bán trong giỏ. Hãy xóa trước khi đặt hàng.";
            return RedirectToAction("Index", "GioHang");
        }
        return View(new DatHangViewModel { Cart = vm });
    }

    [HttpPost("dat-hang")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(DatHangViewModel page)
    {
        var entries = _cart.Read();
        var cart = _builder.Build(entries);
        if (!ModelState.IsValid || cart.IsEmpty || cart.HasUnavailable || cart.Lines.Count != entries.Count)
        {
            if (cart.IsEmpty || cart.HasUnavailable || cart.Lines.Count != entries.Count)
                ModelState.AddModelError("", "Giỏ hàng đã thay đổi. Vui lòng kiểm tra lại.");
            return View(new DatHangViewModel { Input = page.Input, Cart = cart });
        }

        // Authentication is optional; an unauthenticated customer can still place/lookup an order.
        var ticket = await HttpContext.AuthenticateAsync(XacThucKhachHang.Scheme);
        var accountId = ticket.Succeeded ? ticket.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value : null;
        var result = _orders.Place(page.Input, entries, accountId);
        if (!result.Success)
        {
            _logger.LogWarning("{Code} web checkout rejected", result.ErrorCode);
            ModelState.AddModelError("", result.Message ?? "Không thể đặt hàng.");
            return View(new DatHangViewModel { Input = page.Input, Cart = cart });
        }
        _cart.Clear();
        TempData["MaXacThucDonHang"] = result.VerificationCode;
        TempData["MaDonHangMoi"] = result.OrderCode;
        return RedirectToAction(nameof(Success), new { code = result.OrderCode });
    }

    [HttpGet("thanh-cong/{code}")]
    public IActionResult Success(string code)
    {
        // Avoid displaying a forged success page for an arbitrary order code.
        if (TempData["MaDonHangMoi"] is not string createdCode ||
            !string.Equals(createdCode, code, StringComparison.OrdinalIgnoreCase))
            return RedirectToAction(nameof(Lookup));
        ViewBag.OrderCode = code;
        ViewBag.VerificationCode = TempData["MaXacThucDonHang"] as string;
        return View();
    }

    [HttpGet("tra-cuu")]
    public IActionResult Lookup() => View(new TraCuuDonHangViewModel());

    [HttpPost("tra-cuu")]
    [ValidateAntiForgeryToken]
    public IActionResult Lookup(ThongTinTraCuuDonHang input)
    {
        if (!ModelState.IsValid) return View(new TraCuuDonHangViewModel { Input = input });
        var order = _orders.FindWithVerification(input.OrderCode, input.VerificationCode);
        if (order is null)
        {
            ModelState.AddModelError("", "Không tìm thấy đơn hàng hoặc thông tin xác thực không đúng.");
            return View(new TraCuuDonHangViewModel { Input = input });
        }
        return View(new TraCuuDonHangViewModel { Input = input, Order = order });
    }

    [HttpPost("huy")]
    [ValidateAntiForgeryToken]
    public IActionResult Cancel(ThongTinTraCuuDonHang input)
    {
        if (!ModelState.IsValid)
        {
            TempData["ThongBaoLoi"] = "Vui lòng cung cấp mã đơn và mã xác thực hợp lệ.";
            return RedirectToAction(nameof(Lookup));
        }
        var result = _orders.CancelWithVerification(input.OrderCode, input.VerificationCode);
        TempData[result.Success ? "ThongBaoThanhCong" : "ThongBaoLoi"] = result.Message;
        return RedirectToAction(nameof(Lookup));
    }

    [HttpGet("cua-toi")]
    [Authorize(AuthenticationSchemes = XacThucKhachHang.Scheme)]
    public IActionResult MyOrders()
    {
        var customerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(customerId)) return Challenge(XacThucKhachHang.Scheme);
        return View(_orders.Mine(customerId));
    }
}
