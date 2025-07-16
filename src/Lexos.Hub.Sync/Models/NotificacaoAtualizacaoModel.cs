using Lexos.Hub.Sync.Enums;
using System;

namespace Lexos.Hub.Sync
{
    public class NotificacaoAtualizacaoModel
    {
        public TipoProcessoAtualizacao TipoProcesso { get; set; }

        public int? TenantId { get; set; } = null;

        public string Json { get; set; }

        public string Chave { get; set; }

        public DateTime DataHora { get; set; }

        public int? Origem { get; set; }

        public short? PlataformaId { get; set; }
        public bool Compactado { get; set; } = false;
    }
}