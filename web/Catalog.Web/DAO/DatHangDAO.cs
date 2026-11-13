using System.Data;
using Catalog.Web.config;
using Catalog.Web.DTO;
using Microsoft.Data.SqlClient;

namespace Catalog.Web.DAO;

// Ghi mot don hang vao CSDL ma Desktop dang dung: KHACHHANG, PHIEUXUAT, CTPHIEUXUAT.
// TT=2: cho nhan vien duyet. Desktop se tru SL kho KHI duyet, Web khong tru hai lan.
public sealed class DatHangDAO(KetNoi ketNoi, IConfiguration cauHinh)
{
    private static SqlCommand Lenh(string sql, SqlConnection db, SqlTransaction tran)
        => new(sql, db, tran);

    public async Task<KetQuaDatHangDTO> TaoDonAsync(YeuCauDatHangDTO yeuCau,
        IReadOnlyList<MucGioHangDTO> gio, int maKhach, CancellationToken ct = default)
    {
        await using var db = await ketNoi.MoAsync(ct);
        await using var tran = (SqlTransaction)await db.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        try
        {
            // MaYeuCau duoc luu unique, dung cho truong hop double-click/retry.
            KetQuaDatHangDTO? donDaCo = null;
            await using (var trung = Lenh(@"
                SELECT px.MPX, px.TIEN, px.TG
                FROM dbo.DONHANGWEB dw WITH (UPDLOCK, HOLDLOCK)
                JOIN dbo.PHIEUXUAT px ON px.MPX = dw.MPX
                WHERE dw.MAYEUCAU = @yeuCau AND px.MKH = @khach", db, tran))
            {
                trung.Parameters.Add("@yeuCau", SqlDbType.UniqueIdentifier).Value = yeuCau.MaYeuCau;
                trung.Parameters.Add("@khach", SqlDbType.Int).Value = maKhach;
                await using (var r = await trung.ExecuteReaderAsync(ct))
                {
                    if (await r.ReadAsync(ct))
                        donDaCo = new KetQuaDatHangDTO(r.GetInt32(0), r.GetInt32(1), r.GetDateTime(2),
                            "Chờ duyệt", "Thanh toán khi nhận hàng (COD)");
                }
            }
            if (donDaCo is not null)
            {
                await tran.CommitAsync(ct);
                return donDaCo;
            }

            await using (var kiemTraKhach = Lenh(@"
                SELECT COUNT(1)
                FROM dbo.KHACHHANG AS kh
                WHERE kh.MKH = @khach AND kh.TT = 1
                  AND EXISTS (
                      SELECT 1 FROM dbo.TAIKHOAN AS tk
                      INNER JOIN dbo.NHOMQUYEN AS nq ON nq.MNQ = tk.MNQ
                      WHERE tk.MKH = kh.MKH AND tk.MNV IS NULL
                        AND tk.TT = 1 AND nq.TEN = N'Khách hàng' AND nq.TT = 1
                  )", db, tran))
            {
                kiemTraKhach.Parameters.Add("@khach", SqlDbType.Int).Value = maKhach;
                if (Convert.ToInt32(await kiemTraKhach.ExecuteScalarAsync(ct)) == 0)
                    throw new LoiDatHangException("Tài khoản khách hàng không còn hoạt động.");
            }
            var danhSach = new List<(int Ma, int Luong, int Gia)>();
            long tongTien = 0;
            foreach (var item in gio.OrderBy(x => x.MaSanPham))
            {
                // Khoa san pham trong transaction; khong nhan gia tu JavaScript.
                int gia, ton;
                await using (var sp = Lenh(@"
                    SELECT TIENX, SL FROM dbo.SANPHAM WITH (UPDLOCK, HOLDLOCK)
                    WHERE MSP = @ma AND TT = 1", db, tran))
                {
                    sp.Parameters.Add("@ma", SqlDbType.Int).Value = item.MaSanPham;
                    await using var r = await sp.ExecuteReaderAsync(ct);
                    if (!await r.ReadAsync(ct))
                        throw new LoiDatHangException($"Sản phẩm mã {item.MaSanPham} đã ngừng bán.");
                    gia = r.GetInt32(0);
                    ton = r.GetInt32(1);
                }

                // Tranh nhan qua nhieu don Web cho duyet. Desktop van kiem tra ton lan nua khi duyet.
                long choDuyet;
                await using (var cho = Lenh(@"
                    SELECT COALESCE(SUM(CAST(ct.SL AS BIGINT)), 0)
                    FROM dbo.CTPHIEUXUAT ct WITH (HOLDLOCK)
                    INNER JOIN dbo.PHIEUXUAT px WITH (HOLDLOCK) ON px.MPX = ct.MPX
                    WHERE ct.MSP = @ma AND px.TT = 2", db, tran))
                {
                    cho.Parameters.Add("@ma", SqlDbType.Int).Value = item.MaSanPham;
                    choDuyet = Convert.ToInt64(await cho.ExecuteScalarAsync(ct));
                }
                if (gia <= 0) throw new LoiDatHangException($"Sản phẩm mã {item.MaSanPham} chưa có giá bán.");
                if (item.SoLuong > (long)ton - choDuyet)
                    throw new LoiDatHangException($"Sản phẩm mã {item.MaSanPham} không đủ số lượng để đặt. Vui lòng cập nhật giỏ hàng.");

                tongTien = checked(tongTien + (long)gia * item.SoLuong);
                if (tongTien > int.MaxValue)
                    throw new LoiDatHangException("Giá trị đơn hàng vượt giới hạn hệ thống.");
                danhSach.Add((item.MaSanPham, item.SoLuong, gia));
            }

            if ((decimal)tongTien != yeuCau.TongTienXacNhan)
                throw new LoiDatHangException("Giá bán đã thay đổi. Vui lòng quay về giỏ hàng, tải lại và xác nhận tổng tiền mới.");

            // MNV la khoa ngoai bat buoc tren man hinh Desktop (doc bang GetInt32).
            // Dung nhan vien tiep nhan don Web co the cau hinh, mac dinh = 1 trong bo SQL mau.
            var maNhanVien = cauHinh.GetValue("WebOrders:StaffId", 1);
            await using var taoPhieu = Lenh(@"
                INSERT INTO dbo.PHIEUXUAT (MNV, MKH, TIEN, TG, TT)
                OUTPUT INSERTED.MPX
                VALUES (@nhanVien, @khach, @tong, SYSDATETIME(), 2)", db, tran);
            taoPhieu.Parameters.Add("@nhanVien", SqlDbType.Int).Value = maNhanVien;
            taoPhieu.Parameters.Add("@khach", SqlDbType.Int).Value = maKhach;
            taoPhieu.Parameters.Add("@tong", SqlDbType.Int).Value = (int)tongTien;
            var maDon = Convert.ToInt32(await taoPhieu.ExecuteScalarAsync(ct));

            foreach (var item in danhSach)
            {
                await using var ctPhieu = Lenh(@"
                    INSERT INTO dbo.CTPHIEUXUAT (MPX, MSP, SL, TIENXUAT)
                    VALUES (@don, @sp, @luong, @gia)", db, tran);
                ctPhieu.Parameters.Add("@don", SqlDbType.Int).Value = maDon;
                ctPhieu.Parameters.Add("@sp", SqlDbType.Int).Value = item.Ma;
                ctPhieu.Parameters.Add("@luong", SqlDbType.Int).Value = item.Luong;
                ctPhieu.Parameters.Add("@gia", SqlDbType.Int).Value = item.Gia;
                await ctPhieu.ExecuteNonQueryAsync(ct);
            }

            await using (var giao = Lenh(@"
                INSERT INTO dbo.DONHANGWEB
                (MPX, MAYEUCAU, HOTENNHAN, SDTNHAN, DIACHINHAN, GHICHU, PHUONGTHUCTT)
                VALUES (@don, @yeuCau, @ten, @sdt, @diaChi, @ghiChu, 'COD')", db, tran))
            {
                giao.Parameters.Add("@don", SqlDbType.Int).Value = maDon;
                giao.Parameters.Add("@yeuCau", SqlDbType.UniqueIdentifier).Value = yeuCau.MaYeuCau;
                giao.Parameters.Add("@ten", SqlDbType.NVarChar, 100).Value = yeuCau.HoTen;
                giao.Parameters.Add("@sdt", SqlDbType.NVarChar, 11).Value = yeuCau.SoDienThoai;
                giao.Parameters.Add("@diaChi", SqlDbType.NVarChar, 255).Value = yeuCau.DiaChi;
                giao.Parameters.Add("@ghiChu", SqlDbType.NVarChar, 500).Value = (object?)yeuCau.GhiChu ?? DBNull.Value;
                await giao.ExecuteNonQueryAsync(ct);
            }

            await tran.CommitAsync(ct);
            return new KetQuaDatHangDTO(maDon, tongTien, DateTime.Now,
                "Chờ duyệt", "Thanh toán khi nhận hàng (COD)");
        }
        catch
        {
            try { await tran.RollbackAsync(CancellationToken.None); } catch { /* Giữ lỗi gốc. */ }
            throw;
        }
    }

}
