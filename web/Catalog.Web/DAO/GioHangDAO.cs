
using System.Text.Json;
using Catalog.Web.DTO;

namespace Catalog.Web.DAO;

// Gio hang luu tam trong Session.
// Khong can bang GIOHANGWEB trong SQL Server.
public sealed class GioHangDAO(IHttpContextAccessor truyCap)
{
    private ISession Phien =>
        truyCap.HttpContext?.Session
        ?? throw new InvalidOperationException("Session chua duoc bat.");

    private static string Khoa(int maKhach) => $"GioHangWeb_{maKhach}";

    private List<MucGioHangDTO> Doc(int maKhach)
    {
        var json = Phien.GetString(Khoa(maKhach));

        if (string.IsNullOrWhiteSpace(json))
            return new List<MucGioHangDTO>();

        try
        {
            return JsonSerializer.Deserialize<List<MucGioHangDTO>>(json)
                ?? new List<MucGioHangDTO>();
        }
        catch (JsonException)
        {
            return new List<MucGioHangDTO>();
        }
    }

    private void Luu(int maKhach, List<MucGioHangDTO> ds)
    {
        Phien.SetString(Khoa(maKhach), JsonSerializer.Serialize(ds));
    }

    public Task<IReadOnlyList<MucGioHangDTO>> LayAsync(
        int maKhach, CancellationToken ct = default)
    {
        IReadOnlyList<MucGioHangDTO> ds = Doc(maKhach);
        return Task.FromResult(ds);
    }

    public Task<string?> ThemAsync(
        int maKhach, int maSanPham,
        int soLuong, int tonKho,
        CancellationToken ct = default)
    {
        var ds = Doc(maKhach);
        var sp = ds.FirstOrDefault(x => x.MaSanPham == maSanPham);

        int hienCo = sp?.SoLuong ?? 0;
        int tong = hienCo + soLuong;

        if (tong > 99 || tong > tonKho)
            return Task.FromResult<string?>("Số lượng vượt quá tồn kho.");

        if (sp is null)
        {
            if (ds.Count >= 30)
                return Task.FromResult<string?>(
                    "Giỏ hàng tối đa 30 sản phẩm khác nhau.");

            ds.Add(new MucGioHangDTO(maSanPham, soLuong));
        }
        else
        {
            int viTri = ds.FindIndex(x => x.MaSanPham == maSanPham);
            ds[viTri] = new MucGioHangDTO(maSanPham, tong);
        }

        Luu(maKhach, ds);
        return Task.FromResult<string?>(null);
    }

    public Task<string?> CapNhatAsync(
        int maKhach, int maSanPham,
        int soLuong, CancellationToken ct = default)
    {
        var ds = Doc(maKhach);
        int viTri = ds.FindIndex(x => x.MaSanPham == maSanPham);

        if (soLuong == 0)
        {
            ds.RemoveAll(x => x.MaSanPham == maSanPham);
        }
        else
        {
            if (viTri < 0)
                return Task.FromResult<string?>(
                    "Không tìm thấy sản phẩm trong giỏ.");

            ds[viTri] = new MucGioHangDTO(maSanPham, soLuong);
        }

        Luu(maKhach, ds);
        return Task.FromResult<string?>(null);
    }

    public void XoaSauDatHang(int maKhach)
    {
        Phien.Remove(Khoa(maKhach));
    }
}
