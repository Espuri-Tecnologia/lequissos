using Newtonsoft.Json;

namespace LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido
{
    public class Transporte
    {
        /// <summary>Modalidade do transporte (texto conforme wiki).</summary>
        [JsonProperty("modalidade")]
        public string? Modalidade { get; set; }

        [JsonProperty("transportador")]
        public TerceiroRef? Transportador { get; set; }

        [JsonProperty("codigoANTT")]
        public string? CodigoANTT { get; set; }

        [JsonProperty("placaVeiculo")]
        public string? PlacaVeiculo { get; set; }

        [JsonProperty("estadoVeiculo")]
        public string? EstadoVeiculo { get; set; }

        [JsonProperty("quantidade")]
        public long? Quantidade { get; set; }

        [JsonProperty("especie")]
        public string? Especie { get; set; }

        [JsonProperty("marca")]
        public string? Marca { get; set; }

        [JsonProperty("numero")]
        public string? Numero { get; set; }

        [JsonProperty("pesoBruto")]
        public decimal? PesoBruto { get; set; }

        [JsonProperty("pesoLiquido")]
        public decimal? PesoLiquido { get; set; }
    }
}
