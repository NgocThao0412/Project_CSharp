# Ny - Gio hang & dat hang (React JSX + C# Web API + SQL Server)

## Pham vi thay doi

- **Frontend React**: giu nguyen TrangChu, SanPham va ChiTiet; bo sung DatHang.jsx, DatHangThanhCong.jsx; bat nut dat hang trong GioHang.jsx; cap nhat so luong gio hang tren header.
- **Backend C#**: DatHangApiController -> DatHangBUS -> DatHangDAO; gio hang duoc luu Session trong GioHangBUS.
- **Database**: su dung cac bang KHACHHANG, PHIEUXUAT, CTPHIEUXUAT va SANPHAM cua nhom; **them mot bang** DONHANGWEB de luu dia chi giao hang theo don va ma chong gui trung.
- **Desktop**: khong sua file Desktop. Don Web co TT = 2 (cho duyet). Desktop PhieuXuatDAO.DuyetPhieuXuat() kiem tra va tru ton kho khi duyet.
- **Phan Hao**: TaiKhoan.jsx, TraCuuDonHang.jsx, DonHangApiController.cs cu KHONG bi thay doi. Sau khi dat, nguoi mua co the tra cuu bang ma don va SDT.

## 1. Cai bo sung SQL bat buoc (mot lan)

1. Mo SSMS, ket noi SQL Server instance dang co database `quanlykho`.
2. Sao luu DB neu la du lieu chung cua nhom.
3. Mo `database/02_WEB_DAT_HANG.sql` va chay `Execute`. File nay khong DROP bang va co kiem tra ton tai truoc khi tao.
4. Kiem tra: `USE quanlykho; SELECT TOP 5 * FROM dbo.DONHANGWEB;` (ban dau co the 0 dong).

**KHONG CHAY LAI `database/quan_ly.sql` tren database da co du lieu.**

## 2. Cau hinh SQL rieng cho tung may (khong commit ten may)

Trong Terminal PowerShell chay backend, co the dung:

```powershell
cd C:\Users\LENOVO\Videos\Project_CSharp
$env:ConnectionStrings__DefaultConnection = "Server=.\SQLEXPRESS;Database=quanlykho;Integrated Security=True;TrustServerCertificate=True;"
$env:WebOrders__StaffId = "1"
dotnet run --no-launch-profile --project web\Catalog.Web\Catalog.Web.csproj --urls http://127.0.0.1:5080
```

`WebOrders:StaffId` la ma nhan vien tiep nhan phiếu Web, **phai ton tai** trong NHANVIEN (SQL demo co MNV=1); co the chinh theo may/nhom. Cot MNV duoc ghi so cu the de Desktop hien tai (GetInt32) khong bi loi NULL. Nhom nen thong nhat tai khoan nhan vien tiep nhan don Web ve sau.

Terminal 2:

```powershell
cd web\Catalog.Frontend
npm.cmd ci
npm.cmd run dev
```

Mo `http://localhost:5173/`.

## 3. Demo dung luong

1. Vao `Sản phẩm` -> them san pham con hang, hoac chi tiet -> `Thêm vào giỏ hàng`.
2. Mo `Giỏ hàng`: xem gia, anh, so luong, sua so luong/xoa.
3. Chon `Tiến hành đặt hàng`.
4. Nhap ho ten, so dien thoai 10 chu so bat dau 0, dia chi, email/ghi chu (khong bat buoc).
5. Chon `Xác nhận đặt hàng` -> C# kiem tra lai gia va so luong ton kho, sau do luu PHIEUXUAT TT=2 + CTPHIEUXUAT + DONHANGWEB trong mot SQL transaction.
6. Trang thanh cong hien ma don. Vao `Tra cứu đơn` va nhap ma don + SDT.
7. Desktop nhan vien thay phiếu TT=2 va co the duyet; Desktop truc tiep tru kho khi duyet, Web khong tru ton khi dat.

SQL kiem tra sau khi dat:

```sql
USE quanlykho;
SELECT TOP 10 px.MPX, kh.HOTEN, px.TIEN, px.TT,
       web.DIACHINHAN, web.SDTNHAN, web.PHUONGTHUCTT
FROM dbo.PHIEUXUAT px
JOIN dbo.KHACHHANG kh ON kh.MKH = px.MKH
JOIN dbo.DONHANGWEB web ON web.MPX = px.MPX
ORDER BY px.MPX DESC;

SELECT TOP 20 ct.MPX, sp.TEN, ct.SL, ct.TIENXUAT
FROM dbo.CTPHIEUXUAT ct
JOIN dbo.SANPHAM sp ON sp.MSP = ct.MSP
WHERE ct.MPX IN (SELECT MPX FROM dbo.DONHANGWEB)
ORDER BY ct.MPX DESC;
```

## 4. Gioi han va luu y tich hop

- Khach co the dat khong dang nhap vi module tai khoan cua Hao chua co xac thuc that. Phuong thuc thanh toan hien tai chi la COD, KHONG ket noi cong thanh toan that.
- Gia va ton kho duoc tinh lai tren may chu; React khong the tu gan gia. Neu gia thay doi, he thong tu choi dat de nguoi dung kiem tra lai.
- Server ghi phieu **cho duyet**. Don Web khong tru kho, Desktop se tru ton khi duyet. Khi nhieu don cho duyet, C# kiem tra them so luong da duoc ghi trong phieu TT=2. Desktop van quyet dinh xuat kho cuoi cung.
- Bang DONHANGWEB luu thong tin giao hang theo don, khong thay doi KHACHHANG cu neu SDT da ton tai. Email khach cu duoc giu nguyen.
- Can chay va test tren SQL Server that cua nhom. Trong moi truong tao ZIP chua co .NET SDK/SQL Server de test ket noi end-to-end.
- De bao mat va bao tri: can cau hinh CORS, HTTPS, rate limiting/chong spam, xac thuc tai khoan va xac minh SDT truoc khi dua Web len Internet. Hien tai phu hop demo trong phong lab, khong phai website ban hang production.
