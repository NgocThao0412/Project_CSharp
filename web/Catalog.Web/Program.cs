
using Catalog.Web.Services;
using Catalog.Web.Features.Hao;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddProblemDetails();

if (builder.Configuration.GetValue<bool>("Catalog:UseDemoData"))
{
    builder.Services.AddSingleton<IProductRepository, DemoProductRepository>();
}
else
{
    builder.Services.AddScoped<IProductRepository, SqlProductRepository>();
}

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddHaoWebDemo(builder.Environment);
}

builder.Services.AddAuthorization();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();

if (app.Environment.IsDevelopment())
{
    app.UseSession();
    app.UseAuthentication();
}

app.UseAuthorization();
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapFallbackToFile("index.html");

app.Run();
