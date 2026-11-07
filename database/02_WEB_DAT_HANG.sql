-- BO SUNG CHO WEB DAT HANG - CHI CHAY 1 LAN TREN CSDL QUANLYKHO DA CO.
-- KHONG CHAY LAI quan_ly.sql. Khong xoa/ghi de bang goc cua nhom.
-- Nen BACKUP DATABASE truoc khi chay tren du lieu chung.
USE [quanlykho];
GO
IF OBJECT_ID(N'dbo.PHIEUXUAT', N'U') IS NULL OR OBJECT_ID(N'dbo.KHACHHANG', N'U') IS NULL
    THROW 51010, N'CSDL chua co PHIEUXUAT/KHACHHANG. Can khoi tao SQL goc truoc.', 1;
GO
IF OBJECT_ID(N'dbo.DONHANGWEB', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.DONHANGWEB
    (
        MPX            int              NOT NULL PRIMARY KEY,
        MAYEUCAU       uniqueidentifier NOT NULL,
        HOTENNHAN      nvarchar(100)    NOT NULL,
        SDTNHAN        nvarchar(11)     NOT NULL,
        DIACHINHAN     nvarchar(255)    NOT NULL,
        GHICHU         nvarchar(500)    NULL,
        PHUONGTHUCTT   varchar(20)      NOT NULL CONSTRAINT DF_DONHANGWEB_PTTT DEFAULT ('COD'),
        NGAYTAO        datetime2(0)     NOT NULL CONSTRAINT DF_DONHANGWEB_NGAYTAO DEFAULT (SYSDATETIME()),
        CONSTRAINT FK_DONHANGWEB_PHIEUXUAT FOREIGN KEY (MPX) REFERENCES dbo.PHIEUXUAT(MPX),
        CONSTRAINT UQ_DONHANGWEB_MAYEUCAU UNIQUE (MAYEUCAU),
        CONSTRAINT CK_DONHANGWEB_PTTT CHECK (PHUONGTHUCTT = 'COD')
    );
END;
GO
-- Trang thai don van o dbo.PHIEUXUAT: TT=2 cho duyet; TT=1 da duyet; TT=0 da huy.
-- Khi Dat hang: KHONG tru SANPHAM.SL. Desktop tru ton khi DuyetPhieuXuat.
SELECT N'San sang nhan don Web' AS KetQua, COUNT(*) AS SoDonWeb FROM dbo.DONHANGWEB;
GO
