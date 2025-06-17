using Dapper;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Persistence;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace LexosHub.ERP.VarejoOnline.Infra.Data.Repositories.Persistence;

public class ApplicationReadDbConnection : IApplicationReadDbConnection, IDisposable
{
    private readonly IDbConnection _connection;

    public ApplicationReadDbConnection(IConfiguration configuration)
    {
        _connection = new SqlConnection(configuration.GetConnectionString("ErpDBConn"));
    }

    public async Task<T> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        return await _connection.QueryFirstOrDefaultAsync<T>(sql, param, transaction);
    }

    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default)
    {
        return await _connection.QueryAsync<T>(sql, param, transaction);
    }

    public void Dispose()
    {
        _connection.Dispose();
    }
}