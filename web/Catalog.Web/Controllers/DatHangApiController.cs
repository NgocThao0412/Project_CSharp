using Catalog.Web.BUS;
using Catalog.Web.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.Data.SqlClient;

namespace Catalog.Web.Controllers;

[ApiController]
[Authorize]
[Route("api/don-hang")]
public sealed class DatHangApiController(DatHangBUS datHang, ILogger<DatHangApiController> log) : ControllerBase
{
    [HttpPost("dat")]
    public async Task<IActionResult> Dat([FromBody] YeuCauDatHangDTO thongTin, CancellationToken ct)
    {
        try
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var maKhach))
                return Unauthorized();
            var don = await datHang.DatAsync(thongTin, maKhach, ct);
            return Ok(don);
        }
        catch (LoiDatHangException ex)
        {
            return Conflict(new { message = ex.Message });
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
            log.LogWarning(ex, "Vi phạm ràng buộc duy nhất khi đặt hàng.");
            return Conflict(new { message = "Số điện thoại/email đã được sử dụng hoặc yêu cầu đã được gửi. Vui lòng kiểm tra lại." });
        }
        catch (SqlException ex) when (ex.Number is 208 or 207)
        {
            log.LogError(ex, "Chưa cài bảng DONHANGWEB hoặc cấu trúc SQL không đúng.");
            return Problem("Thiếu bảng DONHANGWEB. Hãy chạy database/02_WEB_DAT_HANG.sql một lần trên database quanlykho.", statusCode: 503);
        }
        catch (SqlException ex)
        {
            log.LogError(ex, "Không thể lưu đơn Web vào SQL Server.");
            return Problem("Không thể lưu đơn hàng. Vui lòng thử lại hoặc liên hệ cửa hàng.", statusCode: 503);
        }
        catch (OverflowException ex)
        {
            log.LogWarning(ex, "Đơn hàng vượt giới hạn giá trị.");
            return BadRequest(new { message = "Giá trị đơn hàng vượt giới hạn hệ thống." });
        }
    }
}
