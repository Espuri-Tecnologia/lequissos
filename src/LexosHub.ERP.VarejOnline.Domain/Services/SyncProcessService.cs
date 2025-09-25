using LexosHub.ERP.VarejOnline.Domain.DTOs.SyncProcess;
using LexosHub.ERP.VarejOnline.Domain.Enums;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Repositories.SyncProcess;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
using Microsoft.Extensions.Logging;

namespace LexosHub.ERP.VarejOnline.Domain.Services
{
    public class SyncProcessService : ISyncProcessService
    {
        private readonly ISyncProcessRepository _syncProcessRepository;
        private readonly ISyncProcessItemRepository _syncProcessItemRepository;
        private readonly ILogger<SyncProcessService> _logger;

        public SyncProcessService(
            ISyncProcessRepository syncProcessRepository,
            ISyncProcessItemRepository syncProcessItemRepository,
            ILogger<SyncProcessService> logger)
        {
            _syncProcessRepository = syncProcessRepository ?? throw new ArgumentNullException(nameof(syncProcessRepository));
            _syncProcessItemRepository = syncProcessItemRepository ?? throw new ArgumentNullException(nameof(syncProcessItemRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Response<SyncProcessDto>> StartProcessAsync(CreateSyncProcessDto request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            var process = new SyncProcessDto
            {
                Id = Guid.NewGuid(),
                IntegrationId = request.IntegrationId,
                TypeId = request.TypeId,
                ReferenceDate = request.ReferenceDate == default ? DateTime.UtcNow : request.ReferenceDate,
                PageSize = request.PageSize,
                Page = request.Page,
                InitialSync = request.InitialSync,
                StatusId = request.InitialStatus,
                ParentId = request.ParentId,
                AdditionalInfo = request.AdditionalInfo,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            try
            {
                await _syncProcessRepository.AddAsync(process).ConfigureAwait(false);
                _logger.LogInformation(
                    "Sync process {ProcessId} created for integration {IntegrationId} with type {ProcessType}.",
                    process.Id,
                    process.IntegrationId,
                    process.TypeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create sync process for integration {IntegrationId}.", request.IntegrationId);
                throw;
            }

            return new Response<SyncProcessDto>(process);
        }

        public async Task<Response<SyncProcessDto>> UpdateProcessStatusAsync(Guid processId, SyncProcessStatusEnum status, string? info = null, CancellationToken cancellationToken = default)
        {
            try
            {
                await _syncProcessRepository.UpdateStatusAsync(processId, status, info).ConfigureAwait(false);
                _logger.LogInformation(
                    "Sync process {ProcessId} updated to status {Status}.",
                    processId,
                    status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update sync process {ProcessId} to status {Status}.", processId, status);
                throw;
            }

            var process = new SyncProcessDto
            {
                Id = processId,
                StatusId = status,
                AdditionalInfo = info,
                UpdatedDate = DateTime.UtcNow
            };

            return new Response<SyncProcessDto>(process);
        }

        public async Task<Response<SyncProcessDto>> UpdateProcessProgressAsync(Guid processId, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            try
            {
                await _syncProcessRepository.UpdateProgressAsync(processId, page, pageSize).ConfigureAwait(false);
                _logger.LogDebug(
                    "Sync process {ProcessId} progress updated to page {Page} with size {PageSize}.",
                    processId,
                    page,
                    pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update progress for sync process {ProcessId}.", processId);
                throw;
            }

            var process = new SyncProcessDto
            {
                Id = processId,
                Page = page,
                PageSize = pageSize,
                UpdatedDate = DateTime.UtcNow
            };

            return new Response<SyncProcessDto>(process);
        }

        public async Task<Response<SyncProcessItemDto>> RegisterItemAsync(CreateSyncProcessItemDto request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            if (request.SyncProcessId == Guid.Empty)
            {
                throw new ArgumentException("Sync process id must be informed.", nameof(request.SyncProcessId));
            }

            if (string.IsNullOrWhiteSpace(request.Description))
            {
                throw new ArgumentException("Description must be informed.", nameof(request.Description));
            }

            if (string.IsNullOrWhiteSpace(request.ExternalErpId))
            {
                throw new ArgumentException("External ERP id must be informed.", nameof(request.ExternalErpId));
            }

            var item = new SyncProcessItemDto
            {
                SyncProcessId = request.SyncProcessId,
                TypeId = request.TypeId,
                StatusId = request.InitialStatus,
                Description = request.Description,
                ExternalErpId = request.ExternalErpId,
                AdditionalInfo = request.AdditionalInfo,
                JobId = request.JobId,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            try
            {
                var inserted = await _syncProcessItemRepository.AddAsync(item).ConfigureAwait(false);
                _logger.LogDebug(
                    "Sync process item {ItemId} created for process {ProcessId} with status {Status}.",
                    inserted.Id,
                    inserted.SyncProcessId,
                    inserted.StatusId);
                return new Response<SyncProcessItemDto>(inserted);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Failed to create sync process item for process {ProcessId}.",
                    request.SyncProcessId);
                throw;
            }
        }

        public async Task<Response<SyncProcessItemDto>> UpdateItemStatusAsync(int itemId, SyncProcessStatusEnum status, string? info = null, string? jobId = null, CancellationToken cancellationToken = default)
        {
            try
            {
                await _syncProcessItemRepository.UpdateStatusAsync(itemId, status, info, jobId).ConfigureAwait(false);
                _logger.LogDebug(
                    "Sync process item {ItemId} updated to status {Status}.",
                    itemId,
                    status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update sync process item {ItemId}.", itemId);
                throw;
            }

            var item = new SyncProcessItemDto
            {
                Id = itemId,
                StatusId = status,
                AdditionalInfo = info,
                JobId = jobId,
                UpdatedDate = DateTime.UtcNow
            };

            return new Response<SyncProcessItemDto>(item);
        }
    }
}
