using LexosHub.ERP.VarejoOnline.Domain.DTOs.SyncProcess;
using LexosHub.ERP.VarejoOnline.Domain.Enums;

namespace LexosHub.ERP.VarejoOnline.Domain.Interfaces.Repositories.SyncProcess
{
    public interface ISyncProcessRepository
    {
        Task AddAsync(SyncProcessDto process);
        Task UpdateStatusAsync(Guid id, SyncProcessStatusEnum status, string? info);
    }
}
