using System.Data;
using Catalog.Web.config;
using Catalog.Web.DTO;
using Microsoft.Data.SqlClient;

namespace Catalog.Web.DAO;

public sealed class TaiKhoanDAO(KetNoi ketNoi)
{
    public async Task<KhachHangDangNhapDTO?> TimAsync(string tenDangNhap, CancellationToken ct)
    {
        await using var db = await ketNoi.MoAsync(ct);
        await using var lenh = new SqlCommand(@"
            SELECT MKH, TENDANGNHAP, HOTEN, MATKHAU
            FROM dbo.KHACHHANG
            WHERE TENDANGNHAP = @ten AND TT = 1 AND MATKHAU IS NOT NULL", db);
        lenh.Parameters.Add("@ten", SqlDbType.NVarChar, 50).Value = tenDangNhap;
        await using var r = await lenh.ExecuteReaderAsync(ct);
        if (!await r.ReadAsync(ct)) return null;
        return new KhachHangDangNhapDTO(r.GetInt32(0), r.GetString(1), r.GetString(2), r.GetString(3));
    }

    public async Task TaoAsync(DangKyDTO thongTin, string matKhauBam, CancellationToken ct)
    {
        await using var db = await ketNoi.MoAsync(ct);
        await using var tran = (SqlTransaction)await db.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        try
        {
            await using (var kiemTra = new SqlCommand(@"
                SELECT CASE WHEN EXISTS (SELECT 1 FROM dbo.KHACHHANG WHERE TENDANGNHAP = @ten)
                                 OR EXISTS (SELECT 1 FROM dbo.TAIKHOAN WHERE TDN = @ten)
                            THEN 1 ELSE 0 END", db, tran))
            {
                kiemTra.Parameters.Add("@ten", SqlDbType.NVarChar, 50).Value = thongTin.TenDangNhap;
                if (Convert.ToInt32(await kiemTra.ExecuteScalarAsync(ct)) != 0)
                    throw new LoiDangKyException("Tên đăng nhập đã được sử dụng.");
            }

            await using (var kiemTra = new SqlCommand(@"
                SELECT CASE WHEN EXISTS (SELECT 1 FROM dbo.KHACHHANG WHERE SDT = @sdt OR EMAIL = @email)
                            THEN 1 ELSE 0 END", db, tran))
            {
                kiemTra.Parameters.Add("@sdt", SqlDbType.NVarChar, 11).Value = thongTin.SoDienThoai;
                kiemTra.Parameters.Add("@email", SqlDbType.NVarChar, 50).Value = thongTin.Email;
                if (Convert.ToInt32(await kiemTra.ExecuteScalarAsync(ct)) != 0)
                    throw new LoiDangKyException("Email hoặc số điện thoại đã có trong hệ thống.");
            }

            await using var lenh = new SqlCommand(@"
                INSERT INTO dbo.KHACHHANG
                    (HOTEN, NGAYTHAMGIA, DIACHI, SDT, EMAIL, TT,
                     TENDANGNHAP, MATKHAU, TINHTHANHPHO, PHUONGXA)
                VALUES
                    (@hoTen, CONVERT(date, SYSDATETIME()), @diaChi, @sdt, @email, 1,
                     @ten, @matKhau, @tinh, @phuong)", db, tran);
            lenh.Parameters.Add("@hoTen", SqlDbType.NVarChar, 255).Value = thongTin.HoTen;
            lenh.Parameters.Add("@diaChi", SqlDbType.NVarChar, 255).Value = thongTin.DiaChi;
            lenh.Parameters.Add("@sdt", SqlDbType.NVarChar, 11).Value = thongTin.SoDienThoai;
            lenh.Parameters.Add("@email", SqlDbType.NVarChar, 50).Value = thongTin.Email;
            lenh.Parameters.Add("@ten", SqlDbType.NVarChar, 50).Value = thongTin.TenDangNhap;
            lenh.Parameters.Add("@matKhau", SqlDbType.NVarChar, 255).Value = matKhauBam;
            lenh.Parameters.Add("@tinh", SqlDbType.NVarChar, 100).Value = thongTin.TinhThanhPho;
            lenh.Parameters.Add("@phuong", SqlDbType.NVarChar, 100).Value = thongTin.PhuongXa;
            await lenh.ExecuteNonQueryAsync(ct);
            await tran.CommitAsync(ct);
        }
        catch
        {
            try { await tran.RollbackAsync(CancellationToken.None); } catch { }
            throw;
        }
    }
}
