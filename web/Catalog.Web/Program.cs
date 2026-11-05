using Catalog.Web.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

if (builder.Configuration.GetValue<bool>("Catalog:UseDemoData"))
    builder.Services.AddSingleton<IProductRepository, DemoProductRepository>();
else
    builder.Services.AddScoped<IProductRepository, SqlProductRepository>();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/loi");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
