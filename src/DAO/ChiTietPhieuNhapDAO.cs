
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using src.DTO;
using src.config;

namespace src.DAO
{
    public class ChiTietPhieuNhapDAO
        : ChiTietInterface<ChiTietPhieuNhapDTO>
    {
        private static ChiTietPhieuNhapDAO instance;

        public static ChiTietPhieuNhapDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new ChiTietPhieuNhapDAO();

                return instance;
            }
        }

        private ChiTietPhieuNhapDAO() { }

        // Chuyển dữ liệu SQL Server thành DTO
        private static ChiTietPhieuNhapDTO ReadChiTiet(
            SqlDataReader rs)
        {
            int mpn = rs.GetInt32(rs.GetOrdinal("MPN"));
            int msp = rs.GetInt32(rs.GetOrdinal("MSP"));
            int sl = rs.GetInt32(rs.GetOrdinal("SL"));
            int tiennhap =
                rs.GetInt32(rs.GetOrdinal("TIENNHAP"));
            int hinhthuc =
                rs.GetInt32(rs.GetOrdinal("HINHTHUC"));

            return new ChiTietPhieuNhapDTO(
                mpn, msp, sl, tiennhap, hinhthuc
            );
        }

        // Dùng chung cho insert() và update()
        private static int InsertDetails(
            SqlConnection conn,
            SqlTransaction transaction,
            List<ChiTietPhieuNhapDTO> details)
        {
            const string sql = @"
                INSERT INTO CTPHIEUNHAP
                    (MPN, MSP, SL, TIENNHAP, HINHTHUC)
                VALUES
                    (@mpn, @msp, @sl, @tiennhap, @hinhthuc)";

            int result = 0;

            foreach (ChiTietPhieuNhapDTO item in details)
            {
                using (SqlCommand cmd =
                    new SqlCommand(sql, conn, transaction))
                {
                    cmd.Parameters.Add(
                        "@mpn", SqlDbType.Int
                    ).Value = item.MPN;

                    cmd.Parameters.Add(
                        "@msp", SqlDbType.Int
                    ).Value = item.MSP;

                    cmd.Parameters.Add(
                        "@sl", SqlDbType.Int
                    ).Value = item.SL;

                    cmd.Parameters.Add(
                        "@tiennhap", SqlDbType.Int
                    ).Value = item.TIENNHAP;

                    cmd.Parameters.Add(
                        "@hinhthuc", SqlDbType.Int
                    ).Value = item.HINHTHUC;

                    result += cmd.ExecuteNonQuery();
                }
            }

            return result;
        }

        // 1. Thêm danh sách chi tiết phiếu nhập
        // Chưa cộng tồn kho tại bước này
        public int insert(List<ChiTietPhieuNhapDTO> t)
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
                        "Lỗi Insert CTPhieuNhap: "
                        + ex.Message
                    );

                    return -1;
                }
            }
        }

        // 2. Xóa chi tiết theo mã phiếu nhập
        public int delete(string t)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    const string sql = @"
                        DELETE FROM CTPHIEUNHAP
                        WHERE MPN = @mpn";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@mpn", SqlDbType.Int
                        ).Value = int.Parse(t);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Delete CTPhieuNhap: "
                    + ex.Message
                );

                return -1;
            }
        }

        // 3. Cập nhật chi tiết phiếu nhập
        // Xóa cũ và thêm mới trong cùng transaction
        public int update(
            List<ChiTietPhieuNhapDTO> t,
            string pk)
        {
            if (t == null)
                return -1;

            if (!int.TryParse(pk, out int mpn))
                return -1;

            // Không cho phép thêm chi tiết sang phiếu khác
            foreach (ChiTietPhieuNhapDTO item in t)
            {
                if (item.MPN != mpn)
                    return -1;
            }

            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            using (SqlTransaction transaction =
                conn.BeginTransaction())
            {
                try
                {
                    const string deleteSql = @"
                        DELETE FROM CTPHIEUNHAP
                        WHERE MPN = @mpn";

                    using (SqlCommand deleteCmd =
                        new SqlCommand(
                            deleteSql, conn, transaction))
                    {
                        deleteCmd.Parameters.Add(
                            "@mpn", SqlDbType.Int
                        ).Value = mpn;

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
                        "Lỗi Update CTPhieuNhap: "
                        + ex.Message
                    );

                    return -1;
                }
            }
        }

        // 4. Lấy danh sách chi tiết theo mã phiếu nhập
        public List<ChiTietPhieuNhapDTO> selectAll(string t)
        {
            List<ChiTietPhieuNhapDTO> result =
                new List<ChiTietPhieuNhapDTO>();

            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            {
                const string sql = @"
                    SELECT MPN, MSP, SL, TIENNHAP, HINHTHUC
                    FROM CTPHIEUNHAP
                    WHERE MPN = @mpn
                    ORDER BY MSP";

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(
                        "@mpn", SqlDbType.Int
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
    }
}
