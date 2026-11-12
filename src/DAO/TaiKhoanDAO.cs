
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using src.DTO;
using src.config;
using src.Helper;

namespace src.DAO
{
    public class TaiKhoanDAO : DAOinterface<TaiKhoanDTO>
    {
        private static TaiKhoanDAO instance;

        public static TaiKhoanDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new TaiKhoanDAO();

                return instance;
            }
        }

        private TaiKhoanDAO() { }

        // Chuyển dữ liệu SQL Server thành TaiKhoanDTO
        private static TaiKhoanDTO ReadTaiKhoan(SqlDataReader rs)
        {
            int maTK = rs.GetInt32(rs.GetOrdinal("MaTK"));

            int? mnv = rs.IsDBNull(rs.GetOrdinal("MNV"))
                ? null
                : rs.GetInt32(rs.GetOrdinal("MNV"));

            int? mkh = rs.IsDBNull(rs.GetOrdinal("MKH"))
                ? null
                : rs.GetInt32(rs.GetOrdinal("MKH"));

            string tdn = rs.GetString(rs.GetOrdinal("TDN"));
            string mk = rs.GetString(rs.GetOrdinal("MK"));
            int mnq = rs.GetInt32(rs.GetOrdinal("MNQ"));
            int tt = rs.GetInt32(rs.GetOrdinal("TT"));

            return new TaiKhoanDTO(
                maTK, mnv, mkh, tdn, mk, mnq, tt
            );
        }

        // 1. Thêm tài khoản nhân viên
        // Mật khẩu đã được BCrypt ở tầng BUS
        public int insert(TaiKhoanDTO t)
        {
            try
            {
                if (!t.MNV.HasValue)
                    return 0;

                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    const string sql = @"
                        INSERT INTO dbo.TAIKHOAN
                            (MNV, MKH, TDN, MK, MNQ, TT)
                        VALUES
                            (@mnv, NULL, @tdn, @mk, @mnq, @tt)";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@mnv", SqlDbType.Int
                        ).Value = t.MNV.Value;

                        cmd.Parameters.Add(
                            "@tdn", SqlDbType.NVarChar, 255
                        ).Value = t.TDN;

                        cmd.Parameters.Add(
                            "@mk", SqlDbType.NVarChar, 255
                        ).Value = t.MK;

                        cmd.Parameters.Add(
                            "@mnq", SqlDbType.Int
                        ).Value = t.MNQ;

                        cmd.Parameters.Add(
                            "@tt", SqlDbType.Int
                        ).Value = t.TT;

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Insert TaiKhoan: " + ex.Message);

                return 0;
            }
        }

        // 2. Cập nhật tài khoản theo MaTK
        public int update(TaiKhoanDTO t)
        {
            try
            {
                if (t.MaTK <= 0)
                    return 0;

                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    const string sql = @"
                        UPDATE dbo.TAIKHOAN
                        SET TDN = @tdn,
                            MK = @mk,
                            MNQ = @mnq,
                            TT = @tt
                        WHERE MaTK = @maTK
                          AND TT <> -1";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@maTK", SqlDbType.Int
                        ).Value = t.MaTK;

                        cmd.Parameters.Add(
                            "@tdn", SqlDbType.NVarChar, 255
                        ).Value = t.TDN;

                        cmd.Parameters.Add(
                            "@mk", SqlDbType.NVarChar, 255
                        ).Value = t.MK;

                        cmd.Parameters.Add(
                            "@mnq", SqlDbType.Int
                        ).Value = t.MNQ;

                        cmd.Parameters.Add(
                            "@tt", SqlDbType.Int
                        ).Value = t.TT;

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Update TaiKhoan: " + ex.Message);

                return 0;
            }
        }

        // 3. Đổi mật khẩu theo email nhân viên
        public void updatePass(string email, string newPassword)
        {
            try
            {
                string hashedPassword =
                    BCrypts.hashpw(
                        newPassword,
                        BCrypts.gensalt(12));

                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    const string sql = @"
                        UPDATE tk
                        SET tk.MK = @mk
                        FROM dbo.TAIKHOAN AS tk
                        INNER JOIN dbo.NHANVIEN AS nv
                            ON tk.MNV = nv.MNV
                        WHERE nv.EMAIL = @email
                          AND tk.MNV IS NOT NULL
                          AND tk.TT <> -1";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@mk", SqlDbType.NVarChar, 255
                        ).Value = hashedPassword;

                        cmd.Parameters.Add(
                            "@email", SqlDbType.NVarChar, 255
                        ).Value = email;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi UpdatePass TaiKhoan: " + ex.Message);

                throw;
            }
        }

        // 4. Tìm tài khoản theo email nhân viên
        public TaiKhoanDTO selectByEmail(string email)
        {
            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            {
                const string sql = @"
                    SELECT TOP (1)
                        tk.MaTK, tk.MNV, tk.MKH,
                        tk.TDN, tk.MK, tk.MNQ, tk.TT
                    FROM dbo.TAIKHOAN AS tk
                    INNER JOIN dbo.NHANVIEN AS nv
                        ON tk.MNV = nv.MNV
                    WHERE nv.EMAIL = @email
                      AND tk.MNV IS NOT NULL
                      AND tk.TT <> -1
                    ORDER BY tk.MaTK";

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(
                        "@email", SqlDbType.NVarChar, 255
                    ).Value = email;

                    using (SqlDataReader rs =
                        cmd.ExecuteReader())
                    {
                        if (rs.Read())
                            return ReadTaiKhoan(rs);
                    }
                }
            }

            return null;
        }

        // 5. Lưu OTP khôi phục mật khẩu nhân viên
        public void sendOpt(string email, string otp)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    const string sql = @"
                        UPDATE tk
                        SET tk.OTP = @otp
                        FROM dbo.TAIKHOAN AS tk
                        INNER JOIN dbo.NHANVIEN AS nv
                            ON tk.MNV = nv.MNV
                        WHERE nv.EMAIL = @email
                          AND tk.MNV IS NOT NULL
                          AND tk.TT <> -1";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@otp", SqlDbType.NVarChar, 50
                        ).Value = otp;

                        cmd.Parameters.Add(
                            "@email", SqlDbType.NVarChar, 255
                        ).Value = email;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi SendOTP TaiKhoan: " + ex.Message);

                throw;
            }
        }

        // 6. Kiểm tra OTP nhân viên
        public bool checkOtp(string email, string otp)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    const string sql = @"
                        SELECT COUNT(1)
                        FROM dbo.TAIKHOAN AS tk
                        INNER JOIN dbo.NHANVIEN AS nv
                            ON tk.MNV = nv.MNV
                        WHERE nv.EMAIL = @email
                          AND tk.OTP = @otp
                          AND tk.MNV IS NOT NULL
                          AND tk.TT <> -1";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@email", SqlDbType.NVarChar, 255
                        ).Value = email;

                        cmd.Parameters.Add(
                            "@otp", SqlDbType.NVarChar, 50
                        ).Value = otp;

                        return Convert.ToInt32(
                            cmd.ExecuteScalar()) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi CheckOTP TaiKhoan: " + ex.Message);

                throw;
            }
        }

        // 7. Xóa mềm tài khoản theo MaTK
        public int delete(string t)
        {
            try
            {
                if (!int.TryParse(t, out int maTK) || maTK <= 0)
                    return 0;

                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    const string sql = @"
                        UPDATE dbo.TAIKHOAN
                        SET TT = -1
                        WHERE MaTK = @maTK
                          AND TT <> -1";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@maTK", SqlDbType.Int
                        ).Value = maTK;

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Delete TaiKhoan: " + ex.Message);

                return 0;
            }
        }

        // 8. Lấy tất cả tài khoản nhân viên và khách hàng Web
        public List<TaiKhoanDTO> selectAll()
        {
            List<TaiKhoanDTO> result =
                new List<TaiKhoanDTO>();

            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            {
                const string sql = @"
                    SELECT
                        MaTK, MNV, MKH, TDN, MK, MNQ, TT
                    FROM dbo.TAIKHOAN
                    WHERE TT IN (0, 1, 2)
                    ORDER BY MaTK";

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                using (SqlDataReader rs =
                    cmd.ExecuteReader())
                {
                    while (rs.Read())
                    {
                        result.Add(ReadTaiKhoan(rs));
                    }
                }
            }

            return result;
        }

        // 9. Tìm tài khoản theo MaTK
        public TaiKhoanDTO selectById(string t)
        {
            if (!int.TryParse(t, out int maTK))
                return null;

            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            {
                const string sql = @"
                    SELECT
                        MaTK, MNV, MKH, TDN, MK, MNQ, TT
                    FROM dbo.TAIKHOAN
                    WHERE MaTK = @maTK";

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(
                        "@maTK", SqlDbType.Int
                    ).Value = maTK;

                    using (SqlDataReader rs =
                        cmd.ExecuteReader())
                    {
                        if (rs.Read())
                            return ReadTaiKhoan(rs);
                    }
                }
            }

            return null;
        }

        // 10. Tìm tài khoản theo tên đăng nhập Desktop
        // Không cho tài khoản khách hàng đăng nhập Desktop
        public TaiKhoanDTO selectByUser(string username)
        {
            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            {
                const string sql = @"
                    SELECT
                        MaTK, MNV, MKH, TDN, MK, MNQ, TT
                    FROM dbo.TAIKHOAN
                    WHERE TDN = @tdn
                      AND MNV IS NOT NULL
                      AND TT <> -1";

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(
                        "@tdn", SqlDbType.NVarChar, 255
                    ).Value = username;

                    using (SqlDataReader rs =
                        cmd.ExecuteReader())
                    {
                        if (rs.Read())
                            return ReadTaiKhoan(rs);
                    }
                }
            }

            return null;
        }

        // 11. MaTK được SQL Server tự tăng
        public int getAutoIncrement()
        {
            return -1;
        }
    }
}
