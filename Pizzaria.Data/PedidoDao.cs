using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pizzaria.Core.Model;
using Pizzaria.Core.VO;
using PassOn;
using Pizzaria.Core.Util;
using System.Diagnostics;
using Pizzaria.Components.Web.DataTablesBinder.Interfaces;

namespace Pizzaria.Data
{
    public class PedidoDao : BaseDao
    {
        Logger<PedidoDao> log;

        public PedidoDao()
        {
            log = new Logger<PedidoDao>();
            log.init();
        }

        public IList<PedidoModel> ListarPedidos(IDataTablesFilterRequest<PedidoPesquisaModel> tableQuery, out int total, out string totais)
        {
            totais = string.Empty;
            var sql = string.Empty;
            float totalCalculado = 0;
            float totalDinheiro = 0;
            float totalDebito = 0;
            float totalCredito = 0;
            float totalValeRefeicao = 0;
            try
            {
                var filtros = tableQuery.FilterModel;
                var retorno = new List<PedidoModel>();

                sql = @"SELECT P.ID,
                               P.FUNCIONARIO AS IDFUNCIONARIO,
	                           C.NOME AS CLIENTE,
	                           C.ID AS IDCLIENTE,
	                           P.VALORTOTAL,
                               P.FORMAPAGAMENTO,
                               P.DATAPEDIDO,
                               P.OBSERVACAO,
                               E.ID AS IDENDERECO,
                               E.LOGRADOURO,
                               E.NUMERO,
                               E.COMPLEMENTO,
                               E.BAIRRO,
                               E.CIDADE,
                               E.ESTADO,
                               E.CEP
                       FROM PEDIDO P
                       JOIN CLIENTE C ON P.CLIENTE = C.ID
                       JOIN ENDERECO E ON P.ENDERECO = E.ID
                       WHERE {0}";

                var where = new StringBuilder();
                if (!string.IsNullOrEmpty(filtros.Filtro))
                {
                    where.AppendFormat("(C.NOME LIKE '%{0}%' ", filtros.Filtro);
                    where.AppendFormat("OR P.OBSERVACAO LIKE '%{0}%' ", filtros.Filtro);
                    where.AppendFormat("OR E.LOGRADOURO LIKE '%{0}%' ", filtros.Filtro);
                    where.AppendFormat("OR E.BAIRRO LIKE '%{0}%' ", filtros.Filtro);
                    where.AppendFormat("OR F.NOME LIKE '%{0}%') ", filtros.Filtro);
                }
                if (!string.IsNullOrEmpty(filtros.DataInicial) && !string.IsNullOrEmpty(filtros.DataFinal))
                {
                    if (!string.IsNullOrEmpty(where.ToString()))
                    {
                        where.Append(" AND ");
                    }

                    var dtInicial = Convert.ToDateTime(filtros.DataInicial);
                    var dtFinal = Convert.ToDateTime(filtros.DataFinal);

                    where.AppendFormat("DATAPEDIDO BETWEEN '{0}' AND '{1}'", dtInicial.ToString("yyyy-MM-dd"), dtFinal.ToString("yyyy-MM-dd"));
                }
                if (string.IsNullOrEmpty(where.ToString()))
                {
                    where.Append("1=1");
                }
                sql = string.Format(sql, where.ToString());

                using (var reader = ExecuteSQLCommandDataReader(sql))
                {
                    while (reader.Read())
                    {
                        var vo = new PedidoModel
                        {
                            Id = getInt32("ID", reader),
                            IdUser = getString("IDFUNCIONARIO", reader),
                            IdCliente = getInt32("IDCLIENTE", reader),
                            Cliente = getString("CLIENTE", reader),
                            ValorTotal = getFloat("VALORTOTAL", reader),
                            DataPedido = getDate("DATAPEDIDO", reader),
                            Observacao = getString("OBSERVACAO", reader),
                            FormaPagamento = getString("FORMAPAGAMENTO", reader),
                            Endereco = new EnderecoVO
                            {
                                Id = getInt32("IDENDERECO", reader),
                                Logradouro = getString("LOGRADOURO", reader),
                                Numero = getInt32("NUMERO", reader),
                                Complemento = getString("COMPLEMENTO", reader),
                                Bairro = getString("BAIRRO", reader),
                                Cidade = getString("CIDADE", reader),
                                Estado = getString("ESTADO", reader),
                                CEP = getString("CEP", reader)
                            }
                        };
                        retorno.Add(vo);
                        totalCalculado += vo.ValorTotal;

                        if (vo.FormaPagamento.Contains("DÉBITO"))
                        {
                            totalDebito += vo.ValorTotal;
                        }
                        else if (vo.FormaPagamento.Contains("CRÉDITO"))
                        {
                            totalCredito += vo.ValorTotal;
                        }
                        else if (vo.FormaPagamento.Contains("DINHEIRO"))
                        {
                            totalDinheiro += vo.ValorTotal;
                        }
                        else if (vo.FormaPagamento.Contains("VALE"))
                        {
                            totalValeRefeicao += vo.ValorTotal;
                        }
                    }
                    reader.Close();
                }

                var totaisProdutos = ObterTotaisProdutos(retorno);

                total = retorno.Count();
                var ret = retorno.AsQueryable();
                ret = AplicarPaginacaoOrdenacaoDataTable<PedidoModel, PedidoPesquisaModel>(ret, tableQuery);
                retorno = ret.ToList();

                totais += totaisProdutos.Item1.ToString();
                totais += string.Format("|{0}", totaisProdutos.Item2.ToString());
                totais += string.Format("|{0}", totalDinheiro.ToString("C"));
                totais += string.Format("|{0}", totalDebito.ToString("C"));
                totais += string.Format("|{0}", totalCredito.ToString("C"));
                totais += string.Format("|{0}", totalValeRefeicao.ToString("C"));
                totais += string.Format("|{0}", totalCalculado.ToString("C"));

                return retorno;
            }
            catch (Exception ex)
            {
                log.Error(ex, "Query: {0}", sql);
                throw ex;
            }
        }

