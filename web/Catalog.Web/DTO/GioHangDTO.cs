namespace Catalog.Web.DTO;

public sealed record MucGioHangDTO(int MaSanPham, int SoLuong);
public sealed record DongGioHangDTO(SanPhamDTO SanPham, int SoLuong)
{
    public decimal ThanhTien => SanPham.Gia * SoLuong;
    public bool DuHang => SanPham.Ton >= SoLuong;
}
public sealed record TrangGioHangDTO(IReadOnlyList<DongGioHangDTO> SanPham)
{
    public decimal TongTien => SanPham.Sum(x => x.ThanhTien);
    public bool DuHang => SanPham.Count > 0 && SanPham.All(x => x.DuHang);
}
