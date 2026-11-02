
using System;
using Microsoft.Data.SqlClient;

namespace src.config
{
    public static class SqlServerHelper
    {
        private static readonly string connectionString =
            "Server=localhost;Database=quanlykho;Integrated Security=True;Encrypt=True;TrustServerCertificate=True;";

        public static SqlConnection GetConnection()
        {
            var conn = new SqlConnection(connectionString);
            conn.Open();
            return conn;
        }
    }
}
