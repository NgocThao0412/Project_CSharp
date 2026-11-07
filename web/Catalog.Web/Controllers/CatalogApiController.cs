using Catalog.Web.BUS;
using Catalog.Web.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace Catalog.Web.Controllers;

[ApiController]
[Route("api/catalog")]
public sealed class CatalogApiController(SanPhamBUS sanPham, ILogger<CatalogApiController> log) : ControllerBase
{
    private static object DinhDang(SanPhamDTO sp) => new
    {
        id = sp.Ma,
        name = sp.Ten,
        category = sp.DanhMuc,
        brand = sp.Hang,
        price = sp.Gia,
        availableQuantity = sp.Ton,
        imageUrl = sp.HinhAnh,
        description = sp.MoTa
    };

    [HttpGet("featured")]
    public async Task<IActionResult> NoiBat([FromQuery] int count = 6, CancellationToken ct = default)
    {
        try
        {
            var ketQua = await sanPham.LayNoiBatAsync(Math.Clamp(count, 1, 20), ct);
            return Ok(ketQua.Select(DinhDang));
        }
        catch (SqlException ex)
        {
            log.LogError(ex, "Không đọc được sản phẩm nổi bật từ SQL Server.");
            return Problem("Chưa thể kết nối SQL Server.", statusCode: 503);
        }
    }

    [HttpGet("categories")]
    public async Task<IActionResult> DanhMuc(CancellationToken ct)
    {
        try { return Ok(await sanPham.LayDanhMucAsync(ct)); }
        catch (SqlException ex)
        {
            log.LogError(ex, "Không đọc được danh mục sản phẩm.");
            return Problem("Chưa thể kết nối SQL Server.", statusCode: 503);
        }
    }

    [HttpGet("products")]
    public async Task<IActionResult> DanhSach([FromQuery] string? q, [FromQuery] string? category,
        [FromQuery] int page = 1, [FromQuery] string? sort = null, CancellationToken ct = default)
    {
        try
        {
            var ketQua = await sanPham.TimAsync(q, category, page, sort, ct);
            return Ok(new
            {
                items = ketQua.SanPham.Select(DinhDang),
                total = ketQua.Tong,
                page = ketQua.Trang,
                pageSize = ketQua.SoLuongMoiTrang
            });
        }
        catch (SqlException ex)
        {
            log.LogError(ex, "Không đọc được danh sách sản phẩm.");
            return Problem("Chưa thể kết nối SQL Server.", statusCode: 503);
        }
    }

    [HttpGet("products/{id:int}")]
    public async Task<IActionResult> ChiTiet(int id, CancellationToken ct)
    {
        try
        {
            var sp = await sanPham.LayTheoMaAsync(id, ct);
            return sp is null ? NotFound(new { message = "Không tìm thấy sản phẩm." }) : Ok(DinhDang(sp));
        }
        catch (SqlException ex)
        {
            log.LogError(ex, "Không đọc được chi tiết sản phẩm {Ma}.", id);
            return Problem("Chưa thể kết nối SQL Server.", statusCode: 503);
        }
    }
}
