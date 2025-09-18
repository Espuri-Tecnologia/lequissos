using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Clientes
{
    public class TelefoneTerceiroRequest
    {
        public string Ddi { get; set; } = "55"; // default Brasil
        public string Ddd { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string? Ramal { get; set; }
        public string TipoTelefone { get; set; } = string.Empty;
    }
}
