using Catalog.Web.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddProblemDetails();

// Không đổi Repository: dữ liệu mẫu khi chạy thử, SQL Server khi tích hợp thật.
if (builder.Configuration.GetValue<bool>("Catalog:UseDemoData"))
    builder.Services.AddSingleton<IProductRepository, DemoProductRepository>();
else
    builder.Services.AddScoped<IProductRepository, SqlProductRepository>();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler();
    app.UseHsts();
    app.UseHttpsRedirection();
}

// React (sau npm run build) nằm trong wwwroot; API không đi qua React.
app.UseDefaultFiles();
app.UseStaticFiles();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.Run();
