namespace Catalog.Web.DTO;

public sealed record SanPhamDTO(
    int Ma, string Ten, string DanhMuc, string Hang,
    decimal Gia, int Ton, string? HinhAnh, string MoTa);

public sealed record TimKiemSanPhamDTO(string? TuKhoa, string? DanhMuc, int Trang = 1, string SapXep = "mac-dinh")
{
    public const int SoSanPhamMoiTrang = 8;
    public int TrangAnToan => Math.Clamp(Trang, 1, 10000);
    public string TuKhoaAnToan => (TuKhoa ?? "").Trim()[..Math.Min((TuKhoa ?? "").Trim().Length, 100)];
    public string DanhMucAnToan => (DanhMuc ?? "").Trim()[..Math.Min((DanhMuc ?? "").Trim().Length, 100)];
    public string SapXepAnToan => SapXep is "gia-tang" or "gia-giam" or "ten" ? SapXep : "mac-dinh";
}

public sealed record DanhSachSanPhamDTO(
    IReadOnlyList<SanPhamDTO> SanPham, int Tong, int Trang, int SoLuongMoiTrang,
    string TuKhoa, string DanhMuc, string SapXep)
{
    public int SoTrang => Math.Max(1, (Tong + SoLuongMoiTrang - 1) / SoLuongMoiTrang);
}

public sealed record TrangSanPhamDTO(DanhSachSanPhamDTO KetQua, IReadOnlyList<string> DanhMuc);
