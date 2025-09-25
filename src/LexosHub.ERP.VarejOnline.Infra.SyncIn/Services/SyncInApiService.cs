using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejOnline.Infra.SyncIn.Interfaces;
using LexosHub.ERP.VarejOnline.Infra.SyncIn.Requests;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;

namespace LexosHub.ERP.VarejOnline.Infra.SyncIn.Services;

public class SyncInApiService : ISyncInApiService, IDisposable
{
    private readonly RestClient _client;
    private readonly SyncInConfig _syncInConfig;

    public SyncInApiService(IOptions<SyncInConfig> syncInConfig)
    {
        _client = new RestClient();
        _syncInConfig = syncInConfig.Value;
    }

    public async Task<Response<NotaFiscalExternaDto>> InserirNotaFiscalExterna(NotaFiscalExternaDto nfeExternalRequest)
    {
        var request = new RestRequest($"{_syncInConfig.ApiUrl}NotasFiscais/InserirNotaFiscalEmitidaExternamente", Method.Post)
            .AddHeader("Chave", nfeExternalRequest.Chave)
            .AddJsonBody(new { PedidoHubId = nfeExternalRequest.PedidoHubId, XmlNota = nfeExternalRequest.XmlNota });

        return await ExecuteAsync<NotaFiscalExternaDto>(request);
    }

    #region Utils
    private async Task<Response<T>> ExecuteAsync<T>(RestRequest request)
    {
        try
        {
            request.AddHeader("Content-Type", "application/json");

            var response = await _client!.ExecuteAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                return new Response<T> { Error = new ErrorResult($"{response.ErrorException} - {response.Content}") };
            }

            return new Response<T>(JsonConvert.DeserializeObject<T>(response.Content!)!);
        }
        catch (Exception ex)
        {
            return new Response<T> { Error = new ErrorResult($"{ex.Message}") };
        }
    }
    #endregion

    public void Dispose()
    {
        _client?.Dispose();
        GC.SuppressFinalize(this);
    }

    public async Task<Response<SyncInNotaFiscalDto>> ObterNotaFiscalPorChave(string chaveNotaFiscal, string chave)
    {
        var request = new RestRequest($"{_syncInConfig.ApiUrl}NotasFiscais/ObterPorId", Method.Get)
            .AddHeader("Chave", chave)
            .AddQueryParameter("chaveNfe", chaveNotaFiscal);

        return await ExecuteAsync<SyncInNotaFiscalDto>(request);
    }
}