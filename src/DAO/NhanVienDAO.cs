
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using src.DTO;
using src.config;

namespace src.DAO
{
    public class NhanVienDAO : DAOinterface<NhanVienDTO>
    {
        private static NhanVienDAO instance;

        public static NhanVienDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new NhanVienDAO();

                return instance;
            }
        }

        private NhanVienDAO() { }

        // Chuyển dữ liệu SQL Server thành NhanVienDTO
        private static NhanVienDTO ReadNhanVien(SqlDataReader rs)
        {
            int mnv = rs.GetInt32(rs.GetOrdinal("MNV"));
            string hoten = rs.GetString(rs.GetOrdinal("HOTEN"));
            int gioitinh = rs.GetInt32(rs.GetOrdinal("GIOITINH"));
            DateTime ngaysinh =
                rs.GetDateTime(rs.GetOrdinal("NGAYSINH"));

            int sdtIndex = rs.GetOrdinal("SDT");
            string sdt = rs.IsDBNull(sdtIndex)
                ? "" : rs.GetString(sdtIndex);

            int tt = rs.GetInt32(rs.GetOrdinal("TT"));

            int emailIndex = rs.GetOrdinal("EMAIL");
            string email = rs.IsDBNull(emailIndex)
                ? "" : rs.GetString(emailIndex);

            return new NhanVienDTO(
                mnv, hoten, gioitinh, ngaysinh, sdt, tt, email
            );
        }

        // 1. Thêm nhân viên
        public int insert(NhanVienDTO t)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    string sql = @"
                        INSERT INTO NHANVIEN
                            (HOTEN, GIOITINH, SDT,
                             NGAYSINH, TT, EMAIL)
                        VALUES
                            (@hoten, @gioitinh, @sdt,
                             @ngaysinh, @tt, @email)";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@hoten", SqlDbType.NVarChar, 255
                        ).Value = t.HOTEN;

                        cmd.Parameters.Add(
                            "@gioitinh", SqlDbType.Int
                        ).Value = t.GIOITINH;

                        cmd.Parameters.Add(
                            "@sdt", SqlDbType.NVarChar, 30
                        ).Value = (object?)t.SDT
                            ?? DBNull.Value;

                        cmd.Parameters.Add(
                            "@ngaysinh", SqlDbType.DateTime
                        ).Value = t.NGAYSINH;

                        cmd.Parameters.Add(
                            "@tt", SqlDbType.Int
                        ).Value = t.TT;

                        cmd.Parameters.Add(
                            "@email", SqlDbType.NVarChar, 255
                        ).Value = (object?)t.EMAIL
                            ?? DBNull.Value;

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Insert NhanVien: " + ex.Message
                );
                return 0;
            }
        }

        // 2. Cập nhật thông tin nhân viên
        public int update(NhanVienDTO t)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    string sql = @"
                        UPDATE NHANVIEN
                        SET HOTEN = @hoten,
                            GIOITINH = @gioitinh,
                            NGAYSINH = @ngaysinh,
                            SDT = @sdt,
                            TT = @tt,
                            EMAIL = @email
                        WHERE MNV = @mnv";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@hoten", SqlDbType.NVarChar, 255
                        ).Value = t.HOTEN;

                        cmd.Parameters.Add(
                            "@gioitinh", SqlDbType.Int
                        ).Value = t.GIOITINH;

                        cmd.Parameters.Add(
                            "@ngaysinh", SqlDbType.DateTime
                        ).Value = t.NGAYSINH;

                        cmd.Parameters.Add(
                            "@sdt", SqlDbType.NVarChar, 30
                        ).Value = (object?)t.SDT
                            ?? DBNull.Value;

                        cmd.Parameters.Add(
                            "@tt", SqlDbType.Int
                        ).Value = t.TT;

                        cmd.Parameters.Add(
                            "@email", SqlDbType.NVarChar, 255
                        ).Value = (object?)t.EMAIL
                            ?? DBNull.Value;

                        cmd.Parameters.Add(
                            "@mnv", SqlDbType.Int
                        ).Value = t.MNV;

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Update NhanVien: " + ex.Message
                );
                return 0;
            }
        }

        // 3. Xóa mềm nhân viên
        // Giữ quy ước TT = -1 của source gốc
        public int delete(string t)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    string sql = @"
                        UPDATE NHANVIEN
                        SET TT = -1
                        WHERE MNV = @mnv
                          AND TT <> -1";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@mnv", SqlDbType.Int
                        ).Value = int.Parse(t);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Delete NhanVien: " + ex.Message
                );
                return 0;
            }
        }

        // 4. Lấy danh sách nhân viên đang hoạt động
        public List<NhanVienDTO> selectAll()
        {
            List<NhanVienDTO> result =
                new List<NhanVienDTO>();

            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            {
                string sql = @"
                    SELECT MNV, HOTEN, GIOITINH,
                           NGAYSINH, SDT, TT, EMAIL
                    FROM NHANVIEN
                    WHERE TT = 1
                    ORDER BY MNV";

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                using (SqlDataReader rs =
                    cmd.ExecuteReader())
                {
                    while (rs.Read())
                    {
                        result.Add(ReadNhanVien(rs));
                    }
                }
            }

            return result;
        }

        // 5. Lấy toàn bộ nhân viên, kể cả đã xóa mềm
        public List<NhanVienDTO> SelectAllFull()
        {
            List<NhanVienDTO> result =
                new List<NhanVienDTO>();

            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            {
                string sql = @"
                    SELECT MNV, HOTEN, GIOITINH,
                           NGAYSINH, SDT, TT, EMAIL
                    FROM NHANVIEN
                    ORDER BY MNV";

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                using (SqlDataReader rs =
                    cmd.ExecuteReader())
                {
                    while (rs.Read())
                    {
                        result.Add(ReadNhanVien(rs));
                    }
                }
            }

            return result;
        }

        // 6. Lấy nhân viên đang hoạt động chưa có tài khoản
        public List<NhanVienDTO> SelectAllNVNoAccount()
        {
            List<NhanVienDTO> result =
                new List<NhanVienDTO>();

            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            {
                string sql = @"
                    SELECT nv.MNV, nv.HOTEN, nv.GIOITINH,
                           nv.NGAYSINH, nv.SDT, nv.TT, nv.EMAIL
                    FROM NHANVIEN AS nv
                    WHERE nv.TT = 1
                      AND NOT EXISTS
                      (
                          SELECT 1
                          FROM TAIKHOAN AS tk
                          WHERE tk.MNV = nv.MNV
                      )
                    ORDER BY nv.MNV";

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                using (SqlDataReader rs =
                    cmd.ExecuteReader())
                {
                    while (rs.Read())
                    {
                        result.Add(ReadNhanVien(rs));
                    }
                }
            }

            return result;
        }

        // 7. Lấy nhân viên theo mã
        public NhanVienDTO selectById(string t)
        {
            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            {
                string sql = @"
                    SELECT MNV, HOTEN, GIOITINH,
                           NGAYSINH, SDT, TT, EMAIL
                    FROM NHANVIEN
                    WHERE MNV = @mnv";

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(
                        "@mnv", SqlDbType.Int
                    ).Value = int.Parse(t);

                    using (SqlDataReader rs =
                        cmd.ExecuteReader())
                    {
                        if (rs.Read())
                        {
                            return ReadNhanVien(rs);
                        }
                    }
                }
            }

            return null;
        }

        // 8. Lấy nhân viên theo email
        public NhanVienDTO selectByEmail(string t)
        {
            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            {
                string sql = @"
                    SELECT MNV, HOTEN, GIOITINH,
                           NGAYSINH, SDT, TT, EMAIL
                    FROM NHANVIEN
                    WHERE EMAIL = @email";

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(
                        "@email", SqlDbType.NVarChar, 255
                    ).Value = t;

                    using (SqlDataReader rs =
                        cmd.ExecuteReader())
                    {
                        if (rs.Read())
                        {
                            return ReadNhanVien(rs);
                        }
                    }
                }
            }

            return null;
        }

        // 9. Lấy mã tự tăng kế tiếp để tham khảo
        // Không dùng mã dự đoán để INSERT đồng thời
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
                                OBJECT_ID(N'dbo.NHANVIEN'),
                                N'MNV', 'IsIdentity'
                            ) = 1
                            THEN CAST(
                                IDENT_CURRENT('dbo.NHANVIEN')
                                + IDENT_INCR('dbo.NHANVIEN')
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
                    "Lỗi getAutoIncrement NhanVien: "
                    + ex.Message
                );
                return -1;
            }
        }
    }
}
