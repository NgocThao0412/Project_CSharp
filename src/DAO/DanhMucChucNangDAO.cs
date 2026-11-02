
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using src.DTO;
using src.config;

namespace src.DAO
{
    public class DanhMucChucNangDAO
    {
        private static DanhMucChucNangDAO instance;

        public static DanhMucChucNangDAO Instance
        {
            get
            {
                if (instance == null)
                    instance = new DanhMucChucNangDAO();

                return instance;
            }
        }

        private DanhMucChucNangDAO() { }

        // Lấy tất cả danh mục chức năng từ SQL Server
        public List<DanhMucChucNangDTO> SelectAll()
        {
            List<DanhMucChucNangDTO> result =
                new List<DanhMucChucNangDTO>();

            try
            {
                using (SqlConnection conn =
                    SqlServerHelper.GetConnection())
                {
                    string sql = @"
                        SELECT MCN, TEN
                        FROM DANHMUCCHUCNANG
                        ORDER BY MCN";

                    using (SqlCommand cmd =
                        new SqlCommand(sql, conn))
                    {
                        using (SqlDataReader rs =
                            cmd.ExecuteReader())
                        {
                            while (rs.Read())
                            {
                                string machucnang =
                                    rs.GetString(
                                        rs.GetOrdinal("MCN"));

                                string tenchucnang =
                                    rs.GetString(
                                        rs.GetOrdinal("TEN"));

                                DanhMucChucNangDTO chucnang =
                                    new DanhMucChucNangDTO(
                                        machucnang,
                                        tenchucnang
                                    );

                                result.Add(chucnang);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    "Lỗi SelectAll DanhMucChucNang: "
                    + ex.Message
                );

                throw;
            }

            return result;
        }
    }
}
