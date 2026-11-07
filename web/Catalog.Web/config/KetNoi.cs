using Microsoft.Data.SqlClient;

namespace Catalog.Web.config;

public sealed class KetNoi(IConfiguration cauHinh)
{
    public async Task<SqlConnection> MoAsync(CancellationToken ct = default)
    {
        var chuoi = cauHinh.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(chuoi))
            throw new InvalidOperationException("Chưa cấu hình kết nối SQL Server.");

        var ketNoi = new SqlConnection(chuoi);
        try
        {
            await ketNoi.OpenAsync(ct);
            return ketNoi;
        }
        catch
        {
            await ketNoi.DisposeAsync();
            throw;
        }
    }
}