        public void GravarResumo(ResumoPedidoVO vo)
        {
            if (string.IsNullOrEmpty(vo.DataInicial))
            {
                vo.DataInicial = "2017-07-01";
                vo.DataFinal = DateTime.Now.ToString("yyyy-MM-dd");
            }
            else
            {
                var dtInicial = Convert.ToDateTime(vo.DataInicial);
                vo.DataInicial = dtInicial.ToString("yyyy-MM-dd");
                var dtFinal = Convert.ToDateTime(vo.DataFinal);
                vo.DataFinal = dtFinal.ToString("yyyy-MM-dd");
            }

            var sql = string.Format(
                @"INSERT INTO RESUMO 
                (DATAINICIAL,DATAFINAL,TOTALPIZZA,TOTALBEBIDA,TOTALDINHEIRO,TOTALDEBITO,TOTALCREDITO,TOTALVALEREFEICAO,TOTALGERAL)
                VALUES
                ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}')",
                vo.DataInicial, vo.DataFinal, vo.TotalPizza, vo.TotalBebida, vo.TotalDinheiro, vo.TotalDebito, vo.TotalCredito, vo.TotalValeRefeicao, vo.TotalGeral);

            try
            {
                ExecuteSQLCommandNonQuery(sql);
                MySqlConnection.Close();
                MySqlConnection.Dispose();
            }
            catch (Exception ex)
            {
                Logar(string.Format("GravarResumo => {0}", string.Format("Exception: {0} ==> {1} ==> {2}", ex.Message, (ex.InnerException == null ? "NULL" : ex.InnerException.ToString()), ex.StackTrace)), EventLogEntryType.Error);
                MySqlConnection.Close();
                MySqlConnection.Dispose();
            }
        }

