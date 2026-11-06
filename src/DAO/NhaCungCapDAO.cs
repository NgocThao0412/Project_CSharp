
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using src.DTO;
using src.config;

namespace src.DAO
{
    public class NhaCungCapDAO : DAOinterface<NhaCungCapDTO>
    {
        private static NhaCungCapDAO instance;

        public static NhaCungCapDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new NhaCungCapDAO();

                return instance;
            }
        }

        private NhaCungCapDAO() { }

        // Chuyển dữ liệu SQL Server thành NhaCungCapDTO
        private static NhaCungCapDTO ReadNhaCungCap(
            SqlDataReader rs)
        {
            int mncc = rs.GetInt32(rs.GetOrdinal("MNCC"));
            string ten = rs.GetString(rs.GetOrdinal("TEN"));

            int diachiIndex = rs.GetOrdinal("DIACHI");
            string diachi = rs.IsDBNull(diachiIndex)
                ? ""
                : rs.GetString(diachiIndex);

            int emailIndex = rs.GetOrdinal("EMAIL");
            string email = rs.IsDBNull(emailIndex)
                ? ""
                : rs.GetString(emailIndex);

            int sdtIndex = rs.GetOrdinal("SDT");
            string sdt = rs.IsDBNull(sdtIndex)
                ? ""
                : rs.GetString(sdtIndex);

            int tt = rs.GetInt32(rs.GetOrdinal("TT"));

            return new NhaCungCapDTO(
                mncc, ten, diachi, email, sdt, tt
            );
        }

        // 1. Thêm nhà cung cấp
        public int insert(NhaCungCapDTO t)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    string sql = @"
                        INSERT INTO NHACUNGCAP
                            (TEN, DIACHI, EMAIL, SDT, TT)
                        VALUES
                            (@ten, @diachi, @email, @sdt, 1)";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@ten", SqlDbType.NVarChar, 255
                        ).Value = t.TEN;

                        cmd.Parameters.Add(
                            "@diachi", SqlDbType.NVarChar, 255
                        ).Value = (object?)t.DIACHI
                            ?? DBNull.Value;

                        cmd.Parameters.Add(
                            "@email", SqlDbType.NVarChar, 255
                        ).Value = (object?)t.EMAIL
                            ?? DBNull.Value;

                        cmd.Parameters.Add(
                            "@sdt", SqlDbType.NVarChar, 20
                        ).Value = (object?)t.SDT
                            ?? DBNull.Value;

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Insert NhaCungCap: " + ex.Message
                );
                return 0;
            }
        }

        // 2. Cập nhật nhà cung cấp
        public int update(NhaCungCapDTO t)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    string sql = @"
                        UPDATE NHACUNGCAP
                        SET TEN = @ten,
                            DIACHI = @diachi,
                            EMAIL = @email,
                            SDT = @sdt
                        WHERE MNCC = @mncc
                          AND TT = 1";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@ten", SqlDbType.NVarChar, 255
                        ).Value = t.TEN;

                        cmd.Parameters.Add(
                            "@diachi", SqlDbType.NVarChar, 255
                        ).Value = (object?)t.DIACHI
                            ?? DBNull.Value;

                        cmd.Parameters.Add(
                            "@email", SqlDbType.NVarChar, 255
                        ).Value = (object?)t.EMAIL
                            ?? DBNull.Value;

                        cmd.Parameters.Add(
                            "@sdt", SqlDbType.NVarChar, 20
                        ).Value = (object?)t.SDT
                            ?? DBNull.Value;

                        cmd.Parameters.Add(
                            "@mncc", SqlDbType.Int
                        ).Value = t.MNCC;

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Update NhaCungCap: " + ex.Message
                );
                return 0;
            }
        }

        // 3. Xóa mềm nhà cung cấp
        public int delete(string t)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    string sql = @"
                        UPDATE NHACUNGCAP
                        SET TT = 0
                        WHERE MNCC = @mncc
                          AND TT = 1";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@mncc", SqlDbType.Int
                        ).Value = int.Parse(t);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Delete NhaCungCap: " + ex.Message
                );
                return 0;
            }
        }

        // 4. Lấy tất cả nhà cung cấp đang hoạt động
        public List<NhaCungCapDTO> selectAll()
        {
            List<NhaCungCapDTO> result =
                new List<NhaCungCapDTO>();

            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    string sql = @"
                        SELECT MNCC, TEN, DIACHI,
                               EMAIL, SDT, TT
                        FROM NHACUNGCAP
                        WHERE TT = 1
                        ORDER BY MNCC";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    using (SqlDataReader rs =
                        cmd.ExecuteReader())
                    {
                        while (rs.Read())
                        {
                            result.Add(ReadNhaCungCap(rs));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi SelectAll NhaCungCap: " + ex.Message
                );
                throw;
            }

            return result;
        }

        // 5. Lấy nhà cung cấp theo mã
        public NhaCungCapDTO selectById(string t)
        {
            NhaCungCapDTO result = null;

            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    string sql = @"
                        SELECT MNCC, TEN, DIACHI,
                               EMAIL, SDT, TT
                        FROM NHACUNGCAP
                        WHERE MNCC = @mncc";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@mncc", SqlDbType.Int
                        ).Value = int.Parse(t);

                        using (SqlDataReader rs =
                            cmd.ExecuteReader())
                        {
                            if (rs.Read())
                            {
                                result = ReadNhaCungCap(rs);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi SelectById NhaCungCap: " + ex.Message
                );
                throw;
            }

            return result;
        }

        // 6. Lấy mã IDENTITY kế tiếp để tham khảo
        // Không sử dụng giá trị này để tự cấp khóa chính
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
                                    OBJECT_ID('dbo.NHACUNGCAP'),
                                    'TableHasIdentity'
                                ) = 1
                                THEN CAST(
                                    IDENT_CURRENT('dbo.NHACUNGCAP')
                                    + IDENT_INCR('dbo.NHACUNGCAP')
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
                    "Lỗi getAutoIncrement NhaCungCap: "
                    + ex.Message
                );
                return -1;
            }
        }
    }
}
