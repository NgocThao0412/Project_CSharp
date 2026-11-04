
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using src.DTO;
using src.config;

namespace src.DAO
{
    public class PhieuXuatDAO : DAOinterface<PhieuXuatDTO>
    {
        private static PhieuXuatDAO instance;

        public static PhieuXuatDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new PhieuXuatDAO();

                return instance;
            }
        }

        private PhieuXuatDAO() { }

        // Chuyển dữ liệu SQL Server thành DTO
        private static PhieuXuatDTO ReadPhieuXuat(
            SqlDataReader rs)
        {
            return new PhieuXuatDTO(
                rs.GetInt32(rs.GetOrdinal("MPX")),
                rs.GetInt32(rs.GetOrdinal("MNV")),
                rs.GetInt32(rs.GetOrdinal("MKH")),
                rs.GetInt32(rs.GetOrdinal("TIEN")),
                rs.GetDateTime(rs.GetOrdinal("TG")),
                rs.GetInt32(rs.GetOrdinal("TT"))
            );
        }

        // 1. Thêm phiếu xuất chưa duyệt
        // Giữ phương thức cũ để tương thích.
        // Khi tạo cả phiếu và chi tiết, dùng CreateWithDetails().
        public int insert(PhieuXuatDTO t)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    const string sql = @"
                        INSERT INTO PHIEUXUAT
                            (MNV, MKH, TIEN, TG, TT)
                        OUTPUT INSERTED.MPX
                        VALUES
                            (@mnv, @mkh, @tien, @tg, 2)";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@mnv", SqlDbType.Int)
                            .Value = t.MNV;

                        cmd.Parameters.Add("@mkh", SqlDbType.Int)
                            .Value = t.MKH;

                        cmd.Parameters.Add("@tien", SqlDbType.Int)
                            .Value = t.TIEN;

                        cmd.Parameters.Add("@tg", SqlDbType.DateTime)
                            .Value = t.TG;

                        return Convert.ToInt32(
                            cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Insert PhieuXuat: " + ex.Message);

                return 0;
            }
        }

        // 2. Cập nhật phần đầu phiếu chờ duyệt
        // Khi sửa cả chi tiết, dùng UpdateWithDetails().
        public int update(PhieuXuatDTO t)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    const string sql = @"
                        UPDATE PHIEUXUAT
                        SET MNV = @mnv,
                            MKH = @mkh,
                            TIEN = @tien,
                            TG = @tg
                        WHERE MPX = @mpx
                          AND TT = 2";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@mnv", SqlDbType.Int)
                            .Value = t.MNV;

                        cmd.Parameters.Add("@mkh", SqlDbType.Int)
                            .Value = t.MKH;

                        cmd.Parameters.Add("@tien", SqlDbType.Int)
                            .Value = t.TIEN;

                        cmd.Parameters.Add("@tg", SqlDbType.DateTime)
                            .Value = t.TG;

                        cmd.Parameters.Add("@mpx", SqlDbType.Int)
                            .Value = t.MPX;

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Update PhieuXuat: " + ex.Message);

                return 0;
            }
        }

        // 3. Xóa mềm phiếu chưa duyệt
        public int delete(string t)
        {
            if (!int.TryParse(t, out int mpx))
                return 0;

            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    const string sql = @"
                        UPDATE PHIEUXUAT
                        SET TT = 0
                        WHERE MPX = @mpx AND TT = 2";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@mpx", SqlDbType.Int)
                            .Value = mpx;

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Delete PhieuXuat: " + ex.Message);

                return 0;
            }
        }

        // 4. Lấy tất cả phiếu xuất
        public List<PhieuXuatDTO> selectAll()
        {
            List<PhieuXuatDTO> result =
                new List<PhieuXuatDTO>();

            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            {
                const string sql = @"
                    SELECT MPX, MNV, MKH, TIEN, TG, TT
                    FROM PHIEUXUAT
                    ORDER BY MPX DESC";

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                using (SqlDataReader rs =
                    cmd.ExecuteReader())
                {
                    while (rs.Read())
                    {
                        result.Add(ReadPhieuXuat(rs));
                    }
                }
            }

            return result;
        }

        // 5. Lấy phiếu theo mã
        public PhieuXuatDTO selectById(string t)
        {
            if (!int.TryParse(t, out int mpx))
                return null;

            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            {
                const string sql = @"
                    SELECT MPX, MNV, MKH, TIEN, TG, TT
                    FROM PHIEUXUAT
                    WHERE MPX = @mpx";

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@mpx", SqlDbType.Int)
                        .Value = mpx;

                    using (SqlDataReader rs =
                        cmd.ExecuteReader())
                    {
                        if (rs.Read())
                            return ReadPhieuXuat(rs);
                    }
                }
            }

            return null;
        }

        // 6. Lấy phiếu theo khách hàng
        public List<PhieuXuatDTO> selectByMKH(string t)
        {
            List<PhieuXuatDTO> result =
                new List<PhieuXuatDTO>();

            if (!int.TryParse(t, out int mkh))
                return result;

            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            {
                const string sql = @"
                    SELECT MPX, MNV, MKH, TIEN, TG, TT
                    FROM PHIEUXUAT
                    WHERE MKH = @mkh
                    ORDER BY MPX DESC";

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add("@mkh", SqlDbType.Int)
                        .Value = mkh;

                    using (SqlDataReader rs =
                        cmd.ExecuteReader())
                    {
                        while (rs.Read())
                        {
                            result.Add(ReadPhieuXuat(rs));
                        }
                    }
                }
            }

            return result;
        }

        // 7. Hủy phiếu chưa duyệt
        public int cancelPhieuXuat(int maphieu)
        {
            return delete(maphieu.ToString());
        }

        // 8. Kiểm tra tồn kho theo phiếu
        // Đây chỉ là kiểm tra sơ bộ.
        // Duyệt phiếu sẽ kiểm tra lại trong transaction.
        public bool checkSLPx(int maphieu)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    const string sql = @"
                        SELECT
                            ct.MSP,
                            SUM(CAST(ct.SL AS BIGINT))
                                AS SLYEUCAU,
                            sp.SL AS SLTON
                        FROM CTPHIEUXUAT ct
                        LEFT JOIN SANPHAM sp
                            ON ct.MSP = sp.MSP
                           AND sp.TT = 1
                        WHERE ct.MPX = @mpx
                        GROUP BY ct.MSP, sp.SL";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add("@mpx", SqlDbType.Int)
                            .Value = maphieu;

                        using (SqlDataReader rs =
                            cmd.ExecuteReader())
                        {
                            bool hasDetails = false;

                            while (rs.Read())
                            {
                                hasDetails = true;

                                long soLuongYeuCau =
                                    rs.GetInt64(1);

                                if (rs.IsDBNull(2))
                                    return false;

                                int soLuongTon = rs.GetInt32(2);

                                if (soLuongYeuCau <= 0 ||
                                    soLuongYeuCau > soLuongTon)
                                {
                                    return false;
                                }
                            }

                            return hasDetails;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi checkSLPx: " + ex.Message);

                return false;
            }
        }

        // 9. Lấy mã tự tăng tham khảo
        // Không dùng để tự tạo khóa chính.
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
                                OBJECT_ID(N'dbo.PHIEUXUAT'),
                                N'MPX',
                                'IsIdentity'
                            ) = 1
                            THEN CAST(
                                IDENT_CURRENT('dbo.PHIEUXUAT')
                                + IDENT_INCR('dbo.PHIEUXUAT')
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
                    "Lỗi getAutoIncrement: " + ex.Message);

                return -1;
            }
        }

        // 10. Giữ tương thích phương thức cũ
        public int updateDP(string t)
        {
            if (!int.TryParse(t, out int mpx))
                return 0;

            return DuyetPhieuXuat(mpx);
        }

        // 11. Duyệt phiếu và trừ kho
        // Chỉ duyệt phiếu TT = 2.
        // Phiếu và tồn kho cùng một transaction.
        public int DuyetPhieuXuat(int mpx)
        {
            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            using (SqlTransaction transaction =
                conn.BeginTransaction(
                    IsolationLevel.Serializable))
            {
                try
                {
                    const string checkSql = @"
                        SELECT TT
                        FROM PHIEUXUAT
                            WITH (UPDLOCK, HOLDLOCK)
                        WHERE MPX = @mpx";

                    using (SqlCommand cmd =
                        new SqlCommand(
                            checkSql, conn, transaction))
                    {
                        cmd.Parameters.Add("@mpx", SqlDbType.Int)
                            .Value = mpx;

                        object status = cmd.ExecuteScalar();

                        if (status == null ||
                            status == DBNull.Value ||
                            Convert.ToInt32(status) != 2)
                        {
                            transaction.Rollback();
                            return 0;
                        }
                    }

                    const string detailSql = @"
                        SELECT MSP,
                               SUM(CAST(SL AS BIGINT)) AS TONGSL
                        FROM CTPHIEUXUAT
                        WHERE MPX = @mpx
                        GROUP BY MSP
                        ORDER BY MSP";

                    var details =
                        new List<(int Msp, long SoLuong)>();

                    using (SqlCommand cmd =
                        new SqlCommand(
                            detailSql, conn, transaction))
                    {
                        cmd.Parameters.Add("@mpx", SqlDbType.Int)
                            .Value = mpx;

                        using (SqlDataReader rs =
                            cmd.ExecuteReader())
                        {
                            while (rs.Read())
                            {
                                details.Add((
                                    rs.GetInt32(0),
                                    rs.GetInt64(1)
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
                        if (item.SoLuong <= 0 ||
                            item.SoLuong > int.MaxValue)
                        {
                            throw new InvalidOperationException(
                                "Số lượng xuất không hợp lệ."
                            );
                        }

                        const string stockSql = @"
                            UPDATE SANPHAM
                            SET SL = SL - @soluong
                            WHERE MSP = @msp
                              AND TT = 1
                              AND SL >= @soluong";

                        using (SqlCommand cmd =
                            new SqlCommand(
                                stockSql, conn, transaction))
                        {
                            cmd.Parameters.Add("@msp", SqlDbType.Int)
                                .Value = item.Msp;

                            cmd.Parameters.Add(
                                "@soluong", SqlDbType.Int)
                                .Value = (int)item.SoLuong;

                            if (cmd.ExecuteNonQuery() != 1)
                            {
                                throw new InvalidOperationException(
                                    "Không đủ tồn kho hoặc sản phẩm " +
                                    "không hoạt động. MSP = " +
                                    item.Msp);
                            }
                        }
                    }

                    const string approveSql = @"
                        UPDATE PHIEUXUAT
                        SET TT = 1
                        WHERE MPX = @mpx AND TT = 2";

                    using (SqlCommand cmd =
                        new SqlCommand(
                            approveSql, conn, transaction))
                    {
                        cmd.Parameters.Add("@mpx", SqlDbType.Int)
                            .Value = mpx;

                        if (cmd.ExecuteNonQuery() != 1)
                        {
                            throw new InvalidOperationException(
                                "Không cập nhật được trạng thái phiếu."
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
                    catch { }

                    Console.WriteLine(
                        "Lỗi DuyetPhieuXuat: " + ex.Message);

                    return 0;
                }
            }
        }

        // 12. Tạo phiếu xuất và toàn bộ chi tiết
        // trong một transaction.
        public int CreateWithDetails(
            PhieuXuatDTO phieu,
            List<ChiTietPhieuXuatDTO> chiTiet)
        {
            if (phieu == null ||
                chiTiet == null ||
                chiTiet.Count == 0)
            {
                return 0;
            }

            if (chiTiet.Any(ct =>
                ct.MSP <= 0 ||
                ct.SL <= 0 ||
                ct.TIENXUAT < 0))
            {
                return 0;
            }

            if (chiTiet.GroupBy(ct => ct.MSP)
                       .Any(g => g.Count() > 1))
            {
                return 0;
            }
            // Tính tổng tiền trực tiếp từ chi tiết phiếu xuất
            int tongTien;

            try
            {
                long tong = chiTiet.Sum(ct =>
                    checked((long)ct.SL * ct.TIENXUAT));

                tongTien = checked((int)tong);
            }
            catch (OverflowException)
            {
                return 0;
            }

            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                using (SqlTransaction transaction =
                    conn.BeginTransaction())
                {
                    try
                    {
                        const string insertPhieuSql = @"
                            INSERT INTO PHIEUXUAT
                                (MNV, MKH, TIEN, TG, TT)
                            OUTPUT INSERTED.MPX
                            VALUES
                                (@mnv, @mkh, @tien, @tg, 2)";

                        int newMPX;

                        using (SqlCommand cmd =
                            new SqlCommand(
                                insertPhieuSql,
                                conn,
                                transaction))
                        {
                            cmd.Parameters.Add(
                                "@mnv", SqlDbType.Int
                            ).Value = phieu.MNV;

                            cmd.Parameters.Add(
                                "@mkh", SqlDbType.Int
                            ).Value = phieu.MKH;

                            cmd.Parameters.Add(
                                "@tien", SqlDbType.Int
                            ).Value = tongTien;

                            cmd.Parameters.Add(
                                "@tg", SqlDbType.DateTime
                            ).Value = phieu.TG;

                            newMPX = Convert.ToInt32(
                                cmd.ExecuteScalar());
                        }

                        const string insertDetailSql = @"
                            INSERT INTO CTPHIEUXUAT
                                (MPX, MSP, SL, TIENXUAT)
                            VALUES
                                (@mpx, @msp, @sl, @tienxuat)";

                        foreach (ChiTietPhieuXuatDTO ct in chiTiet)
                        {
                            using (SqlCommand cmd =
                                new SqlCommand(
                                    insertDetailSql,
                                    conn,
                                    transaction))
                            {
                                cmd.Parameters.Add(
                                    "@mpx", SqlDbType.Int
                                ).Value = newMPX;

                                cmd.Parameters.Add(
                                    "@msp", SqlDbType.Int
                                ).Value = ct.MSP;

                                cmd.Parameters.Add(
                                    "@sl", SqlDbType.Int
                                ).Value = ct.SL;

                                cmd.Parameters.Add(
                                    "@tienxuat", SqlDbType.Int
                                ).Value = ct.TIENXUAT;

                                if (cmd.ExecuteNonQuery() != 1)
                                {
                                    throw new InvalidOperationException(
                                        "Không lưu được sản phẩm " +
                                        ct.MSP);
                                }
                            }
                        }

                        transaction.Commit();

                        phieu.MPX = newMPX;

                        foreach (ChiTietPhieuXuatDTO ct in chiTiet)
                        {
                            ct.MPX = newMPX;
                        }

                        return newMPX;
                    }
                    catch
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch { }

                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi CreateWithDetails: " + ex.Message);

                return 0;
            }
        }

        // 13. Cập nhật phiếu xuất và toàn bộ chi tiết
        // trong một transaction.
        // Chỉ sửa phiếu chờ duyệt TT = 2.
        public bool UpdateWithDetails(
            PhieuXuatDTO phieu,
            List<ChiTietPhieuXuatDTO> chiTiet)
        {
            if (phieu == null ||
                chiTiet == null ||
                chiTiet.Count == 0)
            {
                return false;
            }

            if (chiTiet.Any(ct =>
                ct.MSP <= 0 ||
                ct.SL <= 0 ||
                ct.TIENXUAT < 0))
            {
                return false;
            }

            if (chiTiet.GroupBy(ct => ct.MSP)
                       .Any(g => g.Count() > 1))
            {
                return false;
            }
            // Tính lại tổng tiền từ chi tiết phiếu xuất
            int tongTien;

            try
            {
                long tong = chiTiet.Sum(ct =>
                    checked((long)ct.SL * ct.TIENXUAT));

                tongTien = checked((int)tong);
            }
            catch (OverflowException)
            {
                return false;
            }

            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            using (SqlTransaction transaction =
                conn.BeginTransaction(
                    IsolationLevel.Serializable))
            {
                try
                {
                    // 1. Khóa phiếu và kiểm tra trạng thái
                    const string checkSql = @"
                        SELECT TT
                        FROM PHIEUXUAT
                            WITH (UPDLOCK, HOLDLOCK)
                        WHERE MPX = @mpx";

                    using (SqlCommand cmd =
                        new SqlCommand(
                            checkSql, conn, transaction))
                    {
                        cmd.Parameters.Add("@mpx", SqlDbType.Int)
                            .Value = phieu.MPX;

                        object status = cmd.ExecuteScalar();

                        if (status == null ||
                            status == DBNull.Value ||
                            Convert.ToInt32(status) != 2)
                        {
                            transaction.Rollback();
                            return false;
                        }
                    }

                    // 2. Cập nhật phần đầu phiếu
                    const string updateSql = @"
                        UPDATE PHIEUXUAT
                        SET MNV = @mnv,
                            MKH = @mkh,
                            TIEN = @tien,
                            TG = @tg
                        WHERE MPX = @mpx
                          AND TT = 2";

                    using (SqlCommand cmd =
                        new SqlCommand(
                            updateSql, conn, transaction))
                    {
                        cmd.Parameters.Add("@mnv", SqlDbType.Int)
                            .Value = phieu.MNV;

                        cmd.Parameters.Add("@mkh", SqlDbType.Int)
                            .Value = phieu.MKH;

                        cmd.Parameters.Add("@tien", SqlDbType.Int)
                            .Value = tongTien;

                        cmd.Parameters.Add("@tg", SqlDbType.DateTime)
                            .Value = phieu.TG;

                        cmd.Parameters.Add("@mpx", SqlDbType.Int)
                            .Value = phieu.MPX;

                        if (cmd.ExecuteNonQuery() != 1)
                        {
                            throw new InvalidOperationException(
                                "Không cập nhật được phiếu xuất."
                            );
                        }
                    }

                    // 3. Xóa chi tiết cũ
                    const string deleteSql = @"
                        DELETE FROM CTPHIEUXUAT
                        WHERE MPX = @mpx";

                    using (SqlCommand cmd =
                        new SqlCommand(
                            deleteSql, conn, transaction))
                    {
                        cmd.Parameters.Add("@mpx", SqlDbType.Int)
                            .Value = phieu.MPX;

                        cmd.ExecuteNonQuery();
                    }

                    // 4. Thêm chi tiết mới
                    const string insertSql = @"
                        INSERT INTO CTPHIEUXUAT
                            (MPX, MSP, SL, TIENXUAT)
                        VALUES
                            (@mpx, @msp, @sl, @tienxuat)";

                    foreach (ChiTietPhieuXuatDTO ct in chiTiet)
                    {
                        using (SqlCommand cmd =
                            new SqlCommand(
                                insertSql, conn, transaction))
                        {
                            cmd.Parameters.Add(
                                "@mpx", SqlDbType.Int
                            ).Value = phieu.MPX;

                            cmd.Parameters.Add(
                                "@msp", SqlDbType.Int
                            ).Value = ct.MSP;

                            cmd.Parameters.Add(
                                "@sl", SqlDbType.Int
                            ).Value = ct.SL;

                            cmd.Parameters.Add(
                                "@tienxuat", SqlDbType.Int
                            ).Value = ct.TIENXUAT;

                            if (cmd.ExecuteNonQuery() != 1)
                            {
                                throw new InvalidOperationException(
                                    "Không lưu được sản phẩm " +
                                    ct.MSP);
                            }
                        }
                    }

                    transaction.Commit();

                    return true;
                }
                catch (Exception ex)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch { }

                    Console.WriteLine(
                        "Lỗi UpdateWithDetails: " + ex.Message);

                    return false;
                }
            }
        }
    }
}
