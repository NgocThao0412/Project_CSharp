using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace Catalog.Web.Features.BanHang;

public interface IDonHang
{
    KetQuaDatHang Place(ThongTinDatHang customer, IReadOnlyList<MucGioHang> cart, string? customerAccountId);
    DonHangViewModel? FindWithVerification(string code, string verification);
    KetQuaHuyDonHang CancelWithVerification(string code, string verification);
    IReadOnlyList<DonHangViewModel> Mine(string customerAccountId);
}

// DEMO ONLY: the lock prevents overselling inside this ONE application process.
// It does NOT synchronize with SQL Server, Desktop or another Web server.
public sealed class DonHangDemo : IDonHang
{
    private sealed class Record
    {
        public string Code { get; init; } = string.Empty;
        public byte[] VerificationHash { get; init; } = Array.Empty<byte>();
        public string? AccountId { get; init; }
        public string CustomerName { get; init; } = string.Empty;
        public string Address { get; init; } = string.Empty;
        public DateTimeOffset CreatedAtUtc { get; init; }
        public DateTimeOffset ExpiresAtUtc { get; init; }
        public string Status { get; set; } = TrangThaiDonHang.Holding;
        public List<ChiTietDonHang> Lines { get; init; } = new();
    }

    private readonly object _lock = new();
    private readonly Dictionary<string, Record> _orders = new(StringComparer.OrdinalIgnoreCase);
    private readonly IDanhMucSanPham _catalog;
    private readonly ILogger<DonHangDemo> _logger;

    public DonHangDemo(IDanhMucSanPham catalog, ILogger<DonHangDemo> logger)
    {
        _catalog = catalog;
        _logger = logger;
    }

    private static byte[] Hash(string secret) => SHA256.HashData(Encoding.UTF8.GetBytes(secret.Trim()));

    private void ExpireUnlocked(DateTimeOffset now)
    {
        foreach (var o in _orders.Values)
        {
            if (o.Status == TrangThaiDonHang.Holding && o.ExpiresAtUtc <= now)
            {
                o.Status = TrangThaiDonHang.Expired;
                _logger.LogInformation("ORDER-EXPIRE code={OrderCode}", o.Code);
            }
        }
    }

    private DonHangViewModel Snapshot(Record o) => new()
    {
        Code = o.Code,
        CustomerName = o.CustomerName,
        Address = o.Address,
        CreatedAtUtc = o.CreatedAtUtc,
        ExpiresAtUtc = o.ExpiresAtUtc,
        Status = o.Status,
        Lines = o.Lines.Select(x => new ChiTietDonHang
        {
            ProductId = x.ProductId, Name = x.Name,
            Quantity = x.Quantity, UnitPriceVnd = x.UnitPriceVnd
        }).ToList()
    };

    private bool Verified(Record order, string secret)
    {
        if (string.IsNullOrWhiteSpace(secret) || secret.Length != 32) return false;
        return CryptographicOperations.FixedTimeEquals(order.VerificationHash, Hash(secret));
    }

