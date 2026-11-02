
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using src.DTO;
using src.config;

namespace src.DAO
{
    public class SanPhamDAO : DAOinterface<SanPhamDTO>
    {
        private static SanPhamDAO instance;

        public static SanPhamDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new SanPhamDAO();

                return instance;
            }
        }

        private SanPhamDAO() { }

        // Chuyển dữ liệu SQL Server thành SanPhamDTO
        private static SanPhamDTO ReadSanPham(SqlDataReader rs)
        {
            return new SanPhamDTO
            {
                MSP = rs.GetInt32(rs.GetOrdinal("MSP")),
                TEN = rs.GetString(rs.GetOrdinal("TEN")),

                HINHANH = rs.IsDBNull(rs.GetOrdinal("HINHANH"))
                    ? ""
                    : rs.GetString(rs.GetOrdinal("HINHANH")),

                DANHMUC = rs.IsDBNull(rs.GetOrdinal("DANHMUC"))
                    ? ""
                    : rs.GetString(rs.GetOrdinal("DANHMUC")),

                MNSX = rs.GetInt32(rs.GetOrdinal("MNSX")),
                MKVK = rs.GetInt32(rs.GetOrdinal("MKVK")),
                MLSP = rs.GetInt32(rs.GetOrdinal("MLSP")),
                TIENX = rs.GetInt32(rs.GetOrdinal("TIENX")),
                TIENN = rs.GetInt32(rs.GetOrdinal("TIENN")),
                SL = rs.GetInt32(rs.GetOrdinal("SL")),
                TT = rs.GetInt32(rs.GetOrdinal("TT"))
            };
        }

        // 1. Thêm sản phẩm
        public int insert(SanPhamDTO t)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    string sql = @"
                        INSERT INTO SANPHAM
                        (TEN, HINHANH, DANHMUC, MNSX, MKVK,
                         MLSP, TIENX, TIENN, SL, TT)
                        VALUES
                        (@ten, @hinhanh, @danhmuc, @mnsx, @mkvk,
                         @mlsp, @tienx, @tienn, @sl, 1)";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@ten", t.TEN);

                        cmd.Parameters.AddWithValue(
                            "@hinhanh",
                            string.IsNullOrEmpty(t.HINHANH)
                                ? "tmp.png"
                                : t.HINHANH
                        );

                        cmd.Parameters.AddWithValue(
                            "@danhmuc",
                            (object?)t.DANHMUC ?? DBNull.Value
                        );

                        cmd.Parameters.AddWithValue("@mnsx", t.MNSX);
                        cmd.Parameters.AddWithValue("@mkvk", t.MKVK);
                        cmd.Parameters.AddWithValue("@mlsp", t.MLSP);
                        cmd.Parameters.AddWithValue("@tienx", t.TIENX);
                        cmd.Parameters.AddWithValue("@tienn", t.TIENN);
                        cmd.Parameters.AddWithValue("@sl", t.SL);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Insert SanPham: " + ex.Message
                );
                return 0;
            }
        }

        // 2. Cập nhật thông tin sản phẩm
        public int update(SanPhamDTO t)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    string sql = @"
                        UPDATE SANPHAM SET
                            TEN = @ten,
                            HINHANH = @hinhanh,
                            DANHMUC = @danhmuc,
                            MNSX = @mnsx,
                            MKVK = @mkvk,
                            MLSP = @mlsp,
                            TIENX = @tienx,
                            TIENN = @tienn,
                            SL = @sl
                        WHERE MSP = @msp AND TT = 1";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@ten", t.TEN);

                        cmd.Parameters.AddWithValue(
                            "@hinhanh",
                            string.IsNullOrEmpty(t.HINHANH)
                                ? "tmp.png"
                                : t.HINHANH
                        );

                        cmd.Parameters.AddWithValue(
                            "@danhmuc",
                            (object?)t.DANHMUC ?? DBNull.Value
                        );

                        cmd.Parameters.AddWithValue("@mnsx", t.MNSX);
                        cmd.Parameters.AddWithValue("@mkvk", t.MKVK);
                        cmd.Parameters.AddWithValue("@mlsp", t.MLSP);
                        cmd.Parameters.AddWithValue("@tienx", t.TIENX);
                        cmd.Parameters.AddWithValue("@tienn", t.TIENN);
                        cmd.Parameters.AddWithValue("@sl", t.SL);
                        cmd.Parameters.AddWithValue("@msp", t.MSP);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Update SanPham: " + ex.Message
                );
                return 0;
            }
        }

        // 3. Xóa mềm sản phẩm
        // Không xóa vĩnh viễn để bảo toàn lịch sử giao dịch
        public int delete(string t)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    string sql = @"
                        UPDATE SANPHAM
                        SET TT = 0
                        WHERE MSP = @msp AND TT = 1";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@msp", SqlDbType.Int
                        ).Value = int.Parse(t);

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Delete SanPham: " + ex.Message
                );
                return 0;
            }
        }

        // 4. Lấy danh sách sản phẩm đang hoạt động
        public List<SanPhamDTO> selectAll()
        {
            List<SanPhamDTO> result =
                new List<SanPhamDTO>();

            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    string sql = @"
                        SELECT MSP, TEN, HINHANH, DANHMUC,
                               MNSX, MKVK, MLSP,
                               TIENX, TIENN, SL, TT
                        FROM SANPHAM
                        WHERE TT = 1
                        ORDER BY MSP";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    using (SqlDataReader rs =
                        cmd.ExecuteReader())
                    {
                        while (rs.Read())
                        {
                            result.Add(ReadSanPham(rs));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi SelectAll SanPham: " + ex.Message
                );
                throw;
            }

            return result;
        }

        // 5. Lấy sản phẩm theo mã
        public SanPhamDTO selectById(string t)
        {
            SanPhamDTO result = null;

            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    string sql = @"
                        SELECT MSP, TEN, HINHANH, DANHMUC,
                               MNSX, MKVK, MLSP,
                               TIENX, TIENN, SL, TT
                        FROM SANPHAM
                        WHERE MSP = @msp";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@msp", SqlDbType.Int
                        ).Value = int.Parse(t);

                        using (SqlDataReader rs =
                            cmd.ExecuteReader())
                        {
                            if (rs.Read())
                            {
                                result = ReadSanPham(rs);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi SelectById SanPham: " + ex.Message
                );
                throw;
            }

            return result;
        }

        // 6. Lọc sản phẩm theo danh mục
        public List<SanPhamDTO> selectByDanhMuc(string t)
        {
            List<SanPhamDTO> result =
                new List<SanPhamDTO>();

            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    string sql = @"
                        SELECT MSP, TEN, HINHANH, DANHMUC,
                               MNSX, MKVK, MLSP,
                               TIENX, TIENN, SL, TT
                        FROM SANPHAM
                        WHERE DANHMUC = @danhmuc
                          AND TT = 1
                        ORDER BY MSP";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@danhmuc", t
                        );

                        using (SqlDataReader rs =
                            cmd.ExecuteReader())
                        {
                            while (rs.Read())
                            {
                                result.Add(ReadSanPham(rs));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi SelectByDanhMuc: " + ex.Message
                );
                throw;
            }

            return result;
        }

        // 7. Lấy mã IDENTITY kế tiếp để tham khảo
        // Không dùng giá trị này để tự gán khóa chính
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
                                    OBJECT_ID('dbo.SANPHAM'),
                                    'TableHasIdentity'
                                ) = 1
                                THEN CAST(
                                    IDENT_CURRENT('dbo.SANPHAM')
                                    + IDENT_INCR('dbo.SANPHAM')
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
                    "Lỗi GetAutoIncrement: " + ex.Message
                );
                return -1;
            }
        }

        // 8. Cập nhật số lượng tồn kho
        // Nhập: số lượng dương
        // Xuất: số lượng âm
        public int UpdateSoLuongTon(int MSP, int soluong)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    string sql = @"
                        UPDATE SANPHAM
                        SET SL = SL + @soluong
                        WHERE MSP = @msp
                          AND TT = 1
                          AND SL + @soluong >= 0";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@msp", SqlDbType.Int
                        ).Value = MSP;

                        cmd.Parameters.Add(
                            "@soluong", SqlDbType.Int
                        ).Value = soluong;

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi UpdateSoLuongTon: " + ex.Message
                );
                return 0;
            }
        }
    }
}