        private Tuple<int, int> ObterTotaisProdutos(List<PedidoModel> pedidos)
        {
            var totalPizza = 0;
            var totalBebida = 0;

            foreach (var pedido in pedidos)
            {
                var itens = ListarItemPedido(pedido.Id);
                totalPizza += itens.Where(i => i.IdTipo != 10).Count();
                totalBebida += itens.Where(i => i.IdTipo == 10).Count();
            }

            return new Tuple<int, int>(totalPizza, totalBebida);
        }

        public List<PedidoModel> ListarCuponsPendentes()
        {
            var sql = string.Empty;
            try
            {
                var retorno = new List<PedidoModel>();

                sql = @"SELECT P.ID,
                               P.FUNCIONARIO AS IDFUNCIONARIO,
	                           C.NOME AS CLIENTE,
	                           CELULAR,
	                           TELEFONE,
	                           C.ID AS IDCLIENTE,
	                           P.VALORTOTAL,
                               P.DATAPEDIDO,
                               P.OBSERVACAO,
                               P.FORMAPAGAMENTO,
                               E.ID AS IDENDERECO,
                               E.LOGRADOURO,
                               E.NUMERO,
                               E.COMPLEMENTO,
                               E.BAIRRO,
                               E.CIDADE,
                               E.ESTADO,
                               E.CEP
                       FROM PEDIDO P
                       JOIN CLIENTE C ON P.CLIENTE = C.ID
                       JOIN ENDERECO E ON P.ENDERECO = E.ID
                       WHERE P.IMPRESSO = 0 ";

                using (var reader = ExecuteSQLCommandDataReader(sql))
                {
                    while (reader.Read())
                    {
                        retorno.Add(new PedidoModel
                        {
                            Id = getInt32("ID", reader),
                            IdUser = getString("IDFUNCIONARIO", reader),
                            IdCliente = getInt32("IDCLIENTE", reader),
                            Cliente = getString("CLIENTE", reader),
                            Celular = string.Format("{0} {1}", getString("CELULAR", reader), getString("TELEFONE", reader)),
                            ValorTotal = getFloat("VALORTOTAL", reader),
                            DataPedido = getDate("DATAPEDIDO", reader),
                            Observacao = getString("OBSERVACAO", reader),
                            FormaPagamento = getString("FORMAPAGAMENTO", reader),
                            Endereco = new EnderecoVO
                            {
                                Id = getInt32("IDENDERECO", reader),
                                Logradouro = getString("LOGRADOURO", reader),
                                Numero = getInt32("NUMERO", reader),
                                Complemento = getString("COMPLEMENTO", reader),
                                Bairro = getString("BAIRRO", reader),
                                Cidade = getString("CIDADE", reader),
                                Estado = getString("ESTADO", reader),
                                CEP = getString("CEP", reader)
                            },

                        });
                    }
                    reader.Close();
                }

                foreach (var pedido in retorno)
                {
                    pedido.TaxaEntrega = ObterTaxaEntrega(pedido.Endereco.Bairro.ToUpper()).ToString("C");

                    var itens = ListarItemPedido(pedido.Id);
                    pedido.Itens = Pass.ACollectionOf<ItemPedidoVO>.ToAListOf<PedidoItemModel>(itens);
                }

                return retorno.ToList();
            }
            catch (Exception ex)
            {
                log.Error(ex, "Query: {0}", sql);
                throw ex;
            }
        }

        public List<ResumoPedidoVO> ListarResumosPendentes()
        {
            var sql = string.Empty;
            try
            {
                var retorno = new List<ResumoPedidoVO>();

                sql = @"SELECT *
                       FROM RESUMO
                       WHERE IMPRESSO = 0 ";

                using (var reader = ExecuteSQLCommandDataReader(sql))
                {
                    while (reader.Read())
                    {
                        retorno.Add(new ResumoPedidoVO
                        {
                            Id = getInt32("ID", reader),
                            DataInicial = getDate("DATAINICIAL", reader).ToString("dd/MM/yyyy"),
                            DataFinal = getDate("DATAFINAL", reader).ToString("dd/MM/yyyy"),
                            TotalPizza = getString("TOTALPIZZA", reader),
                            TotalBebida = getString("TOTALBEBIDA", reader),
                            TotalDinheiro = getString("TOTALDINHEIRO", reader),
                            TotalDebito = getString("TOTALDEBITO", reader),
                            TotalCredito = getString("TOTALCREDITO", reader),
                            TotalValeRefeicao = getString("TOTALVALEREFEICAO", reader),
                            TotalGeral = getString("TOTALGERAL", reader)
                        });
                    }
                    reader.Close();
                }

                return retorno.ToList();
            }
            catch (Exception ex)
            {
                log.Error(ex, "Query: {0}", sql);
                throw ex;
            }
        }

