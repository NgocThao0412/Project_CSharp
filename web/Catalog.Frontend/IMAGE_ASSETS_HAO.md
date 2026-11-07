# Thư viện ảnh linh kiện (Hào – giao diện khách hàng)

## Nguồn gốc và nguyên tắc

- 38 ảnh PNG được **sao chép từ `src/img_product/` trong chính source đồ án nhóm**; không tải lại từ trang bên ngoài.
- 8 ảnh SVG dạng **minh họa trung tính** cho sản phẩm chưa có ảnh đúng thương hiệu/model.
- Ảnh không khớp thương hiệu hoặc model sẽ không bị gán sai. Dữ liệu API có `imageUrl` hợp lệ được ưu tiên so với ảnh dự phòng.
- File ảnh tồn tại ở `web/Catalog.Frontend/public/images/products/` (Vite 5173) và `web/Catalog.Web/wwwroot/images/products/` (ASP.NET Core 5080).
- Bảng gán ảnh tương ứng nằm trong `src/data/product-image-index.json`; component dùng hàm `getProductImage` trong `src/utils/productImages.js`.
- Khi lấy thêm ảnh thật từ nguồn ngoài, hãy kiểm tra bản quyền/cho phép sử dụng và đúng model rồi thay trong file index.

## 39 sản phẩm từ script `database/quan_ly.sql` của bản source dùng để tạo gói

| Mã | Sản phẩm | File ảnh | Loại |
|---|---|---|---|
| 3 | Chuột Logitech G502 | `mouse_g502.png` | Ảnh sẵn có của nhóm |
| 4 | Bàn phím cơ Keychron | `keychron_k2.png` | Ảnh sẵn có của nhóm |
| 5 | Màn hình LG 27 inch | `minh-hoa-monitor.svg` | Minh họa |
| 15 | Màn hình Samsung Odyssey G5 | `samsung_g5.png` | Ảnh sẵn có của nhóm |
| 16 | Màn hình Dell Ultrasharp U2422 | `dell_u2422.png` | Ảnh sẵn có của nhóm |
| 17 | Màn hình Asus ProArt | `asus_proart.png` | Ảnh sẵn có của nhóm |
| 18 | Chuột Logitech MX Master 3S | `logitech_mx3.png` | Ảnh sẵn có của nhóm |
| 19 | Bàn phím Logitech K380 | `logi_k380.png` | Ảnh sẵn có của nhóm |
| 20 | Tai nghe Sony WH-1000XM5 | `sony_xm5.png` | Ảnh sẵn có của nhóm |
| 22 | Bàn phím cơ Keychron K8 Pro | `keychron_k8.png` | Ảnh sẵn có của nhóm |
| 23 | Lót chuột Razer Gigantus | `razer_pad.png` | Ảnh sẵn có của nhóm |
| 24 | RAM Kingston Fury 16GB | `kingston_ram16.png` | Ảnh sẵn có của nhóm |
| 25 | SSD Samsung 980 Pro 1TB | `ssd_980pro.png` | Ảnh sẵn có của nhóm |
| 26 | SSD Kingston NV2 500GB | `ssd_nv2.png` | Ảnh sẵn có của nhóm |
| 27 | CPU Intel Core i9-13900K | `intel_i9.png` | Ảnh sẵn có của nhóm |
| 28 | CPU Intel Core i5-12400F | `intel_i5.png` | Ảnh sẵn có của nhóm |
| 29 | Mainboard Asus ROG Z790 | `main_z790.png` | Ảnh sẵn có của nhóm |
| 30 | Router Wifi 6 TP-Link Archer | `tplink_ax73.png` | Ảnh sẵn có của nhóm |
| 31 | Switch TP-Link 8 Port | `tplink_switch.png` | Ảnh sẵn có của nhóm |
| 32 | Bộ phát Wifi Mesh Asus | `asus_mesh.png` | Ảnh sẵn có của nhóm |
| 33 | Dây cáp mạng CAT6 100m | `cable_cat6.png` | Ảnh sẵn có của nhóm |
| 1002 | Intel Core i3-14100F | `intel_i3.png` | Ảnh sẵn có của nhóm |
| 1003 | Intel Core i5-14400F | `intel_i5_14400F.png` | Ảnh sẵn có của nhóm |
| 1004 | Intel Core i5-14600KF | `intel_i5_14600KF.png` | Ảnh sẵn có của nhóm |
| 1005 | Intel Core i7-14700F | `intel_i7.png` | Ảnh sẵn có của nhóm |
| 1006 | Intel Core Ultra 5 245K | `intel_core.png` | Ảnh sẵn có của nhóm |
| 1007 | ASUS PRIME B760M-A WIFI D4 | `asus_prime.png` | Ảnh sẵn có của nhóm |
| 1008 | ASUS TUF GAMING B760M-PLUS WIFI | `asus_tuf.png` | Ảnh sẵn có của nhóm |
| 1009 | ASUS ROG STRIX B650-A GAMING WIFI | `asus_rog_b650.png` | Ảnh sẵn có của nhóm |
| 1010 | Kingston FURY Beast DDR4 16GB 3200MHz | `kingston_ddr4.png` | Ảnh sẵn có của nhóm |
| 1011 | Kingston FURY Beast DDR5 16GB 5600MHz | `kingston_ddr5.png` | Ảnh sẵn có của nhóm |
| 1012 | Kingston FURY Beast DDR5 32GB 6000MHz | `kingston_ddr5_32gb.png` | Ảnh sẵn có của nhóm |
| 1013 | Samsung 990 PRO SSD 1TB NVMe | `samsung_990pro_1tb.png` | Ảnh sẵn có của nhóm |
| 1014 | Samsung 990 PRO SSD 2TB NVMe | `samsung_990pro_2tb.png` | Ảnh sẵn có của nhóm |
| 1015 | Kingston NV3 SSD 1TB NVMe | `kingston_nv3_1tb.png` | Ảnh sẵn có của nhóm |
| 1016 | ASUS Dual GeForce RTX 4060 8GB | `asus_rtx4060.png` | Ảnh sẵn có của nhóm |
| 1017 | ASUS Dual GeForce RTX 4070 SUPER 12GB | `asus_rtx4070super.png` | Ảnh sẵn có của nhóm |
| 1018 | ASUS ROG STRIX 750G 750W | `asus_rog_750g.png` | Ảnh sẵn có của nhóm |
| 1019 | ASUS TUF GAMING GT301 Case | `asus_tuf_gt301.png` | Ảnh sẵn có của nhóm |

