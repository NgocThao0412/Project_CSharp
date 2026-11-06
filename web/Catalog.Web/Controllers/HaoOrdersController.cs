using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Catalog.Web.Features.Hao;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Catalog.Web.Controllers;

[Route("don-hang")]
public sealed class HaoOrdersController : Controller
{
    private readonly HaoCart _cart;
    private readonly HaoCartBuilder _builder;
    private readonly IHaoOrders _orders;
    private readonly ILogger<HaoOrdersController> _logger;

    public HaoOrdersController(HaoCart cart, HaoCartBuilder builder, IHaoOrders orders, ILogger<HaoOrdersController> logger)
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
            if (vm.HasUnavailable) TempData["HaoError"] = "Có sản phẩm ngừng bán trong giỏ. Hãy xóa trước khi đặt hàng.";
            return RedirectToAction("Index", "HaoCart");
        }
        return View(new HaoCheckoutPageVm { Cart = vm });
    }

    [HttpPost("dat-hang")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Checkout(HaoCheckoutPageVm page)
    {
        var entries = _cart.Read();
        var cart = _builder.Build(entries);
        if (!ModelState.IsValid || cart.IsEmpty || cart.HasUnavailable || cart.Lines.Count != entries.Count)
        {
            if (cart.IsEmpty || cart.HasUnavailable || cart.Lines.Count != entries.Count)
                ModelState.AddModelError("", "Giỏ hàng đã thay đổi. Vui lòng kiểm tra lại.");
            return View(new HaoCheckoutPageVm { Input = page.Input, Cart = cart });
        }

        // Authentication is optional; an unauthenticated customer can still place/lookup an order.
        var ticket = await HttpContext.AuthenticateAsync(HaoAuth.Scheme);
        var accountId = ticket.Succeeded ? ticket.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value : null;
        var result = _orders.Place(page.Input, entries, accountId);
        if (!result.Success)
        {
            _logger.LogWarning("{Code} web checkout rejected", result.ErrorCode);
            ModelState.AddModelError("", result.Message ?? "Không thể đặt hàng.");
            return View(new HaoCheckoutPageVm { Input = page.Input, Cart = cart });
        }
        _cart.Clear();
        TempData["HaoVerificationCode"] = result.VerificationCode;
        TempData["HaoCreatedOrderCode"] = result.OrderCode;
        return RedirectToAction(nameof(Success), new { code = result.OrderCode });
    }

    [HttpGet("thanh-cong/{code}")]
    public IActionResult Success(string code)
    {
        // Avoid displaying a forged success page for an arbitrary order code.
        if (TempData["HaoCreatedOrderCode"] is not string createdCode ||
            !string.Equals(createdCode, code, StringComparison.OrdinalIgnoreCase))
            return RedirectToAction(nameof(Lookup));
        ViewBag.OrderCode = code;
        ViewBag.VerificationCode = TempData["HaoVerificationCode"] as string;
        return View();
    }

    [HttpGet("tra-cuu")]
    public IActionResult Lookup() => View(new HaoLookupPageVm());

    [HttpPost("tra-cuu")]
    [ValidateAntiForgeryToken]
    public IActionResult Lookup(HaoLookupInput input)
    {
        if (!ModelState.IsValid) return View(new HaoLookupPageVm { Input = input });
        var order = _orders.FindWithVerification(input.OrderCode, input.VerificationCode);
        if (order is null)
        {
            ModelState.AddModelError("", "Không tìm thấy đơn hàng hoặc thông tin xác thực không đúng.");
            return View(new HaoLookupPageVm { Input = input });
        }
        return View(new HaoLookupPageVm { Input = input, Order = order });
    }

    [HttpPost("huy")]
    [ValidateAntiForgeryToken]
    public IActionResult Cancel(HaoLookupInput input)
    {
        if (!ModelState.IsValid)
        {
            TempData["HaoError"] = "Vui lòng cung cấp mã đơn và mã xác thực hợp lệ.";
            return RedirectToAction(nameof(Lookup));
        }
        var result = _orders.CancelWithVerification(input.OrderCode, input.VerificationCode);
        TempData[result.Success ? "HaoMessage" : "HaoError"] = result.Message;
        return RedirectToAction(nameof(Lookup));
    }

    [HttpGet("cua-toi")]
    [Authorize(AuthenticationSchemes = HaoAuth.Scheme)]
    public IActionResult MyOrders()
    {
        var customerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(customerId)) return Challenge(HaoAuth.Scheme);
        return View(_orders.Mine(customerId));
    }
}
