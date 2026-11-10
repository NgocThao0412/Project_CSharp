using System.ComponentModel.DataAnnotations;

namespace Catalog.Web.DTO;

public sealed class DangNhapDTO
{
    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
    public string TenDangNhap { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    public string MatKhau { get; set; } = "";
}

public sealed class DangKyDTO
{
    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
    [RegularExpression(@"^[a-zA-Z0-9._-]{4,50}$", ErrorMessage = "Tên đăng nhập cần từ 4 đến 50 ký tự, chỉ gồm chữ không dấu, số, dấu chấm, gạch dưới hoặc gạch ngang.")]
    public string TenDangNhap { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
    [StringLength(255, MinimumLength = 2)]
    public string HoTen { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [StringLength(50)]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [RegularExpression(@"^0[0-9]{9}$", ErrorMessage = "Số điện thoại phải gồm 10 chữ số, bắt đầu bằng 0.")]
    public string SoDienThoai { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ.")]
    [StringLength(255, MinimumLength = 2)]
    public string DiaChi { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập tỉnh/thành phố.")]
    [StringLength(100)]
    public string TinhThanhPho { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập phường/xã.")]
    [StringLength(100)]
    public string PhuongXa { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Mật khẩu phải từ 8 đến 100 ký tự.")]
    public string MatKhau { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng xác nhận mật khẩu.")]
    [Compare(nameof(MatKhau), ErrorMessage = "Xác nhận mật khẩu không khớp.")]
    public string XacNhanMatKhau { get; set; } = "";
}

public sealed record KhachHangDangNhapDTO(int MaKhach, string TenDangNhap, string HoTen, string MatKhauBam);

public sealed class LoiDangKyException(string thongBao) : Exception(thongBao);

public sealed record ThongTinTaiKhoanDTO(int MaKhach, string TenDangNhap, string HoTen,
    string Email, string SoDienThoai, string DiaChi, string TinhThanhPho, string PhuongXa);

public sealed class CapNhatThongTinDTO
{
    [Required, StringLength(255, MinimumLength = 2)]
    public string HoTen { get; set; } = "";

    [Required, EmailAddress, StringLength(50)]
    public string Email { get; set; } = "";

    [Required, RegularExpression(@"^0[0-9]{9}$")]
    public string SoDienThoai { get; set; } = "";

    [Required, StringLength(255, MinimumLength = 2)]
    public string DiaChi { get; set; } = "";

    [Required, StringLength(100)]
    public string TinhThanhPho { get; set; } = "";

    [Required, StringLength(100)]
    public string PhuongXa { get; set; } = "";
}
