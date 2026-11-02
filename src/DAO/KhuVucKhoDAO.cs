
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using src.DTO;
using src.config;

namespace src.DAO
{
    public class KhuVucKhoDAO : DAOinterface<KhuVucKhoDTO>
    {
        private static KhuVucKhoDAO instance;

        public static KhuVucKhoDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new KhuVucKhoDAO();

                return instance;
            }
        }

        private KhuVucKhoDAO() { }

        // Chuyển dữ liệu từ SQL Server sang DTO
        private static KhuVucKhoDTO ReadKhuVucKho(
            SqlDataReader rs)
        {
            int mkvk = rs.GetInt32(rs.GetOrdinal("MKVK"));
            string ten = rs.GetString(rs.GetOrdinal("TEN"));

            int ghiChuIndex = rs.GetOrdinal("GHICHU");
            string ghichu = rs.IsDBNull(ghiChuIndex)
                ? ""
                : rs.GetString(ghiChuIndex);

            int tt = rs.GetInt32(rs.GetOrdinal("TT"));

            return new KhuVucKhoDTO(
                mkvk, ten, ghichu, tt
            );
        }

        // 1. Thêm khu vực kho
        public int insert(KhuVucKhoDTO t)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    string sql = @"
                        INSERT INTO KHUVUCKHO
                            (TEN, GHICHU, TT)
                        VALUES
                            (@ten, @ghichu, 1)";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@ten", SqlDbType.NVarChar, 255
                        ).Value = t.TEN;

                        cmd.Parameters.Add(
                            "@ghichu", SqlDbType.NVarChar, 255
                        ).Value = (object?)t.GHICHU
                            ?? DBNull.Value;

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Insert KhuVucKho: " + ex.Message
                );
                return 0;
            }
        }

        // 2. Cập nhật thông tin khu vực kho
        public int update(KhuVucKhoDTO t)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    string sql = @"
                        UPDATE KHUVUCKHO
                        SET TEN = @ten,
                            GHICHU = @ghichu
                        WHERE MKVK = @mkvk
                          AND TT = 1";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@ten", SqlDbType.NVarChar, 255
                        ).Value = t.TEN;

                        cmd.Parameters.Add(
                            "@ghichu", SqlDbType.NVarChar, 255
                        ).Value = (object?)t.GHICHU
                            ?? DBNull.Value;

                        cmd.Parameters.Add(
                            "@mkvk", SqlDbType.Int
                        ).Value = t.MKVK;

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Update KhuVucKho: " + ex.Message
                );
                return 0;
            }
        }

        // 3. Xóa mềm khu vực kho
        public int delete(string t)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    string sql = @"
                        UPDATE KHUVUCKHO
                        SET TT = 0
                        WHERE MKVK = @mkvk
                          AND TT = 1";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@mkvk", SqlDbType.Int
                        ).Value = int.Parse(t);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Delete KhuVucKho: " + ex.Message
                );
                return 0;
            }
        }

        // 4. Lấy danh sách khu vực kho đang hoạt động
        public List<KhuVucKhoDTO> selectAll()
        {
            List<KhuVucKhoDTO> result =
                new List<KhuVucKhoDTO>();

            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    string sql = @"
                        SELECT MKVK, TEN, GHICHU, TT
                        FROM KHUVUCKHO
                        WHERE TT = 1
                        ORDER BY MKVK";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    using (SqlDataReader rs =
                        cmd.ExecuteReader())
                    {
                        while (rs.Read())
                        {
                            result.Add(ReadKhuVucKho(rs));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi SelectAll KhuVucKho: "
                    + ex.Message
                );
                throw;
            }

            return result;
        }

        // 5. Lấy khu vực kho theo mã
        public KhuVucKhoDTO selectById(string t)
        {
            KhuVucKhoDTO result = null;

            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    string sql = @"
                        SELECT MKVK, TEN, GHICHU, TT
                        FROM KHUVUCKHO
                        WHERE MKVK = @mkvk";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@mkvk", SqlDbType.Int
                        ).Value = int.Parse(t);

                        using (SqlDataReader rs =
                            cmd.ExecuteReader())
                        {
                            if (rs.Read())
                            {
                                result = ReadKhuVucKho(rs);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi SelectById KhuVucKho: "
                    + ex.Message
                );
                throw;
            }

            return result;
        }

        // 6. Lấy mã IDENTITY kế tiếp để tham khảo
        // Không dùng giá trị này để tự cấp khóa chính
        public int getAutoIncrement()
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    string sql = @"
                        SELECT
                            CASE
                                WHEN OBJECTPROPERTY(
                                    OBJECT_ID('dbo.KHUVUCKHO'),
                                    'TableHasIdentity'
                                ) = 1
                                THEN CAST(
                                    IDENT_CURRENT('dbo.KHUVUCKHO')
                                    + IDENT_INCR('dbo.KHUVUCKHO')
                                    AS INT
                                )
                                ELSE NULL
                            END";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        object value = cmd.ExecuteScalar();

                        if (value == null ||
                            value == DBNull.Value)
                        {
                            return -1;
                        }

                        return Convert.ToInt32(value);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi getAutoIncrement KhuVucKho: "
                    + ex.Message
                );
                return -1;
            }
        }
    }
}
