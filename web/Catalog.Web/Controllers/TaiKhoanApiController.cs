using System.Security.Claims;
using Catalog.Web.DAO;
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
public sealed class TaiKhoanApiController(TaiKhoanBUS taiKhoan, TaiKhoanDAO dao, ILogger<TaiKhoanApiController> log) : ControllerBase
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
            return Problem("Cần đồng bộ dbo.TAIKHOAN (MaTK, MKH) và các cột địa chỉ của dbo.KHACHHANG.", statusCode: 503);
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
            return Problem("Cần đồng bộ dbo.TAIKHOAN (MaTK, MKH) và các cột địa chỉ của dbo.KHACHHANG.", statusCode: 503);
        }
        catch (SqlException ex)
        {
            log.LogError(ex, "Không đăng nhập được tài khoản khách hàng.");
            return Problem("Chưa thể đăng nhập. Vui lòng thử lại sau.", statusCode: 503);
        }
    }

    [Authorize]
    [HttpGet("thong-tin")]
    public async Task<IActionResult> ThongTin(CancellationToken ct)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var maKhach)) return Unauthorized();
        try
        {
            var thongTin = await dao.LayThongTinAsync(maKhach, ct);
            return thongTin is null ? NotFound(new { message = "Tài khoản không còn hoạt động." }) : Ok(thongTin);
        }
        catch (SqlException ex)
        {
            log.LogError(ex, "Không đọc được thông tin khách hàng.");
            return Problem("Không thể tải thông tin khách hàng.", statusCode: 503);
        }
    }

    [Authorize]
    [HttpPost("cap-nhat")]
    public async Task<IActionResult> CapNhat([FromBody] CapNhatThongTinDTO thongTin, CancellationToken ct)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var maKhach)) return Unauthorized();
        try
        {
            if (!await dao.CapNhatAsync(maKhach, thongTin, ct)) return NotFound(new { message = "Tài khoản không còn hoạt động." });
            return Ok(await dao.LayThongTinAsync(maKhach, ct));
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
            return Conflict(new { message = "Email hoặc số điện thoại đã được sử dụng." });
        }
        catch (SqlException ex)
        {
            log.LogError(ex, "Không cập nhật được thông tin khách hàng.");
            return Problem("Không thể cập nhật thông tin khách hàng.", statusCode: 503);
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
