using Lexos.Hub.Sync.Models.Pedido;
using LexosHub.ERP.VarejOnline.Domain.DTOs.Pedido;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Domain.Mappers;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido;
using AlterarStatusResponse = LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses.AlterarStatusPedidoResponse;

namespace LexosHub.ERP.VarejOnline.Domain.Services
{
    public class PedidoService : IPedidoService
    {
        private readonly IIntegrationService _integrationService;
        private readonly IVarejOnlineApiService _apiService;

        public PedidoService(IIntegrationService integrationService, IVarejOnlineApiService apiService)
        {
            _integrationService = integrationService;
            _apiService = apiService;
        }

        public async Task<Response<PedidoResponse>> EnviarPedido(string hubKey, PedidoView pedidoView)
        {
            if (pedidoView == null) throw new ArgumentNullException(nameof(pedidoView));

            var integration = await _integrationService.GetIntegrationByKeyAsync(hubKey);
            if (integration.Result == null)
                return new Response<PedidoResponse> { Error = integration.Error ?? new ErrorResult("integrationNotFound") };

            var token = integration.Result.Token ?? string.Empty;
            var request = VarejoOnlinePedidoMapper.Map(pedidoView);

            return await _apiService.PostPedidoAsync(token, request);
        }

        public async Task<Response<AlterarStatusResponse>> AlterarStatusPedido(string hubKey, AlterarStatusPedidoView payload)
        {
            if (payload == null) throw new ArgumentNullException(nameof(payload));

            var integration = await _integrationService.GetIntegrationByKeyAsync(hubKey);
            if (integration.Result == null)
                return new Response<AlterarStatusResponse> { Error = integration.Error ?? new ErrorResult("integrationNotFound") };

            var token = integration.Result.Token ?? string.Empty;

            var request = new AlterarStatusPedidoRequest
            {
                IdPedido = payload.IdPedido,
                Status = payload.Status,
                StatusPedidoVenda = payload.StatusPedidoVenda == null ? null : new StatusPedidoVendaRequest
                {
                    Id = payload.StatusPedidoVenda.Id,
                    Nome = payload.StatusPedidoVenda.Nome
                }
            };

            return await _apiService.AlterarStatusPedidoAsync(token, request);
        }
    }
}
