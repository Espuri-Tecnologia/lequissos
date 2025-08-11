using Newtonsoft.Json;

namespace LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido
{
    public class ValePresente
    {
        [JsonProperty("numeroValePresente")]
        public string? NumeroValePresente { get; set; }

        [JsonProperty("emailValePresente")]
        public string? EmailValePresente { get; set; }

        /// <summary>FISICO, DIGITAL, AMBOS</summary>
        [JsonProperty("formatoValePresente")]
        public string? FormatoValePresente { get; set; }

        [JsonProperty("nomePresenteado")]
        public string? NomePresenteado { get; set; }

        /// <summary>PROPRIO, PRESENTE</summary>
        [JsonProperty("usoValePresente")]
        public string? UsoValePresente { get; set; }
    }
}
