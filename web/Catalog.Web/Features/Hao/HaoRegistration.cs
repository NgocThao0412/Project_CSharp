using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Catalog.Web.Features.Hao;

public static class HaoAuth
{
    public const string Scheme = "HaoCustomer";
}

public static class HaoRegistration
{
    // Temporary demo wiring. Replace IHaoCatalog/IHaoOrders/IHaoAccounts with SQL-backed
    // shared implementations after Ny/Thao agree on schema and transaction semantics.
    public static IServiceCollection AddHaoWebDemo(this IServiceCollection services, IHostEnvironment environment)
    {
        // Fail closed: an in-memory store must never be used by a deployed Web app.
        if (!environment.IsDevelopment())
            throw new InvalidOperationException("HaoDemo chỉ chạy Development. Hãy tích hợp SQL DAO trước khi deploy.");
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
        services.AddAuthentication().AddCookie(HaoAuth.Scheme, options =>
        {
            options.Cookie.Name = ".Catalog.HaoCustomer";
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Lax;
            options.LoginPath = "/tai-khoan/dang-nhap";
            options.AccessDeniedPath = "/tai-khoan/dang-nhap";
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
            options.SlidingExpiration = true;
        });
        services.TryAddSingleton<IHaoCatalog, HaoDemoCatalog>();
        services.TryAddSingleton<IHaoOrders, HaoDemoOrders>();
        services.TryAddSingleton<IHaoAccounts, HaoDemoAccounts>();
        services.AddScoped<HaoCart>();
        services.AddScoped<HaoCartBuilder>();
        return services;
    }
}
