using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Lexos.Hub.Sync.Models.Pedido;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido;

namespace LexosHub.ERP.VarejOnline.Domain.Mappers
{
    /// <summary>
    /// Realiza o mapeamento de <see cref="PedidoView"/> para <see cref="PedidoRequest"/>.
    /// </summary>
    public static class VarejoOnlinePedidoMapper
    {
        /// <summary>
        /// Converte os dados do <see cref="PedidoView"/> para um <see cref="PedidoRequest"/>.
        /// </summary>
        /// <param name="source">Instância de origem.</param>
        /// <returns>Objeto <see cref="PedidoRequest"/> com as informações necessárias.</returns>
        public static PedidoRequest? Map(PedidoView? source)
        {
            if (source == null)
            {
                return null;
            }

            var request = new PedidoRequest
            {
                NumeroPedidoCliente = source.Codigo,
                Data = source.Data.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
                Horario = source.Data.ToString("HH:mm:ss", CultureInfo.InvariantCulture),
                Entidade = montaEntidade(source),
                Terceiro = long.TryParse(source.ClienteCpfcnpj, out var clienteId) ? new TerceiroRef { Id = clienteId } : null,
                Representante = new RepresentanteRef { Id = 5 },
                ValorDesconto = source.Desconto,
                ValorFrete = source.Frete,
                ValorOutros = source.Acrescimo,
                Observacao = source.Observacao,
                Itens = MapItens(source.Itens),
                Pagamento = MapPagamento(source.ComposicaoPagamento),
                EnderecoEntrega = MapEndereco(source.Enderecos?.FirstOrDefault(e => string.Equals(e.TipoEndereco, "entrega", StringComparison.OrdinalIgnoreCase)))
            };

            return request;
        }

        private static List<ItemPedido>? MapItens(List<PedidoItemView>? itens)
        {
            if (itens == null)
            {
                return null;
            }

            var result = new List<ItemPedido>();

            foreach (var item in itens)
            {
                if (item == null)
                {
                    continue;
                }

                var mapped = new ItemPedido
                {
                    Produto = new ProdutoRef { CodigoSistema = item.Sku },
                    Quantidade = item.Qtde,
                    ValorUnitario = item.Valor,
                    ValorDesconto = item.Desconto
                };

                result.Add(mapped);
            }

            return result;
        }

        private static Pagamento? MapPagamento(List<PedidoComposicaoPagamentoView>? composicao)
        {
            if (composicao == null || composicao.Count == 0)
            {
                return null;
            }

            var pagamento = new Pagamento();

            foreach (var item in composicao)
            {
                if (item == null)
                {
                    continue;
                }

                switch (item.FormaPagamento?.ToLowerInvariant())
                {
                    case "dinheiro":
                        pagamento.ValorDinheiro = (pagamento.ValorDinheiro ?? 0m) + item.Valor;
                        break;
                    case "cartao":
                    case "cartao_credito":
                    case "cartao_debito":
                        pagamento.Cartoes ??= new List<CartaoPagamento>();
                        pagamento.Cartoes.Add(new CartaoPagamento
                        {
                            Valor = item.Valor,
                            QuantidadeParcelas = item.QuantidadeParcelas
                        });
                        break;
                    case "cheque":
                        pagamento.Cheques ??= new List<ChequePagamento>();
                        pagamento.Cheques.Add(new ChequePagamento
                        {
                            Valor = item.Valor,
                            DataEmissao = item.Data.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
                            DataVencimento = item.Data.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture)
                        });
                        break;
                    case "pix":
                        pagamento.Pixes ??= new List<PixPagamento>();
                        pagamento.Pixes.Add(new PixPagamento
                        {
                            Valor = item.Valor,
                            DataPagamento = item.Data.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture)
                        });
                        break;
                    case "boleto":
                        pagamento.Boletos ??= new List<BoletoPagamento>();
                        pagamento.Boletos.Add(new BoletoPagamento
                        {
                            Valor = item.Valor,
                            DataVencimento = item.Data.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture)
                        });
                        break;
                    case "crediario":
                        pagamento.Crediario ??= new CrediarioPagamento { Parcelas = new List<ParcelaCrediario>() };
                        pagamento.Crediario.Parcelas.Add(new ParcelaCrediario
                        {
                            Numero = pagamento.Crediario.Parcelas.Count + 1,
                            Valor = item.Valor,
                            Vencimento = item.Data.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture)
                        });
                        pagamento.Crediario.Valor = (pagamento.Crediario.Valor ?? 0m) + item.Valor;
                        break;
                    case "voucher":
                        pagamento.Vouchers ??= new List<VoucherPagamento>();
                        pagamento.Vouchers.Add(new VoucherPagamento
                        {
                            Valor = item.Valor,
                            Voucher = new VoucherRef()
                        });
                        break;
                    case "adiantamento":
                        pagamento.Adiantamentos ??= new List<AdiantamentoPagamento>();
                        pagamento.Adiantamentos.Add(new AdiantamentoPagamento { Valor = item.Valor });
                        break;
                    default:
                        pagamento.ValorDinheiro = (pagamento.ValorDinheiro ?? 0m) + item.Valor;
                        break;
                }
            }

            return pagamento;
        }

        private static EnderecoEntrega? MapEndereco(PedidoClienteEnderecoView? endereco)
        {
            if (endereco == null)
            {
                return null;
            }

            return new EnderecoEntrega
            {
                Logradouro = endereco.Endereco,
                Numero = endereco.Numero,
                Bairro = endereco.Bairro,
                Complemento = endereco.Complemento,
                Cep = endereco.Cep,
                Cidade = endereco.Cidade,
                Uf = endereco.Uf
            };
        }

        private static EntidadeRef montaEntidade(PedidoView pedido)
        {
            return new EntidadeRef { Id = pedido.LojaFaturamentoId };
        }

        private static Transporte? MapTransporte(PedidoView source)
        {
            if (string.IsNullOrWhiteSpace(source.TransportadoraNome) && string.IsNullOrWhiteSpace(source.TipoFrete))
            {
                return null;
            }

            return new Transporte
            {
                Modalidade = source.TipoFrete,
                Transportador = string.IsNullOrWhiteSpace(source.TransportadoraNome)
                    ? null
                    : new TerceiroRef { Documento = source.TransportadoraNome }
            };
        }
    }
}

