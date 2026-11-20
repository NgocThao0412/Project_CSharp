# Đồ án Xây dựng hệ thống quản lý và bán linh kiện máy tính - C# – NHÓM 03

## Thành viên nhóm

| STT | MSSV | Họ và tên |
| :-: | :--------: | -------------------------------------------------------------------------------------- |
| 1 | 3124411280 | [ LÊ THỊ NGỌC THẢO ](https://www.facebook.com/Ngthao.0412?locale=vi_VN)|
| 2 | 3124411208 | [ PHẠM THỊ BẢO NY ](https://www.facebook.com/tieeeeee1127?locale=vi_VN)|
| 3 | 3124411079 | [ NGUYỄN PHẠM ANH HÀO ](https://www.facebook.com/hao.nguyen.124845?locale=vi_VN)|

---

## Tài liệu liên quan

- **[Mã nguồn dự án trên GitHub](https://github.com/NgocThao0412/Project_CSharp)**
- **[Báo cáo đồ án C#](https://docs.google.com/document/d/1mSQYkmO7HZx96IuY9IXZEO2AytH8JedI7fuAPExuf54/edit?usp=sharing)**
- **[Cơ sở dữ liệu SQL Server](https://github.com/NgocThao0412/Project_CSharp/tree/main/database)**
- **Tài liệu thiết kế hệ thống:** [https://drive.google.com/drive/folders/1VIbYgi-XYQEfzR7sXD95G7WE1Yjw3CCB]
- **Slide thuyết trình:** [https://canva.link/9o2q9jhyxojqwc4]
---

## Hướng dẫn cài đặt và chạy ứng dụng

- **Tải ứng dụng:** Clone hoặc tải ZIP dự án từ GitHub.
- **Cơ sở dữ liệu:** Cài đặt SQL Server, mở SSMS và chạy file `database/quanlykho.sql` để tạo cơ sở dữ liệu `quanlykho` gồm 18 bảng.
- **Kết nối:** Kiểm tra và điều chỉnh chuỗi kết nối SQL Server trong `src/config` (Desktop) và `web/Catalog.Web/appsettings.json` (Backend) nếu cần.

**Chạy ứng dụng Desktop:**
```powershell
cd src
dotnet run
```

**Chạy Backend Web (cổng 5080):**
```powershell
cd web/Catalog.Web
dotnet run --urls "http://localhost:5080"
```

**Chạy Frontend Web (cổng 5173):**
```powershell
cd web/Catalog.Frontend
npm install
npm.cmd run dev
```

- Truy cập Website tại: **http://localhost:5173**
- Lưu ý: Cần khởi động SQL Server và Backend trước khi sử dụng Web.

### Tài khoản đăng nhập demo
+ Quản lý kho hàng: QL1 123456
+ Nhân viên xuất hàng: NV2 123456
+ Nhân viên nhập hàng và kiểm kê: NV3 123456
+ admin: admin 123456
+ Khách hàng: khachhang2 demo123456
