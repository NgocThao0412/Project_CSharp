
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using src.DTO;
using src.config;

namespace src.DAO
{
    public class KhachHangDAO : DAOinterface<KhachHangDTO>
    {
        private static KhachHangDAO instance;

        public static KhachHangDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new KhachHangDAO();

                return instance;
            }
        }

        private KhachHangDAO() { }

        // Chuyển dữ liệu SQL Server thành KhachHangDTO
        private static KhachHangDTO ReadKhachHang(
            SqlDataReader rs)
        {
            int mkh = rs.GetInt32(rs.GetOrdinal("MKH"));
            string hoten = rs.GetString(rs.GetOrdinal("HOTEN"));

            string diachi = rs.IsDBNull(rs.GetOrdinal("DIACHI"))
                ? ""
                : rs.GetString(rs.GetOrdinal("DIACHI"));

            string sdt = rs.IsDBNull(rs.GetOrdinal("SDT"))
                ? ""
                : rs.GetString(rs.GetOrdinal("SDT"));

            string email = rs.IsDBNull(rs.GetOrdinal("EMAIL"))
                ? ""
                : rs.GetString(rs.GetOrdinal("EMAIL"));

            DateTime ngaythamgia =
                rs.GetDateTime(rs.GetOrdinal("NGAYTHAMGIA"));

            int tt = rs.GetInt32(rs.GetOrdinal("TT"));

            return new KhachHangDTO(
                mkh, hoten, ngaythamgia,
                diachi, sdt, email, tt
            );
        }

        // Kiểm tra MKH có phải cột IDENTITY không
        private static bool HasIdentity(SqlConnection conn)
        {
            const string sql = @"
                SELECT COLUMNPROPERTY(
                    OBJECT_ID(N'dbo.KHACHHANG'),
                    N'MKH',
                    'IsIdentity'
                )";

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                object value = cmd.ExecuteScalar();

                return value != null
                    && value != DBNull.Value
                    && Convert.ToInt32(value) == 1;
            }
        }

        // 1. Thêm khách hàng
        public int insert(KhachHangDTO t)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    bool hasIdentity = HasIdentity(conn);

                    string sql = hasIdentity
                        ? @"INSERT INTO KHACHHANG
                            (HOTEN, DIACHI, SDT,
                             EMAIL, NGAYTHAMGIA, TT)
                           VALUES
                            (@hoten, @diachi, @sdt,
                             @email, @ngaythamgia, 1)"
                        : @"INSERT INTO KHACHHANG
                            (MKH, HOTEN, DIACHI, SDT,
                             EMAIL, NGAYTHAMGIA, TT)
                           VALUES
                            (@mkh, @hoten, @diachi, @sdt,
                             @email, @ngaythamgia, 1)";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        if (!hasIdentity)
                        {
                            cmd.Parameters.Add(
                                "@mkh", SqlDbType.Int
                            ).Value = t.MKH;
                        }

                        cmd.Parameters.AddWithValue(
                            "@hoten", t.HOTEN);

                        cmd.Parameters.AddWithValue(
                            "@diachi",
                            (object?)t.DIACHI ?? DBNull.Value);

                        cmd.Parameters.AddWithValue(
                            "@sdt",
                            (object?)t.SDT ?? DBNull.Value);

                        cmd.Parameters.AddWithValue(
                            "@email",
                            (object?)t.EMAIL ?? DBNull.Value);

                        cmd.Parameters.Add(
                            "@ngaythamgia", SqlDbType.DateTime
                        ).Value = DateTime.Now;

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Insert KhachHang: " + ex.Message
                );
                return 0;
            }
        }

        // 2. Cập nhật thông tin khách hàng
        public int update(KhachHangDTO t)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    string sql = @"
                        UPDATE KHACHHANG
                        SET HOTEN = @hoten,
                            DIACHI = @diachi,
                            SDT = @sdt,
                            EMAIL = @email
                        WHERE MKH = @mkh
                          AND TT = 1";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@hoten", t.HOTEN);

                        cmd.Parameters.AddWithValue(
                            "@diachi",
                            (object?)t.DIACHI ?? DBNull.Value);

                        cmd.Parameters.AddWithValue(
                            "@sdt",
                            (object?)t.SDT ?? DBNull.Value);

                        cmd.Parameters.AddWithValue(
                            "@email",
                            (object?)t.EMAIL ?? DBNull.Value);

                        cmd.Parameters.Add(
                            "@mkh", SqlDbType.Int
                        ).Value = t.MKH;

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Update KhachHang: " + ex.Message
                );
                return 0;
            }
        }

        // 3. Xóa mềm khách hàng
        public int delete(string t)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    string sql = @"
                        UPDATE KHACHHANG
                        SET TT = 0
                        WHERE MKH = @mkh
                          AND TT = 1";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@mkh", SqlDbType.Int
                        ).Value = int.Parse(t);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Delete KhachHang: " + ex.Message
                );
                return 0;
            }
        }

        // 4. Lấy danh sách khách hàng đang hoạt động
        public List<KhachHangDTO> selectAll()
        {
            List<KhachHangDTO> result =
                new List<KhachHangDTO>();

            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            {
                string sql = @"
                    SELECT MKH, HOTEN, DIACHI, SDT,
                           EMAIL, NGAYTHAMGIA, TT
                    FROM KHACHHANG
                    WHERE TT = 1
                    ORDER BY MKH";

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                using (SqlDataReader rs =
                    cmd.ExecuteReader())
                {
                    while (rs.Read())
                    {
                        result.Add(ReadKhachHang(rs));
                    }
                }
            }

            return result;
        }

        // 5. Lấy tất cả khách hàng, bao gồm đã xóa mềm
        public List<KhachHangDTO> SelectAllFull()
        {
            List<KhachHangDTO> result =
                new List<KhachHangDTO>();

            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            {
                string sql = @"
                    SELECT MKH, HOTEN, DIACHI, SDT,
                           EMAIL, NGAYTHAMGIA, TT
                    FROM KHACHHANG
                    ORDER BY MKH";

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                using (SqlDataReader rs =
                    cmd.ExecuteReader())
                {
                    while (rs.Read())
                    {
                        result.Add(ReadKhachHang(rs));
                    }
                }
            }

            return result;
        }

        // 6. Lấy khách hàng theo mã
        public KhachHangDTO selectById(string t)
        {
            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            {
                string sql = @"
                    SELECT MKH, HOTEN, DIACHI, SDT,
                           EMAIL, NGAYTHAMGIA, TT
                    FROM KHACHHANG
                    WHERE MKH = @mkh";

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(
                        "@mkh", SqlDbType.Int
                    ).Value = int.Parse(t);

                    using (SqlDataReader rs =
                        cmd.ExecuteReader())
                    {
                        if (rs.Read())
                        {
                            return ReadKhachHang(rs);
                        }
                    }
                }
            }

            return null;
        }

        // 7. Lấy mã tiếp theo để tham khảo
        // Không dùng giá trị này để tự cấp mã đồng thời
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
                                OBJECT_ID(N'dbo.KHACHHANG'),
                                N'MKH', 'IsIdentity'
                            ) = 1
                            THEN CAST(
                                IDENT_CURRENT('dbo.KHACHHANG')
                                + IDENT_INCR('dbo.KHACHHANG')
                                AS INT
                            )
                            ELSE ISNULL(
                                (SELECT MAX(MKH)
                                 FROM KHACHHANG), 0
                            ) + 1
                        END";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        return Convert.ToInt32(
                            cmd.ExecuteScalar());
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi getAutoIncrement KhachHang: "
                    + ex.Message
                );

                return -1;
            }
        }
    }
}
