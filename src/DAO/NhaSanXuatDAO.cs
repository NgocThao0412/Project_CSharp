
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using src.DTO;
using src.config;

namespace src.DAO
{
    public class NhaSanXuatDAO : DAOinterface<NhaSanXuatDTO>
    {
        private static NhaSanXuatDAO instance;

        public static NhaSanXuatDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new NhaSanXuatDAO();

                return instance;
            }
        }

        private NhaSanXuatDAO() { }

        // Chuyển dữ liệu SQL Server thành DTO
        private static NhaSanXuatDTO ReadNhaSanXuat(
            SqlDataReader rs)
        {
            int mnsx = rs.GetInt32(rs.GetOrdinal("MNSX"));
            string ten = rs.GetString(rs.GetOrdinal("TEN"));

            string diachi = rs.IsDBNull(rs.GetOrdinal("DIACHI"))
                ? "" : rs.GetString(rs.GetOrdinal("DIACHI"));

            string email = rs.IsDBNull(rs.GetOrdinal("EMAIL"))
                ? "" : rs.GetString(rs.GetOrdinal("EMAIL"));

            string sdt = rs.IsDBNull(rs.GetOrdinal("SDT"))
                ? "" : rs.GetString(rs.GetOrdinal("SDT"));

            int tt = rs.GetInt32(rs.GetOrdinal("TT"));

            return new NhaSanXuatDTO(
                mnsx, ten, diachi, email, sdt, tt
            );
        }

        // Kiểm tra SQL Server có tự sinh mã MNSX hay không
        private static bool HasIdentity(SqlConnection conn)
        {
            using (SqlCommand cmd = new SqlCommand(
                "SELECT COLUMNPROPERTY(" +
                "OBJECT_ID(N'dbo.NHASANXUAT'), " +
                "N'MNSX', 'IsIdentity')", conn))
            {
                object value = cmd.ExecuteScalar();
                return value != null &&
                       value != DBNull.Value &&
                       Convert.ToInt32(value) == 1;
            }
        }

        // 1. Thêm nhà sản xuất
        public int insert(NhaSanXuatDTO t)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    bool hasIdentity = HasIdentity(conn);

                    string sql = hasIdentity
                        ? @"INSERT INTO NHASANXUAT
                            (TEN, DIACHI, EMAIL, SDT, TT)
                            VALUES
                            (@ten, @diachi, @email, @sdt, 1)"
                        : @"INSERT INTO NHASANXUAT
                            (MNSX, TEN, DIACHI, EMAIL, SDT, TT)
                            VALUES
                            (@mnsx, @ten, @diachi, @email, @sdt, 1)";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        if (!hasIdentity)
                        {
                            cmd.Parameters.Add(
                                "@mnsx", SqlDbType.Int
                            ).Value = t.MNSX;
                        }

                        cmd.Parameters.AddWithValue("@ten", t.TEN);
                        cmd.Parameters.AddWithValue(
                            "@diachi", (object?)t.DIACHI ?? DBNull.Value);
                        cmd.Parameters.AddWithValue(
                            "@email", (object?)t.EMAIL ?? DBNull.Value);
                        cmd.Parameters.AddWithValue(
                            "@sdt", (object?)t.SDT ?? DBNull.Value);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Insert NhaSanXuat: " + ex.Message
                );
                return 0;
            }
        }

        // 2. Cập nhật nhà sản xuất
        public int update(NhaSanXuatDTO t)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    string sql = @"
                        UPDATE NHASANXUAT
                        SET TEN = @ten,
                            DIACHI = @diachi,
                            EMAIL = @email,
                            SDT = @sdt
                        WHERE MNSX = @mnsx
                          AND TT = 1";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@ten", t.TEN);
                        cmd.Parameters.AddWithValue(
                            "@diachi", (object?)t.DIACHI ?? DBNull.Value);
                        cmd.Parameters.AddWithValue(
                            "@email", (object?)t.EMAIL ?? DBNull.Value);
                        cmd.Parameters.AddWithValue(
                            "@sdt", (object?)t.SDT ?? DBNull.Value);

                        cmd.Parameters.Add(
                            "@mnsx", SqlDbType.Int
                        ).Value = t.MNSX;

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Update NhaSanXuat: " + ex.Message
                );
                return 0;
            }
        }

        // 3. Xóa mềm nhà sản xuất
        public int delete(string t)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    string sql = @"
                        UPDATE NHASANXUAT
                        SET TT = 0
                        WHERE MNSX = @mnsx
                          AND TT = 1";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@mnsx", SqlDbType.Int
                        ).Value = int.Parse(t);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Delete NhaSanXuat: " + ex.Message
                );
                return 0;
            }
        }

        // 4. Lấy danh sách nhà sản xuất đang hoạt động
        public List<NhaSanXuatDTO> selectAll()
        {
            List<NhaSanXuatDTO> result =
                new List<NhaSanXuatDTO>();

            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    string sql = @"
                        SELECT MNSX, TEN, DIACHI, EMAIL, SDT, TT
                        FROM NHASANXUAT
                        WHERE TT = 1
                        ORDER BY MNSX";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    using (SqlDataReader rs = cmd.ExecuteReader())
                    {
                        while (rs.Read())
                        {
                            result.Add(ReadNhaSanXuat(rs));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi SelectAll NhaSanXuat: " + ex.Message
                );
                throw;
            }

            return result;
        }

        // 5. Lấy nhà sản xuất theo mã
        public NhaSanXuatDTO selectById(string t)
        {
            NhaSanXuatDTO result = null;

            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    string sql = @"
                        SELECT MNSX, TEN, DIACHI, EMAIL, SDT, TT
                        FROM NHASANXUAT
                        WHERE MNSX = @mnsx";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@mnsx", SqlDbType.Int
                        ).Value = int.Parse(t);

                        using (SqlDataReader rs = cmd.ExecuteReader())
                        {
                            if (rs.Read())
                            {
                                result = ReadNhaSanXuat(rs);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi SelectById NhaSanXuat: " + ex.Message
                );
                throw;
            }

            return result;
        }

        // 6. Lấy mã kế tiếp để tham khảo
        public int getAutoIncrement()
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    string sql = @"
                        SELECT CASE
                            WHEN COLUMNPROPERTY(
                                OBJECT_ID(N'dbo.NHASANXUAT'),
                                N'MNSX',
                                'IsIdentity'
                            ) = 1
                            THEN CAST(
                                IDENT_CURRENT('dbo.NHASANXUAT')
                                + IDENT_INCR('dbo.NHASANXUAT')
                                AS INT
                            )
                            ELSE ISNULL(
                                (SELECT MAX(MNSX)
                                 FROM NHASANXUAT), 0
                            ) + 1
                        END";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        return Convert.ToInt32(cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi getAutoIncrement NhaSanXuat: " + ex.Message
                );
                return -1;
            }
        }
    }
}
