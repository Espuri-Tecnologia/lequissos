using System.Data;

namespace LexosHub.ERP.VarejoOnline.Domain.Interfaces.Persistence;

public interface IApplicationWriteDbConnection : IApplicationReadDbConnection
{
    Task<int> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default);

    Task<T> ExecuteScalarAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default);
}