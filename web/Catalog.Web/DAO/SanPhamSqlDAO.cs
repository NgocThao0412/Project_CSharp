using System.Data;
using Microsoft.Data.SqlClient;
using Catalog.Web.Models;

namespace Catalog.Web.Services;

// Đọc từ view hợp đồng, tránh giả định tên cột ở DB hiện tại của cả nhóm.
// Cần đối chiếu SQL Server của nhóm và tạo dbo.vw_Catalog theo Database/VIEW_CONTRACT.md.
public sealed class SqlProductRepository(IConfiguration config) : IProductRepository
{
    private async Task<SqlConnection> OpenAsync(CancellationToken ct)
    {
        string? connectionString = config.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Chưa cấu hình ConnectionStrings:DefaultConnection.");
        var conn = new SqlConnection(connectionString);
        try { await conn.OpenAsync(ct); return conn; }
        catch { await conn.DisposeAsync(); throw; }
    }

    private const string Fields = "ProductId, ProductName, CategoryName, BrandName, Price, AvailableQuantity, ImageUrl, Description";

    private static CatalogProduct Read(SqlDataReader reader) => new(
        reader.GetInt32(0), reader.GetString(1), reader.GetString(2),
        reader.GetString(3), reader.GetDecimal(4), reader.GetInt32(5),
        reader.IsDBNull(6) ? null : reader.GetString(6), reader.GetString(7));

    private static SqlParameter Parameter(string name, SqlDbType type, object value) =>
        new(name, type) { Value = value };

    public async Task<CatalogProduct?> GetAsync(int id, CancellationToken ct)
    {
        await using var conn = await OpenAsync(ct);
        await using var cmd = new SqlCommand($"SELECT {Fields} FROM dbo.vw_Catalog WHERE ProductId = @id", conn);
        cmd.Parameters.Add(Parameter("@id", SqlDbType.Int, id));
        await using var r = await cmd.ExecuteReaderAsync(ct);
        return await r.ReadAsync(ct) ? Read(r) : null;
    }

    public async Task<IReadOnlyList<CatalogProduct>> FeaturedAsync(int count, CancellationToken ct)
    {
        await using var conn = await OpenAsync(ct);
        await using var cmd = new SqlCommand($"SELECT TOP (@limit) {Fields} FROM dbo.vw_Catalog ORDER BY ProductId DESC", conn);
        cmd.Parameters.Add(Parameter("@limit", SqlDbType.Int, Math.Clamp(count, 1, 20)));
        await using var r = await cmd.ExecuteReaderAsync(ct);
        var items = new List<CatalogProduct>();
        while (await r.ReadAsync(ct)) items.Add(Read(r));
        return items;
    }

    public async Task<IReadOnlyList<string>> CategoriesAsync(CancellationToken ct)
    {
        await using var conn = await OpenAsync(ct);
        await using var cmd = new SqlCommand(
            "SELECT DISTINCT CategoryName FROM dbo.vw_Catalog WHERE CategoryName <> '' ORDER BY CategoryName", conn);
        await using var r = await cmd.ExecuteReaderAsync(ct);
        var items = new List<string>();
        while (await r.ReadAsync(ct)) items.Add(r.GetString(0));
        return items;
    }

    private static string EscapeLike(string s) => s.Replace("!", "!!").Replace("%", "!%").Replace("_", "!_").Replace("[", "![");

    public async Task<PagedCatalog> SearchAsync(CatalogQuery query, CancellationToken ct)
    {
        string keyword = query.SafeKeyword;
        string category = query.SafeCategory;
        string where = "WHERE (@keyword = '' OR ProductName LIKE @pattern ESCAPE '!' OR BrandName LIKE @pattern ESCAPE '!' OR CategoryName LIKE @pattern ESCAPE '!') AND (@category = '' OR CategoryName = @category)";
        await using var conn = await OpenAsync(ct);
        async Task<SqlCommand> PrepareAsync(string sql)
        {
            var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add(Parameter("@keyword", SqlDbType.NVarChar, keyword));
            cmd.Parameters.Add(Parameter("@pattern", SqlDbType.NVarChar, "%" + EscapeLike(keyword) + "%"));
            cmd.Parameters.Add(Parameter("@category", SqlDbType.NVarChar, category));
            return await Task.FromResult(cmd);
        }
        await using var countCmd = await PrepareAsync($"SELECT COUNT(*) FROM dbo.vw_Catalog {where}");
        int total = Convert.ToInt32(await countCmd.ExecuteScalarAsync(ct));
        await using var cmd = await PrepareAsync($"SELECT {Fields} FROM dbo.vw_Catalog {where} ORDER BY ProductId DESC OFFSET @offset ROWS FETCH NEXT @limit ROWS ONLY");
        cmd.Parameters.Add(Parameter("@offset", SqlDbType.Int, (query.SafePage - 1) * CatalogQuery.PageSize));
        cmd.Parameters.Add(Parameter("@limit", SqlDbType.Int, CatalogQuery.PageSize));
        await using var r = await cmd.ExecuteReaderAsync(ct);
        var items = new List<CatalogProduct>();
        while (await r.ReadAsync(ct)) items.Add(Read(r));
        return new PagedCatalog(items, total, query.SafePage, CatalogQuery.PageSize, keyword, category);
    }
}
