using Catalog.Web.DAO;
using Catalog.Web.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Catalog.Web.Controllers;

[ApiController]
[Route("api/don-hang")]
public sealed class DonHangApiController(DonHangDAO donHang, ILogger<DonHangApiController> log) : ControllerBase
{
    [HttpPost("tra-cuu")]
    public async Task<IActionResult> TraCuu([FromBody] TraCuuDonHangDTO thongTin, CancellationToken ct)
    {
        try
        {
            var ketQua = await donHang.TraCuuAsync(thongTin.MaDon, thongTin.SoDienThoai, ct);
            return ketQua is null ? NotFound(new { message = "Không tìm thấy đơn hàng phù hợp." }) : Ok(ketQua);
        }
        catch (SqlException ex)
        {
            log.LogError(ex, "Không tra cứu được đơn hàng.");
            return Problem("Chưa thể kết nối SQL Server.", statusCode: 503);
        }
    }
}