        public bool AtualizarImpressaoCupom(int idPedido)
        {
            try
            {
                var sql = string.Format("UPDATE PEDIDO SET IMPRESSO = 1 WHERE ID = {0}", idPedido);
                ExecuteSQLCommandNonQuery(sql);
                MySqlConnection.Close();
                MySqlConnection.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                Logar(string.Format("AtualizarImpressaoCupom => {0}", string.Format("Exception: {0} ==> {1} ==> {2}", ex.Message, (ex.InnerException == null ? "NULL" : ex.InnerException.ToString()), ex.StackTrace)), EventLogEntryType.Error);
                MySqlConnection.Close();
                MySqlConnection.Dispose();
                return false;
            }
        }

        public bool AtualizarImpressaoResumo(int idResumo)
        {
            try
            {
                var sql = string.Format("UPDATE RESUMO SET IMPRESSO = 1 WHERE ID = {0}", idResumo);
                ExecuteSQLCommandNonQuery(sql);
                MySqlConnection.Close();
                MySqlConnection.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                Logar(string.Format("AtualizarImpressaoResumo => {0}", string.Format("Exception: {0} ==> {1} ==> {2}", ex.Message, (ex.InnerException == null ? "NULL" : ex.InnerException.ToString()), ex.StackTrace)), EventLogEntryType.Error);
                MySqlConnection.Close();
                MySqlConnection.Dispose();
                return false;
            }
        }

