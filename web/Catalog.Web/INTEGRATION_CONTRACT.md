# Thống nhất tích hợp Web danh mục sản phẩm, giỏ hàng và quản trị

## Module danh mục sản phẩm cung cấp
- `GET /` trang chủ, chỉ đọc dữ liệu sản phẩm.
- `GET /san-pham?q=...&category=...&page=...` trang danh sách, tìm/lọc/phân trang.
- `GET /tim-kiem?q=...` kết quả tìm kiếm.
- `GET /san-pham/{id}` chi tiết linh kiện, ID không tồn tại trả HTTP 404.

## Trường dữ liệu tối thiểu
`ProductId`, `ProductName`, `CategoryName`, `BrandName`, `Price`, `AvailableQuantity`, `ImageUrl`, `Description`.
`AvailableQuantity` phải theo logic quản lý giữ hàng của nhóm; không được suy diễn `AvailableQuantity = tồn vật lý` nếu có đơn đang giữ.

## Hào cần cung cấp
- Service/API thêm sản phẩm vào giỏ với đầu vào **ProductId và Quantity**.
- Không dựa vào số lượng hiển thị ở Web để cam kết đặt hàng: phải kiểm tra lại trong transaction SQL khi ghi đơn.
- Trả lỗi nghiệp vụ khi hết hàng, số lượng không hợp lệ, sản phẩm ngừng kinh doanh, đơn hết hạn.

## Thảo cần cung cấp
- DDL và dữ liệu mẫu SQL Server đúng phiên bản của nhóm.
- Quy tắc số lượng tồn và trạng thái đơn được triển khai.
- Trường lưu ảnh và tên loại/nhà sản xuất trong DB.

## Khi tích hợp
Không thay đổi `HomeController`, `_Layout.cshtml`, cấu hình DI và route hiện có của cả nhóm mà chưa rà soát. Nếu repository đã có controller tương ứng, gộp từng action/view thay vì chép đè. Sau ghép cần chạy `dotnet build`, kiểm thử giao diện và các luồng đặt/hủy/duyệt xuất.
