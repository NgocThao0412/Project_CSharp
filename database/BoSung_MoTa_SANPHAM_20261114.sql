SET NOCOUNT ON;
SET XACT_ABORT ON;

IF DB_NAME() <> N'quanlykho_nhom'
    THROW 51001, N'Chi chay script nay trong database quanlykho_nhom.', 1;

IF OBJECT_ID(N'dbo.SANPHAM', N'U') IS NULL
    THROW 51002, N'Khong tim thay bang dbo.SANPHAM.', 1;

BEGIN TRY
    BEGIN TRANSACTION;

    IF COL_LENGTH(N'dbo.SANPHAM', N'MOTASP') IS NULL
        ALTER TABLE dbo.SANPHAM ADD MOTASP NVARCHAR(MAX) NULL;

    CREATE TABLE #MoTaMau (
        MSP INT NOT NULL PRIMARY KEY,
        TEN NVARCHAR(255) NOT NULL,
        MOTASP NVARCHAR(1000) NOT NULL
    );

    INSERT INTO #MoTaMau (MSP, TEN, MOTASP)
    VALUES
    (3, N'Chuột Logitech G502', N'Chuột Logitech G502 hướng đến người dùng chơi game và thao tác trên máy tính. Thiết kế nhiều nút điều khiển, phù hợp nhu cầu làm việc và giải trí.'),
    (4, N'Bàn phím cơ Keychron', N'Bàn phím cơ Keychron phù hợp cho nhập liệu, học tập và chơi game. Kiểu bàn phím cơ mang lại cảm giác gõ rõ ràng, thích hợp cho góc máy tính cá nhân.'),
    (5, N'Màn hình LG 27 inch', N'Màn hình LG kích thước 27 inch cung cấp không gian hiển thị rộng cho công việc, học tập và giải trí. Nên kiểm tra độ phân giải và cổng kết nối của phiên bản thực tế.'),
    (15, N'Màn hình Samsung Odyssey G5', N'Màn hình Samsung Odyssey G5 thuộc dòng sản phẩm hướng đến nhu cầu chơi game và giải trí. Phù hợp người dùng muốn bố trí không gian hiển thị riêng cho máy tính.'),
    (16, N'Màn hình Dell Ultrasharp U2422', N'Màn hình Dell UltraSharp U2422 hướng đến công việc văn phòng, học tập và xử lý tài liệu. Phù hợp thiết lập làm việc cần màn hình ngoài cho máy tính.'),
    (17, N'Màn hình Asus ProArt', N'Màn hình ASUS ProArt hướng đến người dùng thiết kế và làm việc sáng tạo. Khi chọn mua nên đối chiếu độ phân giải, dải màu và các cổng kết nối theo đúng mã phiên bản.'),
    (18, N'Chuột Logitech MX Master 3S', N'Chuột Logitech MX Master 3S hướng đến môi trường văn phòng và công việc sáng tạo. Thiết kế phục vụ thao tác liên tục trên máy tính và nhu cầu làm việc hằng ngày.'),
    (19, N'Bàn phím Logitech K380', N'Bàn phím Logitech K380 có kiểu dáng nhỏ gọn, phù hợp nhập liệu và sử dụng trong không gian học tập hoặc làm việc. Thuận tiện cho góc máy tính cần tiết kiệm diện tích.'),
    (20, N'Tai nghe Sony WH-1000XM5', N'Tai nghe Sony WH-1000XM5 hướng đến nhu cầu nghe nhạc, giải trí và làm việc. Phù hợp người dùng muốn sử dụng tai nghe cá nhân khi di chuyển hoặc tại bàn làm việc.'),
    (22, N'Bàn phím cơ Keychron K8 Pro', N'Bàn phím cơ Keychron K8 Pro phù hợp nhập liệu, làm việc và chơi game. Thiết kế dành cho người thích cảm giác gõ của bàn phím cơ trong bộ máy tính cá nhân.'),
    (23, N'Lót chuột Razer Gigantus', N'Lót chuột Razer Gigantus tạo bề mặt thao tác chuột tại bàn máy tính. Phù hợp bố trí góc chơi game, học tập hoặc làm việc và bảo vệ bề mặt bàn.'),
    (24, N'RAM Kingston Fury 16GB', N'Bộ nhớ RAM Kingston Fury dung lượng 16GB phục vụ nâng cấp khả năng đa nhiệm của máy tính. Cần kiểm tra chuẩn RAM và khả năng tương thích với bo mạch chủ.'),
    (25, N'SSD Samsung 980 Pro 1TB', N'Ổ cứng SSD Samsung 980 Pro dung lượng 1TB dùng lưu hệ điều hành, ứng dụng và dữ liệu. Thuộc dòng SSD NVMe, cần kiểm tra khả năng tương thích với khe lưu trữ trên máy tính.'),
    (26, N'SSD Kingston NV2 500GB', N'Ổ cứng SSD Kingston NV2 dung lượng 500GB phù hợp lưu hệ điều hành và ứng dụng. Thiết kế theo chuẩn SSD NVMe, cần kiểm tra máy tính có khe lắp tương thích.'),
    (27, N'CPU Intel Core i9-13900K', N'Bộ vi xử lý Intel Core i9-13900K hướng đến máy tính hiệu năng cao cho công việc và giải trí. Cần chọn bo mạch chủ, RAM và giải pháp tản nhiệt tương thích.'),
    (28, N'CPU Intel Core i5-12400F', N'Bộ vi xử lý Intel Core i5-12400F phù hợp lắp ráp PC học tập, làm việc và chơi game. Phiên bản F cần card đồ họa rời để xuất hình ảnh.'),
    (29, N'Mainboard Asus ROG Z790', N'Bo mạch chủ ASUS ROG Z790 dùng làm nền tảng kết nối CPU, RAM, ổ lưu trữ và các linh kiện PC. Cần kiểm tra bộ vi xử lý và bộ nhớ tương thích trước khi lắp ráp.'),
    (30, N'Router Wifi 6 TP-Link Archer', N'Bộ định tuyến TP-Link Archer hỗ trợ chuẩn Wi-Fi 6, phục vụ kết nối mạng không dây tại nhà hoặc văn phòng. Phù hợp triển khai mạng cho nhiều thiết bị.'),
    (31, N'Switch TP-Link 8 Port', N'Switch TP-Link 8 cổng giúp mở rộng kết nối mạng có dây giữa các thiết bị trong mạng nội bộ. Phù hợp gia đình, lớp học hoặc văn phòng nhỏ.'),
    (32, N'Bộ phát Wifi Mesh Asus', N'Bộ phát Wi-Fi Mesh ASUS hướng đến nhu cầu mở rộng vùng phủ sóng mạng không dây. Phù hợp nhà ở hoặc không gian làm việc có nhiều khu vực cần kết nối.'),
    (33, N'Dây cáp mạng CAT6 100m', N'Dây cáp mạng CAT6 cuộn 100 mét phục vụ thi công và kết nối mạng Ethernet. Phù hợp đi dây mạng cho gia đình, phòng máy hoặc văn phòng.'),
    (1002, N'Intel Core i3-14100F', N'Bộ vi xử lý Intel Core i3-14100F dành cho máy tính để bàn phục vụ học tập, văn phòng và giải trí cơ bản. Phiên bản F cần sử dụng card đồ họa rời.'),
    (1003, N'Intel Core i5-14400F', N'Bộ vi xử lý Intel Core i5-14400F phù hợp máy tính học tập, làm việc và chơi game. Cần kiểm tra bo mạch chủ tương thích và trang bị card đồ họa rời.'),
    (1004, N'Intel Core i5-14600KF', N'Bộ vi xử lý Intel Core i5-14600KF hướng đến máy tính chơi game và xử lý nhiều tác vụ. Phiên bản KF không có đồ họa tích hợp, cần dùng card đồ họa rời.'),
    (1005, N'Intel Core i7-14700F', N'Bộ vi xử lý Intel Core i7-14700F phục vụ PC làm việc đa nhiệm và chơi game. Cần kiểm tra bo mạch chủ, tản nhiệt và trang bị card đồ họa rời.'),
    (1006, N'Intel Core Ultra 5 245K', N'Bộ vi xử lý Intel Core Ultra 5 245K dành cho hệ thống máy tính để bàn. Cần chọn bo mạch chủ, bộ nhớ và tản nhiệt tương thích với thế hệ CPU này.'),
    (1007, N'ASUS PRIME B760M-A WIFI D4', N'Bo mạch chủ ASUS PRIME B760M-A WIFI D4 dành cho hệ thống PC dùng nền tảng Intel tương thích. Phiên bản D4 sử dụng bộ nhớ DDR4.'),
    (1008, N'ASUS TUF GAMING B760M-PLUS WIFI', N'Bo mạch chủ ASUS TUF GAMING B760M-PLUS WIFI hướng đến lắp ráp máy tính chơi game và làm việc. Cần đối chiếu chuẩn RAM và CPU tương thích theo phiên bản.'),
    (1009, N'ASUS ROG STRIX B650-A GAMING WIFI', N'Bo mạch chủ ASUS ROG STRIX B650-A GAMING WIFI hướng đến hệ thống PC nền tảng AMD tương thích. Phù hợp lắp ráp máy tính cá nhân và chơi game.'),
    (1010, N'Kingston FURY Beast DDR4 16GB 3200MHz', N'RAM Kingston FURY Beast DDR4 dung lượng 16GB, tốc độ danh định 3200MHz theo tên sản phẩm. Phù hợp nâng cấp bộ nhớ cho hệ thống hỗ trợ DDR4.'),
    (1011, N'Kingston FURY Beast DDR5 16GB 5600MHz', N'RAM Kingston FURY Beast DDR5 dung lượng 16GB, tốc độ danh định 5600MHz theo tên sản phẩm. Cần bo mạch chủ và bộ vi xử lý hỗ trợ DDR5.'),
    (1012, N'Kingston FURY Beast DDR5 32GB 6000MHz', N'RAM Kingston FURY Beast DDR5 dung lượng 32GB, tốc độ danh định 6000MHz theo tên sản phẩm. Phù hợp nhu cầu đa nhiệm trên hệ thống hỗ trợ DDR5.'),
    (1013, N'Samsung 990 PRO SSD 1TB NVMe', N'Ổ cứng Samsung 990 PRO SSD NVMe dung lượng 1TB, phù hợp lưu hệ điều hành, trò chơi, ứng dụng và dữ liệu. Cần kiểm tra khe kết nối tương thích trên máy tính.'),
    (1014, N'Samsung 990 PRO SSD 2TB NVMe', N'Ổ cứng Samsung 990 PRO SSD NVMe dung lượng 2TB, dành cho nhu cầu lưu trữ nhiều ứng dụng và dữ liệu. Cần kiểm tra khe kết nối tương thích trên máy tính.'),
    (1015, N'Kingston NV3 SSD 1TB NVMe', N'Ổ cứng Kingston NV3 SSD NVMe dung lượng 1TB phục vụ nâng cấp lưu trữ cho máy tính. Phù hợp cài hệ điều hành, ứng dụng và lưu các tệp cá nhân.'),
    (1016, N'ASUS Dual GeForce RTX 4060 8GB', N'Card đồ họa ASUS Dual GeForce RTX 4060 với bộ nhớ đồ họa 8GB theo tên sản phẩm. Phù hợp lắp ráp PC chơi game hoặc xử lý đồ họa trên hệ thống tương thích.'),
    (1017, N'ASUS Dual GeForce RTX 4070 SUPER 12GB', N'Card đồ họa ASUS Dual GeForce RTX 4070 SUPER với bộ nhớ đồ họa 12GB theo tên sản phẩm. Hướng đến PC chơi game và tác vụ đồ họa, cần kiểm tra nguồn điện và kích thước thùng máy.'),
    (1018, N'ASUS ROG STRIX 750G 750W', N'Bộ nguồn ASUS ROG STRIX 750G có công suất danh định 750W, cung cấp điện cho linh kiện trong máy tính để bàn. Cần đối chiếu công suất và đầu nối trước khi lắp đặt.'),
    (1019, N'ASUS TUF GAMING GT301 Case', N'Vỏ máy tính ASUS TUF GAMING GT301 dùng lắp ráp và bố trí các linh kiện trong bộ PC. Cần kiểm tra kích thước bo mạch chủ, card đồ họa và tản nhiệt trước khi chọn mua.');

    DECLARE @SoCapNhat INT = 0;

    EXEC sys.sp_executesql
        N'UPDATE sp
          SET sp.MOTASP = m.MOTASP
          FROM dbo.SANPHAM AS sp
          INNER JOIN #MoTaMau AS m ON sp.MSP = m.MSP AND sp.TEN = m.TEN
          WHERE NULLIF(LTRIM(RTRIM(sp.MOTASP)), N'''') IS NULL;
          SET @DaCapNhat = @@ROWCOUNT;',
        N'@DaCapNhat INT OUTPUT',
        @DaCapNhat = @SoCapNhat OUTPUT;

    DECLARE @DinhNghiaView NVARCHAR(MAX) = OBJECT_DEFINITION(OBJECT_ID(N'dbo.vw_Catalog', N'V'));
    DECLARE @BieuThucCu NVARCHAR(250) = N'CAST(N'''' AS NVARCHAR(MAX)) AS Description';
    DECLARE @BieuThucMoi NVARCHAR(250) = N'CAST(COALESCE(sp.MOTASP, N'''') AS NVARCHAR(MAX)) AS Description';
    DECLARE @KhaiBaoCu NVARCHAR(200) = N'CREATE   VIEW [dbo].[vw_Catalog]';

    IF @DinhNghiaView IS NOT NULL
       AND CHARINDEX(@BieuThucCu, @DinhNghiaView) > 0
       AND CHARINDEX(@KhaiBaoCu, @DinhNghiaView) > 0
    BEGIN
        SET @DinhNghiaView = REPLACE(@DinhNghiaView, @BieuThucCu, @BieuThucMoi);
        SET @DinhNghiaView = REPLACE(@DinhNghiaView, @KhaiBaoCu, N'ALTER VIEW [dbo].[vw_Catalog]');
        EXEC sys.sp_executesql @DinhNghiaView;
    END
    ELSE IF @DinhNghiaView IS NULL
        PRINT N'Khong tim thay vw_Catalog: chi cap nhat SANPHAM, khong tao view moi.';
    ELSE IF CHARINDEX(N'sp.MOTASP', @DinhNghiaView) = 0
        PRINT N'vw_Catalog da duoc nhom sua: khong ghi de view, can kiem tra thu cong cot Description.';

    COMMIT TRANSACTION;

    SELECT @SoCapNhat AS SoSanPhamVuaThemMoTa;
    EXEC sys.sp_executesql
        N'SELECT COUNT(*) AS SoSanPhamMauCoMoTa
          FROM dbo.SANPHAM AS sp
          INNER JOIN #MoTaMau AS m ON sp.MSP = m.MSP AND sp.TEN = m.TEN
          WHERE NULLIF(LTRIM(RTRIM(sp.MOTASP)), N'''') IS NOT NULL;';

    IF OBJECT_ID(N'dbo.vw_Catalog', N'V') IS NOT NULL
        SELECT TOP (5) ProductId, ProductName, Description
        FROM dbo.vw_Catalog
        ORDER BY ProductId;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
