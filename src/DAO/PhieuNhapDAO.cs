
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using src.DTO;
using src.config;

namespace src.DAO
{
    public class PhieuNhapDAO : DAOinterface<PhieuNhapDTO>
    {
        private static PhieuNhapDAO instance;

        public static PhieuNhapDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new PhieuNhapDAO();

                return instance;
            }
        }

        private PhieuNhapDAO() { }

        // Chuyển dữ liệu SQL Server thành DTO
        private static PhieuNhapDTO ReadPhieuNhap(
            SqlDataReader rs)
        {
            return new PhieuNhapDTO(
                rs.GetInt32(rs.GetOrdinal("MPN")),
                rs.GetInt32(rs.GetOrdinal("MNCC")),
                rs.GetInt32(rs.GetOrdinal("MNV")),
                rs.GetInt32(rs.GetOrdinal("TIEN")),
                rs.GetDateTime(rs.GetOrdinal("TG")),
                rs.GetInt32(rs.GetOrdinal("TT"))
            );
        }

        // 1. Thêm phiếu nhập, mặc định chờ duyệt TT = 2
        // Trả về mã phiếu nhập mới
        public int insert(PhieuNhapDTO t)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    const string sql = @"
                        INSERT INTO PHIEUNHAP
                            (MNV, MNCC, TIEN, TG, TT)
                        OUTPUT INSERTED.MPN
                        VALUES
                            (@mnv, @mncc, @tien, @tg, 2)";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@mnv", SqlDbType.Int
                        ).Value = t.MNV;

                        cmd.Parameters.Add(
                            "@mncc", SqlDbType.Int
                        ).Value = t.MNCC;

                        cmd.Parameters.Add(
                            "@tien", SqlDbType.Int
                        ).Value = t.TIEN;

                        cmd.Parameters.Add(
                            "@tg", SqlDbType.DateTime
                        ).Value = t.TG;

                        return Convert.ToInt32(
                            cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Insert PhieuNhap: " + ex.Message
                );
                return 0;
            }
        }

        // 2. Cập nhật phiếu nhập khi còn chờ duyệt
        public int update(PhieuNhapDTO t)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    const string sql = @"
                        UPDATE PHIEUNHAP
                        SET TG = @tg,
                            MNCC = @mncc,
                            TIEN = @tien
                        WHERE MPN = @mpn
                          AND TT = 2";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@tg", SqlDbType.DateTime
                        ).Value = t.TG;

                        cmd.Parameters.Add(
                            "@mncc", SqlDbType.Int
                        ).Value = t.MNCC;

                        cmd.Parameters.Add(
                            "@tien", SqlDbType.Int
                        ).Value = t.TIEN;

                        cmd.Parameters.Add(
                            "@mpn", SqlDbType.Int
                        ).Value = t.MPN;

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Update PhieuNhap: " + ex.Message
                );
                return 0;
            }
        }

        // 3. Xóa mềm phiếu nhập chưa duyệt
        public int delete(string t)
        {
            if (!int.TryParse(t, out int mpn))
                return 0;

            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    const string sql = @"
                        UPDATE PHIEUNHAP
                        SET TT = 0
                        WHERE MPN = @mpn
                          AND TT = 2";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@mpn", SqlDbType.Int
                        ).Value = mpn;

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Delete PhieuNhap: " + ex.Message
                );
                return 0;
            }
        }

        // 4. Lấy tất cả phiếu nhập
        public List<PhieuNhapDTO> selectAll()
        {
            List<PhieuNhapDTO> result =
                new List<PhieuNhapDTO>();

            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            {
                const string sql = @"
                    SELECT MPN, MNCC, MNV, TIEN, TG, TT
                    FROM PHIEUNHAP
                    ORDER BY MPN DESC";

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                using (SqlDataReader rs =
                    cmd.ExecuteReader())
                {
                    while (rs.Read())
                    {
                        result.Add(ReadPhieuNhap(rs));
                    }
                }
            }

            return result;
        }

        // 5. Lấy phiếu nhập theo mã
        public PhieuNhapDTO selectById(string t)
        {
            if (!int.TryParse(t, out int mpn))
                return null;

            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            {
                const string sql = @"
                    SELECT MPN, MNCC, MNV, TIEN, TG, TT
                    FROM PHIEUNHAP
                    WHERE MPN = @mpn";

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(
                        "@mpn", SqlDbType.Int
                    ).Value = mpn;

                    using (SqlDataReader rs =
                        cmd.ExecuteReader())
                    {
                        if (rs.Read())
                            return ReadPhieuNhap(rs);
                    }
                }
            }

            return null;
        }

        // 6. Thống kê phiếu nhập theo khoảng tiền
        public List<PhieuNhapDTO> statistical(
            long min, long max)
        {
            List<PhieuNhapDTO> result =
                new List<PhieuNhapDTO>();

            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            {
                const string sql = @"
                    SELECT MPN, MNCC, MNV, TIEN, TG, TT
                    FROM PHIEUNHAP
                    WHERE TIEN BETWEEN @min AND @max
                    ORDER BY MPN DESC";

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(
                        "@min", SqlDbType.BigInt
                    ).Value = min;

                    cmd.Parameters.Add(
                        "@max", SqlDbType.BigInt
                    ).Value = max;

                    using (SqlDataReader rs =
                        cmd.ExecuteReader())
                    {
                        while (rs.Read())
                        {
                            result.Add(ReadPhieuNhap(rs));
                        }
                    }
                }
            }

            return result;
        }

        // 7. Kiểm tra tồn kho có đủ để hoàn nhập
        // Chỉ có ý nghĩa đối với phiếu đã duyệt
        public bool checkSLPn(int maphieu)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    const string sql = @"
                        SELECT
                            CASE WHEN EXISTS
                            (
                                SELECT 1
                                FROM CTPHIEUNHAP ct
                                LEFT JOIN SANPHAM sp
                                    ON ct.MSP = sp.MSP
                                WHERE ct.MPN = @mpn
                                  AND (
                                      sp.MSP IS NULL
                                      OR sp.SL < ct.SL
                                  )
                            )
                            THEN 0 ELSE 1 END";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@mpn", SqlDbType.Int
                        ).Value = maphieu;

                        return Convert.ToInt32(
                            cmd.ExecuteScalar()) == 1;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi checkSLPn: " + ex.Message
                );
                return false;
            }
        }

        // 8. Hủy phiếu nhập
        // Chỉ cho phép hủy phiếu đang chờ duyệt.
        // Phiếu đã duyệt cần nghiệp vụ hoàn nhập riêng,
        // không được tự trừ kho và xóa lịch sử.
        public int cancelPhieuNhap(int maphieu)
        {
            return delete(maphieu.ToString());
        }

        // 9. Lấy mã tự tăng tham khảo
        public int getAutoIncrement()
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    const string sql = @"
                        SELECT CASE
                            WHEN COLUMNPROPERTY(
                                OBJECT_ID(N'dbo.PHIEUNHAP'),
                                N'MPN', 'IsIdentity'
                            ) = 1
                            THEN CAST(
                                IDENT_CURRENT('dbo.PHIEUNHAP')
                                + IDENT_INCR('dbo.PHIEUNHAP')
                                AS INT
                            )
                            ELSE NULL
                        END";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        object value = cmd.ExecuteScalar();

                        return value == null ||
                               value == DBNull.Value
                            ? -1
                            : Convert.ToInt32(value);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi getAutoIncrement PhieuNhap: "
                    + ex.Message
                );
                return -1;
            }
        }

        // 10. Duyệt phiếu nhập
        // TT: 2 (chờ duyệt) -> 1 (đã duyệt)
        // Cập nhật số lượng và giá nhập bình quân
        // trong cùng một transaction.
        public int DuyetPhieuNhap(int maphieu)
        {
            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            using (SqlTransaction transaction =
                conn.BeginTransaction(IsolationLevel.Serializable))
            {
                try
                {
                    // Khóa phiếu để tránh duyệt đồng thời.
                    const string checkSql = @"
                        SELECT TT
                        FROM PHIEUNHAP WITH (UPDLOCK, HOLDLOCK)
                        WHERE MPN = @mpn";

                    using (SqlCommand checkCmd =
                        new SqlCommand(
                            checkSql, conn, transaction))
                    {
                        checkCmd.Parameters.Add(
                            "@mpn", SqlDbType.Int
                        ).Value = maphieu;

                        object status =
                            checkCmd.ExecuteScalar();

                        if (status == null ||
                            status == DBNull.Value ||
                            Convert.ToInt32(status) != 2)
                        {
                            transaction.Rollback();
                            return 0;
                        }
                    }

                    // Đọc và khóa dữ liệu sản phẩm theo thứ tự
                    // để tính lại giá nhập bình quân.
                    const string detailSql = @"
                        SELECT MSP, SUM(SL) AS SL,
                               SUM(
                                   CAST(SL AS BIGINT) * TIENNHAP
                               ) AS GIATRI
                        FROM CTPHIEUNHAP
                        WHERE MPN = @mpn
                        GROUP BY MSP
                        ORDER BY MSP";

                    var details =
                        new List<(int Msp, int Sl, long GiaTri)>();

                    using (SqlCommand cmd =
                        new SqlCommand(
                            detailSql, conn, transaction))
                    {
                        cmd.Parameters.Add(
                            "@mpn", SqlDbType.Int
                        ).Value = maphieu;

                        using (SqlDataReader rs =
                            cmd.ExecuteReader())
                        {
                            while (rs.Read())
                            {
                                details.Add((
                                    rs.GetInt32(0),
                                    rs.GetInt32(1),
                                    rs.GetInt64(2)
                                ));
                            }
                        }
                    }

                    if (details.Count == 0)
                    {
                        transaction.Rollback();
                        return 0;
                    }

                    foreach (var item in details)
                    {
                        if (item.Sl <= 0)
                        {
                            transaction.Rollback();
                            return 0;
                        }

                        const string stockSql = @"
                            SELECT SL, TIENN
                            FROM SANPHAM WITH (UPDLOCK, HOLDLOCK)
                            WHERE MSP = @msp
                              AND TT = 1";

                        int slCu;
                        int giaNhapCu;

                        using (SqlCommand cmd =
                            new SqlCommand(
                                stockSql, conn, transaction))
                        {
                            cmd.Parameters.Add(
                                "@msp", SqlDbType.Int
                            ).Value = item.Msp;

                            using (SqlDataReader rs =
                                cmd.ExecuteReader())
                            {
                                if (!rs.Read())
                                {
                                    throw new InvalidOperationException(
                                        "Không tìm thấy sản phẩm đang hoạt động: "
                                        + item.Msp);
                                }

                                slCu = rs.GetInt32(0);
                                giaNhapCu = rs.GetInt32(1);
                            }
                        }

                        int tongSl = checked(slCu + item.Sl);

                        long giaTriCu =
                            (long)slCu * giaNhapCu;

                        int giaBinhQuan = checked((int)(
                            (giaTriCu + item.GiaTri) / tongSl));

                        // Giữ quy tắc giá bán = giá nhập + 10%
                        // từ source gốc.
                        int giaBan = checked(
                            (int)(giaBinhQuan * 1.1m));

                        const string updateStockSql = @"
                            UPDATE SANPHAM
                            SET SL = @sl,
                                TIENN = @tienn,
                                TIENX = @tienx
                            WHERE MSP = @msp
                              AND TT = 1";

                        using (SqlCommand cmd =
                            new SqlCommand(
                                updateStockSql, conn, transaction))
                        {
                            cmd.Parameters.Add(
                                "@sl", SqlDbType.Int
                            ).Value = tongSl;

                            cmd.Parameters.Add(
                                "@tienn", SqlDbType.Int
                            ).Value = giaBinhQuan;

                            cmd.Parameters.Add(
                                "@tienx", SqlDbType.Int
                            ).Value = giaBan;

                            cmd.Parameters.Add(
                                "@msp", SqlDbType.Int
                            ).Value = item.Msp;

                            if (cmd.ExecuteNonQuery() != 1)
                            {
                                throw new InvalidOperationException(
                                    "Không cập nhật được tồn kho sản phẩm "
                                    + item.Msp);
                            }
                        }
                    }

                    // Chỉ đánh dấu duyệt sau khi cập nhật kho.
                    const string approveSql = @"
                        UPDATE PHIEUNHAP
                        SET TT = 1
                        WHERE MPN = @mpn AND TT = 2";

                    using (SqlCommand cmd =
                        new SqlCommand(
                            approveSql, conn, transaction))
                    {
                        cmd.Parameters.Add(
                            "@mpn", SqlDbType.Int
                        ).Value = maphieu;

                        if (cmd.ExecuteNonQuery() != 1)
                        {
                            throw new InvalidOperationException(
                                "Không cập nhật được trạng thái phiếu nhập.");
                        }
                    }

                    transaction.Commit();
                    return 1;
                }
                catch (Exception ex)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch { }

                    Console.WriteLine(
                        "Lỗi DuyetPhieuNhap: " + ex.Message
                    );

                    return 0;
                }
            }
        }
    }
}
