using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Catalog.Web.Features.Hao;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Web.Controllers;

[Route("tai-khoan")]
public sealed class HaoAccountController : Controller
{
    private readonly IHaoAccounts _accounts;
    public HaoAccountController(IHaoAccounts accounts) => _accounts = accounts;

    [HttpGet("dang-ky")]
    public IActionResult Register() => View(new HaoRegisterInput());

    [HttpPost("dang-ky")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(HaoRegisterInput input)
    {
        if (!ModelState.IsValid) return View(input);
        if (!_accounts.TryRegister(input, out var account) || account is null)
        {
            ModelState.AddModelError("", "Không thể tạo tài khoản. Email có thể đã được sử dụng.");
            return View(input);
        }
        await SignInCustomer(account);
        TempData["HaoMessage"] = "Đăng ký tài khoản thành công (chế độ DEMO).";
        return RedirectToAction("MyOrders", "HaoOrders");
    }

    [HttpGet("dang-nhap")]
    public IActionResult Login() => View(new HaoLoginInput());

    [HttpPost("dang-nhap")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(HaoLoginInput input, string? returnUrl = null)
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
        return RedirectToAction("MyOrders", "HaoOrders");
    }

    [HttpPost("dang-xuat")]
    [ValidateAntiForgeryToken]
    [Authorize(AuthenticationSchemes = HaoAuth.Scheme)]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(HaoAuth.Scheme);
        return Redirect("/");
    }

    private Task SignInCustomer(HaoAccountRecord account)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, account.Id),
            new(ClaimTypes.Name, account.Name),
            new(ClaimTypes.Email, account.Email)
        };
        var identity = new ClaimsIdentity(claims, HaoAuth.Scheme);
        return HttpContext.SignInAsync(HaoAuth.Scheme, new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = false });
    }
}
