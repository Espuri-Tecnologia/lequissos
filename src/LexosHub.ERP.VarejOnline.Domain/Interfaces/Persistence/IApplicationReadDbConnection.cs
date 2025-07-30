using System.Data;

namespace LexosHub.ERP.VarejOnline.Domain.Interfaces.Persistence;

public interface IApplicationReadDbConnection
{
    IDbConnection CreateConnection();
    Task<T> QueryFirstOrDefaultAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default);

    Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null, IDbTransaction? transaction = null, CancellationToken cancellationToken = default);
}