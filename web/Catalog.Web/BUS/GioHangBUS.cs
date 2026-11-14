using System.Security.Claims;
using System.Text.Json;
using Catalog.Web.DAO;
using Catalog.Web.DTO;

namespace Catalog.Web.BUS;

public sealed class GioHangBUS(IHttpContextAccessor truyCap, ISanPhamDAO sanPham, GioHangDAO gioHangDao)
{
    private const string KhoaMuaNgay = "MuaNgayWeb";
    private ISession Phien => truyCap.HttpContext?.Session
        ?? throw new InvalidOperationException("Chưa bật Session trong Program.cs.");

    private int MaKhach => int.TryParse(truyCap.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var ma)
        && ma > 0 ? ma : throw new UnauthorizedAccessException("Vui lòng đăng nhập để sử dụng giỏ hàng.");

    private List<MucGioHangDTO> DocMuaNgay()
    {
        try
        {
            var json = Phien.GetString(KhoaMuaNgay);
            if (string.IsNullOrWhiteSpace(json)) return new();
            return (JsonSerializer.Deserialize<List<MucGioHangDTO>>(json) ?? new())
                .Where(x => x.MaSanPham > 0 && x.SoLuong is > 0 and <= 99)
                .GroupBy(x => x.MaSanPham)
                .Select(g => new MucGioHangDTO(g.Key, Math.Min(99, g.Sum(x => x.SoLuong))))
                .Take(30).ToList();
        }
        catch (JsonException)
        {
            return new();
        }
    }

    public async Task<IReadOnlyList<MucGioHangDTO>> LayMucDaChonAsync(int maKhach, bool muaNgay = false, CancellationToken ct = default)
        => muaNgay ? DocMuaNgay() : await gioHangDao.LayAsync(maKhach, ct);

    public void XoaMuaNgaySauDatHang() => Phien.Remove(KhoaMuaNgay);

    public async Task<string?> ChonMuaNgayAsync(int ma, CancellationToken ct = default)
    {
        if (ma <= 0) return "Sản phẩm không hợp lệ.";
        var sp = await sanPham.LayTheoMaAsync(ma, ct);
        if (sp is null) return "Sản phẩm không tồn tại.";
        if (sp.Ton < 1) return "Sản phẩm tạm hết hàng.";
        Phien.SetString(KhoaMuaNgay, JsonSerializer.Serialize(new List<MucGioHangDTO> { new(ma, 1) }));
        return null;
    }

    public async Task<string?> ThemAsync(int ma, int soLuong, CancellationToken ct = default)
    {
        if (ma <= 0 || soLuong is < 1 or > 99) return "Số lượng không hợp lệ.";
        var sp = await sanPham.LayTheoMaAsync(ma, ct);
        if (sp is null) return "Sản phẩm không tồn tại.";
        return await gioHangDao.ThemAsync(MaKhach, ma, soLuong, sp.Ton, ct);
    }

    public async Task<string?> CapNhatAsync(int ma, int soLuong, CancellationToken ct = default)
    {
        if (ma <= 0 || soLuong is < 0 or > 99) return "Số lượng không hợp lệ.";
        if (soLuong > 0)
        {
            var sp = await sanPham.LayTheoMaAsync(ma, ct);
            if (sp is null || sp.Ton < soLuong) return "Không đủ tồn kho.";
        }
        return await gioHangDao.CapNhatAsync(MaKhach, ma, soLuong, ct);
    }

    public async Task<TrangGioHangDTO> LayTrangAsync(CancellationToken ct = default)
        => await TaoTrangAsync(await gioHangDao.LayAsync(MaKhach, ct), ct);

    public Task<TrangGioHangDTO> LayMuaNgayAsync(CancellationToken ct = default)
        => TaoTrangAsync(DocMuaNgay(), ct);

    private async Task<TrangGioHangDTO> TaoTrangAsync(IEnumerable<MucGioHangDTO> danhSach, CancellationToken ct)
    {
        var result = new List<DongGioHangDTO>();
        foreach (var item in danhSach)
        {
            var sp = await sanPham.LayTheoMaAsync(item.MaSanPham, ct);
            if (sp is not null) result.Add(new DongGioHangDTO(sp, item.SoLuong));
        }
        return new TrangGioHangDTO(result);
    }
}
