using System.Security.Claims;
using Catalog.Web.BUS;
using Catalog.Web.DTO;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Catalog.Web.Controllers;

[ApiController]
[Route("api/tai-khoan")]
public sealed class TaiKhoanApiController(TaiKhoanBUS taiKhoan, ILogger<TaiKhoanApiController> log) : ControllerBase
{
    [HttpGet("trang-thai")]
    public IActionResult TrangThai()
    {
        if (User.Identity?.IsAuthenticated != true)
            return Ok(new { daDangNhap = false });
        return Ok(new
        {
            daDangNhap = true,
            tenDangNhap = User.Identity.Name,
            hoTen = User.FindFirstValue("HoTen")
        });
    }

    [HttpPost("dang-ky")]
    public async Task<IActionResult> DangKy([FromBody] DangKyDTO thongTin, CancellationToken ct)
    {
        try
        {
            await taiKhoan.DangKyAsync(thongTin, ct);
            return Ok(new { message = "Đăng ký thành công. Vui lòng đăng nhập." });
        }
        catch (LoiDangKyException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
            return Conflict(new { message = "Tên đăng nhập, email hoặc số điện thoại đã được sử dụng." });
        }
        catch (SqlException ex) when (ex.Number is 207 or 208)
        {
            log.LogError(ex, "Cơ sở dữ liệu chưa có các trường khách hàng Web.");
            return Problem("Cần chạy database/04_KHACH_HANG_WEB.sql trên database quanlykho.", statusCode: 503);
        }
        catch (SqlException ex)
        {
            log.LogError(ex, "Không đăng ký được tài khoản khách hàng.");
            return Problem("Chưa thể đăng ký tài khoản. Vui lòng thử lại sau.", statusCode: 503);
        }
    }

    [HttpPost("dang-nhap")]
    public async Task<IActionResult> DangNhap([FromBody] DangNhapDTO thongTin, CancellationToken ct)
    {
        try
        {
            var khach = await taiKhoan.DangNhapAsync(thongTin, ct);
            if (khach is null)
                return Unauthorized(new { message = "Tên đăng nhập hoặc mật khẩu không đúng." });

            HttpContext.Session.Clear();
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, khach.MaKhach.ToString()),
                new Claim(ClaimTypes.Name, khach.TenDangNhap),
                new Claim("HoTen", khach.HoTen)
            };
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));
            return Ok(new { daDangNhap = true, tenDangNhap = khach.TenDangNhap, hoTen = khach.HoTen });
        }
        catch (SqlException ex) when (ex.Number is 207 or 208)
        {
            log.LogError(ex, "Cơ sở dữ liệu chưa có các trường khách hàng Web.");
            return Problem("Cần chạy database/04_KHACH_HANG_WEB.sql trên database quanlykho.", statusCode: 503);
        }
        catch (SqlException ex)
        {
            log.LogError(ex, "Không đăng nhập được tài khoản khách hàng.");
            return Problem("Chưa thể đăng nhập. Vui lòng thử lại sau.", statusCode: 503);
        }
    }

    [Authorize]
    [HttpPost("dang-xuat")]
    public async Task<IActionResult> DangXuat()
    {
        HttpContext.Session.Clear();
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Ok(new { daDangNhap = false });
    }
}
