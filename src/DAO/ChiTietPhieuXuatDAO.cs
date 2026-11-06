
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using src.DTO;
using src.config;

namespace src.DAO
{
    public class ChiTietPhieuXuatDAO
        : ChiTietInterface<ChiTietPhieuXuatDTO>
    {
        private static ChiTietPhieuXuatDAO instance;

        public static ChiTietPhieuXuatDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new ChiTietPhieuXuatDAO();

                return instance;
            }
        }

        private ChiTietPhieuXuatDAO() { }

        // Chuyển dữ liệu SQL Server thành DTO
        // MKM giữ giá trị 0 vì bảng chi tiết hiện
        // không có cột mã khuyến mãi.
        private static ChiTietPhieuXuatDTO ReadChiTiet(
            SqlDataReader rs)
        {
            int mpx = rs.GetInt32(rs.GetOrdinal("MPX"));
            int msp = rs.GetInt32(rs.GetOrdinal("MSP"));
            int sl = rs.GetInt32(rs.GetOrdinal("SL"));

            int tienxuat =
                rs.GetInt32(rs.GetOrdinal("TIENXUAT"));

            return new ChiTietPhieuXuatDTO(
                mpx, msp, 0, sl, tienxuat
            );
        }

        // Hàm dùng chung để thêm danh sách chi tiết
        private static int InsertDetails(
            SqlConnection conn,
            SqlTransaction transaction,
            List<ChiTietPhieuXuatDTO> details)
        {
            const string sql = @"
                INSERT INTO CTPHIEUXUAT
                    (MPX, MSP, SL, TIENXUAT)
                VALUES
                    (@mpx, @msp, @sl, @tienxuat)";

            int result = 0;

            foreach (ChiTietPhieuXuatDTO item in details)
            {
                if (item.SL <= 0 || item.TIENXUAT < 0)
                {
                    throw new ArgumentException(
                        "Số lượng phải lớn hơn 0 và giá xuất không âm."
                    );
                }

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn, transaction))
                {
                    cmd.Parameters.Add(
                        "@mpx", SqlDbType.Int
                    ).Value = item.MPX;

                    cmd.Parameters.Add(
                        "@msp", SqlDbType.Int
                    ).Value = item.MSP;

                    cmd.Parameters.Add(
                        "@sl", SqlDbType.Int
                    ).Value = item.SL;

                    cmd.Parameters.Add(
                        "@tienxuat", SqlDbType.Int
                    ).Value = item.TIENXUAT;

                    result += cmd.ExecuteNonQuery();
                }
            }

            return result;
        }

        // 1. Thêm chi tiết phiếu xuất
        // Chưa trừ số lượng tồn kho
        public int insert(List<ChiTietPhieuXuatDTO> t)
        {
            if (t == null)
                return -1;

            if (t.Count == 0)
                return 0;

            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            using (SqlTransaction transaction =
                conn.BeginTransaction())
            {
                try
                {
                    int result =
                        InsertDetails(conn, transaction, t);

                    transaction.Commit();
                    return result;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    Console.WriteLine(
                        "Lỗi Insert CTPhieuXuat: "
                        + ex.Message
                    );

                    return -1;
                }
            }
        }

        // 2. Thêm chi tiết từ giỏ hàng
        // Không trừ tồn kho tại bước này
        public int insertGH(List<ChiTietPhieuXuatDTO> t)
        {
            return insert(t);
        }

        // 3. Reset chi tiết phiếu xuất chưa duyệt
        // Không cộng tồn kho vì phiếu chưa duyệt
        // chưa được phép trừ kho.
        public int reset(List<ChiTietPhieuXuatDTO> t)
        {
            if (t == null)
                return -1;

            if (t.Count == 0)
                return 0;

            int mpx = t[0].MPX;

            foreach (ChiTietPhieuXuatDTO item in t)
            {
                if (item.MPX != mpx)
                    return -1;
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
                        // Chỉ cho reset phiếu chờ duyệt (TT=2)
                        const string checkSql = @"
                            SELECT TT
                            FROM PHIEUXUAT WITH (UPDLOCK, HOLDLOCK)
                            WHERE MPX = @mpx";

                        using (SqlCommand checkCmd =
                            new SqlCommand(
                                checkSql, conn, transaction))
                        {
                            checkCmd.Parameters.Add(
                                "@mpx", SqlDbType.Int
                            ).Value = mpx;

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

                        const string deleteSql = @"
                            DELETE FROM CTPHIEUXUAT
                            WHERE MPX = @mpx";

                        int result;

                        using (SqlCommand cmd =
                            new SqlCommand(
                                deleteSql, conn, transaction))
                        {
                            cmd.Parameters.Add(
                                "@mpx", SqlDbType.Int
                            ).Value = mpx;

                            result = cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return result;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Reset CTPhieuXuat: " + ex.Message
                );

                return -1;
            }
        }

        // 4. Xóa chi tiết theo mã phiếu xuất
        // Chỉ áp dụng khi phiếu đang chờ duyệt
        public int delete(string t)
        {
            if (!int.TryParse(t, out int mpx))
                return -1;

            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                using (SqlTransaction transaction =
                    conn.BeginTransaction())
                {
                    try
                    {
                        const string checkSql = @"
                            SELECT TT
                            FROM PHIEUXUAT WITH (UPDLOCK, HOLDLOCK)
                            WHERE MPX = @mpx";

                        using (SqlCommand checkCmd =
                            new SqlCommand(
                                checkSql, conn, transaction))
                        {
                            checkCmd.Parameters.Add(
                                "@mpx", SqlDbType.Int
                            ).Value = mpx;

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

                        const string sql = @"
                            DELETE FROM CTPHIEUXUAT
                            WHERE MPX = @mpx";

                        int result;

                        using (SqlCommand cmd =
                            new SqlCommand(
                                sql, conn, transaction))
                        {
                            cmd.Parameters.Add(
                                "@mpx", SqlDbType.Int
                            ).Value = mpx;

                            result = cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        return result;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Delete CTPhieuXuat: " + ex.Message
                );

                return -1;
            }
        }

        // 5. Cập nhật chi tiết phiếu xuất
        // Xóa cũ và thêm mới trong cùng transaction
        public int update(
            List<ChiTietPhieuXuatDTO> t,
            string pk)
        {
            if (t == null)
                return -1;

            if (!int.TryParse(pk, out int mpx))
                return -1;

            foreach (ChiTietPhieuXuatDTO item in t)
            {
                if (item.MPX != mpx)
                    return -1;
            }

            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            using (SqlTransaction transaction =
                conn.BeginTransaction())
            {
                try
                {
                    const string checkSql = @"
                        SELECT TT
                        FROM PHIEUXUAT WITH (UPDLOCK, HOLDLOCK)
                        WHERE MPX = @mpx";

                    using (SqlCommand checkCmd =
                        new SqlCommand(
                            checkSql, conn, transaction))
                    {
                        checkCmd.Parameters.Add(
                            "@mpx", SqlDbType.Int
                        ).Value = mpx;

                        object status =
                            checkCmd.ExecuteScalar();

                        if (status == null ||
                            status == DBNull.Value ||
                            Convert.ToInt32(status) != 2)
                        {
                            transaction.Rollback();
                            return -1;
                        }
                    }

                    const string deleteSql = @"
                        DELETE FROM CTPHIEUXUAT
                        WHERE MPX = @mpx";

                    using (SqlCommand deleteCmd =
                        new SqlCommand(
                            deleteSql, conn, transaction))
                    {
                        deleteCmd.Parameters.Add(
                            "@mpx", SqlDbType.Int
                        ).Value = mpx;

                        deleteCmd.ExecuteNonQuery();
                    }

                    int result =
                        InsertDetails(conn, transaction, t);

                    transaction.Commit();

                    return result;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    Console.WriteLine(
                        "Lỗi Update CTPhieuXuat: "
                        + ex.Message
                    );

                    return -1;
                }
            }
        }

        // 6. Lấy danh sách chi tiết phiếu xuất
        public List<ChiTietPhieuXuatDTO> selectAll(string t)
        {
            List<ChiTietPhieuXuatDTO> result =
                new List<ChiTietPhieuXuatDTO>();

            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            {
                const string sql = @"
                    SELECT MPX, MSP, SL, TIENXUAT
                    FROM CTPHIEUXUAT
                    WHERE MPX = @mpx
                    ORDER BY MSP";

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(
                        "@mpx", SqlDbType.Int
                    ).Value = int.Parse(t);

                    using (SqlDataReader rs =
                        cmd.ExecuteReader())
                    {
                        while (rs.Read())
                        {
                            result.Add(ReadChiTiet(rs));
                        }
                    }
                }
            }

            return result;
        }

        // 7. Cập nhật tồn kho cho phiếu xuất
        // Không cho phép gọi riêng vì có nguy cơ
        // trừ tồn kho nhiều lần.
        // Chức năng này sẽ được đưa vào
        // PhieuXuatDAO.DuyetPhieuXuat().
        public void updateSL(string t)
        {
            throw new InvalidOperationException(
                "Không cập nhật tồn kho trực tiếp từ " +
                "ChiTietPhieuXuatDAO. " +
                "Hãy duyệt phiếu xuất thông qua " +
                "PhieuXuatDAO.DuyetPhieuXuat()."
            );
        }
    }
}
