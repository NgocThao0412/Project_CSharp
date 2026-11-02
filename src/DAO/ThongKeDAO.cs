
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using src.config;
using src.DTO.ThongKe;

namespace src.DAO
{
    public class ThongKeDAO
    {
        private static ThongKeDAO instance;

        public static ThongKeDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new ThongKeDAO();

                return instance;
            }
        }

        private ThongKeDAO() { }

        // Hàm chuyển số tiền từ SQL Server.
        private static long GetLong(
            SqlDataReader rs, string column)
        {
            int index = rs.GetOrdinal(column);

            return rs.IsDBNull(index)
                ? 0L
                : Convert.ToInt64(rs.GetValue(index));
        }

        // Đọc thống kê doanh thu, chi phí theo ngày.
        private static List<ThongKeTungNgayTrongThangDTO>
            GetThongKeTheoNgay(DateTime start, DateTime end)
        {
            var result =
                new List<ThongKeTungNgayTrongThangDTO>();

            if (end.Date < start.Date)
                return result;

            const string sql = @"
                WITH dates AS
                (
                    SELECT CAST(@start AS DATE) AS ngay

                    UNION ALL

                    SELECT DATEADD(DAY, 1, ngay)
                    FROM dates
                    WHERE ngay < CAST(@end AS DATE)
                ),
                nhap AS
                (
                    SELECT
                        CAST(TG AS DATE) AS ngay,
                        SUM(CAST(TIEN AS BIGINT)) AS chiphi
                    FROM PHIEUNHAP
                    WHERE TT = 1
                      AND TG >= @start
                      AND TG < DATEADD(
                          DAY, 1, CAST(@end AS DATE))
                    GROUP BY CAST(TG AS DATE)
                ),
                xuat AS
                (
                    SELECT
                        CAST(TG AS DATE) AS ngay,
                        SUM(CAST(TIEN AS BIGINT)) AS doanhthu
                    FROM PHIEUXUAT
                    WHERE TT = 1
                      AND TG >= @start
                      AND TG < DATEADD(
                          DAY, 1, CAST(@end AS DATE))
                    GROUP BY CAST(TG AS DATE)
                )
                SELECT
                    d.ngay,
                    COALESCE(n.chiphi, 0) AS chiphi,
                    COALESCE(x.doanhthu, 0) AS doanhthu
                FROM dates d
                LEFT JOIN nhap n ON n.ngay = d.ngay
                LEFT JOIN xuat x ON x.ngay = d.ngay
                ORDER BY d.ngay
                OPTION (MAXRECURSION 0)";

            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(
                        "@start", SqlDbType.DateTime
                    ).Value = start.Date;

                    cmd.Parameters.Add(
                        "@end", SqlDbType.DateTime
                    ).Value = end.Date;

                    using (SqlDataReader rs =
                        cmd.ExecuteReader())
                    {
                        while (rs.Read())
                        {
                            DateTime ngay =
                                rs.GetDateTime(
                                    rs.GetOrdinal("ngay"));

                            int chiPhi = checked((int)
                                GetLong(rs, "chiphi"));

                            int doanhThu = checked((int)
                                GetLong(rs, "doanhthu"));

                            int chenhLech = checked(
                                doanhThu - chiPhi);

                            result.Add(
                                new ThongKeTungNgayTrongThangDTO(
                                    ngay,
                                    chiPhi,
                                    doanhThu,
                                    chenhLech
                                ));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi thống kê theo ngày: "
                    + ex.Message);

                throw;
            }

            return result;
        }

        // 1. Thống kê tồn kho
        public List<ThongKeTonKhoDTO> GetThongKeTonKho(
            string text,
            DateTime timeStart,
            DateTime timeEnd)
        {
            var result = new List<ThongKeTonKhoDTO>();

            if (timeEnd.Date < timeStart.Date)
                return result;

            // Tồn đầu kỳ được suy ngược từ tồn hiện tại
            // và các phiếu đã duyệt kể từ đầu kỳ.
            // Phương pháp này cần sổ điều chỉnh kiểm kê
            // nếu muốn báo cáo tồn lịch sử chính xác.

            const string sql = @"
                WITH nhap AS
                (
                    SELECT
                        ct.MSP,
                        SUM(CAST(ct.SL AS BIGINT)) AS slnhap
                    FROM CTPHIEUNHAP ct
                    JOIN PHIEUNHAP pn ON pn.MPN = ct.MPN
                    WHERE pn.TT = 1
                      AND pn.TG >= @start
                      AND pn.TG < @endExclusive
                    GROUP BY ct.MSP
                ),
                xuat AS
                (
                    SELECT
                        ct.MSP,
                        SUM(CAST(ct.SL AS BIGINT)) AS slxuat
                    FROM CTPHIEUXUAT ct
                    JOIN PHIEUXUAT px ON px.MPX = ct.MPX
                    WHERE px.TT = 1
                      AND px.TG >= @start
                      AND px.TG < @endExclusive
                    GROUP BY ct.MSP
                ),
                nhap_sau AS
                (
                    SELECT
                        ct.MSP,
                        SUM(CAST(ct.SL AS BIGINT)) AS sl
                    FROM CTPHIEUNHAP ct
                    JOIN PHIEUNHAP pn ON pn.MPN = ct.MPN
                    WHERE pn.TT = 1
                      AND pn.TG >= @start
                    GROUP BY ct.MSP
                ),
                xuat_sau AS
                (
                    SELECT
                        ct.MSP,
                        SUM(CAST(ct.SL AS BIGINT)) AS sl
                    FROM CTPHIEUXUAT ct
                    JOIN PHIEUXUAT px ON px.MPX = ct.MPX
                    WHERE px.TT = 1
                      AND px.TG >= @start
                    GROUP BY ct.MSP
                )
                SELECT
                    sp.MSP,
                    sp.TEN,
                    CAST(sp.SL AS BIGINT)
                      - COALESCE(ns.sl, 0)
                      + COALESCE(xs.sl, 0)
                        AS SLdauky,
                    COALESCE(n.slnhap, 0) AS SLnhap,
                    COALESCE(x.slxuat, 0) AS SLxuat,
                    CAST(sp.SL AS BIGINT)
                      - COALESCE(ns.sl, 0)
                      + COALESCE(xs.sl, 0)
                      + COALESCE(n.slnhap, 0)
                      - COALESCE(x.slxuat, 0)
                        AS SLcuoiky
                FROM SANPHAM sp
                LEFT JOIN nhap n ON n.MSP = sp.MSP
                LEFT JOIN xuat x ON x.MSP = sp.MSP
                LEFT JOIN nhap_sau ns ON ns.MSP = sp.MSP
                LEFT JOIN xuat_sau xs ON xs.MSP = sp.MSP
                WHERE
                    sp.TEN LIKE @text
                    OR CAST(sp.MSP AS NVARCHAR(30))
                        LIKE @text
                ORDER BY sp.MSP";

            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(
                        "@start", SqlDbType.DateTime
                    ).Value = timeStart.Date;

                    cmd.Parameters.Add(
                        "@endExclusive", SqlDbType.DateTime
                    ).Value = timeEnd.Date.AddDays(1);

                    cmd.Parameters.Add(
                        "@text", SqlDbType.NVarChar, 255
                    ).Value = "%" + (text ?? "") + "%";

                    using (SqlDataReader rs =
                        cmd.ExecuteReader())
                    {
                        while (rs.Read())
                        {
                            int msp = rs.GetInt32(
                                rs.GetOrdinal("MSP"));

                            string ten = rs.GetString(
                                rs.GetOrdinal("TEN"));

                            int dauKy = checked((int)
                                GetLong(rs, "SLdauky"));

                            int nhap = checked((int)
                                GetLong(rs, "SLnhap"));

                            int xuat = checked((int)
                                GetLong(rs, "SLxuat"));

                            int cuoiKy = checked((int)
                                GetLong(rs, "SLcuoiky"));

                            result.Add(new ThongKeTonKhoDTO(
                                msp,
                                0,
                                ten,
                                0,
                                0,
                                "",
                                dauKy,
                                nhap,
                                xuat,
                                cuoiKy
                            ));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi GetThongKeTonKho: "
                    + ex.Message);
                throw;
            }

            return result;
        }

        // 2. Thống kê doanh thu theo năm
        public List<ThongKeDoanhThuDTO>
            GetDoanhThuTheoTungNam(
                int yearStart, int yearEnd)
        {
            var result = new List<ThongKeDoanhThuDTO>();

            if (yearStart < 1 ||
                yearEnd > 9999 ||
                yearStart > yearEnd)
                return result;

            const string sql = @"
                WITH years AS
                (
                    SELECT @startYear AS nam
                    UNION ALL
                    SELECT nam + 1
                    FROM years
                    WHERE nam < @endYear
                ),
                nhap AS
                (
                    SELECT
                        YEAR(TG) AS nam,
                        SUM(CAST(TIEN AS BIGINT)) AS chiphi
                    FROM PHIEUNHAP
                    WHERE TT = 1
                      AND TG >= @startDate
                      AND TG < @endDate
                    GROUP BY YEAR(TG)
                ),
                xuat AS
                (
                    SELECT
                        YEAR(TG) AS nam,
                        SUM(CAST(TIEN AS BIGINT)) AS doanhthu
                    FROM PHIEUXUAT
                    WHERE TT = 1
                      AND TG >= @startDate
                      AND TG < @endDate
                    GROUP BY YEAR(TG)
                )
                SELECT
                    y.nam,
                    COALESCE(n.chiphi, 0) AS chiphi,
                    COALESCE(x.doanhthu, 0) AS doanhthu
                FROM years y
                LEFT JOIN nhap n ON n.nam = y.nam
                LEFT JOIN xuat x ON x.nam = y.nam
                ORDER BY y.nam
                OPTION (MAXRECURSION 0)";

            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(
                        "@startYear", SqlDbType.Int
                    ).Value = yearStart;

                    cmd.Parameters.Add(
                        "@endYear", SqlDbType.Int
                    ).Value = yearEnd;

                    cmd.Parameters.Add(
                        "@startDate", SqlDbType.Date
                    ).Value = new DateTime(yearStart, 1, 1);

                    // Không dùng DATEADD trên 31/12/9999.
                    DateTime endDate = yearEnd == 9999
                        ? DateTime.MaxValue
                        : new DateTime(yearEnd + 1, 1, 1);

                    cmd.Parameters.Add(
                        "@endDate", SqlDbType.DateTime2
                    ).Value = endDate;

                    using (SqlDataReader rs =
                        cmd.ExecuteReader())
                    {
                        while (rs.Read())
                        {
                            int nam = Convert.ToInt32(
                                rs["nam"]);

                            long chiPhi =
                                GetLong(rs, "chiphi");

                            long doanhThu =
                                GetLong(rs, "doanhthu");

                            result.Add(new ThongKeDoanhThuDTO(
                                nam,
                                chiPhi,
                                doanhThu,
                                doanhThu - chiPhi
                            ));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi GetDoanhThuTheoTungNam: "
                    + ex.Message);
                throw;
            }

            return result;
        }

        // 3. Thống kê khách hàng
        public List<ThongKeKhachHangDTO>
            GetThongKeKhachHang(
                string text,
                DateTime timeStart,
                DateTime timeEnd)
        {
            var result = new List<ThongKeKhachHangDTO>();

            if (timeEnd.Date < timeStart.Date)
                return result;

            const string sql = @"
                SELECT
                    kh.MKH,
                    kh.HOTEN,
                    COUNT_BIG(px.MPX) AS SL,
                    COALESCE(
                        SUM(CAST(px.TIEN AS BIGINT)), 0
                    ) AS total
                FROM KHACHHANG kh
                JOIN PHIEUXUAT px
                    ON px.MKH = kh.MKH
                WHERE px.TT = 1
                  AND px.TG >= @start
                  AND px.TG < @endExclusive
                  AND (
                      kh.HOTEN LIKE @text
                      OR CAST(kh.MKH AS NVARCHAR(30))
                          LIKE @text
                  )
                GROUP BY kh.MKH, kh.HOTEN
                ORDER BY total DESC";

            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add(
                    "@start", SqlDbType.DateTime
                ).Value = timeStart.Date;

                cmd.Parameters.Add(
                    "@endExclusive", SqlDbType.DateTime
                ).Value = timeEnd.Date.AddDays(1);

                cmd.Parameters.Add(
                    "@text", SqlDbType.NVarChar, 255
                ).Value = "%" + (text ?? "") + "%";

                using (SqlDataReader rs =
                    cmd.ExecuteReader())
                {
                    while (rs.Read())
                    {
                        result.Add(new ThongKeKhachHangDTO(
                            Convert.ToInt32(rs["MKH"]),
                            Convert.ToString(rs["HOTEN"]) ?? "",
                            checked((int)GetLong(rs, "SL")),
                            GetLong(rs, "total")
                        ));
                    }
                }
            }

            return result;
        }

        // 4. Thống kê nhà cung cấp
        public List<ThongKeNhaCungCapDTO>
            GetThongKeNCC(
                string text,
                DateTime timeStart,
                DateTime timeEnd)
        {
            var result = new List<ThongKeNhaCungCapDTO>();

            if (timeEnd.Date < timeStart.Date)
                return result;

            const string sql = @"
                SELECT
                    ncc.MNCC,
                    ncc.TEN,
                    COUNT_BIG(pn.MPN) AS SL,
                    COALESCE(
                        SUM(CAST(pn.TIEN AS BIGINT)), 0
                    ) AS total
                FROM NHACUNGCAP ncc
                JOIN PHIEUNHAP pn
                    ON pn.MNCC = ncc.MNCC
                WHERE pn.TT = 1
                  AND pn.TG >= @start
                  AND pn.TG < @endExclusive
                  AND (
                      ncc.TEN LIKE @text
                      OR CAST(ncc.MNCC AS NVARCHAR(30))
                          LIKE @text
                  )
                GROUP BY ncc.MNCC, ncc.TEN
                ORDER BY total DESC";

            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add(
                    "@start", SqlDbType.DateTime
                ).Value = timeStart.Date;

                cmd.Parameters.Add(
                    "@endExclusive", SqlDbType.DateTime
                ).Value = timeEnd.Date.AddDays(1);

                cmd.Parameters.Add(
                    "@text", SqlDbType.NVarChar, 255
                ).Value = "%" + (text ?? "") + "%";

                using (SqlDataReader rs =
                    cmd.ExecuteReader())
                {
                    while (rs.Read())
                    {
                        result.Add(new ThongKeNhaCungCapDTO(
                            Convert.ToInt32(rs["MNCC"]),
                            Convert.ToString(rs["TEN"]) ?? "",
                            checked((int)GetLong(rs, "SL")),
                            GetLong(rs, "total")
                        ));
                    }
                }
            }

            return result;
        }

        // 5. Thống kê 12 tháng trong một năm
        public List<ThongKeTheoThangDTO>
            GetThongKeTheoThang(int nam)
        {
            var result = new List<ThongKeTheoThangDTO>();

            if (nam < 1 || nam > 9999)
                return result;

            const string sql = @"
                WITH months AS
                (
                    SELECT 1 AS thang
                    UNION ALL
                    SELECT thang + 1
                    FROM months
                    WHERE thang < 12
                ),
                nhap AS
                (
                    SELECT
                        MONTH(TG) AS thang,
                        SUM(CAST(TIEN AS BIGINT)) AS chiphi
                    FROM PHIEUNHAP
                    WHERE TT = 1
                      AND YEAR(TG) = @nam
                    GROUP BY MONTH(TG)
                ),
                xuat AS
                (
                    SELECT
                        MONTH(TG) AS thang,
                        SUM(CAST(TIEN AS BIGINT)) AS doanhthu
                    FROM PHIEUXUAT
                    WHERE TT = 1
                      AND YEAR(TG) = @nam
                    GROUP BY MONTH(TG)
                )
                SELECT
                    m.thang,
                    COALESCE(n.chiphi, 0) AS chiphi,
                    COALESCE(x.doanhthu, 0) AS doanhthu
                FROM months m
                LEFT JOIN nhap n ON n.thang = m.thang
                LEFT JOIN xuat x ON x.thang = m.thang
                ORDER BY m.thang";

            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            using (SqlCommand cmd =
                new SqlCommand(sql, conn))
            {
                cmd.Parameters.Add(
                    "@nam", SqlDbType.Int
                ).Value = nam;

                using (SqlDataReader rs =
                    cmd.ExecuteReader())
                {
                    while (rs.Read())
                    {
                        int thang =
                            Convert.ToInt32(rs["thang"]);

                        int chiPhi = checked((int)
                            GetLong(rs, "chiphi"));

                        int doanhThu = checked((int)
                            GetLong(rs, "doanhthu"));

                        result.Add(new ThongKeTheoThangDTO(
                            thang,
                            chiPhi,
                            doanhThu,
                            checked(doanhThu - chiPhi)
                        ));
                    }
                }
            }

            return result;
        }

        // 6. Thống kê từng ngày trong một tháng
        public List<ThongKeTungNgayTrongThangDTO>
            GetThongKeTungNgayTrongThang(
                int thang, int nam)
        {
            if (nam < 1 || nam > 9999 ||
                thang < 1 || thang > 12)
            {
                return new List<
                    ThongKeTungNgayTrongThangDTO>();
            }

            DateTime start = new DateTime(nam, thang, 1);

            DateTime end = new DateTime(
                nam,
                thang,
                DateTime.DaysInMonth(nam, thang));

            return GetThongKeTheoNgay(start, end);
        }

        // 7. Thống kê 7 ngày gần nhất
        // Bao gồm hôm nay và 6 ngày trước đó.
        public List<ThongKeTungNgayTrongThangDTO>
            GetThongKe7NgayGanNhat()
        {
            DateTime end = DateTime.Today;
            DateTime start = end.AddDays(-6);

            return GetThongKeTheoNgay(start, end);
        }

        // 8. Thống kê theo khoảng ngày
        public List<ThongKeTungNgayTrongThangDTO>
            GetThongKeTuNgayDenNgay(
                string start, string end)
        {
            if (!DateTime.TryParse(
                    start, out DateTime startDate)
                || !DateTime.TryParse(
                    end, out DateTime endDate))
            {
                return new List<
                    ThongKeTungNgayTrongThangDTO>();
            }

            return GetThongKeTheoNgay(
                startDate.Date,
                endDate.Date);
        }
    }
}
