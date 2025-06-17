using Dapper;
using LexosHub.ERP.VarejoOnline.Domain.DTOs.Integration;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Persistence;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Repositories.Integration;
using Microsoft.Extensions.Logging;

namespace LexosHub.ERP.VarejoOnline.Infra.Data.Repositories.Integration
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
                await _writeDbConnection.ExecuteAsync(
                    sql: @"INSERT INTO [Integration]
                           (
                               [HubIntegrationId]
                               ,[TenantId]
                               ,[HubKey]
                               ,[Url]
                               ,[User]
                               ,[Password]
                               ,[IsActive]
                               ,[CreatedDate]
                               ,[UpdatedDate]
                           )
                           OUTPUT INSERTED.Id
                           VALUES
                           (
                               @HubIntegrationId
                               ,@TenantId
                               ,@HubKey
                               ,@Url
                               ,@User
                               ,@Password
                               ,@IsActive
                               ,GETDATE()
                               ,GETDATE()
                           );",
                    param: new
                    {
                        integration.HubIntegrationId,
                        integration.TenantId,
                        integration.HubKey,
                        integration.Url,
                        integration.User,
                        integration.Password,
                        integration.IsActive,
                        integration.Id
                    }
                );

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
                               ,[User] = @User
                               ,[Password] = @Password
                               ,[IsActive] = @IsActive
                               ,[LastSyncDate] = @LastSyncDate
                               ,[HasValidVersion] = @HasValidVersion
                           WHERE
                               [Id] = @Id",
                    param: new
                    {
                        integration.Url,
                        integration.User,
                        integration.Password,
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
                               ,[User]
                               ,[Password]
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
                          ,I.[User]
                          ,I.[Password]
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
                           ,[User]
                           ,[Password]
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
                           ,[User]
                           ,[Password]
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
                           ,[User]
                           ,[Password]
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
