# Catalog Frontend (React)

Đây là bản chuyển trực tiếp từ Razor Views sang React của project `Catalog.Web`.
**Không thiết kế lại giao diện**: sử dụng nguyên CSS `src/site.css` và giữ đúng
các class HTML, nội dung, bố cục, header tìm kiếm nhỏ cạnh Trang chủ, banner
không có form tìm kiếm lớn.

## Chạy trên GitHub Codespaces

Terminal 1 — C# API (chỉ dùng module `web/Catalog.Web`):

```sh
dotnet run --project web/Catalog.Web/Catalog.Web.csproj --urls http://0.0.0.0:5080
```

Terminal 2 — React + Vite:

```sh
cd web/Catalog.Frontend
npm install
npm run dev
```

Mở cổng **5173** ở tab PORTS. Vite proxy `/api` sang API chạy ở 5080.

## Địa chỉ

- `/` — Trang chủ
- `/san-pham` — Danh sách và lọc danh mục
- `/san-pham/1` — Chi tiết sản phẩm
- `/tim-kiem?q=ram` — Tìm kiếm qua header

Dữ liệu hiện theo cấu hình `Catalog:UseDemoData` ở `Catalog.Web/appsettings.json`.
Khi SQL Server thật sẵn sàng, backend đổi cờ và cấu hình đúng view/connection string;
React không phải thay đổi giao diện.

## Build frontend

```sh
cd web/Catalog.Frontend
npm run build
```

Vite sẽ build frontend vào `web/Catalog.Web/wwwroot` để ASP.NET Core có thể phục vụ
React trên cùng một cổng. KHÔNG commit `node_modules`, `dist` hay file `bin/obj`.

## Lưu ý tích hợp

Không tự ý giải nén đè lên các phần Web giỏ hàng/đặt hàng của thành viên khác.
Các phần ngoài `web/` (desktop, cơ sở dữ liệu) không nằm trong ZIP này.
