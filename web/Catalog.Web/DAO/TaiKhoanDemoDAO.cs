using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Catalog.Web.Features.BanHang;

public sealed class TaiKhoanKhachHang
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N");
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    internal int FailedAttempts { get; set; }
    internal DateTimeOffset? LockedUntilUtc { get; set; }
}
public interface ITaiKhoanKhachHang
{
    bool TryRegister(ThongTinDangKy input, out TaiKhoanKhachHang? account);
    TaiKhoanKhachHang? Login(string email, string password);
}

// DEMO ONLY: accounts disappear when the process restarts. Do not enable for production.
public sealed class TaiKhoanDemo : ITaiKhoanKhachHang
{
    private readonly object _lock = new();
    private readonly Dictionary<string, TaiKhoanKhachHang> _users = new(StringComparer.OrdinalIgnoreCase);
    private readonly PasswordHasher<TaiKhoanKhachHang> _hasher = new();
    private readonly ILogger<TaiKhoanDemo> _logger;
    public TaiKhoanDemo(ILogger<TaiKhoanDemo> logger) => _logger = logger;

    public bool TryRegister(ThongTinDangKy input, out TaiKhoanKhachHang? account)
    {
        account = null;
        var email = (input.Email ?? "").Trim().ToLowerInvariant();
        if (email.Length > 150 || !new EmailAddressAttribute().IsValid(email) ||
            input.Password is null || input.Password.Length is < 8 or > 128 ||
            string.IsNullOrWhiteSpace(input.Name) || input.Name.Trim().Length is < 2 or > 100 ||
            input.Password != input.ConfirmPassword) return false;
        lock (_lock)
        {
            if (_users.ContainsKey(email)) return false;
            var user = new TaiKhoanKhachHang { Email = email, Name = input.Name.Trim() };
            user.PasswordHash = _hasher.HashPassword(user, input.Password);
            _users.Add(email, user);
            account = user;
            return true;
        }
    }

    public TaiKhoanKhachHang? Login(string email, string password)
    {
        lock (_lock)
        {
            if (!_users.TryGetValue((email ?? "").Trim(), out var user))
            {
                _logger.LogWarning("AUTH-01 incorrect login");
                return null;
            }
            var now = DateTimeOffset.UtcNow;
            if (user.LockedUntilUtc > now) return null;
            if (_hasher.VerifyHashedPassword(user, user.PasswordHash, password ?? "") == PasswordVerificationResult.Failed)
            {
                user.FailedAttempts++;
                if (user.FailedAttempts >= 5)
                {
                    user.LockedUntilUtc = now.AddMinutes(15);
                    user.FailedAttempts = 0;
                }
                _logger.LogWarning("AUTH-01 incorrect login");
                return null;
            }
            user.FailedAttempts = 0;
            user.LockedUntilUtc = null;
            return user;
        }
    }
}
