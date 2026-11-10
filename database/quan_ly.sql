-- ============================================================================
-- QUAN LY VA BAN LINH KIEN MAY TINH | SQL Server
-- File duy nhat: database/quan_ly.sql
-- Muc dich: Khoi tao CSDL MOI tren SQL Server, co du lieu DEMO, rang buoc va View.
-- Khong chay nguyen file tren CSDL quanlykho dang co du lieu!
-- Sao luu database truoc khi thao tac. Script khong DROP database/bang.
-- Chay bang SSMS / sqlcmd (ho tro batch GO). Dung SQL Server co quyen CREATE DATABASE.
-- Du lieu nhan vien/khach hang la gia lap, khong phai thong tin ca nhan that.
-- TK demo giu ma bam goc de khong mat chuc nang dang nhap; BAT BUOC doi mat khau khi trien khai.
-- CANH BAO: Goi script nay tren instance SQL Server MOI, KHONG chay lai tren quanlykho da ton tai.
-- ============================================================================

USE [master];
GO
IF DB_ID(N'quanlykho') IS NOT NULL
    THROW 51000, N'Database quanlykho da ton tai. Chi chay script tren instance SQL Server sach.', 1;
GO
CREATE DATABASE [quanlykho];
GO
USE [quanlykho];
GO

-- 1. BANG VA KHOA CHINH (18 BANG)
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CTPHIEUKIEMKE](
	[MPKK] [int] NOT NULL,
	[MSP] [int] NOT NULL,
	[TRANGTHAISP] [int] NOT NULL,
	[GHICHU] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[MPKK] ASC,
	[MSP] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CTPHIEUNHAP](
	[MPN] [int] NOT NULL,
	[MSP] [int] NOT NULL,
	[SL] [int] NOT NULL,
	[TIENNHAP] [int] NOT NULL,
	[HINHTHUC] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MPN] ASC,
	[MSP] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CTPHIEUXUAT](
	[MPX] [int] NOT NULL,
	[MSP] [int] NOT NULL,
	[SL] [int] NOT NULL,
	[TIENXUAT] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MPX] ASC,
	[MSP] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CTQUYEN](
	[MNQ] [int] NOT NULL,
	[MCN] [nvarchar](50) NOT NULL,
	[HANHDONG] [nvarchar](255) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MNQ] ASC,
	[MCN] ASC,
	[HANHDONG] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DANHMUCCHUCNANG](
	[MCN] [nvarchar](50) NOT NULL,
	[TEN] [nvarchar](255) NOT NULL,
	[TT] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MCN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KHACHHANG](
	[MKH] [int] IDENTITY(1,1) NOT NULL,
	[HOTEN] [nvarchar](255) NOT NULL,
	[NGAYTHAMGIA] [date] NOT NULL,
	[DIACHI] [nvarchar](255) NULL,
	[SDT] [nvarchar](11) NOT NULL,
	[EMAIL] [nvarchar](50) NULL,
	[TENDANGNHAP] [nvarchar](50) NULL,
	[MATKHAU] [nvarchar](255) NULL,
	[TINHTHANHPHO] [nvarchar](100) NULL,
	[PHUONGXA] [nvarchar](100) NULL,
	[TT] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MKH] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;
GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_KHACHHANG_TENDANGNHAP]
ON [dbo].[KHACHHANG] ([TENDANGNHAP])
WHERE [TENDANGNHAP] IS NOT NULL;
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[KHUVUCKHO](
	[MKVK] [int] IDENTITY(1,1) NOT NULL,
	[TEN] [nvarchar](255) NOT NULL,
	[GHICHU] [nvarchar](255) NULL,
	[TT] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MKVK] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LOAISANPHAM](
	[MLSP] [int] IDENTITY(1,1) NOT NULL,
	[TEN] [nvarchar](255) NOT NULL,
	[TLGX] [int] NOT NULL,
	[GHICHU] [nvarchar](255) NULL,
	[TT] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MLSP] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NHACUNGCAP](
	[MNCC] [int] IDENTITY(1,1) NOT NULL,
	[TEN] [nvarchar](255) NOT NULL,
	[DIACHI] [nvarchar](255) NULL,
	[SDT] [nvarchar](12) NULL,
	[EMAIL] [nvarchar](50) NULL,
	[TT] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MNCC] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NHANVIEN](
	[MNV] [int] IDENTITY(1,1) NOT NULL,
	[HOTEN] [nvarchar](255) NOT NULL,
	[GIOITINH] [int] NOT NULL,
	[NGAYSINH] [date] NOT NULL,
	[SDT] [nvarchar](11) NOT NULL,
	[EMAIL] [nvarchar](50) NOT NULL,
	[TT] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MNV] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NHASANXUAT](
	[MNSX] [int] IDENTITY(1,1) NOT NULL,
	[TEN] [nvarchar](255) NOT NULL,
	[DIACHI] [nvarchar](255) NULL,
	[SDT] [nvarchar](12) NULL,
	[EMAIL] [nvarchar](50) NULL,
	[TT] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MNSX] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[NHOMQUYEN](
	[MNQ] [int] IDENTITY(1,1) NOT NULL,
	[TEN] [nvarchar](255) NOT NULL,
	[TT] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MNQ] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PHIEUKIEMKE](
	[MPKK] [int] IDENTITY(1,1) NOT NULL,
	[MNV] [int] NOT NULL,
	[TG] [datetime] NULL,
	[TT] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MPKK] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PHIEUNHAP](
	[MPN] [int] IDENTITY(1,1) NOT NULL,
	[MNV] [int] NOT NULL,
	[MNCC] [int] NOT NULL,
	[TIEN] [int] NOT NULL,
	[TG] [datetime] NULL,
	[TT] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MPN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PHIEUXUAT](
	[MPX] [int] IDENTITY(1,1) NOT NULL,
	[MNV] [int] NULL,
	[MKH] [int] NOT NULL,
	[TIEN] [int] NOT NULL,
	[TG] [datetime] NULL,
	[TT] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MPX] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
