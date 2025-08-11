using System;
using System.Collections.Generic;
using System.Linq;
using Lexos.Hub.Sync.Models.Pedido;
using LexosHub.ERP.VarejOnline.Domain.DTOs.Pedido;
using LexosHub.ERP.VarejOnline.Infra.ErpApi.Request.Pedido;
using LexosHub.ERP.VarejOnline.Infra.VarejOnlineApi.Request;

namespace LexosHub.ERP.VarejOnline.Domain.Mappers
{
    /// <summary>
    /// Realiza o mapeamento de PedidoView para PedidoRequest.
    /// </summary>
    public static class VarejoOnlinePedidoMapper
    {
        /// <summary>
        /// Converte os dados do PedidoView para um PedidoRequest.
        /// </summary>
        /// <param name="source">Instancia de origem.</param>
        /// <returns>Objeto PedidoRequest com as informacoes necessarias.</returns>
        public static PedidoRequest? Map(PedidoView? source)
        {
            if (source == null)
            {
                return null;
            }

            var enderecoEntrega = source.Enderecos?
                .FirstOrDefault(e => e?.TipoEndereco?.Equals("entrega", StringComparison.OrdinalIgnoreCase) == true);

            EnderecoEntrega? endereco = null;
            if (enderecoEntrega != null)
            {
                endereco = new EnderecoEntrega
                {
                    Logradouro = enderecoEntrega.Endereco,
                    Numero = enderecoEntrega.Numero,
                    Bairro = enderecoEntrega.Bairro,
                    Complemento = enderecoEntrega.Complemento,
                    Cep = enderecoEntrega.Cep,
                    Cidade = enderecoEntrega.Cidade,
                    Uf = enderecoEntrega.Uf
                };
            }

            Pagamento? pagamento = null;
            if (source.ComposicaoPagamento?.Any() == true)
            {
                pagamento = new Pagamento();

                foreach (var comp in source.ComposicaoPagamento)
                {
                    if (comp == null)
                    {
                        continue;
                    }

                    var forma = comp.FormaPagamento?.ToUpperInvariant();
                    if (forma == "DINHEIRO")
                    {
                        pagamento.ValorDinheiro = (pagamento.ValorDinheiro ?? 0) + comp.Valor;
                    }
                    else if (forma == "PIX")
                    {
                        pagamento.Pixes ??= new List<PixPagamento>();
                        pagamento.Pixes.Add(new PixPagamento
                        {
                            Valor = comp.Valor,
                            DataPagamento = comp.Data.ToString("dd-MM-yyyy")
                        });
                    }
                    else if (forma == "BOLETO")
                    {
                        pagamento.Boletos ??= new List<BoletoPagamento>();
                        pagamento.Boletos.Add(new BoletoPagamento
                        {
                            Valor = comp.Valor,
                            DataVencimento = comp.Data.ToString("dd-MM-yyyy")
                        });
                    }
                    else if (forma != null && forma.Contains("CARTAO"))
                    {
                        pagamento.Cartoes ??= new List<CartaoPagamento>();
                        pagamento.Cartoes.Add(new CartaoPagamento
                        {
                            Valor = comp.Valor,
                            QuantidadeParcelas = comp.QuantidadeParcelas
                        });
                    }
                    else
                    {
                        pagamento.ValorDinheiro = (pagamento.ValorDinheiro ?? 0) + comp.Valor;
                    }
                }
            }

            return new PedidoRequest
            {
                NumeroPedidoCliente = source.Codigo,
                Data = source.Data.ToString("dd-MM-yyyy"),
                Horario = source.Data.ToString("HH:mm:ss"),
                Entidade = source.CodigoLojaErpExterno.HasValue
                    ? new EntidadeRef { Id = source.CodigoLojaErpExterno }
                    : null,
                Terceiro = new TerceiroRef { Documento = source.ClienteCpfcnpj },
                ValorDesconto = source.Desconto,
                Observacao = source.Observacao,
                ValorFrete = source.Frete,
                ValorOutros = source.Acrescimo,
                VendaConsumidorFinal = string.IsNullOrWhiteSpace(source.ClienteCpfcnpj)
                    ? null
                    : (bool?)(source.ClienteCpfcnpj.Length <= 11),
                Itens = source.Itens?.Select(i => new ItemPedido
                {
                    Produto = new ProdutoRef
                    {
                        Id = i.ProdutoId ?? i.ProdutoIdGlobal,
                        CodigoInterno = i.Sku
                    },
                    Quantidade = i.Qtde,
                    ValorDesconto = i.Desconto,
                    ValorUnitario = i.Valor
                }).ToList(),
                Origem = source.Canal,
                Transporte = string.IsNullOrWhiteSpace(source.TipoFrete)
                    ? null
                    : new Transporte
                    {
                        Modalidade = source.TipoFrete
                    },
                EnderecoEntrega = endereco,
                Pagamento = pagamento
            };
        }
    }
}

