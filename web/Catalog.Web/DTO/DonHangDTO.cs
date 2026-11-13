namespace Catalog.Web.DTO;

public sealed record DongDonHangDTO(int MaSanPham, string Ten, int SoLuong, decimal DonGia)
{
    public decimal ThanhTien => SoLuong * DonGia;
}

public sealed record DonHangTomTatDTO(int Ma, DateTime? NgayDat, int TrangThai, decimal TongTien)
{
    public string TenTrangThai => DonHangTrangThai.Ten(TrangThai);
}

public sealed record DonHangDTO(int Ma, DateTime? NgayDat, int TrangThai, decimal TongTien,
    string HoTenNguoiDat, string SoDienThoaiNguoiDat, string EmailNguoiDat,
    string HoTenNguoiNhan, string SoDienThoaiNguoiNhan, string DiaChiNguoiNhan,
    string GhiChu, string PhuongThucThanhToan, bool LaDonWeb,
    IReadOnlyList<DongDonHangDTO> ChiTiet)
{
    public string TenTrangThai => DonHangTrangThai.Ten(TrangThai);
}

public static class DonHangTrangThai
{
    public static string Ten(int trangThai) => trangThai switch
    {
        0 => "Đã hủy",
        1 => "Đã duyệt",
        2 => "Chờ duyệt",
        _ => "Đang xử lý"
    };
}