        public PedidoModel Obter(int id)
        {
            PedidoModel pedido = null;

            var sql = string.Format(@"SELECT P.ID,
                                             P.FUNCIONARIO AS IDFUNCIONARIO,
	                                         C.NOME AS CLIENTE,
	                                         C.ID AS IDCLIENTE,
	                                         P.VALORTOTAL,
                                             P.DATAPEDIDO,
                                             P.OBSERVACAO,
                                             E.ID AS IDENDERECO,
                                             E.LOGRADOURO,
                                             E.NUMERO,
                                             E.COMPLEMENTO,
                                             E.BAIRRO,
                                             E.CIDADE,
                                             E.ESTADO,
                                             E.CEP
                                        FROM PEDIDO P
                                        JOIN CLIENTE C ON P.CLIENTE = C.ID
                                        JOIN ENDERECO E ON P.ENDERECO = E.ID
                                        WHERE P.ID = {0}", id);

            using (var reader = ExecuteSQLCommandDataReader(sql))
            {
                if (reader.Read())
                {
                    pedido = new PedidoModel
                    {
                        Id = getInt32("ID", reader),
                        IdUser = getString("IDFUNCIONARIO", reader),
                        IdCliente = getInt32("IDCLIENTE", reader),
                        Cliente = getString("CLIENTE", reader),
                        ValorTotal = getFloat("VALORTOTAL", reader),
                        DataPedido = getDate("DATAPEDIDO", reader),
                        Observacao = getString("OBSERVACAO", reader),
                        Endereco = new EnderecoVO
                        {
                            Id = getInt32("IDENDERECO", reader),
                            Logradouro = getString("LOGRADOURO", reader),
                            Numero = getInt32("NUMERO", reader),
                            Complemento = getString("COMPLEMENTO", reader),
                            Bairro = getString("BAIRRO", reader),
                            Cidade = getString("CIDADE", reader),
                            Estado = getString("ESTADO", reader),
                            CEP = getString("CEP", reader)
                        },

                    };
                }
                reader.Close();
            }

            if (pedido != null)
            {
                pedido.TaxaEntrega = ObterTaxaEntrega(pedido.Endereco.Bairro.ToUpper()).ToString("C");

                var itens = ListarItemPedido(pedido.Id);
                pedido.Itens = Pass.ACollectionOf<ItemPedidoVO>.ToAListOf<PedidoItemModel>(itens);
            }

            return pedido;
        }

        public PedidoModel ObterUltimoPedido(int idCliente)
        {
            PedidoModel pedido = null;

            var sql = string.Format(@"SELECT P.ID,
                                             P.FUNCIONARIO AS IDFUNCIONARIO,
	                                         C.NOME AS CLIENTE,
	                                         C.ID AS IDCLIENTE,
	                                         P.VALORTOTAL,
                                             P.DATAPEDIDO,
                                             P.OBSERVACAO,
                                             E.ID AS IDENDERECO,
                                             E.LOGRADOURO,
                                             E.NUMERO,
                                             E.COMPLEMENTO,
                                             E.BAIRRO,
                                             E.CIDADE,
                                             E.ESTADO,
                                             E.CEP
                                        FROM PEDIDO P
                                        JOIN CLIENTE C ON P.CLIENTE = C.ID AND C.ULTIMOPEDIDO = P.ID
                                        JOIN ENDERECO E ON P.ENDERECO = E.ID
                                        WHERE C.ID = {0}", idCliente);

            using (var reader = ExecuteSQLCommandDataReader(sql))
            {
                if (reader.Read())
                {
                    pedido = new PedidoModel
                    {
                        Id = getInt32("ID", reader),
                        IdUser = getString("IDFUNCIONARIO", reader),
                        IdCliente = getInt32("IDCLIENTE", reader),
                        Cliente = getString("CLIENTE", reader),
                        ValorTotal = getFloat("VALORTOTAL", reader),
                        DataPedido = getDate("DATAPEDIDO", reader),
                        Observacao = getString("OBSERVACAO", reader),
                        Endereco = new EnderecoVO
                        {
                            Id = getInt32("IDENDERECO", reader),
                            Logradouro = getString("LOGRADOURO", reader),
                            Numero = getInt32("NUMERO", reader),
                            Complemento = getString("COMPLEMENTO", reader),
                            Bairro = getString("BAIRRO", reader),
                            Cidade = getString("CIDADE", reader),
                            Estado = getString("ESTADO", reader),
                            CEP = getString("CEP", reader)
                        },
                        
                    };
                }
                reader.Close();
            }

            if (pedido != null)
            {
                pedido.TaxaEntrega = ObterTaxaEntrega(pedido.Endereco.Bairro.ToUpper()).ToString("C");

                var itens = ListarItemPedido(pedido.Id);
                pedido.Itens = Pass.ACollectionOf<ItemPedidoVO>.ToAListOf<PedidoItemModel>(itens);
            }

            return pedido;
        }

        public List<ItemPedidoVO> ListarItemPedido(int idPedido)
        {
            var detalhes = new List<ItemPedidoVO>();

            var sql = string.Format(@"SELECT I.ID,
	                                         T.ID AS IDTIPO,
	                                         T.NOME AS TIPO,
	                                         I.TAMANHO,
	                                         I.QTD AS QUANTIDADE,
	                                         I.VALOR
                                        FROM PEDIDO_ITEM I
                                        JOIN TIPO T ON I.TIPO = T.ID
                                        WHERE I.PEDIDO = {0}
                                     ORDER BY I.TIPO", idPedido);

            using (var reader = ExecuteSQLCommandDataReader(sql))
            {
                while (reader.Read())
                {
                    var item = new ItemPedidoVO
                    {
                        Id = getInt32("ID", reader),
                        IdTipo = getInt32("IDTIPO", reader),
                        Tipo = getString("TIPO", reader),
                        Broto = getString("TAMANHO", reader) == "BROTO",
                        Quantidade = getInt32("QUANTIDADE", reader),
                        Valor = getFloat("VALOR", reader)
                    };
                    detalhes.Add(item);
                }
                reader.Close();
            }

            if (detalhes.Count > 0)
            {
                detalhes.ForEach(d => d.ListaDetalhes = ListarDetalheItemPedido(d.Id));
                detalhes.ForEach(d => d.ListaItensAdicionais = ListarItemAdicional(d.Id));
            }

            return detalhes;
        }

        public List<DetalhePedidoVO> ListarDetalheItemPedido(int idPedidoItem)
        {
            var detalhes = new List<DetalhePedidoVO>();

            var sql = string.Format(@"SELECT S.ID AS IDSUBTIPO,
		                                     S.NOME AS SUBTIPO,
		                                     P.ID AS IDPRODUTO,
		                                     P.NOME AS PRODUTO,
                                             P.DESCRICAO,
                                             D.OBSERVACAO
	                                    FROM PEDIDO_ITEM_DETALHE D
	                                    JOIN PRODUTO P ON D.PRODUTO = P.ID
	                                    JOIN SUBTIPO S ON D.SUBTIPO = S.ID
                                       WHERE D.PEDIDOITEM = {0}", idPedidoItem);

            using (var reader = ExecuteSQLCommandDataReader(sql))
            {
                while (reader.Read())
                {
                    var detalhe = new DetalhePedidoVO
                    {
                        IdProduto = getInt32("IDPRODUTO", reader),
                        Produto = getString("PRODUTO", reader),
                        IdSubtipo = getInt32("IDSUBTIPO", reader),
                        Subtipo = getString("SUBTIPO", reader),
                        DescricaoProduto = getString("DESCRICAO", reader),
                        Observacao = getString("OBSERVACAO", reader)
                    };
                    detalhes.Add(detalhe);
                }
                reader.Close();
            }

            return detalhes;
        }

        public bool RepetirItensPedido(int idPedido, int idPedidoNovo)
        {
            long idItemNovo = 0;
            var sql = string.Empty;
            var itensPedidoAntigo = ListarItemPedido(idPedido);

            try
            {
                foreach (var item in itensPedidoAntigo)
                {
                    item.Valor = item.Valor / item.Quantidade;
                    sql = string.Format(
                        @"INSERT INTO PEDIDO_ITEM (PEDIDO, TIPO, QTD, VALOR) VALUES 
                        ({0}, {1}, {2}, {3})",
                        idPedidoNovo, item.IdTipo, item.Quantidade, item.Valor);
                    idItemNovo = ExecuteSQLCommandNonQuery(sql);

                    foreach (var itemDetalhe in item.ListaDetalhes)
                    {
                        sql = string.Format(
                            @"INSERT INTO PEDIDO_ITEM_DETALHE (PEDIDOITEM, PRODUTO, SUBTIPO, OBSERVACAO) VALUES 
                            ({0}, {1}, {2}, '{3}')",
                            idItemNovo, itemDetalhe.IdProduto, itemDetalhe.IdSubtipo, string.IsNullOrEmpty(itemDetalhe.Observacao) ? string.Empty : itemDetalhe.Observacao);
                        ExecuteSQLCommandNonQuery(sql);
                    }

                    foreach (var itemAdicional in item.ListaItensAdicionais)
                    {
                        sql = string.Format(
                            @"INSERT INTO PEDIDO_ITEM_ADICIONAL (PEDIDOITEM, ITEM_ADICIONAL) VALUES 
                            ({0}, {1})",
                            idItemNovo, itemAdicional.Id);
                        ExecuteSQLCommandNonQuery(sql);
                    }

                }

                MySqlConnection.Close();
                MySqlConnection.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                Logar(string.Format("RepetirUltimoPedido => {0}", string.Format("Exception: {0} ==> {1} ==> {2}", ex.Message, (ex.InnerException == null ? "NULL" : ex.InnerException.ToString()), ex.StackTrace)), EventLogEntryType.Error);
                MySqlConnection.Close();
                MySqlConnection.Dispose();
                return false;
            }
        }

        public bool InserirItensPedido(int idPedido, List<OrderItemModel> carrinho)
        {
            long idItemNovo = 0;
            var sql = string.Empty;

            try
            {
                foreach (var item in carrinho)
                {
                    item.Valor = item.Valor / item.Quantidade;
                    sql = string.Format(
                        @"INSERT INTO PEDIDO_ITEM (PEDIDO, TIPO, QTD, VALOR, TAMANHO) VALUES 
                        ({0}, {1}, {2}, '{3}', '{4}')",
                        idPedido, item.IdTipo, item.Quantidade, item.Valor.ToString().Replace(",", "."), item.Tamanho);
                    idItemNovo = ExecuteSQLCommandNonQuery(sql);

                    foreach (var itemDetalhe in item.SubTipos)
                    {
                        sql = string.Format(
                            @"INSERT INTO PEDIDO_ITEM_DETALHE (PEDIDOITEM, PRODUTO, SUBTIPO, OBSERVACAO) VALUES 
                            ({0}, {1}, {2}, '{3}')",
                            idItemNovo, itemDetalhe.IdProduto, itemDetalhe.IdSubtipo, string.IsNullOrEmpty(itemDetalhe.Observacao) ? string.Empty : itemDetalhe.Observacao);
                        ExecuteSQLCommandNonQuery(sql);
                    }

                    if (item.ItensAdicionais != null && item.ItensAdicionais.Count > 0)
                    {
                        foreach (var idItemAdicional in item.ItensAdicionais)
                        {
                            sql = string.Format(
                                @"INSERT INTO PEDIDO_ITEM_ADICIONAL (PEDIDOITEM, ITEM_ADICIONAL) VALUES 
                            ({0}, {1})",
                                idItemNovo, idItemAdicional);
                            ExecuteSQLCommandNonQuery(sql);
                        }
                    }

                }

                MySqlConnection.Close();
                MySqlConnection.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                Logar(string.Format("RepetirUltimoPedido => {0}", string.Format("Exception: {0} ==> {1} ==> {2}", ex.Message, (ex.InnerException == null ? "NULL" : ex.InnerException.ToString()), ex.StackTrace)), EventLogEntryType.Error);
                MySqlConnection.Close();
                MySqlConnection.Dispose();
                return false;
            }
        }

        public bool FecharPedido(string idUser, int idCliente, int idEndereco, string bairro, string formaPagamento, List<OrderItemModel> carrinho, string observacao, bool removerTaxa)
        {
            float taxaEntrega = 0;
            if (!removerTaxa)
                taxaEntrega = ObterTaxaEntrega(bairro.ToUpper());

            var produtoService = new ProdutoDao();
            var valorTotal = produtoService.CalcularTotal(carrinho, taxaEntrega);

            var sql = string.Format(
                @"INSERT INTO PEDIDO 
                (FUNCIONARIO,CLIENTE,ENDERECO,VALORTOTAL,DATAPEDIDO,OBSERVACAO,FORMAPAGAMENTO)
                VALUES
                ('{0}', {1}, {2}, '{3}', NOW(), '{4}', '{5}')",
                idUser, idCliente, idEndereco, valorTotal.ToString().Replace(",", "."), observacao, formaPagamento);

            try
            {
                var idPedidoNovo = ExecuteSQLCommandNonQuery(sql);
                MySqlConnection.Close();
                MySqlConnection.Dispose();

                if (idPedidoNovo > 0)
                {
                    InserirItensPedido((int)idPedidoNovo, carrinho);
                    AtualizarUltimoPedidoCliente(idCliente, (int)idPedidoNovo);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logar(string.Format("FecharPedido => {0}", string.Format("Exception: {0} ==> {1} ==> {2}", ex.Message, (ex.InnerException == null ? "NULL" : ex.InnerException.ToString()), ex.StackTrace)), EventLogEntryType.Error);
                MySqlConnection.Close();
                MySqlConnection.Dispose();
                return false;
            }
        }

        public bool AtualizarUltimoPedidoCliente(int idCliente, int idPedido)
        {
            try
            {
                var sql = string.Format("UPDATE CLIENTE SET ULTIMOPEDIDO = {0} WHERE ID = {1}", idPedido, idCliente);
                ExecuteSQLCommandNonQuery(sql);
                MySqlConnection.Close();
                MySqlConnection.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                Logar(string.Format("AtualizarUltimoPedidoCliente => {0}", string.Format("Exception: {0} ==> {1} ==> {2}", ex.Message, (ex.InnerException == null ? "NULL" : ex.InnerException.ToString()), ex.StackTrace)), EventLogEntryType.Error);
                MySqlConnection.Close();
                MySqlConnection.Dispose();
                return false;
            }
        }

        public bool RepetirUltimoPedido(string idUser, int idCliente, int idEndereco, int idPedido, string formaPagamento)
        {
            var sql = string.Format(
                @"INSERT INTO PEDIDO 
                (FUNCIONARIO,CLIENTE,ENDERECO,VALORTOTAL,DATAPEDIDO,OBSERVACAO,FORMAPAGAMENTO)
                SELECT '{0}', {1}, {2}, VALORTOTAL, NOW(), 'PEDIDO DUPLICADO', '{3}' FROM PEDIDO WHERE ID = {4}",
                idUser, idCliente, idEndereco, formaPagamento, idPedido);

            try
            {

                var idPedidoNovo = ExecuteSQLCommandNonQuery(sql);
                MySqlConnection.Close();
                MySqlConnection.Dispose();

                if (idPedidoNovo > 0)
                {
                    RepetirItensPedido(idPedido, (int)idPedidoNovo);
                    AtualizarUltimoPedidoCliente(idCliente, (int)idPedidoNovo);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logar(string.Format("RepetirUltimoPedido => {0}", string.Format("Exception: {0} ==> {1} ==> {2}", ex.Message, (ex.InnerException == null ? "NULL" : ex.InnerException.ToString()), ex.StackTrace)), EventLogEntryType.Error);
                MySqlConnection.Close();
                MySqlConnection.Dispose();
                return false;
            }
        }

        public List<ItemAdicionalVO> ListarItemAdicional(int idPedidoItem)
        {
            var itensAdicionais = new List<ItemAdicionalVO>();

            var sql = string.Format(@"SELECT I.ID,
		                                     I.NOME,
		                                     I.VALOR
	                                    FROM PEDIDO_ITEM_ADICIONAL A
	                                    JOIN ITEM_ADICIONAL I ON A.ITEM_ADICIONAL = I.ID
                                       WHERE A.PEDIDOITEM = {0}", idPedidoItem);

            using (var reader = ExecuteSQLCommandDataReader(sql))
            {
                while (reader.Read())
                {
                    var itemAdicional = new ItemAdicionalVO
                    {
                        Id = getInt32("ID", reader),
                        Nome = getString("NOME", reader),
                        Valor = getFloat("VALOR", reader)
                    };
                    itensAdicionais.Add(itemAdicional);
                }
                reader.Close();
            }

            return itensAdicionais;
        }

        public float ObterTaxaEntrega(string bairro)
        {
            float retorno = 0;

            var sql = string.Format(@"SELECT TAXA
                                        FROM BAIRRO_TAXA
                                        WHERE BAIRRO = '{0}'", bairro);

            using (var reader = ExecuteSQLCommandDataReader(sql))
            {
                if (reader.Read())
                {
                    retorno = getFloat("TAXA", reader);
                }
                reader.Close();
            }

            return retorno;
        }

        private void Logar(string mensagem, EventLogEntryType tipo)
        {
            switch (tipo)
            {
                case EventLogEntryType.Error:
                    log.Error(mensagem);
                    break;
                case EventLogEntryType.Warning:
                    log.Warning(mensagem);
                    break;
                case EventLogEntryType.Information:
                    log.Information(mensagem);
                    break;
            }
        }
    }
}
