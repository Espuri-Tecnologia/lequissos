using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lexos.Hub.Sync.Models.Produto
{
    public class ProdutoView
    {
        public ProdutoView()
        {
            Anuncios = new List<AnuncioView>();
            AtributoVariacao = new List<ProdutoAtributoVariacaoView>();
            ReferenciasOutrasPlataformas = new List<ProdutoReferenciaView>();
            Categorias = new List<ProdutoCategoriaView>();
            Estoques = new List<ProdutoEstoqueView>();
            Precos = new List<ProdutoPrecoView>();
            Variacoes = new List<ProdutoVariacaoView>();
            ConverterProdutoConfiguravelComUmaVariacaoEmProdutoSimples = true;
            AtributosFichaTecnica = new List<ProdutoAtributoFichaTecnicaView>();
        }

        public long? ProdutoId { get; set; }
        public long ProdutoIdGlobal { get; set; }
        public string ProdutoTipoId { get; set; }

        private string _sku;
        public string Sku
        {
            get => _sku; set => _sku = value?.Trim();
        }

        public string Ean { get; set; }
        public string Nome { get; set; }
        public string Unidade { get; set; }
        public string LocalArm { get; set; }
        public decimal Peso { get; set; }
        public decimal Comprimento { get; set; }
        public decimal Largura { get; set; }
        public decimal Altura { get; set; }
        public string DescricaoResumida { get; set; }
        public string Descricao { get; set; }
        public string DescricaoMarketplace { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public bool Deleted { get; set; }
        public bool IsNew { get; set; }
        public bool IsUpdated { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string Setor { get; set; }
        public string Classificacao { get; set; }
        public string Classificacao3 { get; set; }
        public string SubClassificacao { get; set; }
        public string CategoriaERP { get; set; }
        public string VideoURL { get; set; }
        /// <summary>
        /// Data atual da ultima versao da entidade, utilizada para controlar o fluxo de atualizacoes, portanto, um item so pode ser alterado caso a nova versao seja superior a atual.
        /// (utilizado para resolver problemas devido as atualizacoes assincronas)
        /// </summary>
        public DateTime DateVersion { get; set; }

        /// <summary>
        /// Utilizado para armazenar o sku original da plataforma, pois em casos onde há importação usando a planilha o "Sku" poderá ser alterado, portanto, precisamos ter o sku original para que possamos fazer o mapeamento correto no processamento do anúncio.
        /// É importante ressaltar para sempre verificar como foi implementando o map do anúncio configurável em uma plataforma, pois esse atributo é essencial para o vinculo correto.
        /// </summary>
        public string SkuOriginalDaPlataforma { get; set; }

        public bool? ConverterProdutoConfiguravelComUmaVariacaoEmProdutoSimples { get; set; }
        public List<ProdutoAtributoFichaTecnicaView> AtributosFichaTecnica { get; set; }

        public ProdutoImpostoView ProdutoImposto { get; set; }
        public List<ProdutoEstoqueView> Estoques { get; set; }
        public List<ProdutoPrecoView> Precos { get; set; }
        public List<ProdutoVariacaoView> Variacoes { get; set; }
        public List<ProdutoComposicaoView> Composicao { get; set; }
        public List<AnuncioView> Anuncios { get; set; }
        public List<ProdutoAtributoVariacaoView> AtributoVariacao { get; set; }
        public List<ProdutoCategoriaView> Categorias { get; set; }
        public ProdutoImagemView Imagens { get; set; } = new ProdutoImagemView();
        public List<ProdutoImagemCadastradaView> ImagensCadastradas { get; set; } =
            new List<ProdutoImagemCadastradaView>();
        public List<string> ProdutoEanComplemento { get; set; } = new List<string>();

        public List<ProdutoReferenciaView> ReferenciasOutrasPlataformas { get; set; }

        public long? ImportJobProdutoId { get; set; }

        public bool PossuiComposicao()
        {
            return ProdutoTipoId == Constantes.Produto.COMPOSTO;
        }

        public void ProcessarLocalArmazenamento()
        {
            if (LocalArm != null)
                Estoques?.ForEach(x => x.LocalArm = LocalArm);

        }

        public bool Validar(out string mensagem)
        {
            var isValido = true;
            var msg = new StringBuilder();
            mensagem = null;

            if (string.IsNullOrEmpty(Sku) || Sku.Length > 50)
            {
                isValido = false;
                msg.Append("O SKU não pode ser nulo e deve ter no máximo 50 caracteres.");
            }
            if (string.IsNullOrEmpty(Nome) || Nome.Length > 500)
            {
                isValido = false;
                msg.Append("O Nome não pode ser nulo e deve ter no máximo 500 caracteres.");
            }
            if (Ean?.Length > 25)
            {
                isValido = false;
                msg.Append("O EAN deve ter no máximo 25 caracteres.");
            }
            if (Unidade?.Length > 6)
            {
                isValido = false;
                msg.Append("O Nome deve ter no máximo 6 caracteres.");
            }
            if (DescricaoResumida?.Length > 4000)
            {
                isValido = false;
                msg.Append("A descrição resumida deve ter no máximo 4000 caracteres.");
            }
            if (MetaTitle?.Length > 100)
            {
                isValido = false;
                msg.Append("O MetaTitle deve ter no máximo 100 caracteres.");
            }
            if (MetaDescription?.Length > 500)
            {
                isValido = false;
                msg.Append("O MetaDescription deve ter no máximo 500 caracteres.");
            }
            if (Peso < 0)
            {
                isValido = false;
                msg.Append("O Peso deve ser maior ou igual a zero.");
            }
            if (Comprimento < 0)
            {
                isValido = false;
                msg.Append("O Comprimento deve ser maior ou igual a zero.");
            }
            if (Altura < 0)
            {
                isValido = false;
                msg.Append("A altura deve ser maior ou igual a zero.");
            }
            if (Largura < 0)
            {
                isValido = false;
                msg.Append("A largura deve ser maior ou igual a zero.");
            }

            if (ProdutoTipoId == Constantes.Produto.CONFIGURAVEL && Variacoes.Any() == false)
            {
                isValido = false;
                msg.Append("Você deve informar pelo menos uma variação para um produto configurável.");
            }

            if (string.IsNullOrEmpty(ProdutoTipoId) ||
                ProdutoTipoId != Constantes.Produto.SIMPLES && ProdutoTipoId != Constantes.Produto.COMPOSTO && ProdutoTipoId != Constantes.Produto.VARIACAO &&
                ProdutoTipoId != Constantes.Produto.CONFIGURAVEL)
            {
                isValido = false;
                msg.Append(
                    $"O ProdutoTipoId deve ser uma das seguintes opções {Constantes.Produto.SIMPLES}, {Constantes.Produto.COMPOSTO}, {Constantes.Produto.VARIACAO} ou {Constantes.Produto.CONFIGURAVEL}.");
            }

            if (isValido)
            {
                mensagem = null;
                return true;
            }

            mensagem = msg.ToString();
            return false;
        }
    }
}