using Dapper;
using LexosHub.ERP.VarejOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Persistence;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Repositories.Integration;
using Microsoft.Extensions.Logging;

namespace LexosHub.ERP.VarejOnline.Infra.Data.Repositories.Integration
{
    public class IntegrationRepository : IIntegrationRepository
    {
        private readonly IApplicationReadDbConnection _readDbConnection;
        private readonly IApplicationWriteDbConnection _writeDbConnection;
        private readonly ILogger<IntegrationRepository> _logger;

        public IntegrationRepository(IApplicationReadDbConnection readDbConnection,
                                     IApplicationWriteDbConnection writeDbConnection,
                                     ILogger<IntegrationRepository> logger)
        {
            _readDbConnection = readDbConnection ?? throw new ArgumentNullException(nameof(readDbConnection));
            _writeDbConnection = writeDbConnection ?? throw new ArgumentNullException(nameof(writeDbConnection));
            _logger = logger;
        }

        public async Task<IntegrationDto> AddAsync(IntegrationDto integration)
        {
            try
            {
                var id = await _writeDbConnection.ExecuteScalarAsync<int>(
                    sql: @"INSERT INTO [Integration]
                           (
                               [HubIntegrationId]
                               ,[TenantId]
                               ,[HubKey]
                               ,[Cnpj]
                               ,[Url]
                               ,[Token]
                               ,[RefreshToken]
                               ,[IsActive]
                               ,[CreatedDate]
                               ,[UpdatedDate]
                               ,[HasValidVersion]
                           )
                           OUTPUT INSERTED.Id
                           VALUES
                           (
                               @HubIntegrationId
                               ,@TenantId
                               ,@HubKey
                               ,@Cnpj
                               ,@Url
                               ,@Token
                               ,@RefreshToken
                               ,@IsActive
                               ,GETDATE()
                               ,GETDATE()
                               ,@HasValidVersion
                           );",
                    param: new
                    {
                        integration.HubIntegrationId,
                        integration.TenantId,
                        integration.HubKey,
                        integration.Cnpj,
                        integration.Url,
                        integration.Token,
                        integration.RefreshToken,
                        integration.IsActive,
                        integration.HasValidVersion
                    }
                );
                integration.Id = id;

                return integration;
            }
            catch (Exception e)
            {
                _logger.LogError("----- IntegrationRepository -> Error on AddAsync {integrationId} - {error} ----", integration.Id, e.Message + "INNER EX: " + e.InnerException);
                throw;
            }
        }

