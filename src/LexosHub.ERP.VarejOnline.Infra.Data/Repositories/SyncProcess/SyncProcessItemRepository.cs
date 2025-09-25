using LexosHub.ERP.VarejOnline.Domain.DTOs.SyncProcess;
using LexosHub.ERP.VarejOnline.Domain.Enums;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Persistence;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Repositories.SyncProcess;
using Microsoft.Extensions.Logging;

namespace LexosHub.ERP.VarejOnline.Infra.Data.Repositories.SyncProcess
{
    public class SyncProcessItemRepository : ISyncProcessItemRepository
    {
        private readonly IApplicationWriteDbConnection _writeDbConnection;
        private readonly ILogger<SyncProcessItemRepository> _logger;

        public SyncProcessItemRepository(IApplicationWriteDbConnection writeDbConnection, ILogger<SyncProcessItemRepository> logger)
        {
            _writeDbConnection = writeDbConnection;
            _logger = logger;
        }

        public async Task<SyncProcessItemDto> AddAsync(SyncProcessItemDto item)
        {
            try
            {
                var id = await _writeDbConnection.ExecuteScalarAsync<int>(
                    sql: @"INSERT INTO [SyncProcessItem]
                               ([SyncProcessId]
                               ,[TypeId]
                               ,[StatusId]
                               ,[Description]
                               ,[ExternalErpId]
                               ,[AdditionalInfo]
                               ,[JobId]
                               ,[CreatedDate]
                               ,[UpdatedDate])
                           OUTPUT INSERTED.Id
                           VALUES
                               (@SyncProcessId
                               ,@TypeId
                               ,@StatusId
                               ,@Description
                               ,@ExternalErpId
                               ,@AdditionalInfo
                               ,@JobId
                               ,GETDATE()
                               ,GETDATE());",
                    param: new
                    {
                        item.SyncProcessId,
                        TypeId = (int)item.TypeId,
                        StatusId = (int)item.StatusId,
                        item.Description,
                        item.ExternalErpId,
                        item.AdditionalInfo,
                        item.JobId
                    });

                item.Id = id;
                item.CreatedDate = DateTime.UtcNow;
                item.UpdatedDate = DateTime.UtcNow;

                return item;
            }
            catch (Exception e)
            {
                _logger.LogError("----- SyncProcessItemRepository -> Error on AddAsync {error} ----", e.Message);
                throw;
            }
        }

        public async Task UpdateStatusAsync(int id, SyncProcessStatusEnum status, string? info, string? jobId = null)
        {
            try
            {
                await _writeDbConnection.ExecuteAsync(
                    sql: @"UPDATE [SyncProcessItem]
                           SET [StatusId] = @StatusId,
                               [AdditionalInfo] = @AdditionalInfo,
                               [JobId] = CASE WHEN @JobId IS NULL THEN [JobId] ELSE @JobId END,
                               [UpdatedDate] = GETDATE()
                           WHERE [Id] = @Id;",
                    param: new
                    {
                        Id = id,
                        StatusId = (int)status,
                        AdditionalInfo = info,
                        JobId = jobId
                    });
            }
            catch (Exception e)
            {
                _logger.LogError("----- SyncProcessItemRepository -> Error on UpdateStatusAsync {error} ----", e.Message);
                throw;
            }
        }
    }
}
