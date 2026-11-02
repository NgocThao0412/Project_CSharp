
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using src.DTO;
using src.config;

namespace src.DAO
{
    public class PhieuKiemKeDAO : DAOinterface<PhieuKiemKeDTO>
    {
        private static PhieuKiemKeDAO instance;

        public static PhieuKiemKeDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new PhieuKiemKeDAO();

                return instance;
            }
        }

        private PhieuKiemKeDAO() { }

        // Chuyển dữ liệu SQL Server thành DTO
        private static PhieuKiemKeDTO ReadPhieuKiemKe(
            SqlDataReader rs)
        {
            int mpkk = rs.GetInt32(rs.GetOrdinal("MPKK"));
            int mnv = rs.GetInt32(rs.GetOrdinal("MNV"));
            DateTime tg = rs.GetDateTime(rs.GetOrdinal("TG"));
            int tt = rs.GetInt32(rs.GetOrdinal("TT"));

            return new PhieuKiemKeDTO(mpkk, mnv, tg, tt);
        }

        // 1. Thêm phiếu kiểm kê
        // TT = 2: chờ duyệt
        // Trả về mã phiếu vừa tạo
        public int insert(PhieuKiemKeDTO t)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    const string sql = @"
                        INSERT INTO PHIEUKIEMKE
                            (MNV, TG, TT)
                        OUTPUT INSERTED.MPKK
                        VALUES
                            (@mnv, @tg, 2)";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@mnv", SqlDbType.Int
                        ).Value = t.MNV;

                        cmd.Parameters.Add(
                            "@tg", SqlDbType.DateTime
                        ).Value = t.TG;

                        int newId = Convert.ToInt32(
                            cmd.ExecuteScalar());

                        t.MPKK = newId;
                        return newId;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Insert PhieuKiemKe: " + ex.Message);

                return 0;
            }
        }

        // 2. Cập nhật phiếu kiểm kê
        // Source gốc chưa hỗ trợ phương thức này
        public int update(PhieuKiemKeDTO t)
        {
            throw new NotImplementedException(
                "Chức năng cập nhật phiếu kiểm kê " +
                "chưa được hỗ trợ."
            );
        }

        // 3. Xóa mềm phiếu kiểm kê
        // Chỉ xóa phiếu đang chờ duyệt
        public int delete(string t)
        {
            if (!int.TryParse(t, out int mpkk))
                return 0;

            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    const string sql = @"
                        UPDATE PHIEUKIEMKE
                        SET TT = 0
                        WHERE MPKK = @mpkk
                          AND TT = 2";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@mpkk", SqlDbType.Int
                        ).Value = mpkk;

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Delete PhieuKiemKe: " + ex.Message);

                return 0;
            }
        }

        // 4. Lấy toàn bộ phiếu kiểm kê
        public List<PhieuKiemKeDTO> selectAll()
        {
            List<PhieuKiemKeDTO> result =
                new List<PhieuKiemKeDTO>();

            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            {
                const string sql = @"
                    SELECT MPKK, MNV, TG, TT
                    FROM PHIEUKIEMKE
                    ORDER BY MPKK DESC";

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                using (SqlDataReader rs =
                    cmd.ExecuteReader())
                {
                    while (rs.Read())
                    {
                        result.Add(ReadPhieuKiemKe(rs));
                    }
                }
            }

            return result;
        }

        // 5. Lấy phiếu kiểm kê theo mã
        public PhieuKiemKeDTO selectById(string t)
        {
            if (!int.TryParse(t, out int mpkk))
                return null;

            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            {
                const string sql = @"
                    SELECT MPKK, MNV, TG, TT
                    FROM PHIEUKIEMKE
                    WHERE MPKK = @mpkk";

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(
                        "@mpkk", SqlDbType.Int
                    ).Value = mpkk;

                    using (SqlDataReader rs =
                        cmd.ExecuteReader())
                    {
                        if (rs.Read())
                        {
                            return ReadPhieuKiemKe(rs);
                        }
                    }
                }
            }

            return null;
        }

        // 6. Duyệt phiếu kiểm kê
        // TT = 2: chờ duyệt
        // TT = 1: đã duyệt
        //
        // TRANGTHAISP là số lượng thực tế kiểm kê.
        // Tồn kho sẽ được điều chỉnh về số lượng thực tế.
        //
        // Toàn bộ xử lý nằm trong một transaction.
        public int DuyetPhieuKiemKe(int maphieu)
        {
            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            using (SqlTransaction transaction =
                conn.BeginTransaction(
                    IsolationLevel.Serializable))
            {
                try
                {
                    // 6.1. Khóa phiếu để tránh duyệt hai lần
                    const string checkSql = @"
                        SELECT TT
                        FROM PHIEUKIEMKE WITH (UPDLOCK, HOLDLOCK)
                        WHERE MPKK = @mpkk";

                    using (SqlCommand checkCmd =
                        new SqlCommand(
                            checkSql, conn, transaction))
                    {
                        checkCmd.Parameters.Add(
                            "@mpkk", SqlDbType.Int
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

                    // 6.2. Đọc chi tiết kiểm kê
                    const string detailSql = @"
                        SELECT MSP, TRANGTHAISP
                        FROM CTPHIEUKIEMKE
                        WHERE MPKK = @mpkk
                        ORDER BY MSP";

                    var details =
                        new List<(int Msp, int SoLuongThucTe)>();

                    using (SqlCommand cmd =
                        new SqlCommand(
                            detailSql, conn, transaction))
                    {
                        cmd.Parameters.Add(
                            "@mpkk", SqlDbType.Int
                        ).Value = maphieu;

                        using (SqlDataReader rs =
                            cmd.ExecuteReader())
                        {
                            while (rs.Read())
                            {
                                details.Add((
                                    rs.GetInt32(0),
                                    rs.GetInt32(1)
                                ));
                            }
                        }
                    }

                    // Không duyệt phiếu không có chi tiết
                    if (details.Count == 0)
                    {
                        transaction.Rollback();
                        return 0;
                    }

                    // 6.3. Kiểm tra dữ liệu kiểm kê
                    var maSanPhamDaXuLy = new HashSet<int>();

                    foreach (var item in details)
                    {
                        if (item.SoLuongThucTe < 0)
                        {
                            throw new InvalidOperationException(
                                "Số lượng kiểm kê không được âm."
                            );
                        }

                        if (!maSanPhamDaXuLy.Add(item.Msp))
                        {
                            throw new InvalidOperationException(
                                "Sản phẩm xuất hiện nhiều lần " +
                                "trong cùng phiếu kiểm kê. MSP = " +
                                item.Msp
                            );
                        }
                    }

                    // 6.4. Điều chỉnh tồn kho
                    foreach (var item in details)
                    {
                        // Khóa sản phẩm và kiểm tra tồn tại
                        const string stockSql = @"
                            SELECT SL
                            FROM SANPHAM WITH (UPDLOCK, HOLDLOCK)
                            WHERE MSP = @msp
                              AND TT = 1";

                        using (SqlCommand checkStock =
                            new SqlCommand(
                                stockSql, conn, transaction))
                        {
                            checkStock.Parameters.Add(
                                "@msp", SqlDbType.Int
                            ).Value = item.Msp;

                            object stock =
                                checkStock.ExecuteScalar();

                            if (stock == null ||
                                stock == DBNull.Value)
                            {
                                throw new InvalidOperationException(
                                    "Không tìm thấy sản phẩm " +
                                    "đang hoạt động. MSP = " +
                                    item.Msp
                                );
                            }
                        }

                        // Đặt tồn kho bằng số lượng thực tế
                        const string updateStockSql = @"
                            UPDATE SANPHAM
                            SET SL = @sl
                            WHERE MSP = @msp
                              AND TT = 1";

                        using (SqlCommand cmd =
                            new SqlCommand(
                                updateStockSql, conn, transaction))
                        {
                            cmd.Parameters.Add(
                                "@sl", SqlDbType.Int
                            ).Value = item.SoLuongThucTe;

                            cmd.Parameters.Add(
                                "@msp", SqlDbType.Int
                            ).Value = item.Msp;

                            if (cmd.ExecuteNonQuery() != 1)
                            {
                                throw new InvalidOperationException(
                                    "Không cập nhật được " +
                                    "tồn kho sản phẩm " +
                                    item.Msp
                                );
                            }
                        }
                    }

                    // 6.5. Chuyển trạng thái đã duyệt
                    const string approveSql = @"
                        UPDATE PHIEUKIEMKE
                        SET TT = 1
                        WHERE MPKK = @mpkk
                          AND TT = 2";

                    using (SqlCommand cmd =
                        new SqlCommand(
                            approveSql, conn, transaction))
                    {
                        cmd.Parameters.Add(
                            "@mpkk", SqlDbType.Int
                        ).Value = maphieu;

                        if (cmd.ExecuteNonQuery() != 1)
                        {
                            throw new InvalidOperationException(
                                "Không cập nhật được " +
                                "trạng thái phiếu kiểm kê."
                            );
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
                    catch
                    {
                        // Transaction có thể đã kết thúc
                    }

                    Console.WriteLine(
                        "Lỗi DuyetPhieuKiemKe: " +
                        ex.Message
                    );

                    return 0;
                }
            }
        }

        // 7. Lấy mã IDENTITY tiếp theo để tham khảo
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
                                OBJECT_ID(N'dbo.PHIEUKIEMKE'),
                                N'MPKK', 'IsIdentity'
                            ) = 1
                            THEN CAST(
                                IDENT_CURRENT('dbo.PHIEUKIEMKE')
                                + IDENT_INCR('dbo.PHIEUKIEMKE')
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
                    "Lỗi getAutoIncrement PhieuKiemKe: " +
                    ex.Message
                );

                return -1;
            }
        }
    }
}
