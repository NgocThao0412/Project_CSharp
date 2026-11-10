using System.Globalization;
using System.Security.Cryptography;
using Catalog.Web.DAO;
using Catalog.Web.DTO;

namespace Catalog.Web.BUS;

public sealed class TaiKhoanBUS(TaiKhoanDAO taiKhoan)
{
    private const int SoVongBam = 220_000;

    public Task DangKyAsync(DangKyDTO thongTin, CancellationToken ct)
    {
        thongTin.TenDangNhap = thongTin.TenDangNhap.Trim();
        thongTin.HoTen = thongTin.HoTen.Trim();
        thongTin.Email = thongTin.Email.Trim();
        thongTin.SoDienThoai = thongTin.SoDienThoai.Trim();
        thongTin.DiaChi = thongTin.DiaChi.Trim();
        thongTin.TinhThanhPho = thongTin.TinhThanhPho.Trim();
        thongTin.PhuongXa = thongTin.PhuongXa.Trim();

        if (string.IsNullOrWhiteSpace(thongTin.HoTen) || string.IsNullOrWhiteSpace(thongTin.DiaChi)
            || string.IsNullOrWhiteSpace(thongTin.TinhThanhPho) || string.IsNullOrWhiteSpace(thongTin.PhuongXa)
            || thongTin.HoTen.Length < 2 || thongTin.DiaChi.Length < 2)
            throw new LoiDangKyException("Vui lòng nhập đầy đủ thông tin khách hàng.");

        var muoi = RandomNumberGenerator.GetBytes(16);
        var bam = Rfc2898DeriveBytes.Pbkdf2(thongTin.MatKhau, muoi, SoVongBam,
            HashAlgorithmName.SHA256, 32);
        var matKhauBam = $"PBKDF2${SoVongBam}${Convert.ToBase64String(muoi)}${Convert.ToBase64String(bam)}";
        return taiKhoan.TaoAsync(thongTin, matKhauBam, ct);
    }

    public async Task<KhachHangDangNhapDTO?> DangNhapAsync(DangNhapDTO thongTin, CancellationToken ct)
    {
        var khach = await taiKhoan.TimAsync(thongTin.TenDangNhap.Trim(), ct);
        return khach is not null && DungMatKhau(thongTin.MatKhau, khach.MatKhauBam) ? khach : null;
    }

    private static bool DungMatKhau(string matKhau, string matKhauBam)
    {
        var phan = matKhauBam.Split('$');
        if (phan.Length != 4 || phan[0] != "PBKDF2"
            || !int.TryParse(phan[1], NumberStyles.None, CultureInfo.InvariantCulture, out var soVong)
            || soVong is < 100_000 or > 1_000_000) return false;
        try
        {
            var muoi = Convert.FromBase64String(phan[2]);
            var bam = Convert.FromBase64String(phan[3]);
            if (muoi.Length != 16 || bam.Length != 32) return false;
            var kiemTra = Rfc2898DeriveBytes.Pbkdf2(matKhau, muoi, soVong, HashAlgorithmName.SHA256, 32);
            return CryptographicOperations.FixedTimeEquals(kiemTra, bam);
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
