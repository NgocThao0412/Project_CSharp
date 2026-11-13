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
    [RegularExpression(@"^(?=(?:[^A-Za-z]*[A-Za-z]){4})[A-Za-z0-9._-]{4,50}$",
        ErrorMessage = "Tên đăng nhập phải có ít nhất 4 chữ cái, tối đa 50 ký tự và không chứa khoảng trắng hay ký tự đặc biệt khác.")]
    public string TenDangNhap { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
    [StringLength(255, MinimumLength = 2, ErrorMessage = "Họ và tên phải từ 2 đến 255 ký tự.")]
    [RegularExpression(@"^[\p{L}]+(?: +[\p{L}]+)*$", ErrorMessage = "Họ và tên chỉ được chứa chữ cái và khoảng trắng, không được chứa số hoặc ký tự đặc biệt.")]
    public string HoTen { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [StringLength(50, ErrorMessage = "Email không được vượt quá 50 ký tự.")]
    [RegularExpression(@"(?i)^[a-z0-9._%+\-]+@gmail\.com$", ErrorMessage = "Email phải hợp lệ và có đuôi @gmail.com.")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [RegularExpression(@"^0[0-9]{9}$", ErrorMessage = "Số điện thoại phải đủ 10 chữ số và bắt đầu bằng 0.")]
    public string SoDienThoai { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ.")]
    [StringLength(255, MinimumLength = 2, ErrorMessage = "Địa chỉ phải từ 2 đến 255 ký tự.")]
    public string DiaChi { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng chọn tỉnh/thành phố.")]
    [StringLength(100, ErrorMessage = "Tên tỉnh/thành phố tối đa 100 ký tự.")]
    public string TinhThanhPho { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng chọn phường/xã.")]
    [StringLength(100, ErrorMessage = "Tên phường/xã tối đa 100 ký tự.")]
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
    [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập.")]
    [RegularExpression(@"^(?=(?:[^A-Za-z]*[A-Za-z]){4})[A-Za-z0-9._-]{4,50}$",
        ErrorMessage = "Tên đăng nhập phải có ít nhất 4 chữ cái, tối đa 50 ký tự và không chứa khoảng trắng hay ký tự đặc biệt khác.")]
    public string TenDangNhap { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập họ và tên.")]
    [StringLength(255, MinimumLength = 2, ErrorMessage = "Họ và tên phải từ 2 đến 255 ký tự.")]
    [RegularExpression(@"^[\p{L}]+(?: +[\p{L}]+)*$", ErrorMessage = "Họ và tên chỉ được chứa chữ cái và khoảng trắng, không được chứa số hoặc ký tự đặc biệt.")]
    public string HoTen { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [StringLength(50, ErrorMessage = "Email không được vượt quá 50 ký tự.")]
    [RegularExpression(@"(?i)^[a-z0-9._%+\-]+@gmail\.com$", ErrorMessage = "Email phải hợp lệ và có đuôi @gmail.com.")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [RegularExpression(@"^0[0-9]{9}$", ErrorMessage = "Số điện thoại phải đủ 10 chữ số và bắt đầu bằng 0.")]
    public string SoDienThoai { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ.")]
    [StringLength(255, MinimumLength = 2, ErrorMessage = "Địa chỉ phải từ 2 đến 255 ký tự.")]
    public string DiaChi { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng chọn tỉnh/thành phố.")]
    [StringLength(100, ErrorMessage = "Tên tỉnh/thành phố tối đa 100 ký tự.")]
    public string TinhThanhPho { get; set; } = "";

    [Required(ErrorMessage = "Vui lòng chọn phường/xã.")]
    [StringLength(100, ErrorMessage = "Tên phường/xã tối đa 100 ký tự.")]
    public string PhuongXa { get; set; } = "";
}
