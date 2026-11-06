# Web React - giữ nguyên giao diện

Cấu trúc:
- `Catalog.Frontend/`: React + Vite, giao diện chuyển 1:1 từ Razor gốc.
- `Catalog.Web/`: API ASP.NET Core 10, giữ nguyên Models/Services/SQL repository của Web.

Không thay đổi màu, font, khoảng cách, văn bản hiển thị và bố cục giao diện.
CSS React là bản sao của `Catalog.Web/wwwroot/css/site.css` gốc.
Header có ô tìm kiếm nhỏ cạnh "Trang chủ"; banner không có ô tìm kiếm lớn.

Khởi chạy theo `Catalog.Frontend/README.md`.
