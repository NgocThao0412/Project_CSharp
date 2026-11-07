using Catalog.Web.BUS;
using Catalog.Web.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Catalog.Web.Controllers;

[ApiController]
[Route("api/gio-hang")]
public sealed class GioHangApiController(GioHangBUS gioHang, ILogger<GioHangApiController> log) : ControllerBase
{
    public sealed record ThaoTacGioHang(int ProductId, int Quantity = 1);

    private static object DinhDang(TrangGioHangDTO gio) => new
    {
        items = gio.SanPham.Select(x => new
        {
            id = x.SanPham.Ma,
            name = x.SanPham.Ten,
            imageUrl = x.SanPham.HinhAnh,
            price = x.SanPham.Gia,
            stock = x.SanPham.Ton,
            quantity = x.SoLuong,
            lineTotal = x.ThanhTien,
            available = x.DuHang
        }),
        total = gio.TongTien,
        available = gio.DuHang
    };

    [HttpGet("")]
    public async Task<IActionResult> Lay(CancellationToken ct)
    {
        try { return Ok(DinhDang(await gioHang.LayTrangAsync(ct))); }
        catch (SqlException ex)
        {
            log.LogError(ex, "Không tải được giỏ hàng.");
            return Problem("Chưa thể kết nối SQL Server.", statusCode: 503);
        }
    }

    [HttpPost("them")]
    public async Task<IActionResult> Them([FromBody] ThaoTacGioHang thongTin, CancellationToken ct)
        => await XuLyAsync(() => gioHang.ThemAsync(thongTin.ProductId, thongTin.Quantity, ct), ct);

    [HttpPost("cap-nhat")]
    public async Task<IActionResult> CapNhat([FromBody] ThaoTacGioHang thongTin, CancellationToken ct)
        => await XuLyAsync(() => gioHang.CapNhatAsync(thongTin.ProductId, thongTin.Quantity, ct), ct);

    [HttpPost("xoa")]
    public async Task<IActionResult> Xoa([FromBody] ThaoTacGioHang thongTin, CancellationToken ct)
        => await XuLyAsync(() => gioHang.CapNhatAsync(thongTin.ProductId, 0, ct), ct);

    private async Task<IActionResult> XuLyAsync(Func<Task<string?>> thaoTac, CancellationToken ct)
    {
        try
        {
            var loi = await thaoTac();
            if (loi is not null) return BadRequest(new { message = loi });
            return Ok(DinhDang(await gioHang.LayTrangAsync(ct)));
        }
        catch (SqlException ex)
        {
            log.LogError(ex, "Không thao tác được giỏ hàng.");
            return Problem("Chưa thể kết nối SQL Server.", statusCode: 503);
        }
    }
}
