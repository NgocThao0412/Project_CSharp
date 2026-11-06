# Catalog.Web — API ASP.NET Core 10

React frontend nằm ở `../Catalog.Frontend`. Phần này giữ `Models`, `Services`,
`IProductRepository`, `SqlProductRepository` và cấu hình SQL Server từ project cũ.

API:
- `GET /api/catalog/featured?count=6`
- `GET /api/catalog/products?q=&category=&page=1`
- `GET /api/catalog/products/1`
- `GET /api/catalog/categories`

Mặc định dùng dữ liệu DEMO (`Catalog:UseDemoData = true`). Để dùng SQL Server thật,
phải thiết lập `ConnectionStrings:DefaultConnection` an toàn và triển khai
`dbo.vw_Catalog` theo `Database/VIEW_CONTRACT.md` trước khi chuyển thành false.
Không đưa mật khẩu vào git.

Trong Codespaces, phát triển bằng hai Terminal:
1. `dotnet run --project web/Catalog.Web/Catalog.Web.csproj --urls http://0.0.0.0:5080`
2. `cd web/Catalog.Frontend && npm install && npm run dev`

Mở PORTS `5173` để dùng giao diện React; `5080` cung cấp API.

Để backend phục vụ luôn giao diện sau khi build:
`cd web/Catalog.Frontend && npm run build`, sau đó mở PORTS `5080`.
