# SQL VIEW `dbo.vw_Catalog` cho Web React + C#

File goc `database/quan_ly.sql` da khai bao `CREATE VIEW dbo.vw_Catalog`, khong can tu viet bang/view thay the.

Web C# trong `web/Catalog.Web/DAO/SanPhamSqlDAO.cs` doc cac cot:
`ProductId, ProductName, CategoryName, BrandName, Price, AvailableQuantity, ImageUrl, Description`.

Nguon du lieu tu `dbo.SANPHAM`, `dbo.LOAISANPHAM`, `dbo.NHASANXUAT`; chi hien san pham `sp.TT = 1`. Anh duoc map ben React (thu muc `public/images/products`).

**Kiem tra trong SSMS:**

```sql
USE quanlykho;
SELECT TOP (10) * FROM dbo.vw_Catalog ORDER BY ProductId DESC;
```

Neu VIEW chua co, can chay/ra soat DDL cua nhom sau khi sao luu SQL Server; khong tu dong tao lai database khi chay Web.
