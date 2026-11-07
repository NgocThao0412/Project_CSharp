using Catalog.Web.DAO;
using Catalog.Web.DTO;

namespace Catalog.Web.BUS;

// Nghiep vu danh muc / tim kiem san pham. Controller khong truy cap SQL truc tiep.
public sealed class SanPhamBUS(ISanPhamDAO sanPham)
{
    public Task<IReadOnlyList<SanPhamDTO>> LayNoiBatAsync(int soLuong, CancellationToken ct = default)
        => sanPham.LayNoiBatAsync(Math.Clamp(soLuong, 1, 20), ct);

    public Task<IReadOnlyList<string>> LayDanhMucAsync(CancellationToken ct = default)
        => sanPham.LayDanhMucAsync(ct);

    public Task<DanhSachSanPhamDTO> TimAsync(string? tuKhoa, string? danhMuc, int trang,
        string? sapXep, CancellationToken ct = default)
        => sanPham.TimAsync(new TimKiemSanPhamDTO(tuKhoa, danhMuc, trang, sapXep ?? "mac-dinh"), ct);

    public Task<SanPhamDTO?> LayTheoMaAsync(int ma, CancellationToken ct = default)
        => sanPham.LayTheoMaAsync(ma, ct);
}
