# React + JSX (giao diện khách hàng)

Web đồ án dùng **React cho giao diện** và **C# ASP.NET Core cho API**. Không sử dụng Razor `.cshtml`.

- `src/GUI/*.jsx`: Trang chủ, danh sách/chi tiết sản phẩm, giỏ hàng, tài khoản, tra cứu đơn hàng.
- `src/API/*.js`: gọi `/api/*` của C# bằng `fetch`.
- `src/site.css`, `src/product-images.css`: CSS giao diện.
- `vite.config.js`: proxy `/api` tới `http://127.0.0.1:5080` khi chạy dev.

## Chạy trên máy (Windows PowerShell)

Mở hai Terminal tại thư mục gốc `Project_CSharp`.

**Terminal 1:**

```powershell
dotnet run --project web\Catalog.Web\Catalog.Web.csproj --urls http://127.0.0.1:5080
```

**Terminal 2:**

```powershell
cd web\Catalog.Frontend
npm.cmd ci
npm.cmd run dev
```

Mở **http://localhost:5173/**.

## Đóng gói React cho ASP.NET Core

```powershell
cd web\Catalog.Frontend
npm.cmd run build
```

Lệnh này tạo trang React vào `../Catalog.Web/wwwroot` (không xóa `images/`). Sau đó chạy backend tại cổng 5080 và mở **http://127.0.0.1:5080/** để dùng React + API C# cùng một cổng.

> Sản phẩm, danh mục và tồn kho được lấy từ `dbo.vw_Catalog` trong SQL Server; không có chế độ demo khi không kết nối CSDL. Đăng nhập/đăng ký và đặt hàng trực tuyến vẫn chưa có API xác thực/tạo đơn trong nguồn gốc, vì vậy các nút submit chưa được bật.

Xem thêm [hướng dẫn Web tổng thể](../../HUONG_DAN_WEB_REACT_CSHARP.md).
