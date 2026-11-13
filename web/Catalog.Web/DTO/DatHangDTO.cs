using System.ComponentModel.DataAnnotations;

namespace Catalog.Web.DTO;

public sealed class YeuCauDatHangDTO
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên người nhận.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Họ tên người nhận phải từ 2 đến 100 ký tự.")]
    [RegularExpression(@"^[\p{L}]+(?: +[\p{L}]+)*$", ErrorMessage = "Họ tên người nhận chỉ được chứa chữ cái và khoảng trắng, không chứa ký tự đặc biệt.")]
    public string HoTen { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại người nhận.")]
    [RegularExpression(@"^0[0-9]{9}$", ErrorMessage = "Số điện thoại người nhận phải đủ 10 chữ số và bắt đầu bằng 0.")]
    public string SoDienThoai { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng.")]
    [StringLength(255, MinimumLength = 10, ErrorMessage = "Địa chỉ giao hàng phải từ 10 đến 255 ký tự.")]
    public string DiaChi { get; set; } = "";

    [EmailAddress(ErrorMessage = "Email người nhận không đúng định dạng.")]
    [StringLength(50, ErrorMessage = "Email người nhận tối đa 50 ký tự.")]
    public string? Email { get; set; }

    [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự.")]
    public string? GhiChu { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Giỏ hàng không hợp lệ.")]
    public decimal TongTienXacNhan { get; set; }

    public Guid MaYeuCau { get; set; }
    public bool MuaNgay { get; set; }
}

public sealed record KetQuaDatHangDTO(int MaDon, decimal TongTien, DateTime ThoiGian,
    string TrangThai, string HinhThucThanhToan);

public sealed class LoiDatHangException(string thongBao) : Exception(thongBao);
