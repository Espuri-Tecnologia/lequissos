using System;

namespace LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Responses
{
    public class PedidoResponse
    {
        public long Id { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public DateTime? Data { get; set; }
    }
}
