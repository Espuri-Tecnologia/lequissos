using LexosHub.ERP.VarejOnline.Domain.DTOs.SyncProcess;
using LexosHub.ERP.VarejOnline.Domain.Enums;

namespace LexosHub.ERP.VarejOnline.Domain.Interfaces.Repositories.SyncProcess
{
    public interface ISyncProcessRepository
    {
        Task AddAsync(SyncProcessDto process);
        Task UpdateStatusAsync(Guid id, SyncProcessStatusEnum status, string? info);
        Task UpdateProgressAsync(Guid id, int page, int pageSize);
    }
}
