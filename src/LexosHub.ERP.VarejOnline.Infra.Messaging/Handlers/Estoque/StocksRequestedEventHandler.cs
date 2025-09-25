using Lexos.Hub.Sync;
using Lexos.Hub.Sync.Enums;
using Lexos.SQS.Interface;
using LexosHub.ERP.VarejOnline.Domain.DTOs.SyncProcess;
using LexosHub.ERP.VarejOnline.Domain.Enums;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Mappers.Estoque;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Request;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers
{
    public class StocksRequestedEventHandler : IEventHandler<StocksRequested>
    {
        private readonly ILogger<StocksRequestedEventHandler> _logger;
        private readonly IIntegrationService _integrationService;
        private readonly IVarejOnlineApiService _apiService;
        private readonly ISyncProcessService _syncProcessService;
        private readonly ISqsRepository _syncOutSqsRepository;
        private readonly int _defaultPageSize;

        public StocksRequestedEventHandler(
            ILogger<StocksRequestedEventHandler> logger,
            IIntegrationService integrationService,
            IVarejOnlineApiService apiService,
            ISyncProcessService syncProcessService,
            ISqsRepository syncOutSqsRepository,
            IOptions<SyncOutConfig> syncOutConfig,
            IConfiguration configuration)
        {
            _logger = logger;
            _integrationService = integrationService;
            _apiService = apiService;
            _syncProcessService = syncProcessService;
            _syncOutSqsRepository = syncOutSqsRepository;

            var syncOut = syncOutConfig?.Value ?? throw new ArgumentNullException(nameof(syncOutConfig));
            _syncOutSqsRepository.IniciarFila($"{syncOut.SQSBaseUrl}{syncOut.SQSAccessKeyId}/{syncOut.SQSName}");

            _defaultPageSize = configuration.GetValue<int>("VarejOnlineApiSettings:DefaultPageSize");
        }

        public async Task HandleAsync(StocksRequested @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stocks requested for hub {HubKey}", @event.HubKey);

            var integrationResponse = await _integrationService.GetIntegrationByKeyAsync(@event.HubKey);
            if (integrationResponse.Result is null)
            {
                _logger.LogWarning("Integration not found for hub {HubKey}", @event.HubKey);
                return;
            }

            var integration = integrationResponse.Result;
            var token = integration.Token ?? string.Empty;

            var start = @event.Inicio ?? 0;
            var pageSize = (@event.Quantidade.HasValue && @event.Quantidade.Value > 0)
                ? @event.Quantidade.Value
                : _defaultPageSize;

            var processResponse = await _syncProcessService.StartProcessAsync(new CreateSyncProcessDto
            {
                IntegrationId = integration.Id,
                TypeId = SyncProcessTypeEnum.Stock,
                ReferenceDate = DateTime.UtcNow,
                PageSize = pageSize,
                Page = start,
                InitialSync = false,
                AdditionalInfo = JsonConvert.SerializeObject(new
                {
                    @event.Inicio,
                    @event.Quantidade,
                    @event.Produtos,
                    @event.Entidades,
                    @event.AlteradoApos,
                    @event.Data,
                    @event.SomenteEcommerce,
                    @event.SomenteMarketplace
                })
            });

            var process = processResponse.Result;

            if (process is null)
            {
                _logger.LogWarning("Unable to create sync process for hub {HubKey}", @event.HubKey);
                return;
            }

            int count;
            var pageIndex = 0;
            var totalProcessed = 0;

            try
            {
                do
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var currentStart = start;
                    var currentPage = pageSize > 0 ? (currentStart / pageSize) + 1 : 1;

                    SyncProcessItemDto? currentItem = null;

                    try
                    {
                        var itemResponse = await _syncProcessService.RegisterItemAsync(new CreateSyncProcessItemDto
                        {
                            SyncProcessId = process.Id,
                            TypeId = SyncProcessTypeEnum.Stock,
                            Description = $"Stocks page {currentPage}",
                            ExternalErpId = $"{integration.Id}-{currentPage}",
                            AdditionalInfo = JsonConvert.SerializeObject(new { Start = currentStart, PageSize = pageSize })
                        });

                        currentItem = itemResponse.Result;

                        var request = new EstoqueRequest
                        {
                            Produtos = @event.Produtos,
                            Entidades = @event.Entidades,
                            Inicio = currentStart,
                            Quantidade = pageSize,
                            AlteradoApos = @event.AlteradoApos,
                            Data = @event.Data,
                            SomenteEcommerce = @event.SomenteEcommerce,
                            SomenteMarketplace = @event.SomenteMarketplace
                        };

                        var response = await _apiService.GetEstoquesAsync(token, request);
                        var estoques = response.Result ?? new List<EstoqueResponse>();
                        count = estoques.Count;
                        totalProcessed += count;

                        if (count > 0)
                        {
                            var mapped = EstoqueViewMapper.Map(estoques);

                            if (mapped.Count > 0)
                            {
                                var notification = new NotificacaoAtualizacaoModel
                                {
                                    Chave = @event.HubKey,
                                    DataHora = DateTime.Now,
                                    Json = JsonConvert.SerializeObject(mapped),
                                    TipoProcesso = TipoProcessoAtualizacao.Estoque,
                                    PlataformaId = 41
                                };

                                _syncOutSqsRepository.AdicionarMensagemFilaFifo(notification, $"notificacao-syncout-{notification.Chave}");
                            }
                        }
                        else
                        {
                            _logger.LogInformation("Nenhum estoque retornado para hub {HubKey} na página iniciando em {Start}", @event.HubKey, currentStart);
                        }

                        if (currentItem is not null)
                        {
                            await _syncProcessService.UpdateItemStatusAsync(
                                currentItem.Id,
                                SyncProcessStatusEnum.Finished,
                                info: JsonConvert.SerializeObject(new { Page = currentPage, Retrieved = count }));
                        }
                    }
                    catch (Exception ex)
                    {
                        if (currentItem is not null)
                        {
                            await _syncProcessService.UpdateItemStatusAsync(
                                currentItem.Id,
                                SyncProcessStatusEnum.Failed,
                                info: ex.Message);
                        }

                        throw;
                    }

                    pageIndex++;
                    await _syncProcessService.UpdateProcessProgressAsync(process.Id, pageIndex, pageSize);
                    start += pageSize;
                }
                while (count >= pageSize && !cancellationToken.IsCancellationRequested);

                await _syncProcessService.UpdateProcessStatusAsync(
                    process.Id,
                    SyncProcessStatusEnum.Finished,
                    info: JsonConvert.SerializeObject(new { Pages = pageIndex, Processed = totalProcessed }));
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Stocks requested handling cancelled for hub {HubKey}", @event.HubKey);
                await _syncProcessService.UpdateProcessStatusAsync(process.Id, SyncProcessStatusEnum.Cancelled, info: "Cancelled");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process stocks for hub {HubKey}", @event.HubKey);
                await _syncProcessService.UpdateProcessStatusAsync(process.Id, SyncProcessStatusEnum.Failed, info: ex.Message);
                throw;
            }
        }
    }
}