using LexosHub.ERP.VarejOnline.Domain.DTOs.SyncProcess;
using LexosHub.ERP.VarejOnline.Domain.Enums;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;

namespace LexosHub.ERP.VarejOnline.Domain.Interfaces.Services
{
    public interface ISyncProcessService
    {
        Task<Response<SyncProcessDto>> StartProcessAsync(CreateSyncProcessDto request, CancellationToken cancellationToken = default);
        Task<Response<SyncProcessDto>> UpdateProcessStatusAsync(Guid processId, SyncProcessStatusEnum status, string? info = null, CancellationToken cancellationToken = default);
        Task<Response<SyncProcessDto>> UpdateProcessProgressAsync(Guid processId, int page, int pageSize, CancellationToken cancellationToken = default);
        Task<Response<SyncProcessItemDto>> RegisterItemAsync(CreateSyncProcessItemDto request, CancellationToken cancellationToken = default);
        Task<Response<SyncProcessItemDto>> UpdateItemStatusAsync(int itemId, SyncProcessStatusEnum status, string? info = null, string? jobId = null, CancellationToken cancellationToken = default);
    }
}
