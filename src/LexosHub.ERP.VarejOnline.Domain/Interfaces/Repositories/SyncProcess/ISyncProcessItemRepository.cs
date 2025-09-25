using LexosHub.ERP.VarejOnline.Domain.DTOs.SyncProcess;
using LexosHub.ERP.VarejOnline.Domain.Enums;

namespace LexosHub.ERP.VarejOnline.Domain.Interfaces.Repositories.SyncProcess
{
    public interface ISyncProcessItemRepository
    {
        Task<SyncProcessItemDto> AddAsync(SyncProcessItemDto item);
        Task UpdateStatusAsync(int id, SyncProcessStatusEnum status, string? info, string? jobId = null);
    }
}
