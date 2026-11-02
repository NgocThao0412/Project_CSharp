
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
        private static TaiKhoanDTO ReadTaiKhoan(
            SqlDataReader rs)
        {
            int mnv = rs.GetInt32(rs.GetOrdinal("MNV"));
            string tdn = rs.GetString(rs.GetOrdinal("TDN"));
            string mk = rs.GetString(rs.GetOrdinal("MK"));
            int mnq = rs.GetInt32(rs.GetOrdinal("MNQ"));
            int tt = rs.GetInt32(rs.GetOrdinal("TT"));

            return new TaiKhoanDTO(mnv, tdn, mk, mnq, tt);
        }

        // 1. Thêm tài khoản
        // Mật khẩu t.MK đã được BCrypt ở tầng BUS/GUI
        public int insert(TaiKhoanDTO t)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    const string sql = @"
                        INSERT INTO TAIKHOAN
                            (MNV, TDN, MK, MNQ, TT)
                        VALUES
                            (@mnv, @tdn, @mk, @mnq, @tt)";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@mnv", SqlDbType.Int
                        ).Value = t.MNV;

                        cmd.Parameters.Add(
                            "@tdn", SqlDbType.NVarChar, 100
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

        // 2. Cập nhật thông tin tài khoản
        // Giữ nguyên cơ chế: t.MK là mật khẩu đã băm
        public int update(TaiKhoanDTO t)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    const string sql = @"
                        UPDATE TAIKHOAN
                        SET TDN = @tdn,
                            MK = @mk,
                            TT = @tt,
                            MNQ = @mnq
                        WHERE MNV = @mnv";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@tdn", t.TDN);

                        cmd.Parameters.AddWithValue(
                            "@mk", t.MK);

                        cmd.Parameters.Add(
                            "@tt", SqlDbType.Int
                        ).Value = t.TT;

                        cmd.Parameters.Add(
                            "@mnq", SqlDbType.Int
                        ).Value = t.MNQ;

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
                    "Lỗi Update TaiKhoan: " + ex.Message);
                return 0;
            }
        }

        // 3. Cập nhật trạng thái chờ xử lý TT = 2
        // Chuyển cú pháp UPDATE JOIN của MySQL sang SQL Server
        public int updateTTCXL(string email)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    const string sql = @"
                        UPDATE tk
                        SET tk.TT = 2
                        FROM TAIKHOAN AS tk
                        INNER JOIN NHANVIEN AS nv
                            ON tk.MNV = nv.MNV
                        WHERE nv.EMAIL = @email";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@email", email);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi UpdateTTCXL TaiKhoan: "
                    + ex.Message);
                return 0;
            }
        }

        // 4. Đổi mật khẩu
        // BCrypt mật khẩu mới trước khi lưu
        public void updatePass(
            string email,
            string newPassword)
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
                        FROM TAIKHOAN AS tk
                        INNER JOIN NHANVIEN AS nv
                            ON tk.MNV = nv.MNV
                        WHERE nv.EMAIL = @email";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@mk", hashedPassword);

                        cmd.Parameters.AddWithValue(
                            "@email", email);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi UpdatePass TaiKhoan: "
                    + ex.Message);
                throw;
            }
        }

        // 5. Tìm tài khoản theo email nhân viên
        public TaiKhoanDTO selectByEmail(string email)
        {
            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            {
                const string sql = @"
                    SELECT tk.MNV, tk.TDN, tk.MK,
                           tk.MNQ, tk.TT
                    FROM TAIKHOAN AS tk
                    INNER JOIN NHANVIEN AS nv
                        ON tk.MNV = nv.MNV
                    WHERE nv.EMAIL = @email";

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue(
                        "@email", email);

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

        // 6. Lưu OTP khôi phục mật khẩu
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
                        FROM TAIKHOAN AS tk
                        INNER JOIN NHANVIEN AS nv
                            ON tk.MNV = nv.MNV
                        WHERE nv.EMAIL = @email";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@otp", otp);

                        cmd.Parameters.AddWithValue(
                            "@email", email);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi SendOTP TaiKhoan: "
                    + ex.Message);
                throw;
            }
        }

        // 7. Kiểm tra OTP
        public bool checkOtp(string email, string otp)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    const string sql = @"
                        SELECT COUNT(1)
                        FROM TAIKHOAN AS tk
                        INNER JOIN NHANVIEN AS nv
                            ON tk.MNV = nv.MNV
                        WHERE nv.EMAIL = @email
                          AND tk.OTP = @otp";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@email", email);

                        cmd.Parameters.AddWithValue(
                            "@otp", otp);

                        return Convert.ToInt32(
                            cmd.ExecuteScalar()) > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi CheckOTP TaiKhoan: "
                    + ex.Message);
                throw;
            }
        }

        // 8. Xóa mềm/khóa tài khoản
        public int delete(string t)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    const string sql = @"
                        UPDATE TAIKHOAN
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
                    "Lỗi Delete TaiKhoan: "
                    + ex.Message);
                return 0;
            }
        }

        // 9. Lấy danh sách tài khoản
        // Không lấy tài khoản đã xóa mềm TT = -1
        public List<TaiKhoanDTO> selectAll()
        {
            List<TaiKhoanDTO> result =
                new List<TaiKhoanDTO>();

            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            {
                const string sql = @"
                    SELECT MNV, TDN, MK, MNQ, TT
                    FROM TAIKHOAN
                    WHERE TT IN (0, 1, 2)
                    ORDER BY MNV";

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                using (SqlDataReader rs =
                    cmd.ExecuteReader())
                {
                    while (rs.Read())
                        result.Add(ReadTaiKhoan(rs));
                }
            }

            return result;
        }

        // 10. Tìm tài khoản theo mã nhân viên
        public TaiKhoanDTO selectById(string t)
        {
            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            {
                const string sql = @"
                    SELECT MNV, TDN, MK, MNQ, TT
                    FROM TAIKHOAN
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
                            return ReadTaiKhoan(rs);
                    }
                }
            }

            return null;
        }

        // 11. Tìm tài khoản theo tên đăng nhập
        // Giữ nguyên để TaiKhoanBUS.DangNhap() sử dụng
        public TaiKhoanDTO selectByUser(string username)
        {
            using (SqlConnection conn =
                SqlServerHelper.GetConnection())
            {
                const string sql = @"
                    SELECT MNV, TDN, MK, MNQ, TT
                    FROM TAIKHOAN
                    WHERE TDN = @tdn";

                using (SqlCommand cmd =
                    new SqlCommand(sql, conn))
                {
                    cmd.Parameters.Add(
                        "@tdn", SqlDbType.NVarChar, 100
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

        // 12. TAIKHOAN dùng MNV làm khóa chính/khóa ngoại.
        // Không có mã tài khoản IDENTITY riêng.
        public int getAutoIncrement()
        {
            return -1;
        }
    }
}
