using System.Security.Claims;
using Catalog.Web.DAO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Catalog.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/don-hang")]
public sealed class DonHangApiController(DonHangDAO donHang, ILogger<DonHangApiController> log) : ControllerBase
{
    [HttpGet("cua-toi")]
    public async Task<IActionResult> CuaToi(CancellationToken ct)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var maKhach))
            return Unauthorized(new { message = "Vui lòng đăng nhập để xem đơn hàng." });
        try
        {
            return Ok(await donHang.DanhSachAsync(maKhach, ct));
        }
        catch (SqlException ex)
        {
            log.LogError(ex, "Không tải được danh sách đơn hàng.");
            return Problem("Không thể tải danh sách đơn hàng. Vui lòng thử lại.", statusCode: 503);
        }
    }

    [HttpGet("{maDon:int}")]
    public async Task<IActionResult> ChiTiet(int maDon, CancellationToken ct)
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var maKhach))
            return Unauthorized(new { message = "Vui lòng đăng nhập để xem đơn hàng." });
        if (maDon <= 0) return BadRequest(new { message = "Mã đơn hàng không hợp lệ." });
        try
        {
            var don = await donHang.ChiTietAsync(maKhach, maDon, ct);
            if (don is null) return NotFound(new { message = "Không tìm thấy đơn hàng trong tài khoản của bạn." });
            return Ok(don);
        }
        catch (SqlException ex)
        {
            log.LogError(ex, "Không tải được chi tiết đơn hàng.");
            return Problem("Không thể tải chi tiết đơn hàng. Vui lòng thử lại.", statusCode: 503);
        }
    }
}
