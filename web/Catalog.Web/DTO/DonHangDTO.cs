using System.ComponentModel.DataAnnotations;

namespace Catalog.Web.DTO;

public sealed class TraCuuDonHangDTO
{
    [Range(1, int.MaxValue, ErrorMessage = "Mã đơn hàng không hợp lệ.")]
    public int MaDon { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại gồm 10 chữ số và bắt đầu bằng 0.")]
    public string SoDienThoai { get; set; } = "";
}

public sealed record DongDonHangDTO(int MaSanPham, string Ten, int SoLuong, decimal DonGia)
{
    public decimal ThanhTien => SoLuong * DonGia;
}

public sealed record DonHangDTO(int Ma, string HoTen, DateTime? NgayDat, int TrangThai,
    decimal TongTien, IReadOnlyList<DongDonHangDTO> ChiTiet)
{
    public string TenTrangThai => TrangThai switch
    {
        0 => "Đã hủy",
        1 => "Đã duyệt",
        2 => "Chờ duyệt",
        _ => "Đang xử lý"
    };
}

public sealed class TrangTraCuuDTO
{
    public TraCuuDonHangDTO ThongTin { get; set; } = new();
    public DonHangDTO? DonHang { get; set; }
    public bool DaTraCuu { get; set; }
}
