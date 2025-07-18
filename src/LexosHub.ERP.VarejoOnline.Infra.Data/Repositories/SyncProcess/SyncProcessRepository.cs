using LexosHub.ERP.VarejoOnline.Domain.DTOs.SyncProcess;
using LexosHub.ERP.VarejoOnline.Domain.Enums;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Persistence;
using LexosHub.ERP.VarejoOnline.Domain.Interfaces.Repositories.SyncProcess;
using Microsoft.Extensions.Logging;

namespace LexosHub.ERP.VarejoOnline.Infra.Data.Repositories.SyncProcess
{
    public class SyncProcessRepository : ISyncProcessRepository
    {
        private readonly IApplicationWriteDbConnection _writeDbConnection;
        private readonly ILogger<SyncProcessRepository> _logger;

        public SyncProcessRepository(IApplicationWriteDbConnection writeDbConnection, ILogger<SyncProcessRepository> logger)
        {
            _writeDbConnection = writeDbConnection;
            _logger = logger;
        }

        public async Task AddAsync(SyncProcessDto process)
        {
            try
            {
                if (process.Id == Guid.Empty)
                {
                    process.Id = Guid.NewGuid();
                }

                await _writeDbConnection.ExecuteAsync(
                    sql: @"INSERT INTO [SyncProcess]
                               ([Id]
                               ,[IntegrationId]
                               ,[TypeId]
                               ,[ReferenceDate]
                               ,[PageSize]
                               ,[Page]
                               ,[InitialSync]
                               ,[StatusId]
                               ,[ParentId]
                               ,[AdditionalInfo]
                               ,[CreatedDate]
                               ,[UpdatedDate])
                           VALUES
                               (@Id
                               ,@IntegrationId
                               ,@TypeId
                               ,@ReferenceDate
                               ,@PageSize
                               ,@Page
                               ,@InitialSync
                               ,@StatusId
                               ,@ParentId
                               ,@AdditionalInfo
                               ,GETDATE()
                               ,GETDATE());",
                    param: new
                    {
                        process.Id,
                        process.IntegrationId,
                        TypeId = (int)process.TypeId,
                        process.ReferenceDate,
                        process.PageSize,
                        process.Page,
                        process.InitialSync,
                        StatusId = (int)process.StatusId,
                        process.ParentId,
                        process.AdditionalInfo
                    });
            }
            catch (Exception e)
            {
                _logger.LogError("----- SyncProcessRepository -> Error on AddAsync {error} ----", e.Message);
                throw;
            }
        }

        public async Task UpdateStatusAsync(Guid id, SyncProcessStatusEnum status, string? info)
        {
            try
            {
                await _writeDbConnection.ExecuteAsync(
                    sql: @"UPDATE [SyncProcess]
                           SET [StatusId] = @StatusId,
                               [AdditionalInfo] = @AdditionalInfo,
                               [UpdatedDate] = GETDATE()
                           WHERE [Id] = @Id;",
                    param: new
                    {
                        Id = id,
                        StatusId = (int)status,
                        AdditionalInfo = info
                    });
            }
            catch (Exception e)
            {
                _logger.LogError("----- SyncProcessRepository -> Error on UpdateStatusAsync {error} ----", e.Message);
                throw;
            }
        }
    }
}
