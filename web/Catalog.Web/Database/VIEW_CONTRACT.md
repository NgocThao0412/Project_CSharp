# Hợp đồng dữ liệu SQL Server của module danh mục sản phẩm

Để tránh đoán sai tên cột của repository hiện tại, `SqlProductRepository` chỉ đọc từ `dbo.vw_Catalog`.
DBA/Thảo cần tạo một VIEW đọc (không sửa dữ liệu) với các cột chính xác:

| Tên cột | Kiểu | Ý nghĩa |
|---|---|---|
| ProductId | int, NOT NULL | ID sản phẩm ổn định |
| ProductName | nvarchar | Tên linh kiện |
| CategoryName | nvarchar | Danh mục |
| BrandName | nvarchar | Thương hiệu; dùng chuỗi rỗng nếu thiếu |
| Price | decimal(18,2) | Giá bán không âm |
| AvailableQuantity | int | MAX(0, tồn thực tế - tổng đang giữ) |
| ImageUrl | nvarchar NULL | Đường dẫn ảnh, có thể NULL |
| Description | nvarchar | Mô tả, chuỗi rỗng nếu thiếu |

Cần xác minh bảng/khóa thực tế trước khi tạo VIEW. Đây là mẫu logic, KHÔNG PHẢI câu SQL có thể chạy ngay:

```sql
CREATE OR ALTER VIEW dbo.vw_Catalog AS
SELECT
  sp.MSP AS ProductId,
  sp.TENSP AS ProductName,     -- thay theo DDL thực tế
  lo.TENLOAI AS CategoryName,  -- thay theo DDL thực tế
  N'' AS BrandName,
  CAST(sp.GIABAN AS decimal(18,2)) AS Price,
  CAST(sp.SOLUONGTON - COALESCE(giu.SoLuongGiu, 0) AS int) AS AvailableQuantity,
  CAST(NULL AS nvarchar(500)) AS ImageUrl,
  CAST(N'' AS nvarchar(max)) AS Description
FROM dbo.SANPHAM sp
JOIN dbo.LOAISANPHAM lo ON lo.MLSP = sp.MLSP
OUTER APPLY (SELECT SUM(ct.SOLUONG) AS SoLuongGiu
    FROM dbo.CTDONHANGWEB ct
    JOIN dbo.DONHANGWEB dh ON dh.MDHW = ct.MDHW
    WHERE ct.MSP = sp.MSP AND dh.TRANGTHAI = N'DANG_GIU' AND dh.HANGIU > SYSUTCDATETIME()) giu;
```

Mã trên chỉ thể hiện quy tắc dữ liệu; tên bảng/khóa/cột và múi giờ phải đối chiếu SQL thực tế. Không chạy nguyên mẫu vào cơ sở dữ liệu của nhóm. Điều kiện giữ hàng phải đồng bộ với quy tắc ở phần Hào và Desktop.