        public async Task<IntegrationDto> UpdateAsync(IntegrationDto integration)
        {
            try
            {
                await _writeDbConnection.ExecuteAsync(
                    sql: @"UPDATE
                               Integration
                           SET
                                [Url] = @Url
                               ,[Token] = @Token
                               ,[RefreshToken] = @RefreshToken
                               ,[IsActive] = @IsActive
                               ,[LastSyncDate] = @LastSyncDate
                               ,[HasValidVersion] = @HasValidVersion
                           WHERE
                               [Id] = @Id",
                    param: new
                    {
                        integration.Url,
                        integration.Token,
                        integration.RefreshToken,
                        integration.IsActive,
                        integration.LastSyncDate,
                        integration.Id,
                        integration.HasValidVersion
                    }
                );

                return integration;
            }
            catch (Exception e)
            {
                _logger.LogError("----- IntegrationRepository -> Error on UpdateAsync {integrationId} - {error} ----", integration.Id, e.Message + "INNER EX: " + e.InnerException);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(string key)
        {
            try
            {
                await _writeDbConnection.ExecuteAsync(
                    sql: "DELETE FROM Integration WHERE [HubKey] = @HubKey",
                    param: new { HubKey = key });

                return true;
            }
            catch (Exception e)
            {
                _logger.LogError("----- IntegrationRepository -> Error on DeleteAsync {hubKey} - {error} ----", key, e.Message + "INNER EX: " + e.InnerException);
                throw;
            }
        }

        public async Task<IntegrationDto> GetByIdAsync(int id)
        {
            try
            {
                return await _readDbConnection.QueryFirstOrDefaultAsync<IntegrationDto>(
                    sql: @"SELECT
                               [Id]
                               ,[HubIntegrationId]
                               ,[TenantId]
                               ,[HubKey]
                               ,[Url]
                               ,[Token]
                               ,[RefreshToken]
                               ,[IsActive]
                               ,[CreatedDate]
                               ,[UpdatedDate]
                               ,[LastSyncDate]
                               ,[HasValidVersion]
                           FROM
                               [Integration] NOLOCK
                           WHERE
                               [Id] = @Id
                           ORDER BY
                               [CreatedDate] DESC",
                    param: new { Id = id });
            }
            catch (Exception e)
            {
                _logger.LogError("----- IntegrationRepository -> Error on GetByIdAsync {integrationId} - {error} ----", id, e.Message + "INNER EX: " + e.InnerException);
                throw;
            }
        }


    public async Task<IntegrationDto> GetByDocument(string cnpj)
    {
      try
      {
        return await _readDbConnection.QueryFirstOrDefaultAsync<IntegrationDto>(
            sql: @"SELECT
                               [Id]
                               ,[HubIntegrationId]
                               ,[TenantId]
                               ,[HubKey]
                               ,[Url]
                               ,[Token]
                               ,[RefreshToken]
                               ,[IsActive]
                               ,[CreatedDate]
                               ,[UpdatedDate]
                               ,[LastSyncDate]
                               ,[HasValidVersion]
                           FROM
                               [Integration] NOLOCK
                           WHERE
                               [Cnpj] = @Cnpj
                           ORDER BY
                               [CreatedDate] DESC",
            param: new { Cnpj = cnpj });
      }
      catch (Exception e)
      {
        _logger.LogError("----- IntegrationRepository -> Error on GetByDocument {integrationId} - {error} ----", cnpj, e.Message + "INNER EX: " + e.InnerException);
        throw;
      }
    }

    public async Task<IntegrationDto> GetByIdWithLastOrderDateAsync(int id)
        {
            try
            {
                return await _readDbConnection.QueryFirstOrDefaultAsync<IntegrationDto>(
                    sql: @"SELECT TOP 1
                           SP.ReferenceDate AS LastOrderSyncDate
                          ,I.[Id]
                          ,I.[HubIntegrationId]
                          ,I.[TenantId]
                          ,I.[HubKey]
                          ,I.[Url]
                          ,I.[Token]
                          ,I.[RefreshToken]
                          ,I.[IsActive]
                          ,I.[CreatedDate]
                          ,I.[UpdatedDate]
                          ,I.[LastSyncDate]
                          ,I.[HasValidVersion]
                       FROM
                           [Integration] I (NOLOCK)
                           LEFT JOIN
                           (
                               SELECT TOP 1
                                   IntegrationId,
                                   ReferenceDate
                               FROM
                                   [SyncProcess] SP (NOLOCK)
                               WHERE 
                                   SP.IntegrationId = @Id
                                   AND SP.StatusId = 2
                                   AND SP.TypeId = 6
                               ORDER BY ReferenceDate DESC
                           ) AS SP ON SP.IntegrationId = I.Id
                       WHERE
                           I.Id = @Id",
                    param: new { Id = id });
            }
            catch (Exception e)
            {
                _logger.LogError("----- IntegrationRepository -> Error on GetByIdWithLastOrderDateAsync {integrationId} - {error} ----", id, e.Message + "INNER EX: " + e.InnerException);
                throw;
            }
        }

        public async Task<IntegrationDto> GetByKeyAsync(string key)
        {
            try
            {
                return await _readDbConnection.QueryFirstOrDefaultAsync<IntegrationDto>(
                    sql: @"SELECT TOP 1
                           [Id]
                           ,[HubIntegrationId]
                           ,[TenantId]
                           ,[HubKey]
                           ,[Url]
                           ,[Cnpj]
                           ,[Token]
                           ,[RefreshToken]
                           ,[IsActive]
                           ,[CreatedDate]
                           ,[UpdatedDate]
                           ,[LastSyncDate]
                           ,[HasValidVersion]
                        FROM
                            [Integration] NOLOCK
                        WHERE
                            [HubKey] = @HubKey
                        ORDER BY
                            [CreatedDate] DESC",
                    param: new { HubKey = key });
            }
            catch (Exception e)
            {
                _logger.LogError("----- IntegrationRepository -> Error on GetByKeyAsync {hubKey} - {error} ----", key, e.Message + "INNER EX: " + e.InnerException);
                throw;
            }
        }

        public async Task<IEnumerable<IntegrationDto>> GetActiveIntegrations()
        {
            try
            {
                return await _readDbConnection.QueryAsync<IntegrationDto>(
                    sql: @"SELECT 
                           [Id]
                           ,[HubIntegrationId]
                           ,[TenantId]
                           ,[HubKey]
                           ,[Url]
                           ,[Token]
                           ,[RefreshToken]
                           ,[IsActive]
                           ,[CreatedDate]
                           ,[UpdatedDate]
                           ,[LastSyncDate]
                           ,[HasValidVersion]
                       FROM
                           [Integration] NOLOCK
                       WHERE
                           [IsActive] = 1");
            }
            catch (Exception e)
            {
                _logger.LogError("----- IntegrationRepository -> Error on GetActiveIntegrations - {error} ----", e.Message + "INNER EX: " + e.InnerException);
                throw;
            }
        }

        public async Task<IntegrationDto> GetNextToFirstSync()
        {
            try
            {
                return await _readDbConnection.QueryFirstOrDefaultAsync<IntegrationDto>(
                    sql: @"SELECT TOP 1
                           [Id]
                           ,[HubIntegrationId]
                           ,[TenantId]
                           ,[HubKey]
                           ,[Url]
                           ,[Token]
                           ,[RefreshToken]
                           ,[IsActive]
                           ,[CreatedDate]
                           ,[UpdatedDate]
                           ,[LastSyncDate]
                           ,[HasValidVersion]
                       FROM
                           [Integration] NOLOCK
                       WHERE
                           [LastSyncDate] IS NULL 
                           AND [IsActive] = 1
                       ORDER BY
                           [CreatedDate] ASC");
            }
            catch (Exception e)
            {
                _logger.LogError("----- IntegrationRepository -> Error on GetNextToFirstSync - {error} ----", e.Message + "INNER EX: " + e.InnerException);
                throw;
            }
        }
    }
}
