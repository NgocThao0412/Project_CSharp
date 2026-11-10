using Catalog.Web.BUS;
using Catalog.Web.config;
using Catalog.Web.DAO;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = ".Catalog.KhachHang";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;
        options.Events.OnRedirectToLogin = ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = ctx =>
        {
            ctx.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(2);
    options.Cookie.Name = ".Catalog.GioHang";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

builder.Services.AddScoped<KetNoi>();
builder.Services.AddScoped<ISanPhamDAO, SanPhamSqlDAO>();
builder.Services.AddScoped<DonHangDAO>();
builder.Services.AddScoped<TaiKhoanDAO>();
builder.Services.AddScoped<TaiKhoanBUS>();
builder.Services.AddScoped<DatHangDAO>();
builder.Services.AddScoped<GioHangBUS>();
builder.Services.AddScoped<DatHangBUS>();
builder.Services.AddScoped<SanPhamBUS>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

// Neu da chay "npm run build", React SPA duoc phuc vu tu wwwroot.
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
// Cho phep refresh cac URL cua React Router (vd: /san-pham/12).
// Khong can Razor / .cshtml: API C# va giao dien React la hai phan rieng.
app.MapFallbackToFile("index.html");

app.Run();
