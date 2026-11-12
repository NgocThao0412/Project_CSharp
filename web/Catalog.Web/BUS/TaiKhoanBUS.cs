
using Catalog.Web.DAO;
using Catalog.Web.DTO;

namespace Catalog.Web.BUS;

public sealed class TaiKhoanBUS(TaiKhoanDAO taiKhoan)
{
    public Task DangKyAsync(DangKyDTO thongTin, CancellationToken ct)
    {
        thongTin.TenDangNhap = thongTin.TenDangNhap.Trim();
        thongTin.HoTen = thongTin.HoTen.Trim();
        thongTin.Email = thongTin.Email.Trim();
        thongTin.SoDienThoai = thongTin.SoDienThoai.Trim();
        thongTin.DiaChi = thongTin.DiaChi.Trim();
        thongTin.TinhThanhPho = thongTin.TinhThanhPho.Trim();
        thongTin.PhuongXa = thongTin.PhuongXa.Trim();

        if (string.IsNullOrWhiteSpace(thongTin.TenDangNhap)
    || string.IsNullOrWhiteSpace(thongTin.MatKhau)
    || string.IsNullOrWhiteSpace(thongTin.HoTen)
            || string.IsNullOrWhiteSpace(thongTin.DiaChi)
            || string.IsNullOrWhiteSpace(thongTin.TinhThanhPho)
            || string.IsNullOrWhiteSpace(thongTin.PhuongXa)
            || thongTin.HoTen.Length < 2
            || thongTin.DiaChi.Length < 2)
        {
            throw new LoiDangKyException(
                "Vui lòng nhập đầy đủ thông tin khách hàng.");
        }

        // Ma hoa mat khau bang BCrypt, giong Desktop
        string matKhauBam = BCrypt.Net.BCrypt.HashPassword(
            thongTin.MatKhau, workFactor: 12);

        return taiKhoan.TaoAsync(thongTin, matKhauBam, ct);
    }

    public async Task<KhachHangDangNhapDTO?> DangNhapAsync(
        DangNhapDTO thongTin,
        CancellationToken ct)
    {
        var khach = await taiKhoan.TimAsync(
            thongTin.TenDangNhap.Trim(), ct);

        return khach is not null
            && DungMatKhau(thongTin.MatKhau, khach.MatKhauBam)
            ? khach
            : null;
    }

    private static bool DungMatKhau(
        string matKhau,
        string matKhauBam)
    {
        if (string.IsNullOrEmpty(matKhauBam)
            || !matKhauBam.StartsWith(
                "$2", StringComparison.Ordinal))
            return false;

        try
        {
            return BCrypt.Net.BCrypt.Verify(
                matKhau, matKhauBam);
        }
        catch (Exception)
        {
            return false;
        }
    }
}
