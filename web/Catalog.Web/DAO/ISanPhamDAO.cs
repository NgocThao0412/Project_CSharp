using Catalog.Web.DTO;

namespace Catalog.Web.DAO;

public interface ISanPhamDAO
{
    Task<IReadOnlyList<SanPhamDTO>> LayNoiBatAsync(int soLuong, CancellationToken ct = default);
    Task<IReadOnlyList<string>> LayDanhMucAsync(CancellationToken ct = default);
    Task<DanhSachSanPhamDTO> TimAsync(TimKiemSanPhamDTO dieuKien, CancellationToken ct = default);
    Task<SanPhamDTO?> LayTheoMaAsync(int ma, CancellationToken ct = default);
}
