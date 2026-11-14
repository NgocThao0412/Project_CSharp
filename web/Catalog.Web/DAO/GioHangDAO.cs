using System.Data;
using Catalog.Web.config;
using Catalog.Web.DTO;
using Microsoft.Data.SqlClient;

namespace Catalog.Web.DAO;

public sealed class GioHangDAO(KetNoi ketNoi)
{
    public async Task<IReadOnlyList<MucGioHangDTO>> LayAsync(int maKhach, CancellationToken ct = default)
    {
        await using var db = await ketNoi.MoAsync(ct);
        await using var lenh = new SqlCommand(@"
            SELECT MSP, SL
            FROM dbo.GIOHANGWEB
            WHERE MKH = @khach
            ORDER BY MSP", db);
        lenh.Parameters.Add("@khach", SqlDbType.Int).Value = maKhach;

        var danhSach = new List<MucGioHangDTO>();
        await using var doc = await lenh.ExecuteReaderAsync(ct);
        while (await doc.ReadAsync(ct))
            danhSach.Add(new MucGioHangDTO(doc.GetInt32(0), doc.GetInt32(1)));
        return danhSach;
    }

    public async Task<string?> ThemAsync(int maKhach, int maSanPham, int soLuong, int tonKho, CancellationToken ct = default)
    {
        await using var db = await ketNoi.MoAsync(ct);
        await using var giaoDich = (SqlTransaction)await db.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        try
        {
            await using var tim = new SqlCommand(@"
                SELECT SL FROM dbo.GIOHANGWEB WITH (UPDLOCK, HOLDLOCK)
                WHERE MKH = @khach AND MSP = @sanPham", db, giaoDich);
            tim.Parameters.Add("@khach", SqlDbType.Int).Value = maKhach;
            tim.Parameters.Add("@sanPham", SqlDbType.Int).Value = maSanPham;
            var giaTri = await tim.ExecuteScalarAsync(ct);
            var hienCo = giaTri is null or DBNull ? 0 : Convert.ToInt32(giaTri);

            if (hienCo + soLuong > 99 || hienCo + soLuong > tonKho)
                return "Số lượng vượt quá tồn kho.";

            if (hienCo == 0)
            {
                await using var dem = new SqlCommand(@"
                    SELECT COUNT(*) FROM dbo.GIOHANGWEB WITH (UPDLOCK, HOLDLOCK)
                    WHERE MKH = @khach", db, giaoDich);
                dem.Parameters.Add("@khach", SqlDbType.Int).Value = maKhach;
                if (Convert.ToInt32(await dem.ExecuteScalarAsync(ct)) >= 30)
                    return "Giỏ hàng tối đa 30 sản phẩm khác nhau.";
            }

            await using var luu = new SqlCommand(hienCo == 0
                ? "INSERT INTO dbo.GIOHANGWEB (MKH, MSP, SL) VALUES (@khach, @sanPham, @soLuong)"
                : "UPDATE dbo.GIOHANGWEB SET SL = @soLuong WHERE MKH = @khach AND MSP = @sanPham", db, giaoDich);
            luu.Parameters.Add("@khach", SqlDbType.Int).Value = maKhach;
            luu.Parameters.Add("@sanPham", SqlDbType.Int).Value = maSanPham;
            luu.Parameters.Add("@soLuong", SqlDbType.Int).Value = hienCo + soLuong;
            await luu.ExecuteNonQueryAsync(ct);
            await giaoDich.CommitAsync(ct);
            return null;
        }
        catch
        {
            try { await giaoDich.RollbackAsync(CancellationToken.None); } catch { }
            throw;
        }
    }

    public async Task<string?> CapNhatAsync(int maKhach, int maSanPham, int soLuong, CancellationToken ct = default)
    {
        await using var db = await ketNoi.MoAsync(ct);
        await using var lenh = new SqlCommand(soLuong == 0
            ? "DELETE FROM dbo.GIOHANGWEB WHERE MKH = @khach AND MSP = @sanPham"
            : "UPDATE dbo.GIOHANGWEB SET SL = @soLuong WHERE MKH = @khach AND MSP = @sanPham", db);
        lenh.Parameters.Add("@khach", SqlDbType.Int).Value = maKhach;
        lenh.Parameters.Add("@sanPham", SqlDbType.Int).Value = maSanPham;
        lenh.Parameters.Add("@soLuong", SqlDbType.Int).Value = soLuong;
        var soDong = await lenh.ExecuteNonQueryAsync(ct);
        return soLuong > 0 && soDong == 0 ? "Không tìm thấy sản phẩm trong giỏ." : null;
    }
}
