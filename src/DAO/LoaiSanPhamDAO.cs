
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using src.config;
using src.DTO;

namespace src.DAO
{
    public class LoaiSanPhamDAO
    {
        private static LoaiSanPhamDAO instance;

        public static LoaiSanPhamDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new LoaiSanPhamDAO();

                return instance;
            }
        }

        private LoaiSanPhamDAO() { }

        // 1. Lấy danh sách loại sản phẩm đang hoạt động
        public List<LoaiSanPhamDTO> selectAll()
        {
            List<LoaiSanPhamDTO> result =
                new List<LoaiSanPhamDTO>();

            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    string sql = @"
                        SELECT MLSP, TEN, TLGX, GHICHU, TT
                        FROM LOAISANPHAM
                        WHERE TT = 1
                        ORDER BY MLSP";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    using (SqlDataReader rs =
                        cmd.ExecuteReader())
                    {
                        while (rs.Read())
                        {
                            LoaiSanPhamDTO lsp =
                                new LoaiSanPhamDTO();

                            lsp.MLSP =
                                rs.GetInt32(rs.GetOrdinal("MLSP"));

                            lsp.TEN =
                                rs.GetString(rs.GetOrdinal("TEN"));

                            lsp.TLGX =
                                rs.GetInt32(rs.GetOrdinal("TLGX"));

                            int ghiChuIndex =
                                rs.GetOrdinal("GHICHU");

                            lsp.GHICHU =
                                rs.IsDBNull(ghiChuIndex)
                                    ? ""
                                    : rs.GetString(ghiChuIndex);

                            lsp.TT =
                                rs.GetInt32(rs.GetOrdinal("TT"));

                            result.Add(lsp);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi SelectAll LoaiSanPham: " + ex.Message);
                throw;
            }

            return result;
        }

        // 2. Thêm loại sản phẩm
        public int insert(LoaiSanPhamDTO lsp)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    string sql = @"
                        INSERT INTO LOAISANPHAM
                            (TEN, TLGX, GHICHU, TT)
                        VALUES
                            (@TEN, @TLGX, @GHICHU, 1)";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@TEN", lsp.TEN);

                        cmd.Parameters.Add(
                            "@TLGX", SqlDbType.Int
                        ).Value = lsp.TLGX;

                        cmd.Parameters.AddWithValue(
                            "@GHICHU", lsp.GHICHU ?? "");

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Insert LoaiSanPham: " + ex.Message);
                return 0;
            }
        }

        // 3. Cập nhật loại sản phẩm
        public int update(LoaiSanPhamDTO lsp)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    string sql = @"
                        UPDATE LOAISANPHAM
                        SET TEN = @TEN,
                            TLGX = @TLGX,
                            GHICHU = @GHICHU
                        WHERE MLSP = @MLSP
                          AND TT = 1";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue(
                            "@TEN", lsp.TEN);

                        cmd.Parameters.Add(
                            "@TLGX", SqlDbType.Int
                        ).Value = lsp.TLGX;

                        cmd.Parameters.AddWithValue(
                            "@GHICHU", lsp.GHICHU ?? "");

                        cmd.Parameters.Add(
                            "@MLSP", SqlDbType.Int
                        ).Value = lsp.MLSP;

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Update LoaiSanPham: " + ex.Message);
                return 0;
            }
        }

        // 4. Xóa mềm loại sản phẩm
        public int delete(int mlsp)
        {
            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    string sql = @"
                        UPDATE LOAISANPHAM
                        SET TT = 0
                        WHERE MLSP = @MLSP
                          AND TT = 1";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@MLSP", SqlDbType.Int
                        ).Value = mlsp;

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Delete LoaiSanPham: " + ex.Message);
                return 0;
            }
        }

        // 5. Lấy mã IDENTITY kế tiếp để tham khảo
        // Không dùng kết quả này để tự cấp khóa chính
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
                                    OBJECT_ID('dbo.LOAISANPHAM'),
                                    'TableHasIdentity'
                                ) = 1
                                THEN CAST(
                                    IDENT_CURRENT('dbo.LOAISANPHAM')
                                    + IDENT_INCR('dbo.LOAISANPHAM')
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
                    "Lỗi getAutoIncrement LoaiSanPham: "
                    + ex.Message);
                return -1;
            }
        }
    }
}
