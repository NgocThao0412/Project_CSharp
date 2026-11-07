using System.Text.Json;
using Catalog.Web.DAO;
using Catalog.Web.DTO;

namespace Catalog.Web.BUS;

public sealed class GioHangBUS(IHttpContextAccessor truyCap, ISanPhamDAO sanPham)
{
    private const string Khoa = "GioHangWeb";
    private ISession Phien => truyCap.HttpContext?.Session
        ?? throw new InvalidOperationException("Chưa bật Session trong Program.cs.");

    private List<MucGioHangDTO> Doc()
    {
        try
        {
            var json = Phien.GetString(Khoa);
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

    private void Luu(List<MucGioHangDTO> gio) => Phien.SetString(Khoa, JsonSerializer.Serialize(gio));

    // Session la nguon du lieu gio hang duoc may chu tin cay khi tao don.
    public IReadOnlyList<MucGioHangDTO> LayMucDaChon() => Doc();
    public void XoaSauDatHang() => Phien.Remove(Khoa);

    public async Task<string?> ThemAsync(int ma, int soLuong, CancellationToken ct = default)
    {
        if (ma <= 0 || soLuong is < 1 or > 99) return "Số lượng không hợp lệ.";
        var sp = await sanPham.LayTheoMaAsync(ma, ct);
        if (sp is null) return "Sản phẩm không tồn tại.";
        var gio = Doc();
        var cu = gio.FirstOrDefault(x => x.MaSanPham == ma)?.SoLuong ?? 0;
        if (cu + soLuong > sp.Ton || cu + soLuong > 99) return "Số lượng vượt quá tồn kho.";
        if (cu == 0)
        {
            if (gio.Count >= 30) return "Giỏ hàng tối đa 30 sản phẩm khác nhau.";
            gio.Add(new MucGioHangDTO(ma, soLuong));
        }
        else gio[gio.FindIndex(x => x.MaSanPham == ma)] = new MucGioHangDTO(ma, cu + soLuong);
        Luu(gio);
        return null;
    }

    public async Task<string?> CapNhatAsync(int ma, int soLuong, CancellationToken ct = default)
    {
        if (ma <= 0 || soLuong is < 0 or > 99) return "Số lượng không hợp lệ.";
        var gio = Doc();
        var viTri = gio.FindIndex(x => x.MaSanPham == ma);
        if (soLuong == 0)
        {
            if (viTri >= 0) gio.RemoveAt(viTri);
            Luu(gio);
            return null;
        }
        if (viTri < 0) return "Không tìm thấy sản phẩm trong giỏ.";
        var sp = await sanPham.LayTheoMaAsync(ma, ct);
        if (sp is null || sp.Ton < soLuong) return "Không đủ tồn kho.";
        gio[viTri] = new MucGioHangDTO(ma, soLuong);
        Luu(gio);
        return null;
    }

    public async Task<TrangGioHangDTO> LayTrangAsync(CancellationToken ct = default)
    {
        var result = new List<DongGioHangDTO>();
        foreach (var item in Doc())
        {
            var sp = await sanPham.LayTheoMaAsync(item.MaSanPham, ct);
            if (sp is not null) result.Add(new DongGioHangDTO(sp, item.SoLuong));
        }
        return new TrangGioHangDTO(result);
    }
}
