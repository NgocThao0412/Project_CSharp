using Catalog.Web.BUS;
using Catalog.Web.config;
using Catalog.Web.DAO;

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

builder.Services.AddScoped<KetNoi>();
builder.Services.AddScoped<ISanPhamDAO, SanPhamSqlDAO>();
builder.Services.AddScoped<DonHangDAO>();
builder.Services.AddScoped<GioHangBUS>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.MapControllers();

app.Run();
