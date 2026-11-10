using Catalog.Web.DAO;
using Catalog.Web.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Data.SqlClient;

namespace Catalog.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/don-hang")]
public sealed class DonHangApiController(DonHangDAO donHang, ILogger<DonHangApiController> log) : ControllerBase
{
    [HttpPost("tra-cuu")]
    public async Task<IActionResult> TraCuu([FromBody] TraCuuDonHangDTO thongTin, CancellationToken ct)
    {
        try
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var maKhach))
                return Unauthorized();
            var ketQua = await donHang.TraCuuAsync(maKhach, thongTin.MaDon, thongTin.SoDienThoai, ct);
            return ketQua is null ? NotFound(new { message = "Không tìm thấy đơn hàng phù hợp." }) : Ok(ketQua);
        }
        catch (SqlException ex)
        {
            log.LogError(ex, "Không tra cứu được đơn hàng.");
            return Problem("Chưa thể kết nối SQL Server.", statusCode: 503);
        }
    }
}
