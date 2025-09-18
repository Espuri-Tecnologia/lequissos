using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Clientes
{
    public class EnderecoTerceiroRequest
    {
        public string Tipo { get; set; } = string.Empty;
        public string Logradouro { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Bairro { get; set; } = string.Empty;
        public string Uf { get; set; } = string.Empty;
        public string? Complemento { get; set; }
        public string Cep { get; set; } = string.Empty;
        public string CodigoIBGECidade { get; set; } = string.Empty;
        public string TipoEndereco { get; set; } = string.Empty;
    }
}
