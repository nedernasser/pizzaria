using Pizzaria.Core.VO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Pizzaria.Core.Model
{
    public class PedidoPesquisaModel
    {
        [Display(Name = "Filtro")]
        public string Filtro { get; set; }

        [Display(Name = "Data Inicial")]
        public string DataInicial { get; set; }

        [Display(Name = "Data Final")]
        public string DataFinal { get; set; }
    }

    public class PedidoModel
    {
        public int Id { get; set; }
        public string IdUser { get; set; }
        public int IdCliente { get; set; }
        public string Cliente { get; set; }
        public string Celular { get; set; }
        public EnderecoVO Endereco { get; set; }
        public DateTime DataPedido { get; set; }
        public string Observacao { get; set; }
        public string TaxaEntrega { get; set; }
        public string FormaPagamento { get; set; }
        public List<PedidoItemModel> Itens { get; set; }
        public float ValorTotal { get; set; }
        public string ValorTotalFormatado
        {
            get
            {
                return ValorTotal.ToString("C");
            }
        }
    }

    public class PedidoItemModel
    {
        public int Id { get; set; }
        public int IdPedido { get; set; }
        public int IdTipo { get; set; }
        public string Tipo { get; set; }
        public bool Broto  { get; set; }
        public float Valor { get; set; }
        public int Quantidade { get; set; }
        public string Observacao { get; set; }
        public List<DetalhePedidoVO> ListaDetalhes { get; set; }
        public string Detalhes
        {
            get
            {
                var ehBebida = false;
                var retorno = new StringBuilder();
                foreach (var item in ListaDetalhes)
                {
                    ehBebida = item.IdSubtipo == 12;
                    if (!ehBebida)
                    {
                        if (item.IdProduto == 53)
                        {
                            retorno.Append(
                                string.Format("{0} {1}", 
                                item.IdSubtipo == 1 ? string.Empty : item.Subtipo, 
                                item.IdProduto != 53 ? item.Produto : item.Observacao));
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(retorno.ToString()))
                            {
                                retorno.Append(
                                    string.Format("{0} de {1}{2}", 
                                    item.Subtipo, 
                                    item.Produto, 
                                    string.IsNullOrEmpty(item.Observacao) ? string.Empty : string.Format(" - {0}", item.Observacao)));
                            }
                            else
                            {
                                retorno.AppendFormat(
                                    string.Format(", {0} de {1}{2}", 
                                    item.Subtipo, 
                                    item.Produto, 
                                    string.IsNullOrEmpty(item.Observacao) ? string.Empty : string.Format(" - {0}", item.Observacao)));
                            }
                        }
                    }
                    else
                    {
                        retorno.AppendFormat(string.Format("{0}: {1}", item.Produto, item.DescricaoProduto));
                    }
                }
                return string.Format(
                    "{0}{1}{2}",
                    ehBebida ? Quantidade.ToString() : string.Format("{0} PIZZA{1}{2}: ", 
                    Quantidade.ToString(), Quantidade > 1 ? "S" : string.Empty, Broto ? " BROTO" : string.Empty),
                    retorno.ToString(), string.IsNullOrEmpty(Observacao) ? string.Empty : string.Format(" - {0}", Observacao));
            }
        }
        public List<ItemAdicionalVO> ListaItensAdicionais { get; set; }
        public string ItensAdicionais
        {
            get
            {
                var retorno = new StringBuilder();
                if (ListaItensAdicionais != null && ListaItensAdicionais.Count > 0)
                {
                    foreach (var item in ListaItensAdicionais)
                    {
                        if (string.IsNullOrEmpty(retorno.ToString()))
                        { retorno.Append(item.Nome); }
                        else
                        { retorno.AppendFormat(" | {0}", item.Nome); }
                    }
                }
                return retorno.ToString();
            }
        }
    }
}