## 10 sản phẩm demo ASP.NET Core

| Mã | Sản phẩm | File ảnh | Loại |
|---|---|---|---|
| 1 | CPU Intel Core i5 | `intel_i5.png` | Ảnh sẵn có của nhóm |
| 2 | CPU AMD Ryzen 5 | `minh-hoa-cpu.svg` | Minh họa |
| 3 | RAM DDR4 16GB | `kingston_ram16.png` | Ảnh sẵn có của nhóm |
| 4 | RAM DDR5 32GB | `minh-hoa-ram.svg` | Minh họa |
| 5 | SSD NVMe 1TB | `samsung_990pro_1tb.png` | Ảnh sẵn có của nhóm |
| 6 | SSD SATA 512GB | `minh-hoa-ssd.svg` | Minh họa |
| 7 | Mainboard B760 | `asus_prime.png` | Ảnh sẵn có của nhóm |
| 8 | Card đồ họa RTX 4060 | `minh-hoa-vga.svg` | Minh họa |
| 9 | Nguồn máy tính 650W | `minh-hoa-psu.svg` | Minh họa |
| 10 | Vỏ case Mid Tower | `minh-hoa-case.svg` | Minh họa |

## Lưu ý khi cập nhật `main`

- Gói này được dựng từ `Project_CSharp-main.zip` người dùng đã gửi trong cuộc trò chuyện. Vì Ny đang chỉnh sửa Web, cần chạy `git apply --check` trên **nhánh Hào mới nhất** trước khi áp dụng; không ghi đè thủ công nếu patch báo lỗi.
- Không thay đổi schema SQL, lớp DAO, route của React, `Program.cs` hay source WinForms.
- Nguồn ảnh đã tìm kiếm trên web cho một số model còn thiếu được giữ ở dạng tham khảo, không tự động tải ảnh có bản quyền vào project.
