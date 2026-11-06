
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using src.DTO;
using src.config;

namespace src.DAO
{
    public class ChiTietPhieuKiemKeDAO
        : ChiTietInterface<ChiTietPhieuKiemKeDTO>
    {
        private static ChiTietPhieuKiemKeDAO instance;

        public static ChiTietPhieuKiemKeDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new ChiTietPhieuKiemKeDAO();

                return instance;
            }
        }

        private ChiTietPhieuKiemKeDAO() { }

        // Chuyển dữ liệu SQL Server thành DTO
        private static ChiTietPhieuKiemKeDTO ReadChiTiet(
            SqlDataReader reader)
        {
            ChiTietPhieuKiemKeDTO ct =
                new ChiTietPhieuKiemKeDTO();

            ct.MPKK = reader.GetInt32(
                reader.GetOrdinal("MPKK"));

            ct.MSP = reader.GetInt32(
                reader.GetOrdinal("MSP"));

            ct.TRANGTHAISP = reader.GetInt32(
                reader.GetOrdinal("TRANGTHAISP"));

            int ghiChuIndex = reader.GetOrdinal("GHICHU");

            ct.GHICHU = reader.IsDBNull(ghiChuIndex)
                ? ""
                : reader.GetString(ghiChuIndex);

            return ct;
        }

        // 1. Thêm danh sách chi tiết kiểm kê
        // Transaction bảo đảm tất cả cùng thành công
        // hoặc tất cả cùng được hoàn tác.
        public int insert(List<ChiTietPhieuKiemKeDTO> t)
        {
            if (t == null)
                return -1;

            if (t.Count == 0)
                return 0;

            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                using (SqlTransaction transaction =
                    conn.BeginTransaction())
                {
                    try
                    {
                        const string sql = @"
                            INSERT INTO CTPHIEUKIEMKE
                                (MPKK, MSP, TRANGTHAISP, GHICHU)
                            VALUES
                                (@mpkk, @msp, @tt, @ghichu)";

                        int result = 0;

                        foreach (ChiTietPhieuKiemKeDTO item in t)
                        {
                            using (SqlCommand cmd =
                                new SqlCommand(
                                    sql, conn, transaction))
                            {
                                cmd.Parameters.Add(
                                    "@mpkk", SqlDbType.Int
                                ).Value = item.MPKK;

                                cmd.Parameters.Add(
                                    "@msp", SqlDbType.Int
                                ).Value = item.MSP;

                                cmd.Parameters.Add(
                                    "@tt", SqlDbType.Int
                                ).Value = item.TRANGTHAISP;

                                cmd.Parameters.Add(
                                    "@ghichu",
                                    SqlDbType.NVarChar, -1
                                ).Value =
                                    (object?)item.GHICHU
                                    ?? DBNull.Value;

                                result += cmd.ExecuteNonQuery();
                            }
                        }

                        transaction.Commit();
                        return result;
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Insert CTPhieuKiemKe: "
                    + ex.Message);

                return -1;
            }
        }

        // 2. Xóa chi tiết theo mã phiếu kiểm kê
        public int delete(string t)
        {
            if (!int.TryParse(t, out int mpkk))
                return -1;

            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    const string sql = @"
                        DELETE FROM CTPHIEUKIEMKE
                        WHERE MPKK = @mpkk";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@mpkk", SqlDbType.Int
                        ).Value = mpkk;

                        return cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi Delete CTPhieuKiemKe: "
                    + ex.Message);

                return -1;
            }
        }

        // 3. Cập nhật chi tiết kiểm kê
        // Source gốc chưa triển khai phương thức này.
        public int update(
            List<ChiTietPhieuKiemKeDTO> t,
            string pk)
        {
            throw new NotImplementedException(
                "Chức năng cập nhật chi tiết kiểm kê " +
                "chưa được triển khai."
            );
        }

        // 4. Lấy danh sách chi tiết theo mã phiếu kiểm kê
        public List<ChiTietPhieuKiemKeDTO> selectAll(string t)
        {
            List<ChiTietPhieuKiemKeDTO> result =
                new List<ChiTietPhieuKiemKeDTO>();

            if (!int.TryParse(t, out int mpkk))
                return result;

            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    const string sql = @"
                        SELECT MPKK, MSP,
                               TRANGTHAISP, GHICHU
                        FROM CTPHIEUKIEMKE
                        WHERE MPKK = @mpkk
                        ORDER BY MSP";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.Add(
                            "@mpkk", SqlDbType.Int
                        ).Value = mpkk;

                        using (SqlDataReader reader =
                            cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                result.Add(
                                    ReadChiTiet(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi SelectAll CTPhieuKiemKe: "
                    + ex.Message);

                throw;
            }

            return result;
        }
    }
}
