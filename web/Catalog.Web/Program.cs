using Catalog.Web.BUS;
using Catalog.Web.config;
using Catalog.Web.DAO;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
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

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "Catalog.KhachHang";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

        options.ExpireTimeSpan = TimeSpan.FromHours(2);
        options.SlidingExpiration = true;

        // API tra ma loi thay vi chuyen sang trang dang nhap
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };

        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });

builder.Services.AddScoped<KetNoi>();
builder.Services.AddScoped<ISanPhamDAO, SanPhamSqlDAO>();
builder.Services.AddScoped<DonHangDAO>();
builder.Services.AddScoped<DatHangDAO>();
builder.Services.AddScoped<GioHangBUS>();
builder.Services.AddScoped<DatHangBUS>();
builder.Services.AddScoped<SanPhamBUS>();
builder.Services.AddScoped<TaiKhoanDAO>();
builder.Services.AddScoped<TaiKhoanBUS>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}


app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