CREATE TABLE [dbo].[DONHANGWEB](
    [MPX] int NOT NULL,
    [MAYEUCAU] uniqueidentifier NOT NULL,
    [HOTENNHAN] nvarchar(100) NOT NULL,
    [SDTNHAN] nvarchar(11) NOT NULL,
    [DIACHINHAN] nvarchar(255) NOT NULL,
    [GHICHU] nvarchar(500) NULL,
    [PHUONGTHUCTT] varchar(20) NOT NULL CONSTRAINT [DF_DONHANGWEB_PTTT] DEFAULT ('COD'),
    [NGAYTAO] datetime2(0) NOT NULL CONSTRAINT [DF_DONHANGWEB_NGAYTAO] DEFAULT (SYSDATETIME()),
    CONSTRAINT [PK_DONHANGWEB] PRIMARY KEY ([MPX]),
    CONSTRAINT [FK_DONHANGWEB_PHIEUXUAT] FOREIGN KEY ([MPX]) REFERENCES [dbo].[PHIEUXUAT]([MPX]),
    CONSTRAINT [UQ_DONHANGWEB_MAYEUCAU] UNIQUE ([MAYEUCAU]),
    CONSTRAINT [CK_DONHANGWEB_PTTT] CHECK ([PHUONGTHUCTT] = 'COD')
);
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SANPHAM](
	[MSP] [int] IDENTITY(1,1) NOT NULL,
	[TEN] [nvarchar](255) NOT NULL,
	[HINHANH] [nvarchar](255) NULL,
	[DANHMUC] [nvarchar](255) NULL,
	[MNSX] [int] NOT NULL,
	[MKVK] [int] NOT NULL,
	[MLSP] [int] NOT NULL,
	[TIENX] [int] NOT NULL,
	[TIENN] [int] NOT NULL,
	[SL] [int] NOT NULL,
	[TT] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MSP] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TAIKHOAN](
	[MNV] [int] NOT NULL,
	[MK] [nvarchar](255) NOT NULL,
	[TDN] [nvarchar](255) NOT NULL,
	[MNQ] [int] NOT NULL,
	[TT] [int] NOT NULL,
	[OTP] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[MNV] ASC,
	[TDN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
-- 2. DU LIEU MAU
INSERT [dbo].[CTPHIEUKIEMKE] ([MPKK], [MSP], [TRANGTHAISP], [GHICHU]) VALUES (2, 3, 1, N'Đang kiểm đếm, tình trạng tốt')
INSERT [dbo].[CTPHIEUKIEMKE] ([MPKK], [MSP], [TRANGTHAISP], [GHICHU]) VALUES (2, 4, 1, N'Mới nhập kho hôm qua')
INSERT [dbo].[CTPHIEUKIEMKE] ([MPKK], [MSP], [TRANGTHAISP], [GHICHU]) VALUES (2, 18, 0, N'Phát hiện thiếu dây cáp đi kèm')
INSERT [dbo].[CTPHIEUKIEMKE] ([MPKK], [MSP], [TRANGTHAISP], [GHICHU]) VALUES (3, 5, 1, N'Nhân viên kiểm đếm sai quy trình')
INSERT [dbo].[CTPHIEUKIEMKE] ([MPKK], [MSP], [TRANGTHAISP], [GHICHU]) VALUES (5, 3, 0, N'Hộp bị rách seal 2 cái')
INSERT [dbo].[CTPHIEUKIEMKE] ([MPKK], [MSP], [TRANGTHAISP], [GHICHU]) VALUES (5, 4, 1, N'Đủ số lượng')
GO
INSERT [dbo].[CTPHIEUNHAP] ([MPN], [MSP], [SL], [TIENNHAP], [HINHTHUC]) VALUES (1, 3, 2, 900000, 0)
INSERT [dbo].[CTPHIEUNHAP] ([MPN], [MSP], [SL], [TIENNHAP], [HINHTHUC]) VALUES (1, 4, 1, 1400000, 0)
INSERT [dbo].[CTPHIEUNHAP] ([MPN], [MSP], [SL], [TIENNHAP], [HINHTHUC]) VALUES (2, 5, 2, 4800000, 0)
INSERT [dbo].[CTPHIEUNHAP] ([MPN], [MSP], [SL], [TIENNHAP], [HINHTHUC]) VALUES (5, 3, 50, 900000, 0)
INSERT [dbo].[CTPHIEUNHAP] ([MPN], [MSP], [SL], [TIENNHAP], [HINHTHUC]) VALUES (5, 4, 20, 1400000, 0)
INSERT [dbo].[CTPHIEUNHAP] ([MPN], [MSP], [SL], [TIENNHAP], [HINHTHUC]) VALUES (7, 4, 10, 3000000, 0)
INSERT [dbo].[CTPHIEUNHAP] ([MPN], [MSP], [SL], [TIENNHAP], [HINHTHUC]) VALUES (8, 3, 40, 900000, 0)
INSERT [dbo].[CTPHIEUNHAP] ([MPN], [MSP], [SL], [TIENNHAP], [HINHTHUC]) VALUES (8, 4, 20, 500000, 0)
INSERT [dbo].[CTPHIEUNHAP] ([MPN], [MSP], [SL], [TIENNHAP], [HINHTHUC]) VALUES (9, 5, 20, 4800000, 0)
INSERT [dbo].[CTPHIEUNHAP] ([MPN], [MSP], [SL], [TIENNHAP], [HINHTHUC]) VALUES (11, 3, 50, 15000000, 0)
INSERT [dbo].[CTPHIEUNHAP] ([MPN], [MSP], [SL], [TIENNHAP], [HINHTHUC]) VALUES (11, 4, 20, 10000000, 0)
GO
INSERT [dbo].[CTPHIEUXUAT] ([MPX], [MSP], [SL], [TIENXUAT]) VALUES (1, 3, 2, 1200000)
INSERT [dbo].[CTPHIEUXUAT] ([MPX], [MSP], [SL], [TIENXUAT]) VALUES (1, 4, 1, 1800000)
INSERT [dbo].[CTPHIEUXUAT] ([MPX], [MSP], [SL], [TIENXUAT]) VALUES (2, 4, 2, 1800000)
INSERT [dbo].[CTPHIEUXUAT] ([MPX], [MSP], [SL], [TIENXUAT]) VALUES (2, 5, 1, 5500000)
INSERT [dbo].[CTPHIEUXUAT] ([MPX], [MSP], [SL], [TIENXUAT]) VALUES (3, 3, 1, 1200000)
INSERT [dbo].[CTPHIEUXUAT] ([MPX], [MSP], [SL], [TIENXUAT]) VALUES (5, 4, 1, 1800000)
INSERT [dbo].[CTPHIEUXUAT] ([MPX], [MSP], [SL], [TIENXUAT]) VALUES (5, 5, 1, 5500000)
INSERT [dbo].[CTPHIEUXUAT] ([MPX], [MSP], [SL], [TIENXUAT]) VALUES (6, 3, 1, 1200000)
INSERT [dbo].[CTPHIEUXUAT] ([MPX], [MSP], [SL], [TIENXUAT]) VALUES (8, 3, 5, 1200000)
INSERT [dbo].[CTPHIEUXUAT] ([MPX], [MSP], [SL], [TIENXUAT]) VALUES (8, 4, 5, 1800000)
INSERT [dbo].[CTPHIEUXUAT] ([MPX], [MSP], [SL], [TIENXUAT]) VALUES (8, 5, 5, 5500000)
INSERT [dbo].[CTPHIEUXUAT] ([MPX], [MSP], [SL], [TIENXUAT]) VALUES (9, 4, 1, 1800000)
INSERT [dbo].[CTPHIEUXUAT] ([MPX], [MSP], [SL], [TIENXUAT]) VALUES (11, 3, 1, 1000000)
INSERT [dbo].[CTPHIEUXUAT] ([MPX], [MSP], [SL], [TIENXUAT]) VALUES (11, 4, 1, 500000)
INSERT [dbo].[CTPHIEUXUAT] ([MPX], [MSP], [SL], [TIENXUAT]) VALUES (13, 5, 1, 5500000)
INSERT [dbo].[CTPHIEUXUAT] ([MPX], [MSP], [SL], [TIENXUAT]) VALUES (15, 3, 1, 1200000)
INSERT [dbo].[CTPHIEUXUAT] ([MPX], [MSP], [SL], [TIENXUAT]) VALUES (16, 4, 1, 12000000)
INSERT [dbo].[CTPHIEUXUAT] ([MPX], [MSP], [SL], [TIENXUAT]) VALUES (16, 5, 1, 5500000)
INSERT [dbo].[CTPHIEUXUAT] ([MPX], [MSP], [SL], [TIENXUAT]) VALUES (17, 3, 2, 1200000)
GO
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'khachhang', N'create')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'khachhang', N'delete')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'khachhang', N'update')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'khachhang', N'view')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'khuvuckho', N'create')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'khuvuckho', N'delete')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'khuvuckho', N'update')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'khuvuckho', N'view')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'kiemke', N'approve')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'kiemke', N'create')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'kiemke', N'delete')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'kiemke', N'update')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'kiemke', N'view')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'loaisanpham', N'create')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'loaisanpham', N'delete')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'loaisanpham', N'update')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'loaisanpham', N'view')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'nhacungcap', N'create')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'nhacungcap', N'delete')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'nhacungcap', N'update')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'nhacungcap', N'view')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'nhanvien', N'create')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'nhanvien', N'delete')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'nhanvien', N'update')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'nhanvien', N'view')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'nhaphang', N'approve')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'nhaphang', N'create')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'nhaphang', N'delete')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'nhaphang', N'update')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'nhaphang', N'view')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'nhasanxuat', N'create')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'nhasanxuat', N'delete')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'nhasanxuat', N'update')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'nhasanxuat', N'view')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'sanpham', N'create')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'sanpham', N'delete')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'sanpham', N'update')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'sanpham', N'view')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'thongke', N'create')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'thongke', N'delete')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'thongke', N'update')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'thongke', N'view')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'xuathang', N'approve')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'xuathang', N'create')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'xuathang', N'delete')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'xuathang', N'update')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (1, N'xuathang', N'view')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (2, N'khachhang', N'create')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (2, N'khachhang', N'delete')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (2, N'khachhang', N'update')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (2, N'khachhang', N'view')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (2, N'loaisanpham', N'view')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (2, N'nhasanxuat', N'view')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (2, N'sanpham', N'view')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (2, N'xuathang', N'create')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (2, N'xuathang', N'update')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (2, N'xuathang', N'view')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (3, N'kiemke', N'create')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (3, N'kiemke', N'delete')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (3, N'kiemke', N'update')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (3, N'kiemke', N'view')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (3, N'loaisanpham', N'create')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (3, N'loaisanpham', N'delete')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (3, N'loaisanpham', N'update')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (3, N'loaisanpham', N'view')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (3, N'nhacungcap', N'create')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (3, N'nhacungcap', N'delete')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (3, N'nhacungcap', N'update')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (3, N'nhacungcap', N'view')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (3, N'nhaphang', N'create')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (3, N'nhaphang', N'delete')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (3, N'nhaphang', N'update')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (3, N'nhaphang', N'view')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (3, N'nhasanxuat', N'create')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (3, N'nhasanxuat', N'delete')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (3, N'nhasanxuat', N'update')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (3, N'nhasanxuat', N'view')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (3, N'sanpham', N'create')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (3, N'sanpham', N'delete')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (3, N'sanpham', N'update')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (3, N'sanpham', N'view')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (4, N'nhomquyen', N'create')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (4, N'nhomquyen', N'delete')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (4, N'nhomquyen', N'update')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (4, N'nhomquyen', N'view')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (4, N'taikhoan', N'create')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (4, N'taikhoan', N'delete')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (4, N'taikhoan', N'update')
INSERT [dbo].[CTQUYEN] ([MNQ], [MCN], [HANHDONG]) VALUES (4, N'taikhoan', N'view')
GO
INSERT [dbo].[DANHMUCCHUCNANG] ([MCN], [TEN], [TT]) VALUES (N'khachhang', N'Quản lý khách hàng', 1)
INSERT [dbo].[DANHMUCCHUCNANG] ([MCN], [TEN], [TT]) VALUES (N'khuvuckho', N'Quản lý khu vực kho', 1)
INSERT [dbo].[DANHMUCCHUCNANG] ([MCN], [TEN], [TT]) VALUES (N'kiemke', N'Quản lý kiểm kê', 1)
INSERT [dbo].[DANHMUCCHUCNANG] ([MCN], [TEN], [TT]) VALUES (N'loaisanpham', N'Quản lý loại sản phẩm', 1)
INSERT [dbo].[DANHMUCCHUCNANG] ([MCN], [TEN], [TT]) VALUES (N'nhacungcap', N'Quản lý nhà cung cấp', 1)
INSERT [dbo].[DANHMUCCHUCNANG] ([MCN], [TEN], [TT]) VALUES (N'nhanvien', N'Quản lý nhân viên', 1)
INSERT [dbo].[DANHMUCCHUCNANG] ([MCN], [TEN], [TT]) VALUES (N'nhaphang', N'Quản lý nhập hàng', 1)
INSERT [dbo].[DANHMUCCHUCNANG] ([MCN], [TEN], [TT]) VALUES (N'nhasanxuat', N'Quản lý nhà sản xuất', 1)
INSERT [dbo].[DANHMUCCHUCNANG] ([MCN], [TEN], [TT]) VALUES (N'nhomquyen', N'Quản lý nhóm quyền', 1)
INSERT [dbo].[DANHMUCCHUCNANG] ([MCN], [TEN], [TT]) VALUES (N'sanpham', N'Quản lý sản phẩm', 1)
INSERT [dbo].[DANHMUCCHUCNANG] ([MCN], [TEN], [TT]) VALUES (N'taikhoan', N'Quản lý tài khoản', 1)
INSERT [dbo].[DANHMUCCHUCNANG] ([MCN], [TEN], [TT]) VALUES (N'thongke', N'Quản lý thống kê', 1)
INSERT [dbo].[DANHMUCCHUCNANG] ([MCN], [TEN], [TT]) VALUES (N'xuathang', N'Quản lý xuất hàng', 1)
GO
SET IDENTITY_INSERT [dbo].[KHACHHANG] ON 

