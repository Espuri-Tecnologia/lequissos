using System;

namespace LexosHub.ERP.VarejOnline.Infra.ErpApi.Requests.Produto
{
    public class TabelaPrecoRequest
    {
        /// <summary>
        /// ID da Tabela Preço
        /// </summary>
        public int? Id { get; set; }
        /// <summary>
        /// IDs das entidades (lojas) para filtro. Se vazio, retorna todas.
        /// </summary>
        public List<long>? Entidades { get; set; }

        /// <summary>
        /// Índice inicial da busca para paginação (default: 0).
        /// </summary>
        public int? Inicio { get; set; } = 0;

        /// <summary>
        /// Quantidade de registros por página (default: 50).
        /// </summary>
        public int? Quantidade { get; set; } = 50;

        /// <summary>
        /// Busca apenas tabelas alteradas após essa data/hora.
        /// </summary>
        public DateTimeOffset? AlteradoApos { get; set; }
    }
}
