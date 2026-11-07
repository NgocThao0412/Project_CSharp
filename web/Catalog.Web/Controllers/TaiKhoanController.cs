using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Catalog.Web.Features.BanHang;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Web.Controllers;

[Route("tai-khoan")]
public sealed class TaiKhoanController : Controller
{
    private readonly ITaiKhoanKhachHang _accounts;
    public TaiKhoanController(ITaiKhoanKhachHang accounts) => _accounts = accounts;

    [HttpGet("dang-ky")]
    public IActionResult Register() => View(new ThongTinDangKy());

    [HttpPost("dang-ky")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(ThongTinDangKy input)
    {
        if (!ModelState.IsValid) return View(input);
        if (!_accounts.TryRegister(input, out var account) || account is null)
        {
            ModelState.AddModelError("", "Không thể tạo tài khoản. Email có thể đã được sử dụng.");
            return View(input);
        }
        await SignInCustomer(account);
        TempData["ThongBaoThanhCong"] = "Đăng ký tài khoản thành công (chế độ DEMO).";
        return RedirectToAction("MyOrders", "DonHang");
    }

    [HttpGet("dang-nhap")]
    public IActionResult Login() => View(new ThongTinDangNhap());

    [HttpPost("dang-nhap")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(ThongTinDangNhap input, string? returnUrl = null)
    {
        if (!ModelState.IsValid) return View(input);
        var account = _accounts.Login(input.Email, input.Password);
        if (account is null)
        {
            ModelState.AddModelError("", "Email hoặc mật khẩu không đúng, hoặc tài khoản tạm khóa.");
            return View(input);
        }
        await SignInCustomer(account);
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return LocalRedirect(returnUrl);
        return RedirectToAction("MyOrders", "DonHang");
    }

    [HttpPost("dang-xuat")]
    [ValidateAntiForgeryToken]
    [Authorize(AuthenticationSchemes = XacThucKhachHang.Scheme)]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(XacThucKhachHang.Scheme);
        return Redirect("/");
    }

    private Task SignInCustomer(TaiKhoanKhachHang account)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, account.Id),
            new(ClaimTypes.Name, account.Name),
            new(ClaimTypes.Email, account.Email)
        };
        var identity = new ClaimsIdentity(claims, XacThucKhachHang.Scheme);
        return HttpContext.SignInAsync(XacThucKhachHang.Scheme, new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = false });
    }
}
