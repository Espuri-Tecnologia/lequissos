using Lexos.Hub.Sync.Models.Loja;
using Lexos.Log.Pedido.Models;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Default;
using LexosHub.ERP.VarejOnline.Infra.CrossCutting.Settings;
using LexosHub.ERP.VarejOnline.Infra.SyncOut.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RestSharp;

namespace LexosHub.ERP.VarejOnline.Infra.SyncOut.Services;

public class SyncOutApiService : ISyncOutApiService, IDisposable
{
    private readonly SyncOutConfig _syncOutConfig;
    private RestClient _client;

    public SyncOutApiService(IOptions<SyncOutConfig> syncOutConfig)
    {
        _syncOutConfig = syncOutConfig.Value;

        var options = new RestClientOptions(_syncOutConfig.ApiUrl!);
        _client = new RestClient(options);
    }

    public async Task<Response<IntegracaoErpHubDto>> IntegrarLojas(IntegracaoErpHubDto integracaoRequest)
    {
        var request = new RestRequest("Settings/AtualizarInformacoesHubVindasErpExterno", Method.Post)
            .AddHeader("Content-Type", "application/json")
            .AddHeader("Chave", integracaoRequest.Chave)
            .AddBody(JsonConvert.SerializeObject(integracaoRequest));

        return await PostAsync<IntegracaoErpHubDto>(request);
    }

    public async Task<Response<bool>> AtualizarSituacaoImportacaoProdutos(string hubKey, bool importacaoConcluida)
    {
        var request = new RestRequest($"Settings/AtualizarSituacaoImportacaoProdutos?importacaoConcluida={importacaoConcluida}", Method.Put)
            .AddHeader("Content-Type", "application/json")
            .AddHeader("Chave", hubKey);

        return await PostAsync<bool>(request);
    }

    public async Task<Response<string>> RegistrarTimeline(string hubKey, NotificacaoTimelineView notificacaoTimeline)
    {
        var request = new RestRequest("Out/RegistrarTimeline", Method.Post)
            .AddHeader("Content-Type", "application/json")
            .AddHeader("Chave", hubKey)
            .AddBody(JsonConvert.SerializeObject(notificacaoTimeline));

        return await PostAsync<string>(request);
    }

    #region Utils
    private async Task<Response<T>> GetAsync<T>(RestRequest request)
    {
        request.AddHeader("Content-Type", "application/json");

        var response = await _client.ExecuteGetAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            return new Response<T> { Error = new ErrorResult($"{response.ErrorException} - {response.Content}") };
        }

        return new Response<T>(JsonConvert.DeserializeObject<T>(response.Content));
    }

    private async Task<Response<T>> PostAsync<T>(RestRequest request)
    {
        request.AddHeader("Content-Type", "application/json");

        var response = await _client.ExecuteAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            return new Response<T> { Error = new ErrorResult($"{response.ErrorException} - {response.Content}") };
        }

        if (!string.IsNullOrEmpty(response.Content))
        {
            return new Response<T>(JsonConvert.DeserializeObject<T>(response.Content));
        }

        return default;
    }
    #endregion

    public void Dispose()
    {
        _client?.Dispose();
        GC.SuppressFinalize(this);
    }
}