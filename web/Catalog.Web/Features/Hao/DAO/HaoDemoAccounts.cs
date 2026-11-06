using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Catalog.Web.Features.Hao;

public sealed class HaoAccountRecord
{
    public string Id { get; init; } = Guid.NewGuid().ToString("N");
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    internal int FailedAttempts { get; set; }
    internal DateTimeOffset? LockedUntilUtc { get; set; }
}
public interface IHaoAccounts
{
    bool TryRegister(HaoRegisterInput input, out HaoAccountRecord? account);
    HaoAccountRecord? Login(string email, string password);
}

// DEMO ONLY: accounts disappear when the process restarts. Do not enable for production.
public sealed class HaoDemoAccounts : IHaoAccounts
{
    private readonly object _lock = new();
    private readonly Dictionary<string, HaoAccountRecord> _users = new(StringComparer.OrdinalIgnoreCase);
    private readonly PasswordHasher<HaoAccountRecord> _hasher = new();
    private readonly ILogger<HaoDemoAccounts> _logger;
    public HaoDemoAccounts(ILogger<HaoDemoAccounts> logger) => _logger = logger;

    public bool TryRegister(HaoRegisterInput input, out HaoAccountRecord? account)
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
            var user = new HaoAccountRecord { Email = email, Name = input.Name.Trim() };
            user.PasswordHash = _hasher.HashPassword(user, input.Password);
            _users.Add(email, user);
            account = user;
            return true;
        }
    }

    public HaoAccountRecord? Login(string email, string password)
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
