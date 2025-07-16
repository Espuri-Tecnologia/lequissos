using System.Collections.Generic;

namespace Lexos.Hub.Sync.Models.Loja
{
    public class IntegracaoErpHubDto
    {
        public List<LojaDto> Lojas { get; set; }

        public string Chave { get; set; }

        public string Retorno { get; set; }
    }
}
