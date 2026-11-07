using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Catalog.Web.Features.BanHang;

public static class XacThucKhachHang
{
    public const string Scheme = "HaoCustomer";
}

public static class DangKyDichVu
{
    // Temporary demo wiring. Replace IDanhMucSanPham/IDonHang/ITaiKhoanKhachHang with SQL-backed
    // shared implementations after Ny/Thao agree on schema and transaction semantics.
    public static IServiceCollection AddBanHangDemo(this IServiceCollection services, IHostEnvironment environment)
    {
        // Fail closed: an in-memory store must never be used by a deployed Web app.
        if (!environment.IsDevelopment())
            throw new InvalidOperationException("Demo chỉ chạy Development. Hãy tích hợp SQL DAO trước khi deploy.");
        services.AddHttpContextAccessor();
        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.Cookie.Name = ".Catalog.HaoCart";
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
            options.IdleTimeout = TimeSpan.FromHours(2);
        });
        services.AddAuthentication().AddCookie(XacThucKhachHang.Scheme, options =>
        {
            options.Cookie.Name = ".Catalog.HaoCustomer";
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
            options.LoginPath = "/tai-khoan/dang-nhap";
            options.AccessDeniedPath = "/tai-khoan/dang-nhap";
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
            options.SlidingExpiration = true;
        });
        services.TryAddSingleton<IDanhMucSanPham, DanhMucSanPhamDemo>();
        services.TryAddSingleton<IDonHang, DonHangDemo>();
        services.TryAddSingleton<ITaiKhoanKhachHang, TaiKhoanDemo>();
        services.AddScoped<GioHang>();
        services.AddScoped<TaoGioHang>();
        return services;
    }
}