    public KetQuaDatHang Place(ThongTinDatHang customer, IReadOnlyList<MucGioHang> cart, string? customerAccountId)
    {
        if (string.IsNullOrWhiteSpace(customer.CustomerName) || customer.CustomerName.Trim().Length is < 2 or > 100 ||
            !Regex.IsMatch(customer.Phone ?? "", @"^0\d{9}$") ||
            string.IsNullOrWhiteSpace(customer.Address) || customer.Address.Trim().Length is < 5 or > 250 ||
            (!string.IsNullOrWhiteSpace(customer.Email) &&
             (customer.Email.Length > 150 || !new EmailAddressAttribute().IsValid(customer.Email))))
            return KetQuaDatHang.Rejected("ORDER-INPUT", "Thông tin nhận hàng không hợp lệ.");
        if (cart.Count == 0 || cart.Count > 30 || cart.Any(x => x.ProductId <= 0 || x.Quantity is < 1 or > 99) ||
            cart.Select(x => x.ProductId).Distinct().Count() != cart.Count)
            return KetQuaDatHang.Rejected("ORDER-INPUT", "Giỏ hàng không hợp lệ.");

        lock (_lock)
        {
            var now = DateTimeOffset.UtcNow;
            ExpireUnlocked(now);
            var lines = new List<ChiTietDonHang>();
            foreach (var item in cart.OrderBy(x => x.ProductId))
            {
                var product = _catalog.Get(item.ProductId);
                if (product is null || !product.IsOnSale || product.PriceVnd < 0)
                    return KetQuaDatHang.Rejected("ORDER-01", "Sản phẩm không còn được bán.");
                var held = _orders.Values
                    .Where(o => o.Status == TrangThaiDonHang.Holding)
                    .SelectMany(o => o.Lines)
                    .Where(x => x.ProductId == item.ProductId)
                    .Sum(x => x.Quantity);
                if (product.ActualStock - held < item.Quantity)
                {
                    _logger.LogWarning("STOCK-01 product={ProductId} requested={Quantity}", item.ProductId, item.Quantity);
                    return KetQuaDatHang.Rejected("STOCK-01", $"Không đủ tồn khả dụng cho {product.Name}.");
                }
                lines.Add(new ChiTietDonHang
                {
                    ProductId = product.ProductId, Name = product.Name,
                    UnitPriceVnd = product.PriceVnd, Quantity = item.Quantity
                });
            }
            var code = "DH" + Convert.ToHexString(RandomNumberGenerator.GetBytes(7));
            var secret = Convert.ToHexString(RandomNumberGenerator.GetBytes(16));
            var record = new Record
            {
                Code = code,
                VerificationHash = Hash(secret),
                AccountId = customerAccountId,
                CustomerName = customer.CustomerName.Trim(),
                Address = customer.Address.Trim(),
                CreatedAtUtc = now,
                ExpiresAtUtc = now.AddMinutes(30),
                Lines = lines
            };
            _orders.Add(code, record);
            _logger.LogInformation("ORDER-PLACE code={OrderCode} lines={Count}", code, lines.Count);
            return new KetQuaDatHang(true, code, secret, null, null);
        }
    }

    public DonHangViewModel? FindWithVerification(string code, string verification)
    {
        lock (_lock)
        {
            ExpireUnlocked(DateTimeOffset.UtcNow);
            if (!_orders.TryGetValue((code ?? "").Trim(), out var order) || !Verified(order, verification))
            {
                _logger.LogWarning("ORDER-LOOKUP-FAILED");
                return null;
            }
            return Snapshot(order);
        }
    }

    public KetQuaHuyDonHang CancelWithVerification(string code, string verification)
    {
        lock (_lock)
        {
            ExpireUnlocked(DateTimeOffset.UtcNow);
            if (!_orders.TryGetValue((code ?? "").Trim(), out var order) || !Verified(order, verification))
                return new KetQuaHuyDonHang(false, "ORDER-AUTH", "Mã đơn hoặc mã xác thực không chính xác.");
            if (order.Status != TrangThaiDonHang.Holding)
            {
                _logger.LogWarning("ORDER-01 code={OrderCode} status={Status}", order.Code, order.Status);
                return new KetQuaHuyDonHang(false, "ORDER-01", "Đơn đã xử lý hoặc hết hạn, không thể hủy theo quy trình Web.");
            }
            order.Status = TrangThaiDonHang.Cancelled; // no change to actual stock
            _logger.LogInformation("ORDER-CANCEL code={OrderCode}", order.Code);
            return new KetQuaHuyDonHang(true, null, "Đã hủy đơn và giải phóng lượng hàng giữ.");
        }
    }

    public IReadOnlyList<DonHangViewModel> Mine(string customerAccountId)
    {
        lock (_lock)
        {
            ExpireUnlocked(DateTimeOffset.UtcNow);
            return _orders.Values
                .Where(o => !string.IsNullOrEmpty(o.AccountId) && o.AccountId == customerAccountId)
                .OrderByDescending(o => o.CreatedAtUtc)
                .Select(Snapshot).ToList();
        }
    }
}
