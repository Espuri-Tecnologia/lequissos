using LexosHub.ERP.VarejOnline.Domain.DTOs.SyncProcess;
using LexosHub.ERP.VarejOnline.Domain.Enums;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Repositories.SyncProcess;
using LexosHub.ERP.VarejOnline.Domain.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace LexosHub.ERP.VarejOnline.Domain.Tests.Services
{
    public class SyncProcessServiceTests
    {
        private readonly Mock<ISyncProcessRepository> _processRepository = new();
        private readonly Mock<ISyncProcessItemRepository> _itemRepository = new();
        private readonly Mock<ILogger<SyncProcessService>> _logger = new();

        private SyncProcessService CreateService() => new(_processRepository.Object, _itemRepository.Object, _logger.Object);

        [Fact]
        public async Task StartProcessAsync_ShouldPersistProcess()
        {
            var service = CreateService();
            var request = new CreateSyncProcessDto
            {
                IntegrationId = 42,
                TypeId = SyncProcessTypeEnum.Stock,
                Page = 0,
                PageSize = 25,
                InitialSync = true,
                InitialStatus = SyncProcessStatusEnum.Running
            };

            SyncProcessDto? capturedProcess = null;
            _processRepository
                .Setup(repo => repo.AddAsync(It.IsAny<SyncProcessDto>()))
                .Callback<SyncProcessDto>(dto => capturedProcess = dto)
                .Returns(Task.CompletedTask);

            var response = await service.StartProcessAsync(request);

            Assert.NotNull(response.Result);
            Assert.Equal(request.IntegrationId, response.Result!.IntegrationId);
            Assert.Equal(request.TypeId, response.Result.TypeId);
            Assert.Equal(request.InitialStatus, response.Result.StatusId);
            Assert.NotEqual(Guid.Empty, response.Result.Id);

            _processRepository.Verify(repo => repo.AddAsync(It.Is<SyncProcessDto>(p =>
                p.IntegrationId == request.IntegrationId &&
                p.TypeId == request.TypeId &&
                p.StatusId == request.InitialStatus)), Times.Once);

            Assert.NotNull(capturedProcess);
            Assert.Equal(response.Result!.Id, capturedProcess!.Id);
        }

        [Fact]
        public async Task RegisterItemAsync_ShouldAddItem()
        {
            var service = CreateService();
            var processId = Guid.NewGuid();
            var request = new CreateSyncProcessItemDto
            {
                SyncProcessId = processId,
                TypeId = SyncProcessTypeEnum.Stock,
                Description = "Processing page 1",
                ExternalErpId = "1"
            };

            _itemRepository
                .Setup(repo => repo.AddAsync(It.IsAny<SyncProcessItemDto>()))
                .ReturnsAsync((SyncProcessItemDto dto) =>
                {
                    dto.Id = 10;
                    return dto;
                });

            var response = await service.RegisterItemAsync(request);

            Assert.NotNull(response.Result);
            Assert.Equal(10, response.Result!.Id);
            Assert.Equal(processId, response.Result.SyncProcessId);
            Assert.Equal(request.Description, response.Result.Description);

            _itemRepository.Verify(repo => repo.AddAsync(It.Is<SyncProcessItemDto>(item =>
                item.SyncProcessId == processId &&
                item.Description == request.Description)), Times.Once);
        }

        [Fact]
        public async Task UpdateProcessStatusAsync_ShouldInvokeRepository()
        {
            var service = CreateService();
            var processId = Guid.NewGuid();

            var response = await service.UpdateProcessStatusAsync(processId, SyncProcessStatusEnum.Finished, "done");

            Assert.NotNull(response.Result);
            Assert.Equal(SyncProcessStatusEnum.Finished, response.Result!.StatusId);

            _processRepository.Verify(repo => repo.UpdateStatusAsync(processId, SyncProcessStatusEnum.Finished, "done"), Times.Once);
        }

        [Fact]
        public async Task UpdateItemStatusAsync_ShouldInvokeRepository()
        {
            var service = CreateService();

            var response = await service.UpdateItemStatusAsync(5, SyncProcessStatusEnum.Failed, "error");

            Assert.NotNull(response.Result);
            Assert.Equal(SyncProcessStatusEnum.Failed, response.Result!.StatusId);

            _itemRepository.Verify(repo => repo.UpdateStatusAsync(5, SyncProcessStatusEnum.Failed, "error", null), Times.Once);
        }

        [Fact]
        public async Task RegisterItemAsync_ShouldThrowWhenDescriptionMissing()
        {
            var service = CreateService();

            var request = new CreateSyncProcessItemDto
            {
                SyncProcessId = Guid.NewGuid(),
                TypeId = SyncProcessTypeEnum.Stock,
                Description = " ",
                ExternalErpId = "ext"
            };

            await Assert.ThrowsAsync<ArgumentException>(() => service.RegisterItemAsync(request));
        }
    }
}
