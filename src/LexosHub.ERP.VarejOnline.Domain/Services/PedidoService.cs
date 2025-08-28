using Lexos.Hub.Sync.Models.Pedido;
using LexosHub.ERP.VarejOnline.Domain.DTOs.Pedido;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Domain.Mappers;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Request;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses;
using System.Linq;
using System.Net;

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

            long? terceiroId = null;

            if (!string.IsNullOrWhiteSpace(pedidoView.ClienteCpfcnpj))
            {
                var terceiroResponse = await _apiService.GetTerceiroByDocumentoAsync(token, pedidoView.ClienteCpfcnpj);
                if (!terceiroResponse.IsSuccess)
                    return new Response<PedidoResponse> { Error = terceiroResponse.Error, StatusCode = terceiroResponse.StatusCode };

                terceiroId = terceiroResponse.Result?.FirstOrDefault()?.Id;

                if (!terceiroId.HasValue)
                {
                    var terceiroRequest = BuildTerceiroRequest(pedidoView);
                    var createResponse = await _apiService.CreateTerceiroAsync(token, terceiroRequest);

                    if (createResponse.Result == null)
                        return new Response<PedidoResponse> { Error = createResponse.Error ?? new ErrorResult("terceiroCreationFailed"), StatusCode = createResponse.StatusCode };

                    terceiroId = createResponse.Result.Id;
                }
            }

            var request = VarejoOnlinePedidoMapper.Map(pedidoView);

            if (request != null && terceiroId.HasValue)
                request.Terceiro = new TerceiroRef { Id = terceiroId };

            return await _apiService.PostPedidoAsync(token, request);
        }

        private static TerceiroRequest BuildTerceiroRequest(PedidoView pedidoView)
        {
            var contato = pedidoView.Contatos?.FirstOrDefault();
            var endereco = pedidoView.Enderecos?.FirstOrDefault();

            return new TerceiroRequest
            {
                Nome = pedidoView.ClienteNome ?? string.Empty,
                Documento = pedidoView.ClienteCpfcnpj ?? string.Empty,
                Contato = contato?.Nome,
                Telefone = contato?.Telefone,
                Email = contato?.Email,
                Endereco = endereco?.Endereco,
                Numero = endereco?.Numero,
                Complemento = endereco?.Complemento,
                Bairro = endereco?.Bairro,
                Cidade = endereco?.Cidade,
                Uf = endereco?.Uf,
                Pais = endereco?.Pais,
                Cep = endereco?.Cep
            };
        }

        public async Task<Response<PedidoResponse>> AlterarStatusPedido(string hubKey, long pedidoNumero, string novoStatus)
        {
            var integration = await _integrationService.GetIntegrationByKeyAsync(hubKey);
            if (integration.Result == null)
                return new Response<PedidoResponse>
                {
                    Error = integration.Error ?? new ErrorResult("integrationNotFound"),
                    StatusCode = HttpStatusCode.BadRequest
                };

            var token = integration.Result.Token ?? string.Empty;
            return await _apiService.AlterarStatusPedidoAsync(token, pedidoNumero, novoStatus);
        }
    }
}
