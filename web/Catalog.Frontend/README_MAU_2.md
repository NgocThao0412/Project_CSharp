# Giao diện React - Mẫu 2 (trắng/xanh)

Bản này giữ nguyên toàn bộ API/backend C# và logic tìm kiếm, chi tiết, danh sách sản phẩm từ `Web_React_Giu_Nguyen_Giao_Dien.zip`. Chỉ thay giao diện React của trang chủ và header.

## Các file đã đổi

- `src/components/SiteLayout.jsx`: header trắng, logo kim cương, Trang chủ/Sản phẩm, hai mục dự kiến Tin tức/Liên hệ, ô tìm kiếm nhỏ, biểu tượng giỏ hàng/tài khoản để sẵn cho tích hợp.
- `src/pages/HomePage.jsx`: banner, CTA tới danh sách, bốn lợi ích, giữ danh sách sản phẩm nổi bật gọi API cũ.
- `src/site.css`: thiết kế responsive cùng bảng màu, layout như ảnh demo đã duyệt.
- `public/images/hero-banner.webp` và `public/images/hero-parts.webp`: ảnh phần cứng lấy từ ảnh demo đã được duyệt.

Giỏ hàng, tài khoản, Tin tức và Liên hệ chưa có logic trong project của Ny. Các biểu tượng/mục đó chỉ để hiển thị theo demo, không tự nhận là chức năng đã hoàn thành; số lượng trong giỏ mặc định 0.

## Chạy trên Visual Studio / Windows

Mở PowerShell ở thư mục gốc `Project_CSharp`:

**Terminal 1 (API C#):**

```powershell
dotnet run --project .\web\Catalog.Web\Catalog.Web.csproj --urls http://localhost:5080
```

**Terminal 2 (React):**

```powershell
cd .\web\Catalog.Frontend
npm.cmd install
npm.cmd run dev
```

Mở `http://127.0.0.1:5173/`.

Nếu React đã chạy sẵn thì chỉ cần giải nén ghi đè frontend và tải lại trang (`Ctrl+Shift+R`).

## Các điểm tích hợp

- Vite có proxy `/api` -> `http://127.0.0.1:5080`.
- Trang chủ truy vấn `/api/catalog/featured?count=6`.
- Tìm kiếm trên header dẫn tới `/tim-kiem?q=...` và gọi API hiện có.
- Không chép đè thư mục `src/` Desktop hoặc `database/`.
- Không commit `bin/`, `obj/`, `node_modules/`, `dist/`.
