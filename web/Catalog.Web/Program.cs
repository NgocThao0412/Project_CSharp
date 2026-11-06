
using Catalog.Web.Services;
using Catalog.Web.Features.Hao;

var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddControllersWithViews();

if (builder.Configuration.GetValue<bool>("Catalog:UseDemoData"))
    builder.Services.AddSingleton<IProductRepository, DemoProductRepository>();
else
    builder.Services.AddScoped<IProductRepository, SqlProductRepository>();
    builder.Services.AddHaoWebDemo(builder.Environment);
    builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/loi");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
