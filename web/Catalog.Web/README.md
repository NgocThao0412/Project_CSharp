# Web danh mục sản phẩm - ASP.NET Core MVC 10

## Phạm vi
- Trang chủ `/` có giới thiệu và sản phẩm nổi bật
- Danh sách `/san-pham`, tìm theo tên/thương hiệu, lọc danh mục, phân trang
- Kết quả tìm kiếm `/tim-kiem?q=ram`
- Trang chi tiết `/san-pham/{id}`, thông báo còn hàng/hết hàng
- Responsive desktop/mobile. Không tạo chức năng giỏ hàng hoặc đặt hàng của Hào.

## Chạy thử giao diện độc lập
1. Cài .NET SDK 10.
2. Tại thư mục chứa `Catalog.Web.csproj`, chạy `dotnet restore`, `dotnet run`.
3. Mở URL được in ra. `Catalog:UseDemoData = true` nên ứng dụng dùng 10 sản phẩm minh họa; **đây không phải dữ liệu SQL thực tế**.

## Kết nối SQL Server của nhóm
1. Kiểm tra DDL/tên cột hiện tại; định nghĩa `dbo.vw_Catalog` theo `Database/VIEW_CONTRACT.md`. Đừng chạy ví dụ SQL chưa chỉnh sửa.
2. Cấu hình `ConnectionStrings:DefaultConnection` qua user-secrets hoặc biến môi trường (tránh đẩy mật khẩu lên GitHub).
3. Đổi `Catalog:UseDemoData = false`.
4. Xác nhận công thức tồn khả dụng giống Thảo + Hào: tồn vật lý - lượng đang giữ hợp lệ, không trừ khi chỉ xem.
5. Chạy và thử tìm/lọc/xem chi tiết. Nếu thiếu VIEW hoặc sai mapping, phần SQL sẽ báo lỗi thay vì bịa sản phẩm.

## Khi ghép vào Project_CSharp
Đây là **mã nguồn độc lập**, chưa chèn vào repository vì chưa đọc được code trên GitHub. Không copy đè `Program.cs`, `HomeController` hay `_Layout.cshtml` của cả nhóm. Nên chuyển `ProductsController`, `Models`, `Services`, `Views/Products` và CSS vào project Web hiện có rồi kết nối DI và menu bằng cách chỉnh các điểm cần thiết. Kiểm tra trùng route/controller trước khi copy. Phần Hào sẽ tự kết nối nút thêm giỏ hàng qua service/API riêng sau khi thống nhất hợp đồng với module danh mục sản phẩm.

## Checklist tích hợp
- [ ] Xác nhận project Web đang dùng ASP.NET Core MVC 10
- [ ] Đối chiếu tên model/route đã tồn tại
- [ ] Ghép giao diện theo layout của nhóm (không thay layout chung)
- [ ] Tạo VIEW SQL từ DDL đã kiểm chứng
- [ ] Đảm bảo chỉ dùng hàng khả dụng trong truy vấn
- [ ] Test tìm tiếng Việt, lọc danh mục, không có kết quả, tồn 0, product không tồn tại
- [ ] Test đồng bộ khi Hào giữ/hủy đơn, Thảo duyệt xuất

## Giới hạn kiểm chứng
Môi trường tạo gói không có .NET SDK và không truy cập được mã nguồn GitHub, nên chưa chạy `dotnet build`, chưa chạy SQL Server và chưa thể cam kết tương thích với repository thật. Đây là module nguồn tham khảo để tích hợp và kiểm thử, không phải patch đã merge.
