using Dapper;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace LexosHub.ERP.VarejoOnline.Infra.Data.Repositories.Persistence;

public class ApplicationWriteDbConnection : IApplicationWriteDbConnection
{
    private readonly IConfiguration _configuration;

    public ApplicationWriteDbConnection(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        await using var conn = new SqlConnection(_configuration.GetConnectionString("ErpDBConn"));
        return await conn.ExecuteAsync(sql, param, transaction);
    }

    public async Task<T> ExecuteScalarAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        await using var conn = new SqlConnection(_configuration.GetConnectionString("ErpDBConn"));
        return await conn.ExecuteScalarAsync<T>(sql, param, transaction);
    }

    public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        await using var conn = new SqlConnection(_configuration.GetConnectionString("ErpDBConn"));
        return await conn.QueryFirstOrDefaultAsync<T>(sql, param, transaction);
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        await using var conn = new SqlConnection(_configuration.GetConnectionString("ErpDBConn"));
        return await conn.QueryAsync<T>(sql, param, transaction);
    }

    public IDbConnection CreateConnection()
    {
        var conn = new SqlConnection(_configuration.GetConnectionString("ErpDBConn"));
        conn.Open();
        return conn;
    }
}