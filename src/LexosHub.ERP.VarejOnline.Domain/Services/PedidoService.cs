using Lexos.Hub.Sync.Models.Pedido;
using LexosHub.ERP.VarejOnline.Domain.Interfaces.Services;
using LexosHub.ERP.VarejOnline.Domain.Mappers;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Clientes;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses;
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

            long terceiroId = 0;

            if (!string.IsNullOrWhiteSpace(pedidoView.ClienteCpfcnpj))
            {
                var terceiroRequested = new Infra.ErpApi.Request.Clientes.TerceiroQueryRequest
                {
                    Documento = pedidoView.ClienteCpfcnpj
                };
                var terceiroResponse = await _apiService.GetTerceirosAsync(token, terceiroRequested);
                if (!terceiroResponse.IsSuccess)
                    return new Response<PedidoResponse> { Error = terceiroResponse.Error, StatusCode = terceiroResponse.StatusCode };

                terceiroId = terceiroResponse.Result!.Any() ? terceiroResponse.Result!.FirstOrDefault()!.Id : 0;

                if (terceiroId <= 0)
                {
                    var terceiroRequest = BuildTerceiroRequest(pedidoView);
                    var createResponse = await _apiService.CreateTerceiroAsync(token, terceiroRequest);

                    if (createResponse.Result == null)
                        return new Response<PedidoResponse> { Error = createResponse.Error ?? new ErrorResult("terceiroCreationFailed"), StatusCode = createResponse.StatusCode };

                    long.TryParse(createResponse!.Result.IdRecurso, out terceiroId);
                }
            }

            var request = VarejoOnlinePedidoMapper.Map(pedidoView);

            if (request != null && terceiroId > 0)
                request.Terceiro = new TerceiroRef { Id = terceiroId };

            return await _apiService.PostPedidoAsync(token, request);
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
                        Logradouro = endereco.Endereco,
                        Cep = endereco.Cep,
                        Bairro = endereco.Bairro,
                        Uf = endereco.Uf,
                        Complemento = endereco.Complemento,
                        Numero = endereco.Numero                        
                    }
                }
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
