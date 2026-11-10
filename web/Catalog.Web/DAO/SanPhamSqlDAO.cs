using System.Data;
using Catalog.Web.config;
using Catalog.Web.DTO;
using Microsoft.Data.SqlClient;

namespace Catalog.Web.DAO;

// Giữ truy vấn dbo.vw_Catalog của nhóm; chỉ thống nhất tên lớp và cấu hình kết nối.
public sealed class SanPhamSqlDAO(KetNoi ketNoi) : ISanPhamDAO
{
    private const string Cot = "ProductId, ProductName, CategoryName, BrandName, Price, AvailableQuantity, ImageUrl, Description";

    private static SanPhamDTO Doc(SqlDataReader r) => new(
        r.GetInt32(0), r.GetString(1), r.GetString(2), r.GetString(3),
        r.GetDecimal(4), r.GetInt32(5),
        r.IsDBNull(6) ? null : r.GetString(6),
        r.IsDBNull(7) ? "" : r.GetString(7));

    public async Task<SanPhamDTO?> LayTheoMaAsync(int ma, CancellationToken ct = default)
    {
        if (ma <= 0) return null;
        await using var db = await ketNoi.MoAsync(ct);
        await using var cmd = new SqlCommand($"SELECT {Cot} FROM dbo.vw_Catalog WHERE ProductId = @ma", db);
        cmd.Parameters.Add("@ma", SqlDbType.Int).Value = ma;
        await using var r = await cmd.ExecuteReaderAsync(ct);
        return await r.ReadAsync(ct) ? Doc(r) : null;
    }

    public async Task<IReadOnlyList<SanPhamDTO>> LayNoiBatAsync(int soLuong, CancellationToken ct = default)
    {
        await using var db = await ketNoi.MoAsync(ct);
        await using var cmd = new SqlCommand(
            $"SELECT TOP (@soLuong) {Cot} FROM dbo.vw_Catalog " +
            "ORDER BY CASE WHEN AvailableQuantity > 0 THEN 0 ELSE 1 END, NEWID()", db);
        cmd.Parameters.Add("@soLuong", SqlDbType.Int).Value = Math.Clamp(soLuong, 1, 20);
        await using var r = await cmd.ExecuteReaderAsync(ct);
        var result = new List<SanPhamDTO>();
        while (await r.ReadAsync(ct)) result.Add(Doc(r));
        return result;
    }

    public async Task<IReadOnlyList<string>> LayDanhMucAsync(CancellationToken ct = default)
    {
        await using var db = await ketNoi.MoAsync(ct);
        await using var cmd = new SqlCommand(
            "SELECT DISTINCT CategoryName FROM dbo.vw_Catalog WHERE CategoryName <> '' ORDER BY CategoryName", db);
        await using var r = await cmd.ExecuteReaderAsync(ct);
        var result = new List<string>();
        while (await r.ReadAsync(ct)) result.Add(r.GetString(0));
        return result;
    }

    private static string EscapeLike(string s) => s.Replace("!", "!!").Replace("%", "!%").Replace("_", "!_").Replace("[", "![");

    public async Task<DanhSachSanPhamDTO> TimAsync(TimKiemSanPhamDTO dieuKien, CancellationToken ct = default)
    {
        var tuKhoa = dieuKien.TuKhoaAnToan;
        var danhMuc = dieuKien.DanhMucAnToan;
        var sapXep = dieuKien.SapXepAnToan;
        var thuTu = sapXep switch
        {
            "gia-tang" => "Price ASC, ProductId DESC",
            "gia-giam" => "Price DESC, ProductId DESC",
            "ten" => "ProductName ASC, ProductId DESC",
            _ => "ProductId DESC"
        };
        const string dieuKienSql = "WHERE (@tuKhoa = '' OR ProductName LIKE @mau ESCAPE '!' " +
            "OR BrandName LIKE @mau ESCAPE '!' OR CategoryName LIKE @mau ESCAPE '!') " +
            "AND (@danhMuc = '' OR CategoryName = @danhMuc)";

        await using var db = await ketNoi.MoAsync(ct);
        SqlCommand TaoLenh(string sql)
        {
            var cmd = new SqlCommand(sql, db);
            cmd.Parameters.Add("@tuKhoa", SqlDbType.NVarChar, 100).Value = tuKhoa;
            cmd.Parameters.Add("@mau", SqlDbType.NVarChar, 220).Value = "%" + EscapeLike(tuKhoa) + "%";
            cmd.Parameters.Add("@danhMuc", SqlDbType.NVarChar, 100).Value = danhMuc;
            return cmd;
        }

        await using var dem = TaoLenh($"SELECT COUNT(*) FROM dbo.vw_Catalog {dieuKienSql}");
        var tong = Convert.ToInt32(await dem.ExecuteScalarAsync(ct));
        var soTrang = Math.Max(1, (tong + TimKiemSanPhamDTO.SoSanPhamMoiTrang - 1) / TimKiemSanPhamDTO.SoSanPhamMoiTrang);
        var trang = Math.Min(dieuKien.TrangAnToan, soTrang);
        await using var lay = TaoLenh($"SELECT {Cot} FROM dbo.vw_Catalog {dieuKienSql} ORDER BY {thuTu} OFFSET @boQua ROWS FETCH NEXT @lay ROWS ONLY");
        lay.Parameters.Add("@boQua", SqlDbType.Int).Value = (trang - 1) * TimKiemSanPhamDTO.SoSanPhamMoiTrang;
        lay.Parameters.Add("@lay", SqlDbType.Int).Value = TimKiemSanPhamDTO.SoSanPhamMoiTrang;
        await using var r = await lay.ExecuteReaderAsync(ct);
        var result = new List<SanPhamDTO>();
        while (await r.ReadAsync(ct)) result.Add(Doc(r));
        return new DanhSachSanPhamDTO(result, tong, trang, TimKiemSanPhamDTO.SoSanPhamMoiTrang, tuKhoa, danhMuc, sapXep);
    }
}
