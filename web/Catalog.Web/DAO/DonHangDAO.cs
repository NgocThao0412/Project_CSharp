using System.Data;
using Catalog.Web.config;
using Catalog.Web.DTO;
using Microsoft.Data.SqlClient;

namespace Catalog.Web.DAO;

public sealed class DonHangDAO(KetNoi ketNoi)
{
    public async Task<IReadOnlyList<DonHangTomTatDTO>> DanhSachAsync(int maKhach, CancellationToken ct = default)
    {
        await using var db = await ketNoi.MoAsync(ct);
        await using var lenh = new SqlCommand(@"
            SELECT MPX, TG, TT, TIEN
            FROM dbo.PHIEUXUAT
            WHERE MKH = @khach
            ORDER BY TG DESC, MPX DESC", db);
        lenh.Parameters.Add("@khach", SqlDbType.Int).Value = maKhach;
        var danhSach = new List<DonHangTomTatDTO>();
        await using var r = await lenh.ExecuteReaderAsync(ct);
        while (await r.ReadAsync(ct))
            danhSach.Add(new DonHangTomTatDTO(r.GetInt32(0),
                r.IsDBNull(1) ? null : r.GetDateTime(1),
                r.GetInt32(2), Convert.ToDecimal(r.GetValue(3))));
        return danhSach;
    }

    public async Task<DonHangDTO?> ChiTietAsync(int maKhach, int maDon, CancellationToken ct = default)
    {
        if (maKhach <= 0 || maDon <= 0) return null;
        await using var db = await ketNoi.MoAsync(ct);
        await using var lenh = new SqlCommand(@"
            SELECT px.MPX, px.TG, px.TT, px.TIEN,
                   kh.HOTEN, kh.SDT, COALESCE(kh.EMAIL, N''),
                   COALESCE(dw.HOTENNHAN, kh.HOTEN),
                   COALESCE(dw.SDTNHAN, kh.SDT),
                   COALESCE(dw.DIACHINHAN, CONCAT_WS(N', ',
                       NULLIF(kh.DIACHI, N''), NULLIF(kh.PHUONGXA, N''), NULLIF(kh.TINHTHANHPHO, N''))),
                   COALESCE(dw.GHICHU, N''),
                   CASE WHEN dw.MPX IS NULL THEN N'Không có thông tin'
                        WHEN dw.PHUONGTHUCTT = 'COD' THEN N'Thanh toán khi nhận hàng (COD)'
                        ELSE CONVERT(NVARCHAR(50), dw.PHUONGTHUCTT) END,
                   CASE WHEN dw.MPX IS NULL THEN 0 ELSE 1 END
            FROM dbo.PHIEUXUAT AS px
            INNER JOIN dbo.KHACHHANG AS kh ON kh.MKH = px.MKH
            LEFT JOIN dbo.DONHANGWEB AS dw ON dw.MPX = px.MPX
            WHERE px.MPX = @don AND px.MKH = @khach", db);
        lenh.Parameters.Add("@don", SqlDbType.Int).Value = maDon;
        lenh.Parameters.Add("@khach", SqlDbType.Int).Value = maKhach;

        DateTime? ngayDat;
        int trangThai;
        decimal tongTien;
        string hoTenDat, sdtDat, emailDat, hoTenNhan, sdtNhan, diaChiNhan, ghiChu, thanhToan;
        bool laDonWeb;
        await using (var r = await lenh.ExecuteReaderAsync(ct))
        {
            if (!await r.ReadAsync(ct)) return null;
            ngayDat = r.IsDBNull(1) ? null : r.GetDateTime(1);
            trangThai = r.GetInt32(2);
            tongTien = Convert.ToDecimal(r.GetValue(3));
            hoTenDat = r.GetString(4);
            sdtDat = r.GetString(5);
            emailDat = r.GetString(6);
            hoTenNhan = r.GetString(7);
            sdtNhan = r.GetString(8);
            diaChiNhan = r.GetString(9);
            ghiChu = r.GetString(10);
            thanhToan = r.GetString(11);
            laDonWeb = r.GetInt32(12) != 0;
        }

        await using var chiTiet = new SqlCommand(@"
            SELECT ct.MSP, sp.TEN, ct.SL, ct.TIENXUAT
            FROM dbo.CTPHIEUXUAT AS ct
            INNER JOIN dbo.SANPHAM AS sp ON sp.MSP = ct.MSP
            WHERE ct.MPX = @don
            ORDER BY ct.MSP", db);
        chiTiet.Parameters.Add("@don", SqlDbType.Int).Value = maDon;
        var sanPham = new List<DongDonHangDTO>();
        await using var ds = await chiTiet.ExecuteReaderAsync(ct);
        while (await ds.ReadAsync(ct))
            sanPham.Add(new DongDonHangDTO(ds.GetInt32(0), ds.GetString(1), ds.GetInt32(2),
                Convert.ToDecimal(ds.GetValue(3))));

        return new DonHangDTO(maDon, ngayDat, trangThai, tongTien,
            hoTenDat, sdtDat, emailDat, hoTenNhan, sdtNhan, diaChiNhan,
            ghiChu, thanhToan, laDonWeb, sanPham);
    }
}
