using System.Data;
using Catalog.Web.config;
using Catalog.Web.DTO;
using Microsoft.Data.SqlClient;

namespace Catalog.Web.DAO;

public sealed class DonHangDAO(KetNoi ketNoi)
{
    public async Task<DonHangDTO?> TraCuuAsync(int maDon, string soDienThoai, CancellationToken ct = default)
    {
        if (maDon <= 0 || string.IsNullOrWhiteSpace(soDienThoai)) return null;
        await using var db = await ketNoi.MoAsync(ct);
        await using var lenh = new SqlCommand(@"
            SELECT px.MPX, kh.HOTEN, px.TG, px.TT, px.TIEN
            FROM dbo.PHIEUXUAT px
            INNER JOIN dbo.KHACHHANG kh ON px.MKH = kh.MKH
            WHERE px.MPX = @ma AND kh.SDT = @sdt", db);
        lenh.Parameters.Add("@ma", SqlDbType.Int).Value = maDon;
        lenh.Parameters.Add("@sdt", SqlDbType.NVarChar, 11).Value = soDienThoai.Trim();

        int ma, trangThai;
        string hoTen;
        DateTime? ngayDat;
        decimal tongTien;
        await using (var reader = await lenh.ExecuteReaderAsync(ct))
        {
            if (!await reader.ReadAsync(ct)) return null;
            ma = reader.GetInt32(0);
            hoTen = reader.GetString(1);
            ngayDat = reader.IsDBNull(2) ? null : reader.GetDateTime(2);
            trangThai = reader.GetInt32(3);
            tongTien = Convert.ToDecimal(reader.GetValue(4));
        }

        await using var chiTiet = new SqlCommand(@"
            SELECT ct.MSP, sp.TEN, ct.SL, ct.TIENXUAT
            FROM dbo.CTPHIEUXUAT ct
            INNER JOIN dbo.SANPHAM sp ON sp.MSP = ct.MSP
            WHERE ct.MPX = @ma
            ORDER BY ct.MSP", db);
        chiTiet.Parameters.Add("@ma", SqlDbType.Int).Value = maDon;
        var danhSach = new List<DongDonHangDTO>();
        await using var r = await chiTiet.ExecuteReaderAsync(ct);
        while (await r.ReadAsync(ct))
            danhSach.Add(new DongDonHangDTO(r.GetInt32(0), r.GetString(1), r.GetInt32(2), Convert.ToDecimal(r.GetValue(3))));
        return new DonHangDTO(ma, hoTen, ngayDat, trangThai, tongTien, danhSach);
    }
}
