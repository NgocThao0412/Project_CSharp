
using System.Data;
using Catalog.Web.config;
using Catalog.Web.DTO;
using Microsoft.Data.SqlClient;

namespace Catalog.Web.DAO;

public sealed class TaiKhoanDAO(KetNoi ketNoi)
{
    // 1. Tim tai khoan khach hang theo ten dang nhap
    public async Task<KhachHangDangNhapDTO?> TimAsync(
        string tenDangNhap, CancellationToken ct)
    {
        await using var db = await ketNoi.MoAsync(ct);

        await using var lenh = new SqlCommand(@"
            SELECT kh.MKH, tk.TDN, kh.HOTEN, tk.MK
            FROM dbo.TAIKHOAN AS tk
            INNER JOIN dbo.KHACHHANG AS kh
                ON tk.MKH = kh.MKH
            INNER JOIN dbo.NHOMQUYEN AS nq
                ON tk.MNQ = nq.MNQ
            WHERE tk.TDN = @ten
              AND tk.MNV IS NULL
              AND tk.TT = 1
              AND kh.TT = 1
              AND nq.TEN = N'Khách hàng'
              AND nq.TT = 1", db);

        lenh.Parameters.Add(
            "@ten", SqlDbType.NVarChar, 255).Value = tenDangNhap;

        await using var r = await lenh.ExecuteReaderAsync(ct);

        if (!await r.ReadAsync(ct))
            return null;

        return new KhachHangDangNhapDTO(
            r.GetInt32(0),
            r.GetString(1),
            r.GetString(2),
            r.GetString(3));
    }

    // 2. Dang ky: tao KHACHHANG va TAIKHOAN
    public async Task TaoAsync(
        DangKyDTO thongTin,
        string matKhauBam,
        CancellationToken ct)
    {
        await using var db = await ketNoi.MoAsync(ct);

        await using var tran = (SqlTransaction)
            await db.BeginTransactionAsync(
                IsolationLevel.Serializable, ct);

        try
        {
            // Kiem tra trung ten dang nhap
            await using (var kiemTra = new SqlCommand(@"
                SELECT CASE WHEN EXISTS (
                    SELECT 1
                    FROM dbo.TAIKHOAN WITH (UPDLOCK, HOLDLOCK)
                    WHERE TDN = @ten
                ) THEN 1 ELSE 0 END", db, tran))
            {
                kiemTra.Parameters.Add(
                    "@ten", SqlDbType.NVarChar, 255)
                    .Value = thongTin.TenDangNhap;

                if (Convert.ToInt32(
                    await kiemTra.ExecuteScalarAsync(ct)) != 0)
                {
                    throw new LoiDangKyException(
                        "Tên đăng nhập đã được sử dụng.");
                }
            }

            // Kiem tra trung email hoac SDT
            await using (var kiemTra = new SqlCommand(@"
                SELECT CASE WHEN EXISTS (
                    SELECT 1
                    FROM dbo.KHACHHANG WITH (UPDLOCK, HOLDLOCK)
                    WHERE SDT = @sdt OR EMAIL = @email
                ) THEN 1 ELSE 0 END", db, tran))
            {
                kiemTra.Parameters.Add(
                    "@sdt", SqlDbType.NVarChar, 11)
                    .Value = thongTin.SoDienThoai;

                kiemTra.Parameters.Add(
                    "@email", SqlDbType.NVarChar, 50)
                    .Value = thongTin.Email;

                if (Convert.ToInt32(
                    await kiemTra.ExecuteScalarAsync(ct)) != 0)
                {
                    throw new LoiDangKyException(
                        "Email hoặc số điện thoại đã có trong hệ thống.");
                }
            }

            // Lay ma nhom quyen Khach hang
            int nhomQuyen;

            await using (var timQuyen = new SqlCommand(@"
                SELECT MNQ
                FROM dbo.NHOMQUYEN
                WHERE TEN = N'Khách hàng'
                  AND TT = 1", db, tran))
            {
                var ma = await timQuyen.ExecuteScalarAsync(ct);

                if (ma is null || ma is DBNull)
                {
                    throw new LoiDangKyException(
                        "Chưa cấu hình nhóm quyền Khách hàng.");
                }

                nhomQuyen = Convert.ToInt32(ma);
            }

            // Them ho so khach hang
            int maKhach;

            await using (var themKhach = new SqlCommand(@"
                INSERT INTO dbo.KHACHHANG
                    (HOTEN, NGAYTHAMGIA, DIACHI,
                     SDT, EMAIL, TINHTHANHPHO,
                     PHUONGXA, TT)
                OUTPUT INSERTED.MKH
                VALUES
                    (@hoTen, CONVERT(date, SYSDATETIME()),
                     @diaChi, @sdt, @email,
                     @tinh, @phuong, 1)", db, tran))
            {
                themKhach.Parameters.Add(
                    "@hoTen", SqlDbType.NVarChar, 255)
                    .Value = thongTin.HoTen;

                themKhach.Parameters.Add(
                    "@diaChi", SqlDbType.NVarChar, 255)
                    .Value = thongTin.DiaChi;

                themKhach.Parameters.Add(
                    "@sdt", SqlDbType.NVarChar, 11)
                    .Value = thongTin.SoDienThoai;

                themKhach.Parameters.Add(
                    "@email", SqlDbType.NVarChar, 50)
                    .Value = thongTin.Email;

                themKhach.Parameters.Add(
                    "@tinh", SqlDbType.NVarChar, 100)
                    .Value = thongTin.TinhThanhPho;

                themKhach.Parameters.Add(
                    "@phuong", SqlDbType.NVarChar, 100)
                    .Value = thongTin.PhuongXa;

                maKhach = Convert.ToInt32(
                    await themKhach.ExecuteScalarAsync(ct));
            }

            // Tao tai khoan dung chung voi Desktop
            await using (var themTaiKhoan = new SqlCommand(@"
                INSERT INTO dbo.TAIKHOAN
                    (MNV, MKH, TDN, MK, MNQ, TT)
                VALUES
                    (NULL, @maKhach, @ten,
                     @matKhau, @nhomQuyen, 1)", db, tran))
            {
                themTaiKhoan.Parameters.Add(
                    "@maKhach", SqlDbType.Int)
                    .Value = maKhach;

                themTaiKhoan.Parameters.Add(
                    "@ten", SqlDbType.NVarChar, 255)
                    .Value = thongTin.TenDangNhap;

                themTaiKhoan.Parameters.Add(
                    "@matKhau", SqlDbType.NVarChar, 255)
                    .Value = matKhauBam;

                themTaiKhoan.Parameters.Add(
                    "@nhomQuyen", SqlDbType.Int)
                    .Value = nhomQuyen;

                await themTaiKhoan.ExecuteNonQueryAsync(ct);
            }

            await tran.CommitAsync(ct);
        }
        catch
        {
            try
            {
                await tran.RollbackAsync(CancellationToken.None);
            }
            catch
            {
                // Giu nguyen loi goc
            }

            throw;
        }
    }

    // 3. Lay thong tin tai khoan khach hang
    public async Task<ThongTinTaiKhoanDTO?> LayThongTinAsync(
        int maKhach, CancellationToken ct)
    {
        await using var db = await ketNoi.MoAsync(ct);

        await using var lenh = new SqlCommand(@"
            SELECT kh.MKH, tk.TDN, kh.HOTEN,
                   kh.EMAIL, kh.SDT, kh.DIACHI,
                   kh.TINHTHANHPHO, kh.PHUONGXA
            FROM dbo.KHACHHANG AS kh
            INNER JOIN dbo.TAIKHOAN AS tk
                ON tk.MKH = kh.MKH
            INNER JOIN dbo.NHOMQUYEN AS nq
                ON tk.MNQ = nq.MNQ
            WHERE kh.MKH = @ma
              AND kh.TT = 1
              AND tk.TT = 1
              AND tk.MNV IS NULL
              AND nq.TEN = N'Khách hàng'
              AND nq.TT = 1", db);

        lenh.Parameters.Add(
            "@ma", SqlDbType.Int).Value = maKhach;

        await using var r = await lenh.ExecuteReaderAsync(ct);

        if (!await r.ReadAsync(ct))
            return null;

        string Doc(int i) =>
            r.IsDBNull(i) ? "" : r.GetString(i);

        return new ThongTinTaiKhoanDTO(
            r.GetInt32(0),
            Doc(1), Doc(2), Doc(3), Doc(4),
            Doc(5), Doc(6), Doc(7));
    }

    // 4. Cap nhat thong tin ca nhan
    public async Task<bool> CapNhatAsync(
        int maKhach,
        CapNhatThongTinDTO thongTin,
        CancellationToken ct)
    {
        await using var db = await ketNoi.MoAsync(ct);

        await using var lenh = new SqlCommand(@"
            UPDATE kh
            SET HOTEN = @ten,
                EMAIL = @email,
                SDT = @sdt,
                DIACHI = @diaChi,
                TINHTHANHPHO = @tinh,
                PHUONGXA = @phuong
            FROM dbo.KHACHHANG AS kh
            WHERE kh.MKH = @ma
              AND kh.TT = 1
              AND EXISTS (
                  SELECT 1
                  FROM dbo.TAIKHOAN AS tk
                  INNER JOIN dbo.NHOMQUYEN AS nq
                      ON tk.MNQ = nq.MNQ
                  WHERE tk.MKH = kh.MKH
                    AND tk.MNV IS NULL
                    AND tk.TT = 1
                    AND nq.TEN = N'Khách hàng'
                    AND nq.TT = 1
              )", db);

        lenh.Parameters.Add(
            "@ma", SqlDbType.Int).Value = maKhach;

        lenh.Parameters.Add(
            "@ten", SqlDbType.NVarChar, 255)
            .Value = thongTin.HoTen.Trim();

        lenh.Parameters.Add(
            "@email", SqlDbType.NVarChar, 50)
            .Value = thongTin.Email.Trim();

        lenh.Parameters.Add(
            "@sdt", SqlDbType.NVarChar, 11)
            .Value = thongTin.SoDienThoai.Trim();

        lenh.Parameters.Add(
            "@diaChi", SqlDbType.NVarChar, 255)
            .Value = thongTin.DiaChi.Trim();

        lenh.Parameters.Add(
            "@tinh", SqlDbType.NVarChar, 100)
            .Value = thongTin.TinhThanhPho.Trim();

        lenh.Parameters.Add(
            "@phuong", SqlDbType.NVarChar, 100)
            .Value = thongTin.PhuongXa.Trim();

        return await lenh.ExecuteNonQueryAsync(ct) == 1;
    }
}