INSERT [dbo].[KHACHHANG] ([MKH], [HOTEN], [NGAYTHAMGIA], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (1, N'Khach hang mau 01', CAST(N'2024-04-15' AS Date), N'Dia chi mau 01', N'0900000001', NULL, 1)
INSERT [dbo].[KHACHHANG] ([MKH], [HOTEN], [NGAYTHAMGIA], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (2, N'Khach hang mau 02', CAST(N'2024-04-15' AS Date), N'Dia chi mau 02', N'0900000002', NULL, 1)
INSERT [dbo].[KHACHHANG] ([MKH], [HOTEN], [NGAYTHAMGIA], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (3, N'Khach hang mau 03', CAST(N'2024-04-15' AS Date), N'Dia chi mau 03', N'0900000003', NULL, 1)
INSERT [dbo].[KHACHHANG] ([MKH], [HOTEN], [NGAYTHAMGIA], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (4, N'Khach hang mau 04', CAST(N'2024-04-15' AS Date), N'Dia chi mau 04', N'0900000004', NULL, 1)
INSERT [dbo].[KHACHHANG] ([MKH], [HOTEN], [NGAYTHAMGIA], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (5, N'Khach hang mau 05', CAST(N'2024-04-16' AS Date), N'Dia chi mau 05', N'0900000005', NULL, 1)
INSERT [dbo].[KHACHHANG] ([MKH], [HOTEN], [NGAYTHAMGIA], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (6, N'Khach hang mau 06', CAST(N'2024-04-16' AS Date), N'Dia chi mau 06', N'0900000006', NULL, 1)
INSERT [dbo].[KHACHHANG] ([MKH], [HOTEN], [NGAYTHAMGIA], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (7, N'Khach hang mau 07', CAST(N'2024-04-16' AS Date), N'Dia chi mau 07', N'0900000007', NULL, 1)
INSERT [dbo].[KHACHHANG] ([MKH], [HOTEN], [NGAYTHAMGIA], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (8, N'Khach hang mau 08', CAST(N'2024-04-16' AS Date), N'Dia chi mau 08', N'0900000008', NULL, 1)
INSERT [dbo].[KHACHHANG] ([MKH], [HOTEN], [NGAYTHAMGIA], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (9, N'Khach hang mau 09', CAST(N'2024-04-16' AS Date), N'Dia chi mau 09', N'0900000009', NULL, 1)
INSERT [dbo].[KHACHHANG] ([MKH], [HOTEN], [NGAYTHAMGIA], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (10, N'Khach hang mau 10', CAST(N'2024-04-16' AS Date), N'Dia chi mau 10', N'0900000010', NULL, 1)
INSERT [dbo].[KHACHHANG] ([MKH], [HOTEN], [NGAYTHAMGIA], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (11, N'Khach hang mau 11', CAST(N'2024-04-16' AS Date), N'Dia chi mau 11', N'0900000011', NULL, 1)
INSERT [dbo].[KHACHHANG] ([MKH], [HOTEN], [NGAYTHAMGIA], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (12, N'Khach hang mau 12', CAST(N'2024-04-30' AS Date), N'Dia chi mau 12', N'0900000012', NULL, 1)
INSERT [dbo].[KHACHHANG] ([MKH], [HOTEN], [NGAYTHAMGIA], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (13, N'Khach hang mau 13', CAST(N'2024-04-16' AS Date), N'Dia chi mau 13', N'0900000013', NULL, 1)
INSERT [dbo].[KHACHHANG] ([MKH], [HOTEN], [NGAYTHAMGIA], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (14, N'Khach hang mau 14', CAST(N'2024-04-16' AS Date), N'Dia chi mau 14', N'0900000014', NULL, 1)
INSERT [dbo].[KHACHHANG] ([MKH], [HOTEN], [NGAYTHAMGIA], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (15, N'Khach hang mau 15', CAST(N'2024-04-16' AS Date), N'Dia chi mau 15', N'0900000015', NULL, 1)
INSERT [dbo].[KHACHHANG] ([MKH], [HOTEN], [NGAYTHAMGIA], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (16, N'Khach hang mau 16', CAST(N'2024-04-16' AS Date), N'Dia chi mau 16', N'0900000016', NULL, 1)
INSERT [dbo].[KHACHHANG] ([MKH], [HOTEN], [NGAYTHAMGIA], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (17, N'Khach hang mau 17', CAST(N'2024-04-17' AS Date), N'Dia chi mau 17', N'0900000017', NULL, 1)
INSERT [dbo].[KHACHHANG] ([MKH], [HOTEN], [NGAYTHAMGIA], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (18, N'Khach hang mau 18', CAST(N'2024-04-17' AS Date), N'Dia chi mau 18', N'0900000018', NULL, 1)
SET IDENTITY_INSERT [dbo].[KHACHHANG] OFF
GO
SET IDENTITY_INSERT [dbo].[KHUVUCKHO] ON 

INSERT [dbo].[KHUVUCKHO] ([MKVK], [TEN], [GHICHU], [TT]) VALUES (1, N'Khu vực A - Hàng công nghệ cao', N'Lưu trữ Laptop, Điện thoại, Máy tính bảng, Màn hình LCD', 1)
INSERT [dbo].[KHUVUCKHO] ([MKVK], [TEN], [GHICHU], [TT]) VALUES (2, N'Khu vực B - Linh phụ kiện', N'Lưu trữ RAM, SSD, Mainboard, Chuột, Phím, Tai nghe', 1)
INSERT [dbo].[KHUVUCKHO] ([MKVK], [TEN], [GHICHU], [TT]) VALUES (3, N'Khu vực C - Thiết bị văn phòng', N'Lưu trữ Máy in, Máy chiếu, Router Wifi, Switch, Cáp mạng', 1)
INSERT [dbo].[KHUVUCKHO] ([MKVK], [TEN], [GHICHU], [TT]) VALUES (4, N'Khu vực D - Kho bảo hành/Đổi trả', N'Lưu trữ hàng lỗi chờ xử lý hoặc hàng chờ bảo hành', 1)
SET IDENTITY_INSERT [dbo].[KHUVUCKHO] OFF
GO
SET IDENTITY_INSERT [dbo].[LOAISANPHAM] ON 

INSERT [dbo].[LOAISANPHAM] ([MLSP], [TEN], [TLGX], [GHICHU], [TT]) VALUES (1, N'Màn hình máy tính', 10, N'Màn hình máy tính phục vụ học tập, làm việc và chơi game', 1)
INSERT [dbo].[LOAISANPHAM] ([MLSP], [TEN], [TLGX], [GHICHU], [TT]) VALUES (2, N'Phụ kiện máy tính', 20, N'Bàn phím, Chuột, Tai nghe, Lót chuột', 1)
INSERT [dbo].[LOAISANPHAM] ([MLSP], [TEN], [TLGX], [GHICHU], [TT]) VALUES (3, N'Linh kiện máy tính', 15, N'CPU, Mainboard, RAM, SSD, HDD, VGA và các linh kiện máy tính', 1)
INSERT [dbo].[LOAISANPHAM] ([MLSP], [TEN], [TLGX], [GHICHU], [TT]) VALUES (4, N'Thiết bị mạng', 12, N'Router, Switch, Dây cáp mạng', 1)
INSERT [dbo].[LOAISANPHAM] ([MLSP], [TEN], [TLGX], [GHICHU], [TT]) VALUES (5, N'Thiết bị lưu trữ', 10, N'USB, ổ cứng di động, thẻ nhớ và thiết bị lưu trữ ngoài', 1)
SET IDENTITY_INSERT [dbo].[LOAISANPHAM] OFF
GO
SET IDENTITY_INSERT [dbo].[NHACUNGCAP] ON 

INSERT [dbo].[NHACUNGCAP] ([MNCC], [TEN], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (1, N'Công ty Synnex FPT', N'KCX Tân Thuận, Quận 7, TP.HCM', N'02873006666', N'phanphoi@fpt.com.vn', 1)
INSERT [dbo].[NHACUNGCAP] ([MNCC], [TEN], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (2, N'Digiworld (DGW)', N'195-197 Nguyễn Thái Bình, Quận 1, TP.HCM', N'02839290059', N'support@dgw.com.vn', 1)
INSERT [dbo].[NHACUNGCAP] ([MNCC], [TEN], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (3, N'Viễn Sơn Technology', N'175 Nguyễn Thị Minh Khai, Quận 1, TP.HCM', N'02839250713', N'sales@vienson.com.vn', 1)
INSERT [dbo].[NHACUNGCAP] ([MNCC], [TEN], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (4, N'Thùy Minh Technology (TMC)', N'Số 5 ngõ 15 Thái Hà, Đống Đa, Hà Nội', N'02435375566', N'info@thuyminh.vn', 1)
INSERT [dbo].[NHACUNGCAP] ([MNCC], [TEN], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (5, N'Công ty PSD (Petrosetco)', N'Số 1 Nam Kỳ Khởi Nghĩa, Quận 1, TP.HCM', N'02839115578', N'contact@psd.com.vn', 1)
INSERT [dbo].[NHACUNGCAP] ([MNCC], [TEN], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (6, N'Hải Anh Computer', N'103 Lê Thanh Nghị, Hai Bà Trưng, Hà Nội', N'02436288137', N'kinhdoanh@haianh.com.vn', 1)
INSERT [dbo].[NHACUNGCAP] ([MNCC], [TEN], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (7, N'GearVN Enterprise', N'78-80 Hoàng Hoa Thám, Tân Bình, TP.HCM', N'18006975', N'business@gearvn.com', 1)
INSERT [dbo].[NHACUNGCAP] ([MNCC], [TEN], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (8, N'Elite JSC', N'289/1 Ung Văn Khiêm, Bình Thạnh, TP.HCM', N'02835123959', N'sales@elite-jsc.com', 1)
SET IDENTITY_INSERT [dbo].[NHACUNGCAP] OFF
GO
SET IDENTITY_INSERT [dbo].[NHANVIEN] ON 


INSERT [dbo].[NHANVIEN] ([MNV], [HOTEN], [GIOITINH], [NGAYSINH], [SDT], [EMAIL], [TT]) VALUES (1, N'Nhan vien mau 01', 0, CAST(N'2000-01-01' AS Date), N'0900000001', N'nhanvien1@example.test', 1)
INSERT [dbo].[NHANVIEN] ([MNV], [HOTEN], [GIOITINH], [NGAYSINH], [SDT], [EMAIL], [TT]) VALUES (2, N'Nhan vien mau 02', 0, CAST(N'2000-01-01' AS Date), N'0900000002', N'nhanvien2@example.test', 1)
INSERT [dbo].[NHANVIEN] ([MNV], [HOTEN], [GIOITINH], [NGAYSINH], [SDT], [EMAIL], [TT]) VALUES (3, N'Nhan vien mau 03', 1, CAST(N'2000-01-01' AS Date), N'0900000003', N'nhanvien3@example.test', 1)

SET IDENTITY_INSERT [dbo].[NHANVIEN] OFF
GO
SET IDENTITY_INSERT [dbo].[NHASANXUAT] ON 

INSERT [dbo].[NHASANXUAT] ([MNSX], [TEN], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (1, N'Dell Inc.', N'Round Rock, Texas, USA', N'18005550199', N'contact@dell.com', 1)
INSERT [dbo].[NHASANXUAT] ([MNSX], [TEN], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (2, N'Apple Inc.', N'Cupertino, California, USA', N'18006927753', N'support@apple.com', 1)
INSERT [dbo].[NHASANXUAT] ([MNSX], [TEN], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (3, N'Logitech', N'Lausanne, Switzerland', N'02831234567', N'sales@logitech.com', 1)
INSERT [dbo].[NHASANXUAT] ([MNSX], [TEN], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (4, N'Keychron', N'Hong Kong', N'0909888777', N'support@keychron.com', 1)
INSERT [dbo].[NHASANXUAT] ([MNSX], [TEN], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (5, N'LG Electronics', N'Seoul, South Korea', N'02435558888', N'service@lg.com', 1)
INSERT [dbo].[NHASANXUAT] ([MNSX], [TEN], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (6, N'Samsung', N'Seoul, South Korea', N'02839112233', N'support@samsung.com', 1)
INSERT [dbo].[NHASANXUAT] ([MNSX], [TEN], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (7, N'Asus', N'Taipei, Taiwan', N'18006588', N'contact@asus.com', 1)
INSERT [dbo].[NHASANXUAT] ([MNSX], [TEN], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (8, N'Kingston', N'Fountain Valley, USA', N'1800333444', N'sales@kingston.com', 1)
INSERT [dbo].[NHASANXUAT] ([MNSX], [TEN], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (9, N'TP-Link', N'Shenzhen, China', N'02862615079', N'support@tp-link.com', 1)
INSERT [dbo].[NHASANXUAT] ([MNSX], [TEN], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (10, N'Canon', N'Tokyo, Japan', N'02838200466', N'service@canon.com.vn', 1)
INSERT [dbo].[NHASANXUAT] ([MNSX], [TEN], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (11, N'Sony', N'Tokyo, Japan', N'1800588885', N'info@sony.com.vn', 1)
INSERT [dbo].[NHASANXUAT] ([MNSX], [TEN], [DIACHI], [SDT], [EMAIL], [TT]) VALUES (12, N'Intel', N'Santa Clara, USA', N'1800555999', N'support@intel.com', 1)
SET IDENTITY_INSERT [dbo].[NHASANXUAT] OFF
GO
SET IDENTITY_INSERT [dbo].[NHOMQUYEN] ON 

INSERT [dbo].[NHOMQUYEN] ([MNQ], [TEN], [TT]) VALUES (1, N'Quản lý kho hàng', 1)
INSERT [dbo].[NHOMQUYEN] ([MNQ], [TEN], [TT]) VALUES (2, N'Nhân viên xuất hàng', 1)
INSERT [dbo].[NHOMQUYEN] ([MNQ], [TEN], [TT]) VALUES (3, N'Nhân viên nhập hàng', 1)
INSERT [dbo].[NHOMQUYEN] ([MNQ], [TEN], [TT]) VALUES (4, N'Admin', 1)
SET IDENTITY_INSERT [dbo].[NHOMQUYEN] OFF
GO
SET IDENTITY_INSERT [dbo].[PHIEUKIEMKE] ON 

INSERT [dbo].[PHIEUKIEMKE] ([MPKK], [MNV], [TG], [TT]) VALUES (2, 2, CAST(N'2026-10-10T14:30:00.000' AS DateTime), 2)
INSERT [dbo].[PHIEUKIEMKE] ([MPKK], [MNV], [TG], [TT]) VALUES (3, 3, CAST(N'2026-06-15T10:00:00.000' AS DateTime), 0)
INSERT [dbo].[PHIEUKIEMKE] ([MPKK], [MNV], [TG], [TT]) VALUES (5, 2, CAST(N'2026-10-18T14:00:00.000' AS DateTime), 2)
SET IDENTITY_INSERT [dbo].[PHIEUKIEMKE] OFF
GO
SET IDENTITY_INSERT [dbo].[PHIEUNHAP] ON 

INSERT [dbo].[PHIEUNHAP] ([MPN], [MNV], [MNCC], [TIEN], [TG], [TT]) VALUES (1, 1, 1, 60000, CAST(N'2026-04-01T01:09:27.000' AS DateTime), 1)
INSERT [dbo].[PHIEUNHAP] ([MPN], [MNV], [MNCC], [TIEN], [TG], [TT]) VALUES (2, 1, 1, 120000, CAST(N'2026-04-02T01:09:27.000' AS DateTime), 1)
INSERT [dbo].[PHIEUNHAP] ([MPN], [MNV], [MNCC], [TIEN], [TG], [TT]) VALUES (5, 3, 3, 73000000, CAST(N'2026-04-20T15:45:00.000' AS DateTime), 1)
INSERT [dbo].[PHIEUNHAP] ([MPN], [MNV], [MNCC], [TIEN], [TG], [TT]) VALUES (7, 2, 2, 30000000, CAST(N'2026-10-03T14:15:00.000' AS DateTime), 1)
INSERT [dbo].[PHIEUNHAP] ([MPN], [MNV], [MNCC], [TIEN], [TG], [TT]) VALUES (8, 3, 3, 46000000, CAST(N'2026-10-05T09:45:00.000' AS DateTime), 1)
INSERT [dbo].[PHIEUNHAP] ([MPN], [MNV], [MNCC], [TIEN], [TG], [TT]) VALUES (9, 1, 5, 96000000, CAST(N'2026-10-07T10:00:00.000' AS DateTime), 1)
INSERT [dbo].[PHIEUNHAP] ([MPN], [MNV], [MNCC], [TIEN], [TG], [TT]) VALUES (11, 2, 3, 25000000, CAST(N'2026-10-17T13:30:00.000' AS DateTime), 1)
SET IDENTITY_INSERT [dbo].[PHIEUNHAP] OFF
GO
SET IDENTITY_INSERT [dbo].[PHIEUXUAT] ON 

INSERT [dbo].[PHIEUXUAT] ([MPX], [MNV], [MKH], [TIEN], [TG], [TT]) VALUES (1, 1, 1, 4200000, CAST(N'2026-04-18T17:34:12.000' AS DateTime), 1)
INSERT [dbo].[PHIEUXUAT] ([MPX], [MNV], [MKH], [TIEN], [TG], [TT]) VALUES (2, 1, 2, 9100000, CAST(N'2026-04-17T18:19:51.000' AS DateTime), 1)
INSERT [dbo].[PHIEUXUAT] ([MPX], [MNV], [MKH], [TIEN], [TG], [TT]) VALUES (3, 2, 3, 1200000, CAST(N'2026-05-01T09:00:00.000' AS DateTime), 1)
INSERT [dbo].[PHIEUXUAT] ([MPX], [MNV], [MKH], [TIEN], [TG], [TT]) VALUES (5, 3, 8, 7300000, CAST(N'2026-05-10T10:15:00.000' AS DateTime), 1)
INSERT [dbo].[PHIEUXUAT] ([MPX], [MNV], [MKH], [TIEN], [TG], [TT]) VALUES (6, 1, 1, 1200000, CAST(N'2026-10-02T10:30:00.000' AS DateTime), 1)
INSERT [dbo].[PHIEUXUAT] ([MPX], [MNV], [MKH], [TIEN], [TG], [TT]) VALUES (8, 1, 3, 42500000, CAST(N'2026-10-06T09:15:00.000' AS DateTime), 1)
INSERT [dbo].[PHIEUXUAT] ([MPX], [MNV], [MKH], [TIEN], [TG], [TT]) VALUES (9, 3, 4, 1800000, CAST(N'2026-10-07T11:45:00.000' AS DateTime), 1)
INSERT [dbo].[PHIEUXUAT] ([MPX], [MNV], [MKH], [TIEN], [TG], [TT]) VALUES (11, 2, 3, 1500000, CAST(N'2026-10-15T14:15:00.000' AS DateTime), 1)
INSERT [dbo].[PHIEUXUAT] ([MPX], [MNV], [MKH], [TIEN], [TG], [TT]) VALUES (13, 3, 5, 5500000, CAST(N'2026-10-16T16:45:00.000' AS DateTime), 1)
INSERT [dbo].[PHIEUXUAT] ([MPX], [MNV], [MKH], [TIEN], [TG], [TT]) VALUES (15, 1, 6, 1200000, CAST(N'2026-10-18T11:20:00.000' AS DateTime), 1)
INSERT [dbo].[PHIEUXUAT] ([MPX], [MNV], [MKH], [TIEN], [TG], [TT]) VALUES (16, 3, 2, 17500000, CAST(N'2026-10-19T15:10:00.000' AS DateTime), 1)
INSERT [dbo].[PHIEUXUAT] ([MPX], [MNV], [MKH], [TIEN], [TG], [TT]) VALUES (17, 1, 1, 2400000, CAST(N'2026-11-03T09:28:55.820' AS DateTime), 1)
SET IDENTITY_INSERT [dbo].[PHIEUXUAT] OFF
GO
SET IDENTITY_INSERT [dbo].[SANPHAM] ON 

INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (3, N'Chuột Logitech G502', N'mouse_g502.png', N'Phụ kiện máy tính', 3, 2, 2, 1200000, 900000, 98, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (4, N'Bàn phím cơ Keychron', N'keychron_k2.png', N'Phụ kiện máy tính', 4, 2, 2, 1800000, 1400000, 50, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (5, N'Màn hình LG 27 inch', N'lg_27.png', N'Màn hình máy tính', 5, 1, 1, 5500000, 4800000, 15, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (15, N'Màn hình Samsung Odyssey G5', N'samsung_g5.png', N'Màn hình máy tính', 6, 1, 1, 7500000, 6200000, 10, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (16, N'Màn hình Dell Ultrasharp U2422', N'dell_u2422.png', N'Màn hình máy tính', 1, 1, 1, 6200000, 5000000, 12, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (17, N'Màn hình Asus ProArt', N'asus_proart.png', N'Màn hình máy tính', 7, 1, 1, 9000000, 7800000, 6, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (18, N'Chuột Logitech MX Master 3S', N'logitech_mx3.png', N'Phụ kiện máy tính', 3, 2, 2, 2500000, 1900000, 25, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (19, N'Bàn phím Logitech K380', N'logi_k380.png', N'Phụ kiện máy tính', 3, 2, 2, 750000, 550000, 40, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (20, N'Tai nghe Sony WH-1000XM5', N'sony_xm5.png', N'Phụ kiện máy tính', 11, 2, 2, 8500000, 6900000, 15, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (22, N'Bàn phím cơ Keychron K8 Pro', N'keychron_k8.png', N'Phụ kiện máy tính', 4, 2, 2, 2800000, 2100000, 15, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (23, N'Lót chuột Razer Gigantus', N'razer_pad.png', N'Phụ kiện máy tính', 3, 2, 2, 450000, 250000, 50, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (24, N'RAM Kingston Fury 16GB', N'kingston_ram16.png', N'Linh kiện máy tính', 8, 2, 3, 1200000, 950000, 60, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (25, N'SSD Samsung 980 Pro 1TB', N'ssd_980pro.png', N'Linh kiện máy tính', 6, 2, 3, 2500000, 1800000, 35, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (26, N'SSD Kingston NV2 500GB', N'ssd_nv2.png', N'Linh kiện máy tính', 8, 2, 3, 950000, 700000, 45, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (27, N'CPU Intel Core i9-13900K', N'intel_i9.png', N'Linh kiện máy tính', 12, 2, 3, 14500000, 12000000, 8, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (28, N'CPU Intel Core i5-12400F', N'intel_i5.png', N'Linh kiện máy tính', 12, 2, 3, 3500000, 2900000, 25, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (29, N'Mainboard Asus ROG Z790', N'main_z790.png', N'Linh kiện máy tính', 7, 2, 3, 8500000, 7000000, 5, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (30, N'Router Wifi 6 TP-Link Archer', N'tplink_ax73.png', N'Thiết bị mạng', 9, 3, 4, 2200000, 1600000, 20, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (31, N'Switch TP-Link 8 Port', N'tplink_switch.png', N'Thiết bị mạng', 9, 3, 4, 350000, 220000, 60, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (32, N'Bộ phát Wifi Mesh Asus', N'asus_mesh.png', N'Thiết bị mạng', 7, 3, 4, 4500000, 3600000, 10, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (33, N'Dây cáp mạng CAT6 100m', N'cable_cat6.png', N'Thiết bị mạng', 9, 3, 4, 800000, 500000, 15, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (1002, N'Intel Core i3-14100F', NULL, N'Linh kiện máy tính', 12, 2, 3, 2500000, 2100000, 0, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (1003, N'Intel Core i5-14400F', NULL, N'Linh kiện máy tính', 12, 2, 3, 4200000, 3700000, 0, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (1004, N'Intel Core i5-14600KF', NULL, N'Linh kiện máy tính', 12, 2, 3, 6500000, 5800000, 0, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (1005, N'Intel Core i7-14700F', NULL, N'Linh kiện máy tính', 12, 2, 3, 8900000, 8000000, 0, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (1006, N'Intel Core Ultra 5 245K', NULL, N'Linh kiện máy tính', 12, 2, 3, 7600000, 6900000, 0, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (1007, N'ASUS PRIME B760M-A WIFI D4', NULL, N'Linh kiện máy tính', 7, 2, 3, 3400000, 2900000, 0, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (1008, N'ASUS TUF GAMING B760M-PLUS WIFI', NULL, N'Linh kiện máy tính', 7, 2, 3, 4600000, 3900000, 0, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (1009, N'ASUS ROG STRIX B650-A GAMING WIFI', NULL, N'Linh kiện máy tính', 7, 2, 3, 6200000, 5500000, 0, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (1010, N'Kingston FURY Beast DDR4 16GB 3200MHz', NULL, N'Linh kiện máy tính', 8, 2, 3, 1100000, 850000, 0, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (1011, N'Kingston FURY Beast DDR5 16GB 5600MHz', NULL, N'Linh kiện máy tính', 8, 2, 3, 1700000, 1350000, 0, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (1012, N'Kingston FURY Beast DDR5 32GB 6000MHz', NULL, N'Linh kiện máy tính', 8, 2, 3, 3200000, 2700000, 0, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (1013, N'Samsung 990 PRO SSD 1TB NVMe', NULL, N'Linh kiện máy tính', 6, 2, 3, 3300000, 2800000, 0, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (1014, N'Samsung 990 PRO SSD 2TB NVMe', NULL, N'Linh kiện máy tính', 6, 2, 3, 5600000, 4800000, 0, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (1015, N'Kingston NV3 SSD 1TB NVMe', NULL, N'Linh kiện máy tính', 8, 2, 3, 1900000, 1500000, 0, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (1016, N'ASUS Dual GeForce RTX 4060 8GB', NULL, N'Linh kiện máy tính', 7, 2, 3, 8300000, 7500000, 0, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (1017, N'ASUS Dual GeForce RTX 4070 SUPER 12GB', NULL, N'Linh kiện máy tính', 7, 2, 3, 17500000, 16000000, 0, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (1018, N'ASUS ROG STRIX 750G 750W', NULL, N'Linh kiện máy tính', 7, 2, 3, 3200000, 2700000, 0, 1)
INSERT [dbo].[SANPHAM] ([MSP], [TEN], [HINHANH], [DANHMUC], [MNSX], [MKVK], [MLSP], [TIENX], [TIENN], [SL], [TT]) VALUES (1019, N'ASUS TUF GAMING GT301 Case', NULL, N'Linh kiện máy tính', 7, 2, 3, 2200000, 1800000, 0, 1)
SET IDENTITY_INSERT [dbo].[SANPHAM] OFF
GO
INSERT [dbo].[TAIKHOAN] ([MNV], [MK], [TDN], [MNQ], [TT], [OTP]) VALUES (1, N'$2a$12$6GSkiQ05XjTRvCW9MB6MNuf7hOJEbbeQx11Eb8oELil1OrCq6uBXm', N'admin', 4, 1, NULL)
INSERT [dbo].[TAIKHOAN] ([MNV], [MK], [TDN], [MNQ], [TT], [OTP]) VALUES (2, N'$2a$12$6GSkiQ05XjTRvCW9MB6MNuf7hOJEbbeQx11Eb8oELil1OrCq6uBXm', N'NV2', 2, 1, NULL)
INSERT [dbo].[TAIKHOAN] ([MNV], [MK], [TDN], [MNQ], [TT], [OTP]) VALUES (3, N'$2a$12$6GSkiQ05XjTRvCW9MB6MNuf7hOJEbbeQx11Eb8oELil1OrCq6uBXm', N'NV3', 3, 1, NULL)
GO
-- 3. CHI MUC UNIQUE, DEFAULT VA RANG BUOC (05 FK CO SAN)
SET ANSI_PADDING ON
GO
ALTER TABLE [dbo].[KHACHHANG] ADD UNIQUE NONCLUSTERED 
(
	[SDT] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
CREATE UNIQUE NONCLUSTERED INDEX [UX_KHACHHANG_EMAIL] ON [dbo].[KHACHHANG]
(
	[EMAIL] ASC
)
WHERE ([EMAIL] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
ALTER TABLE [dbo].[NHANVIEN] ADD UNIQUE NONCLUSTERED 
(
	[EMAIL] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
ALTER TABLE [dbo].[TAIKHOAN] ADD UNIQUE NONCLUSTERED 
(
	[TDN] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[CTPHIEUNHAP] ADD  DEFAULT ((0)) FOR [HINHTHUC]
GO
ALTER TABLE [dbo].[DANHMUCCHUCNANG] ADD  DEFAULT ((1)) FOR [TT]
GO
ALTER TABLE [dbo].[KHACHHANG] ADD  DEFAULT ((1)) FOR [TT]
GO
ALTER TABLE [dbo].[KHUVUCKHO] ADD  DEFAULT (N'') FOR [GHICHU]
GO
ALTER TABLE [dbo].[KHUVUCKHO] ADD  DEFAULT ((1)) FOR [TT]
GO
ALTER TABLE [dbo].[LOAISANPHAM] ADD  DEFAULT (N'') FOR [GHICHU]
GO
ALTER TABLE [dbo].[LOAISANPHAM] ADD  DEFAULT ((1)) FOR [TT]
GO
ALTER TABLE [dbo].[NHACUNGCAP] ADD  DEFAULT ((1)) FOR [TT]
GO
ALTER TABLE [dbo].[NHANVIEN] ADD  DEFAULT ((1)) FOR [TT]
GO
ALTER TABLE [dbo].[NHASANXUAT] ADD  DEFAULT ((1)) FOR [TT]
GO
ALTER TABLE [dbo].[NHOMQUYEN] ADD  DEFAULT ((1)) FOR [TT]
GO
ALTER TABLE [dbo].[PHIEUKIEMKE] ADD  DEFAULT (sysdatetime()) FOR [TG]
GO
ALTER TABLE [dbo].[PHIEUKIEMKE] ADD  DEFAULT ((1)) FOR [TT]
GO
ALTER TABLE [dbo].[PHIEUNHAP] ADD  DEFAULT (sysdatetime()) FOR [TG]
GO
ALTER TABLE [dbo].[PHIEUNHAP] ADD  DEFAULT ((1)) FOR [TT]
GO
ALTER TABLE [dbo].[PHIEUXUAT] ADD  DEFAULT ((1)) FOR [MNV]
GO
ALTER TABLE [dbo].[PHIEUXUAT] ADD  DEFAULT (sysdatetime()) FOR [TG]
GO
ALTER TABLE [dbo].[PHIEUXUAT] ADD  DEFAULT ((1)) FOR [TT]
GO
ALTER TABLE [dbo].[SANPHAM] ADD  DEFAULT (NULL) FOR [HINHANH]
GO
ALTER TABLE [dbo].[SANPHAM] ADD  DEFAULT (NULL) FOR [DANHMUC]
GO
ALTER TABLE [dbo].[SANPHAM] ADD  DEFAULT ((0)) FOR [TIENX]
GO
ALTER TABLE [dbo].[SANPHAM] ADD  DEFAULT ((0)) FOR [TIENN]
GO
ALTER TABLE [dbo].[SANPHAM] ADD  DEFAULT ((0)) FOR [SL]
GO
ALTER TABLE [dbo].[SANPHAM] ADD  DEFAULT ((1)) FOR [TT]
GO
ALTER TABLE [dbo].[TAIKHOAN] ADD  DEFAULT ((1)) FOR [TT]
GO
ALTER TABLE [dbo].[TAIKHOAN] ADD  DEFAULT (NULL) FOR [OTP]
GO
ALTER TABLE [dbo].[CTQUYEN]  WITH CHECK ADD  CONSTRAINT [FK_MCN_CTQUYEN] FOREIGN KEY([MCN])
REFERENCES [dbo].[DANHMUCCHUCNANG] ([MCN])
GO
ALTER TABLE [dbo].[CTQUYEN] CHECK CONSTRAINT [FK_MCN_CTQUYEN]
GO
ALTER TABLE [dbo].[CTQUYEN]  WITH CHECK ADD  CONSTRAINT [FK_MNQ_CTQUYEN] FOREIGN KEY([MNQ])
REFERENCES [dbo].[NHOMQUYEN] ([MNQ])
GO
ALTER TABLE [dbo].[CTQUYEN] CHECK CONSTRAINT [FK_MNQ_CTQUYEN]
GO
ALTER TABLE [dbo].[PHIEUXUAT]  WITH CHECK ADD  CONSTRAINT [FK_MNV_PHIEUXUAT] FOREIGN KEY([MNV])
REFERENCES [dbo].[NHANVIEN] ([MNV])
GO
ALTER TABLE [dbo].[PHIEUXUAT] CHECK CONSTRAINT [FK_MNV_PHIEUXUAT]
GO
ALTER TABLE [dbo].[TAIKHOAN]  WITH CHECK ADD  CONSTRAINT [FK_MNQ_TAIKHOAN] FOREIGN KEY([MNQ])
REFERENCES [dbo].[NHOMQUYEN] ([MNQ])
GO
ALTER TABLE [dbo].[TAIKHOAN] CHECK CONSTRAINT [FK_MNQ_TAIKHOAN]
GO
ALTER TABLE [dbo].[TAIKHOAN]  WITH CHECK ADD  CONSTRAINT [FK_MNV_TAIKHOAN] FOREIGN KEY([MNV])
REFERENCES [dbo].[NHANVIEN] ([MNV])
GO
ALTER TABLE [dbo].[TAIKHOAN] CHECK CONSTRAINT [FK_MNV_TAIKHOAN]
GO

GO
USE [quanlykho]
GO
-- 3. CAP NHAT TEN FILE ANH SAN PHAM DEMO
UPDATE [dbo].[SANPHAM] SET [HINHANH] = N'intel_i3.png' WHERE [MSP] = 1002;
UPDATE [dbo].[SANPHAM] SET [HINHANH] = N'intel_i5_14400F.png' WHERE [MSP] = 1003;
UPDATE [dbo].[SANPHAM] SET [HINHANH] = N'intel_i5_14600KF.png' WHERE [MSP] = 1004;
UPDATE [dbo].[SANPHAM] SET [HINHANH] = N'intel_i7.png' WHERE [MSP] = 1005;
UPDATE [dbo].[SANPHAM] SET [HINHANH] = N'intel_core.png' WHERE [MSP] = 1006;
UPDATE [dbo].[SANPHAM] SET [HINHANH] = N'asus_prime.png' WHERE [MSP] = 1007;
UPDATE [dbo].[SANPHAM] SET [HINHANH] = N'asus_tuf.png' WHERE [MSP] = 1008;
UPDATE [dbo].[SANPHAM] SET [HINHANH] = N'asus_rog_b650.png' WHERE [MSP] = 1009;
UPDATE [dbo].[SANPHAM] SET [HINHANH] = N'kingston_ddr4.png' WHERE [MSP] = 1010;
UPDATE [dbo].[SANPHAM] SET [HINHANH] = N'kingston_ddr5.png' WHERE [MSP] = 1011;
UPDATE [dbo].[SANPHAM] SET [HINHANH] = N'kingston_ddr5_32gb.png' WHERE [MSP] = 1012;
UPDATE [dbo].[SANPHAM] SET [HINHANH] = N'samsung_990pro_1tb.png' WHERE [MSP] = 1013;
UPDATE [dbo].[SANPHAM] SET [HINHANH] = N'samsung_990pro_2tb.png' WHERE [MSP] = 1014;
UPDATE [dbo].[SANPHAM] SET [HINHANH] = N'kingston_nv3_1tb.png' WHERE [MSP] = 1015;
UPDATE [dbo].[SANPHAM] SET [HINHANH] = N'asus_rtx4060.png' WHERE [MSP] = 1016;
UPDATE [dbo].[SANPHAM] SET [HINHANH] = N'asus_rtx4070super.png' WHERE [MSP] = 1017;
UPDATE [dbo].[SANPHAM] SET [HINHANH] = N'asus_rog_750g.png' WHERE [MSP] = 1018;
UPDATE [dbo].[SANPHAM] SET [HINHANH] = N'asus_tuf_gt301.png' WHERE [MSP] = 1019;
GO


USE [quanlykho]
GO
-- 4. KHOA NGOAI BO SUNG (13 MOI + 5 CU = 18 FK KHONG TRUNG)

IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = 'FK_SP_NSX'
)
BEGIN
    ALTER TABLE dbo.[SANPHAM] WITH CHECK
    ADD CONSTRAINT [FK_SP_NSX]
    FOREIGN KEY ([MNSX])
    REFERENCES dbo.[NHASANXUAT]([MNSX]);
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = 'FK_SP_LSP'
)
BEGIN
    ALTER TABLE dbo.[SANPHAM] WITH CHECK
    ADD CONSTRAINT [FK_SP_LSP]
    FOREIGN KEY ([MLSP])
    REFERENCES dbo.[LOAISANPHAM]([MLSP]);
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = 'FK_SP_KVK'
)
BEGIN
    ALTER TABLE dbo.[SANPHAM] WITH CHECK
    ADD CONSTRAINT [FK_SP_KVK]
    FOREIGN KEY ([MKVK])
    REFERENCES dbo.[KHUVUCKHO]([MKVK]);
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = 'FK_PN_NV'
)
BEGIN
    ALTER TABLE dbo.[PHIEUNHAP] WITH CHECK
    ADD CONSTRAINT [FK_PN_NV]
    FOREIGN KEY ([MNV])
    REFERENCES dbo.[NHANVIEN]([MNV]);
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = 'FK_PN_NCC'
)
BEGIN
    ALTER TABLE dbo.[PHIEUNHAP] WITH CHECK
    ADD CONSTRAINT [FK_PN_NCC]
    FOREIGN KEY ([MNCC])
    REFERENCES dbo.[NHACUNGCAP]([MNCC]);
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = 'FK_PX_KH'
)
BEGIN
    ALTER TABLE dbo.[PHIEUXUAT] WITH CHECK
    ADD CONSTRAINT [FK_PX_KH]
    FOREIGN KEY ([MKH])
    REFERENCES dbo.[KHACHHANG]([MKH]);
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = 'FK_PK_NV'
)
BEGIN
    ALTER TABLE dbo.[PHIEUKIEMKE] WITH CHECK
    ADD CONSTRAINT [FK_PK_NV]
    FOREIGN KEY ([MNV])
    REFERENCES dbo.[NHANVIEN]([MNV]);
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = 'FK_CTPN_PN'
)
BEGIN
    ALTER TABLE dbo.[CTPHIEUNHAP] WITH CHECK
    ADD CONSTRAINT [FK_CTPN_PN]
    FOREIGN KEY ([MPN])
    REFERENCES dbo.[PHIEUNHAP]([MPN]);
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = 'FK_CTPN_SP'
)
BEGIN
    ALTER TABLE dbo.[CTPHIEUNHAP] WITH CHECK
    ADD CONSTRAINT [FK_CTPN_SP]
    FOREIGN KEY ([MSP])
    REFERENCES dbo.[SANPHAM]([MSP]);
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = 'FK_CTPX_PX'
)
BEGIN
    ALTER TABLE dbo.[CTPHIEUXUAT] WITH CHECK
    ADD CONSTRAINT [FK_CTPX_PX]
    FOREIGN KEY ([MPX])
    REFERENCES dbo.[PHIEUXUAT]([MPX]);
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = 'FK_CTPX_SP'
)
BEGIN
    ALTER TABLE dbo.[CTPHIEUXUAT] WITH CHECK
    ADD CONSTRAINT [FK_CTPX_SP]
    FOREIGN KEY ([MSP])
    REFERENCES dbo.[SANPHAM]([MSP]);
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = 'FK_CTPK_PK'
)
BEGIN
    ALTER TABLE dbo.[CTPHIEUKIEMKE] WITH CHECK
    ADD CONSTRAINT [FK_CTPK_PK]
    FOREIGN KEY ([MPKK])
    REFERENCES dbo.[PHIEUKIEMKE]([MPKK]);
END;
GO

IF NOT EXISTS (
    SELECT 1 FROM sys.foreign_keys
    WHERE name = 'FK_CTPK_SP'
)
BEGIN
    ALTER TABLE dbo.[CTPHIEUKIEMKE] WITH CHECK
    ADD CONSTRAINT [FK_CTPK_SP]
    FOREIGN KEY ([MSP])
    REFERENCES dbo.[SANPHAM]([MSP]);
END;
GO


GO

USE quanlykho;
GO

-- 5. VIEW SAN PHAM PHUC VU WEB
CREATE VIEW dbo.vw_Catalog
AS
SELECT
    sp.MSP AS ProductId,
    CAST(sp.TEN AS NVARCHAR(255)) AS ProductName,

    CAST(
        COALESCE(NULLIF(sp.DANHMUC, ''), lsp.TEN, N'Khác')
        AS NVARCHAR(255)
    ) AS CategoryName,

    CAST(
        COALESCE(nsx.TEN, N'Không xác định')
        AS NVARCHAR(255)
    ) AS BrandName,

    CAST(sp.TIENX AS DECIMAL(18, 2)) AS Price,

    CASE
        WHEN sp.SL > 0 THEN sp.SL
        ELSE 0
    END AS AvailableQuantity,

    CAST(NULLIF(sp.HINHANH, '') AS NVARCHAR(500)) AS ImageUrl,

    CAST(N'' AS NVARCHAR(MAX)) AS Description

FROM dbo.SANPHAM AS sp
LEFT JOIN dbo.NHASANXUAT AS nsx ON sp.MNSX = nsx.MNSX
LEFT JOIN dbo.LOAISANPHAM AS lsp ON sp.MLSP = lsp.MLSP

WHERE sp.TT = 1;
GO


-- 6. KIEM TRA SO BO (SAU KHI CHAY THANH CONG)
SELECT (SELECT COUNT(*) FROM sys.tables) AS SoBang,
       (SELECT COUNT(*) FROM sys.key_constraints WHERE [type] = 'PK') AS SoKhoaChinh,
       (SELECT COUNT(*) FROM sys.foreign_keys) AS SoKhoaNgoai,
       (SELECT COUNT(*) FROM dbo.vw_Catalog) AS SoSanPhamWeb;
GO
