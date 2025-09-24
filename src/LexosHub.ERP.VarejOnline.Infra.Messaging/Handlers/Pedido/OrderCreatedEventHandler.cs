using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Lexos.Hub.Sync.Models.Pedido;
using Lexos.SQS.Interface;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Domain.Mappers;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Clientes;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido;
using LexosHub.ERP.VarejOnline.Infra.Messaging.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LexosHub.ERP.VarejOnline.Infra.Messaging.Handlers.Pedido
{
    public class OrderCreatedEventHandler : PedidoEventHandlerBase, IEventHandler<OrderCreated>
    {
        private readonly ILogger<OrderCreatedEventHandler> _logger;
        private readonly IIntegrationService _integrationService;
        private readonly IVarejOnlineApiService _apiService;

        public OrderCreatedEventHandler(
            ILogger<OrderCreatedEventHandler> logger,
            IIntegrationService integrationService,
            IVarejOnlineApiService apiService,
            ISqsRepository syncOutSqsRepository,
            IOptions<SyncInConfig> syncInSqsConfig)
            : base(syncOutSqsRepository, syncInSqsConfig)
        {
            _logger = logger;
            _integrationService = integrationService;
            _apiService = apiService;
        }

        public async Task HandleAsync(OrderCreated @event, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Pedido criado: {@event.HubKey}");
            if (@event?.Pedido is PedidoView pedidoView)
            {
                var hubKey = @event.HubKey;

                var integration = await _integrationService.GetIntegrationByKeyAsync(hubKey);
                var token = integration.Result!.Token ?? string.Empty;

                long terceiroId = 0;

                if (!string.IsNullOrWhiteSpace(pedidoView.ClienteCpfcnpj))
                {
                    var terceiroRequested = new Infra.ErpApi.Request.Clientes.TerceiroQueryRequest
                    {
                        Documento = pedidoView.ClienteCpfcnpj
                    };
                    var terceiroResponse = await _apiService.GetTerceirosAsync(token, terceiroRequested);
                    if (!terceiroResponse.IsSuccess)
                        return;

                    terceiroId = terceiroResponse.Result!.Any() ? terceiroResponse.Result!.FirstOrDefault()!.Id : 0;

                    if (terceiroId <= 0)
                    {
                        var terceiroRequest = BuildTerceiroRequest(pedidoView);
                        var createResponse = await _apiService.CreateTerceiroAsync(token, terceiroRequest);

                        if (createResponse?.Result == null)
                            return;

                        long.TryParse(createResponse.Result.IdRecurso, out terceiroId);
                    }
                }

                var request = VarejoOnlinePedidoMapper.Map(@event.Pedido);

                if (request != null && terceiroId > 0)
                    request.Terceiro = new TerceiroRef { Id = terceiroId };

                var operacaoResponse = await _apiService.PostPedidoAsync(token, request);

                if (operacaoResponse.IsSuccess)
                {
                    var retorno = BuildPedidoRetornoView(pedidoView, operacaoResponse.Result?.IdRecurso, pedidoIncluido: true);
                    PublishPedidoRetorno(hubKey ?? string.Empty, pedidoView.CanalId, retorno);
                }
            }
        }

        private static TerceiroRequest BuildTerceiroRequest(PedidoView pedidoView)
        {
            var contato = pedidoView.Contatos?.FirstOrDefault();
            var endereco = pedidoView.Enderecos?.Where(x => x.TipoEndereco.Contains("cobranca")).FirstOrDefault();

            return new TerceiroRequest
            {
                Nome = pedidoView.ClienteNome ?? string.Empty,
                Documento = pedidoView.ClienteCpfcnpj ?? string.Empty,
                Enderecos = new List<EnderecoTerceiroRequest>()
                {
                    new EnderecoTerceiroRequest
                    {
                        Tipo = "OUTROS",
                        TipoEndereco = "ENDERECO_COBRANCA",
                        Logradouro = endereco?.Endereco,
                        Cep = endereco?.Cep,
                        Bairro = endereco?.Bairro,
                        Uf = endereco?.Uf,
                        Complemento = endereco?.Complemento,
                        Numero = endereco?.Numero
                    }
                }
            };
        }
    }
}
