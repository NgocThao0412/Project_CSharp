
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using src.DTO;
using src.config;

namespace src.DAO
{
    public class NhomQuyenDAO : DAOinterface<NhomQuyenDTO>
    {
        private static NhomQuyenDAO instance;

        public static NhomQuyenDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new NhomQuyenDAO();

                return instance;
            }
        }

        private NhomQuyenDAO() { }

        // Chuyển dữ liệu SQL Server thành NhomQuyenDTO
        private static NhomQuyenDTO ReadNhomQuyen(
            SqlDataReader rs)
        {
            int mnq = rs.GetInt32(rs.GetOrdinal("MNQ"));
            string ten = rs.GetString(rs.GetOrdinal("TEN"));

            return new NhomQuyenDTO(mnq, ten);
        }

        // 1. Thêm nhóm quyền mới
        public int insert(NhomQuyenDTO t)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    const string sql = @"
                        INSERT INTO NHOMQUYEN (TEN, TT)
                        VALUES (@ten, 1)";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@ten", SqlDbType.NVarChar, 255
                        ).Value = t.Tennhomquyen;

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Insert NhomQuyen: " + ex.Message);
                return 0;
            }
        }

        // 2. Cập nhật tên nhóm quyền
        public int update(NhomQuyenDTO t)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    const string sql = @"
                        UPDATE NHOMQUYEN
                        SET TEN = @ten
                        WHERE MNQ = @mnq";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@ten", SqlDbType.NVarChar, 255
                        ).Value = t.Tennhomquyen;

                        cmd.Parameters.Add(
                            "@mnq", SqlDbType.Int
                        ).Value = t.Manhomquyen;

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Update NhomQuyen: " + ex.Message);
                return 0;
            }
        }

        // 3. Xóa mềm nhóm quyền (TT = 0)
        public int delete(string t)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    const string sql = @"
                        UPDATE NHOMQUYEN
                        SET TT = 0
                        WHERE MNQ = @mnq AND TT = 1";

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
                    "Lỗi Delete NhomQuyen: " + ex.Message);
                return 0;
            }
        }

        // 4. Lấy danh sách nhóm quyền đang hoạt động
        public List<NhomQuyenDTO> selectAll()
        {
            List<NhomQuyenDTO> result =
                new List<NhomQuyenDTO>();

            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    const string sql = @"
                        SELECT MNQ, TEN
                        FROM NHOMQUYEN
                        WHERE TT = 1
                        ORDER BY MNQ";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    using (SqlDataReader rs =
                        cmd.ExecuteReader())
                    {
                        while (rs.Read())
                        {
                            result.Add(ReadNhomQuyen(rs));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi SelectAll NhomQuyen: " + ex.Message);
                throw;
            }

            return result;
        }

        // 5. Lấy nhóm quyền theo mã
        // Được sử dụng khi đăng nhập
        public NhomQuyenDTO selectById(string t)
        {
            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            {
                const string sql = @"
                    SELECT MNQ, TEN
                    FROM NHOMQUYEN
                    WHERE MNQ = @mnq";

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(
                        "@mnq", SqlDbType.Int
                    ).Value = int.Parse(t);

                    using (SqlDataReader rs =
                        cmd.ExecuteReader())
                    {
                        if (rs.Read())
                        {
                            return ReadNhomQuyen(rs);
                        }
                    }
                }
            }

            return null;
        }

        // 6. Lấy mã tự tăng kế tiếp để tham khảo
        // Không sử dụng giá trị này để tự cấp khóa chính
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
                                OBJECT_ID(N'dbo.NHOMQUYEN'),
                                N'MNQ',
                                'IsIdentity'
                            ) = 1
                            THEN CAST(
                                IDENT_CURRENT('dbo.NHOMQUYEN')
                                + IDENT_INCR('dbo.NHOMQUYEN')
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
                    "Lỗi getAutoIncrement NhomQuyen: "
                    + ex.Message);
                return -1;
            }
        }
    }
}
