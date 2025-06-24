using Dapper;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

public class ApplicationReadDbConnection : IApplicationReadDbConnection
{
    private readonly IConfiguration _configuration;

    public ApplicationReadDbConnection(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDbConnection CreateConnection()
    {
        var conn = new SqlConnection(_configuration.GetConnectionString("ErpDBConn"));
        conn.Open();
        return conn;
    }

    public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        using var conn = CreateConnection();
        return await conn.QueryFirstOrDefaultAsync<T>(sql, param, transaction);
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        using var conn = CreateConnection();
        return await conn.QueryAsync<T>(sql, param, transaction);
    }
}
