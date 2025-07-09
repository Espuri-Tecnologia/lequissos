using LexosHub.ERP.VarejoOnline.Infra.VarejoOnlineApi.Responses;
using Lexos.Hub.Sync.Models.Produto;

namespace LexosHub.ERP.VarejoOnline.Infra.Messaging.Mappers.Produto
{
    public static class ProdutoViewMapper
    {
        public static ProdutoView? Map(this ProdutoResponse? source)
        {
            return source == null ? null : new ProdutoView
            {
                ProdutoIdGlobal = source.Id,
                Nome = source.Descricao,
                DescricaoResumida = source.DescricaoSimplificada,
                Ean = source.CodigoBarras,
                Sku = source.CodigoSku,
                Peso = source.Peso ?? source.Peso.Value,
                Comprimento = source.Comprimento.HasValue ? source.Comprimento.Value : 0,
                Largura = source.Largura.HasValue ? source.Largura.Value : 0,
                Altura = source.Altura.HasValue ? source.Altura.Value : 0,
                Unidade = source.Unidade,
                Deleted = !source.Ativo,
                Classificacao = source.Classificacao,
                ImagensCadastradas = source.UrlsFotosProduto?.MapImages(),
                Marca = source.Categorias.FirstOrDefault(x => x.Nivel == "MARCA")?.Nome,
                Modelo = source.Categorias.FirstOrDefault(x => x.Nivel == "COLEÇÃO")?.Nome,
                Setor = source.Categorias.FirstOrDefault(x => x.Nivel == "DEPARTAMENTO")?.Nome,
                CategoriaERP = source.Categorias.FirstOrDefault(x => x.Nivel == "TRIBUTAÇÃO")?.Nome
            };
        }

        public static List<ProdutoImagemCadastradaView> MapImages(this List<string> imagens)
        {
            return imagens.Select(u => new ProdutoImagemCadastradaView { Url = u }).ToList();
        }

        public static List<ProdutoView> Map(this List<ProdutoResponse> source)
        {
            return source?.Select(Map)
                .Where(v => v != null)
                .Cast<ProdutoView>()
                .ToList() ?? new List<ProdutoView>();
        }
    }
}
