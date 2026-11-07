USE quanlykho;
GO

CREATE OR ALTER VIEW dbo.vw_Catalog
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
