
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using src.DTO;
using src.config;

namespace src.DAO
{
    public class ChiTietQuyenDAO : ChiTietInterface<ChiTietQuyenDTO>
    {
        private static ChiTietQuyenDAO instance;

        public static ChiTietQuyenDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new ChiTietQuyenDAO();

                return instance;
            }
        }

        private ChiTietQuyenDAO() { }

        // 1. Xóa tất cả quyền của một nhóm
        public int delete(string t)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    const string sql = @"
                        DELETE FROM CTQUYEN
                        WHERE MNQ = @mnq";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@mnq", SqlDbType.Int
                        ).Value = int.Parse(t);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Delete ChiTietQuyen: " + ex.Message
                );
                return -1;
            }
        }

        // 2. Thêm danh sách quyền vào nhóm
        // Sử dụng Transaction để tránh thêm dở dang
        public int insert(List<ChiTietQuyenDTO> t)
        {
            if (t == null || t.Count == 0)
                return 0;

            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            using (SqlTransaction transaction =
                conn.BeginTransaction())
            {
                try
                {
                    int result = 0;

                    const string sql = @"
                        INSERT INTO CTQUYEN
                            (MNQ, MCN, HANHDONG)
                        VALUES
                            (@mnq, @mcn, @hanhdong)";

                    foreach (ChiTietQuyenDTO item in t)
                    {
                        using (SqlCommand cmd =
                            new SqlCommand(sql, conn, transaction))
                        {
                            cmd.Parameters.Add(
                                "@mnq", SqlDbType.Int
                            ).Value = item.Manhomquyen;

                            cmd.Parameters.Add(
                                "@mcn", SqlDbType.NVarChar, 100
                            ).Value = item.Machucnang;

                            cmd.Parameters.Add(
                                "@hanhdong", SqlDbType.NVarChar, 50
                            ).Value = item.Hanhdong;

                            result += cmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                    return result;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    Console.WriteLine(
                        "Lỗi Insert ChiTietQuyen: "
                        + ex.Message
                    );

                    return -1;
                }
            }
        }

        // 3. Lấy danh sách quyền của một nhóm
        public List<ChiTietQuyenDTO> selectAll(string t)
        {
            List<ChiTietQuyenDTO> result =
                new List<ChiTietQuyenDTO>();

            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            {
                const string sql = @"
                    SELECT MNQ, MCN, HANHDONG
                    FROM CTQUYEN
                    WHERE MNQ = @mnq
                    ORDER BY MCN, HANHDONG";

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(
                        "@mnq", SqlDbType.Int
                    ).Value = int.Parse(t);

                    using (SqlDataReader rs =
                        cmd.ExecuteReader())
                    {
                        while (rs.Read())
                        {
                            int mnq = rs.GetInt32(
                                rs.GetOrdinal("MNQ"));

                            string mcn = rs.GetString(
                                rs.GetOrdinal("MCN"));

                            string hanhdong = rs.GetString(
                                rs.GetOrdinal("HANHDONG"));

                            ChiTietQuyenDTO quyen =
                                new ChiTietQuyenDTO(
                                    mnq, mcn, hanhdong
                                );

                            result.Add(quyen);
                        }
                    }
                }
            }

            return result;
        }

        // 4. Cập nhật toàn bộ quyền của nhóm
        // Xóa quyền cũ và thêm quyền mới
        // trong cùng một Transaction
        public int update(List<ChiTietQuyenDTO> t, string pk)
        {
            if (t == null)
                return -1;

            if (!int.TryParse(pk, out int mnq))
                return -1;

            // Không cho phép ghi quyền của nhóm khác
            foreach (ChiTietQuyenDTO item in t)
            {
                if (item.Manhomquyen != mnq)
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
                        DELETE FROM CTQUYEN
                        WHERE MNQ = @mnq";

                    using (SqlCommand deleteCmd =
                        new SqlCommand(
                            deleteSql, conn, transaction))
                    {
                        deleteCmd.Parameters.Add(
                            "@mnq", SqlDbType.Int
                        ).Value = mnq;

                        deleteCmd.ExecuteNonQuery();
                    }

                    const string insertSql = @"
                        INSERT INTO CTQUYEN
                            (MNQ, MCN, HANHDONG)
                        VALUES
                            (@mnq, @mcn, @hanhdong)";

                    int result = 0;

                    foreach (ChiTietQuyenDTO item in t)
                    {
                        using (SqlCommand insertCmd =
                            new SqlCommand(
                                insertSql, conn, transaction))
                        {
                            insertCmd.Parameters.Add(
                                "@mnq", SqlDbType.Int
                            ).Value = item.Manhomquyen;

                            insertCmd.Parameters.Add(
                                "@mcn", SqlDbType.NVarChar, 100
                            ).Value = item.Machucnang;

                            insertCmd.Parameters.Add(
                                "@hanhdong", SqlDbType.NVarChar, 50
                            ).Value = item.Hanhdong;

                            result += insertCmd.ExecuteNonQuery();
                        }
                    }

                    transaction.Commit();
                    return result;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();

                    Console.WriteLine(
                        "Lỗi Update ChiTietQuyen: "
                        + ex.Message
                    );

                    return -1;
                }
            }
        }
    }
}
