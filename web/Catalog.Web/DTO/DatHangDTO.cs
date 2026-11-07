using System.ComponentModel.DataAnnotations;

namespace Catalog.Web.DTO;

// Chi nhan thong tin giao hang: san pham, so luong va don gia luon lay tu Session va SQL Server.
public sealed class YeuCauDatHangDTO
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên người nhận.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Họ tên phải từ 2 đến 100 ký tự.")]
    public string HoTen { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải gồm 10 chữ số và bắt đầu bằng 0.")]
    public string SoDienThoai { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng.")]
    [StringLength(255, MinimumLength = 10, ErrorMessage = "Địa chỉ phải từ 10 đến 255 ký tự.")]
    public string DiaChi { get; set; } = "";

    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    [StringLength(50, ErrorMessage = "Email tối đa 50 ký tự.")]
    public string? Email { get; set; }

    [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự.")]
    public string? GhiChu { get; set; }

    // Ngay luc thanh toan, khach phai nhin thay dung tong tien se ghi vao SQL Server.
    [Range(1, int.MaxValue, ErrorMessage = "Giỏ hàng không hợp lệ.")]
    public decimal TongTienXacNhan { get; set; }

    // Token do giao dien tao, de tranh ghi trung don neu nguoi dung gui lai yeu cau.
    public Guid MaYeuCau { get; set; }
}

public sealed record KetQuaDatHangDTO(int MaDon, decimal TongTien, DateTime ThoiGian,
    string TrangThai, string HinhThucThanhToan);

public sealed class LoiDatHangException(string thongBao) : Exception(thongBao);
