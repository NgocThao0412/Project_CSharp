using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Catalog.Web.Features.Hao;

// HAO-only types. No dependency on Ny's CatalogModels.cs or Thao's Desktop code.
public sealed record HaoProduct(int ProductId, string Name, decimal PriceVnd, int ActualStock, bool IsOnSale = true);
public sealed record HaoCartEntry(int ProductId, int Quantity);
public sealed record HaoCartLine(int ProductId, string Name, decimal PriceVnd, int Quantity, int ActualStock, bool IsAvailable = true)
{
    public decimal SubtotalVnd => PriceVnd * Quantity;
}
public sealed class HaoCartPageVm
{
    public List<HaoCartLine> Lines { get; init; } = new();
    public decimal TotalVnd => Lines.Sum(x => x.SubtotalVnd);
    public bool IsEmpty => Lines.Count == 0;
    public bool HasUnavailable => Lines.Any(x => !x.IsAvailable);
}
public sealed class HaoCheckoutInput
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Họ tên phải từ 2 đến 100 ký tự.")]
    public string CustomerName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
    [RegularExpression(@"^0\d{9}$", ErrorMessage = "Số điện thoại phải có 10 chữ số, bắt đầu bằng 0.")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vui lòng nhập địa chỉ nhận hàng.")]
    [StringLength(250, MinimumLength = 5, ErrorMessage = "Địa chỉ phải từ 5 đến 250 ký tự.")]
    public string Address { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [StringLength(150)]
    public string? Email { get; set; }
}
public sealed class HaoCheckoutPageVm
{
    public HaoCheckoutInput Input { get; init; } = new();
    public HaoCartPageVm Cart { get; init; } = new();
}
public sealed class HaoOrderLine
{
    public int ProductId { get; init; }
    public string Name { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPriceVnd { get; init; }
    public decimal SubtotalVnd => UnitPriceVnd * Quantity;
}
public static class HaoStatuses
{
    public const string Holding = "DANG_GIU";
    public const string Cancelled = "DA_HUY";
    public const string Expired = "HET_HAN";
    public const string Issued = "DA_XUAT";
    public static string Display(string status) => status switch
    {
        Holding => "Đang giữ hàng",
        Cancelled => "Đã hủy",
        Expired => "Hết hạn giữ hàng",
        Issued => "Đã xuất kho",
        _ => "Không xác định"
    };
}
public sealed class HaoOrderView
{
    public string Code { get; init; } = string.Empty;
    public string CustomerName { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;
    public DateTimeOffset CreatedAtUtc { get; init; }
    public DateTimeOffset ExpiresAtUtc { get; init; }
    public string Status { get; init; } = HaoStatuses.Holding;
    public List<HaoOrderLine> Lines { get; init; } = new();
    public decimal TotalVnd => Lines.Sum(x => x.SubtotalVnd);
    public bool CanCancel => Status == HaoStatuses.Holding && ExpiresAtUtc > DateTimeOffset.UtcNow;
}
public sealed class HaoLookupInput
{
    [Required(ErrorMessage = "Vui lòng nhập mã đơn hàng.")]
    [StringLength(40)]
    public string OrderCode { get; set; } = string.Empty;
    [Required(ErrorMessage = "Vui lòng nhập mã xác thực.")]
    [StringLength(64)]
    public string VerificationCode { get; set; } = string.Empty;
}
public sealed class HaoLookupPageVm
{
    public HaoLookupInput Input { get; init; } = new();
    public HaoOrderView? Order { get; init; }
}
public sealed record HaoPlaceResult(bool Success, string? OrderCode, string? VerificationCode, string? ErrorCode, string? Message)
{
    public static HaoPlaceResult Rejected(string code, string message) => new(false, null, null, code, message);
}
public sealed record HaoCancelResult(bool Success, string? ErrorCode, string Message);

public sealed class HaoRegisterInput
{
    [Required(ErrorMessage = "Vui lòng nhập họ tên.")]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;
    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    [StringLength(128, MinimumLength = 8, ErrorMessage = "Mật khẩu phải có từ 8 đến 128 ký tự.")]
    public string Password { get; set; } = string.Empty;
    [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu.")]
    [Compare(nameof(Password), ErrorMessage = "Mật khẩu xác nhận không trùng khớp.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
public sealed class HaoLoginInput
{
    [Required(ErrorMessage = "Vui lòng nhập email.")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "Vui lòng nhập mật khẩu.")]
    public string Password { get; set; } = string.Empty;
}
