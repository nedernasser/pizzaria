using Pizzaria.Components.Web.DataTablesBinder.Interfaces;
using Pizzaria.Core.Model;
using Pizzaria.Core.Util;
using Pizzaria.Core.VO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzaria.Data
{
    public class ProdutoDao : BaseDao
    {
        Logger<PedidoDao> log;

        public ProdutoDao()
        {
            log = new Logger<PedidoDao>();
            log.init();
        }

        public List<TipoVO> ListarTipos(string filtro, bool tudo = false, bool broto = false)
        {
            var retorno = new List<TipoVO>();
            var sql = string.Format(@"SELECT ID, NOME
                                        FROM TIPO 
                                       WHERE ID <> 10 AND (NOME LIKE ""%{0}%"" OR ID = ""{1}"") 
                                    ORDER BY NOME", filtro, filtro);

            if (tudo)
            {
                sql = "SELECT ID, NOME FROM TIPO WHERE ID <> 10 ORDER BY NOME";
            }
            if (broto)
            {
                sql = string.Format(@"SELECT ID, NOME
                                        FROM TIPO 
                                       WHERE ID IN (1,2) AND (NOME LIKE ""%{0}%"" OR ID = ""{1}"") 
                                    ORDER BY NOME", filtro, filtro);
            }

            using (var reader = ExecuteSQLCommandDataReader(sql))
            {
                while (reader.Read())
                {
                    retorno.Add(new TipoVO
                    {
                        Id = getInt32("ID", reader),
                        Nome = getString("NOME", reader)
                    });
                }
                reader.Close();
            }
            return retorno.ToList();
        }

        public List<TipoVO> ListarSubTipos(int idTipo, bool tudo = false)
        {
            var retorno = new List<TipoVO>();
            var sql = string.Format(@"SELECT ID, NOME
                                        FROM SUBTIPO 
                                       WHERE TIPO = {0}", idTipo);

            if (tudo)
            {
                sql = "SELECT ID, TIPO, NOME FROM SUBTIPO ";
            }

            using (var reader = ExecuteSQLCommandDataReader(sql))
            {
                while (reader.Read())
                {
                    retorno.Add(new TipoVO
                    {
                        Id = getInt32("ID", reader),
                        Nome = getString("NOME", reader)
                    });
                }
                reader.Close();
            }
            return retorno.ToList();
        }

        public List<TipoVO> ListarProdutos(string filtro, bool ehPizza, int? subTipo = null)
        {
            var retorno = new List<TipoVO>();
            var sql = string.Format(@"SELECT ID, NOME, DESCRICAO, VALOR, BROTO
                                        FROM PRODUTO 
                                       WHERE {0} AND (DESCRICAO LIKE ""%{1}%"" OR NOME LIKE ""%{2}%"" OR ID = ""{3}"") 
                                    ORDER BY NOME", ehPizza ? "ID < 90" : "ID >= 90", filtro, filtro, filtro);

            using (var reader = ExecuteSQLCommandDataReader(sql))
            {
                while (reader.Read())
                {
                    var id = getInt32("ID", reader);

                    if (id != 53)
                    {
                        PopularProdutoVO(retorno, reader, id);
                    }
                    else
                    {
                        if (!subTipo.HasValue || (subTipo.HasValue && subTipo.Value == 1))
                            PopularProdutoVO(retorno, reader, id);
                    }
                    
                        
                }
                reader.Close();
            }
            return retorno;
        }

        private void PopularProdutoVO(List<TipoVO> retorno, IDataReader reader, int id)
        {
            retorno.Add(new TipoVO
            {
                Id = id,
                Nome = string.Format(
                    "{0} - {1}: {2}",
                    id.ToString(),
                    getString("NOME", reader),
                    getString("DESCRICAO", reader)),
                Valor = getFloat("VALOR", reader),
                Broto = getFloat("BROTO", reader)
            });
        }

        public Dictionary<int, string> ListarBrindes()
        {
            var retorno = new List<TipoVO>();
            var sql = "SELECT ID, NOME, DESCRICAO FROM PRODUTO WHERE VALOR = 0 ORDER BY NOME";

            using (var reader = ExecuteSQLCommandDataReader(sql))
            {
                while (reader.Read())
                {
                    retorno.Add(new TipoVO
                    {
                        Id = getInt32("ID", reader),
                        Nome = string.Format(
                            "{0} - {1}: {2}", 
                            getInt32("ID", reader).ToString(), 
                            getString("NOME", reader), 
                            getString("DESCRICAO", reader))
                    });
                }
                reader.Close();
            }
            return retorno.ToDictionary(b => b.Id, b => b.Nome);
        }

        public List<TipoVO> ListarTodosProdutos(bool ehPizza)
        {
            var retorno = new List<TipoVO>();
            var sql = string.Format(@"SELECT ID, NOME, DESCRICAO, VALOR, BROTO
                                        FROM PRODUTO 
                                       WHERE {0}
                                    ORDER BY NOME", ehPizza ? "ID < 90" : "ID >= 90");

            using (var reader = ExecuteSQLCommandDataReader(sql))
            {
                while (reader.Read())
                {
                    retorno.Add(new TipoVO
                    {
                        Id = getInt32("ID", reader),
                        Nome = getString("NOME", reader),
                        Descricao = getString("DESCRICAO", reader)
                    });
                }
                reader.Close();
            }
            return retorno.ToList();
        }

        public List<IngredienteVO> ListarIngredientes()
        {
            var retorno = new List<IngredienteVO>();
            var sql = "SELECT ID, NOME, PRINCIPAL FROM INGREDIENTE ORDER BY NOME";

            using (var reader = ExecuteSQLCommandDataReader(sql))
            {
                while (reader.Read())
                {
                    retorno.Add(new IngredienteVO
                    {
                        Id = getInt32("ID", reader),
                        Nome = getString("NOME", reader),
                        Principal = getInt32("PRINCIPAL", reader) == 1
                    });
                }
                reader.Close();
            }
            return retorno.ToList();
        }

        public Dictionary<int, string> ListarPromocoes()
        {
            var retorno = new List<TipoVO>();
            var sql = "SELECT ID, DESCRICAO FROM PROMOCAO ORDER BY DESCRICAO";

            using (var reader = ExecuteSQLCommandDataReader(sql))
            {
                while (reader.Read())
                {
                    retorno.Add(new TipoVO
                    {
                        Id = getInt32("ID", reader),
                        Nome = getString("DESCRICAO", reader)
                    });
                }
                reader.Close();
            }
            return retorno.ToDictionary(p => p.Id, p => p.Nome);
        }

        public List<ItemAdicionalVO> ListarItensAdicionais()
        {
            var retorno = new List<ItemAdicionalVO>();
            var sql = "SELECT ID, NOME, VALOR FROM ITEM_ADICIONAL ORDER BY NOME";

            using (var reader = ExecuteSQLCommandDataReader(sql))
            {
                while (reader.Read())
                {
                    retorno.Add(new ItemAdicionalVO
                    {
                        Id = getInt32("ID", reader),
                        Nome = getString("NOME", reader),
                        Valor = getFloat("VALOR", reader)
                    });
                }
                reader.Close();
            }
            return retorno.ToList();
        }

        private IDataReader ListarProdutos(List<DetalhePedidoVO> subtipos)
        {
            var listaProdutos = new List<int>();
            subtipos.ForEach(s => listaProdutos.Add(s.IdProduto));
            var produtos = string.Join(",", listaProdutos.ToArray());
            var sql = string.Format("SELECT * FROM PRODUTO WHERE ID IN ({0})", produtos);
            return ExecuteSQLCommandDataReader(sql);
        }

        public float CalcularTotalItensCarrinho(OrderItemModel item)
        {
            float valorMaior = 0;
            var produtoMaiorCusto = 0;

            if (item.SubTipos != null && item.SubTipos.Count > 0)
            {
                using (var reader = ListarProdutos(item.SubTipos))
                {
                    while (reader.Read())
                    {
                        var valor = item.Tamanho == "BROTO" ? getFloat(item.Tamanho, reader) : getFloat("VALOR", reader);
                        if (valor > valorMaior)
                        {
                            valorMaior = valor;
                            produtoMaiorCusto = getInt32("ID", reader);
                        }
                    }
                    reader.Close();
                }
            }

            return valorMaior;
        }

        public float CalcularTotalItensAdicionais(List<int> itensAdicionais)
        {
            float retorno = 0;

            if (itensAdicionais != null && itensAdicionais.Count > 0)
            {
                var itens = string.Join(",", itensAdicionais.ToArray());

                var sql = string.Format(@"SELECT VALOR
	                                        FROM ITEM_ADICIONAL
                                           WHERE ID IN ({0})", itens);

                using (var reader = ExecuteSQLCommandDataReader(sql))
                {
                    while (reader.Read())
                    {
                        retorno += getFloat("VALOR", reader);
                    }
                    reader.Close();
                }
            }

            return retorno;
        }

        public float ObterTotalItemCarrinho(OrderItemModel item)
        {
            float totalItensCarrinho = 0;
            if (item.SubTipos.Count == 1 && item.SubTipos[0].IdProduto == 53)
            {
                totalItensCarrinho = item.Valor;
            }
            else
            {
                var totalPadrao = CalcularTotalItensCarrinho(item);
                if (item.Valor > totalPadrao)
                {
                    totalItensCarrinho = item.Valor;
                }
                else
                {
                    totalItensCarrinho = totalPadrao;
                }
            }
            var totalItensAdicionais = CalcularTotalItensAdicionais(item.ItensAdicionais);
            var subtotal = Math.Round((decimal)((totalItensCarrinho + totalItensAdicionais) * item.Quantidade), 2);

            return (float)subtotal;
        }

        public float CalcularTotal(List<OrderItemModel> carrinho, float taxaEntrega)
        {
            float total = 0;
            foreach (var item in carrinho)
            {
                var valor = ObterTotalItemCarrinho(item);
                if (item.Valor == 0)
                    item.Valor = valor;
                total += valor;
            }
            total += taxaEntrega;

            return total;
        }

        public string MontarDescricaoCarrinho(List<OrderItemModel> carrinho)
        {
            var retorno = new StringBuilder();
            var itens = ListarItemCarrinho(carrinho);

            foreach (var item in itens)
            {
                retorno.Append(item.Detalhes);

                if (item.ListaItensAdicionais != null && item.ListaItensAdicionais.Count > 0)
                { retorno.AppendFormat(" ({0})", item.ItensAdicionais); }

                retorno.AppendFormat(" => {0} ", item.Valor.ToString("C"));

                retorno.Append("<br />");
            }

            return retorno.ToString();
        }

        public List<PedidoItemModel> ListarItemCarrinho(List<OrderItemModel> carrinho)
        {
            var retorno = new List<PedidoItemModel>();
            var tipos = ListarTipos(string.Empty, true);
            var subTipos = ListarSubTipos(0, true);
            var itens = ListarItensAdicionais();
            var pizzas = ListarTodosProdutos(true);
            var bebidas = ListarTodosProdutos(false);

            foreach (var item in carrinho)
            {
                var tipo = tipos.FirstOrDefault(t => t.Id == item.IdTipo);
                var model = new PedidoItemModel
                {
                    IdTipo = item.IdTipo,
                    Broto = item.Tamanho == "BROTO",
                    Quantidade = item.Quantidade,
                    Observacao = item.Observacao,
                    Valor = ObterTotalItemCarrinho(item)
                };

                if (tipo != null)
                {
                    model.Tipo = tipo.Nome;
                }
                else
                {
                    model.Tipo = "BEBIDA";
                }
                
                if (item.SubTipos != null && item.SubTipos.Count > 0)
                {
                    var detalhes = new List<DetalhePedidoVO>();
                    foreach (var subtipo in item.SubTipos)
                    {
                        var detalhe = new DetalhePedidoVO
                        {
                            IdSubtipo = subtipo.IdSubtipo,
                            Subtipo = subTipos.FirstOrDefault(t => t.Id == subtipo.IdSubtipo).Nome,
                            IdProduto = subtipo.IdProduto
                        };

                        var produto = new TipoVO();
                        if (item.IdTipo != 10)
                            produto = pizzas.FirstOrDefault(t => t.Id == subtipo.IdProduto);
                        else
                            produto = bebidas.FirstOrDefault(t => t.Id == subtipo.IdProduto);

                        detalhe.Produto = produto.Nome;
                        detalhe.DescricaoProduto = produto.Descricao;
                        detalhes.Add(detalhe);
                    }
                    model.ListaDetalhes = detalhes;
                }

                if (item.ItensAdicionais != null && item.ItensAdicionais.Count > 0)
                {
                    var itensAdicionais = new List<ItemAdicionalVO>();
                    foreach (var idItemAdicional in item.ItensAdicionais)
                    {
                        var itemAdicional = new ItemAdicionalVO
                        {
                            Id = idItemAdicional,
                            Nome = itens.FirstOrDefault(t => t.Id == idItemAdicional).Nome,
                            Valor = itens.FirstOrDefault(t => t.Id == idItemAdicional).Valor
                        };
                        itensAdicionais.Add(itemAdicional);
                    }
                    model.ListaItensAdicionais = itensAdicionais;
                }

                retorno.Add(model);
            }

            return retorno.OrderBy(s => s.IdTipo).ToList();
        }

        public ProdutoVO ObterProduto(int id)
        {
            var sql = string.Format(
                    @"SELECT ID, NOME, DESCRICAO, VALOR, BROTO, PROMOCAO
                        FROM PRODUTO 
                       WHERE ID = {0}", id);
            try
            {
                ProdutoVO retorno = null;

                using (var reader = ExecuteSQLCommandDataReader(sql))
                {
                    if (reader.Read())
                    {
                        int? promocao = null;
                        if (!isNull("PROMOCAO", reader))
                        {
                            promocao = getInt32("PROMOCAO", reader);
                        }
                        retorno = new ProdutoVO
                        {
                            Id = getInt32("ID", reader),
                            Nome = getString("NOME", reader),
                            Descricao = getString("DESCRICAO", reader),
                            ValorUnitario = getFloat("VALOR", reader).ToString("C"),
                            ValorBroto = getFloat("BROTO", reader).ToString("C"),
                            Promocao = promocao
                        };
                    }
                    reader.Close();
                }

                return retorno;
            }
            catch (Exception ex)
            {
                log.Error(ex, "Query: {0}", sql);
                throw ex;
            }
        }

        public IList<ProdutoVO> ListarProdutos(IDataTablesFilterRequest<CardapioModel> tableQuery, out int total)
        {
            var sql =
                    @"SELECT ID, NOME, DESCRICAO, VALOR, BROTO, PROMOCAO
                        FROM PRODUTO 
                    ORDER BY NOME";
            try
            {
                var filtros = tableQuery.FilterModel;
                var retorno = new List<ProdutoVO>();
                
                using (var reader = ExecuteSQLCommandDataReader(sql))
                {
                    while (reader.Read())
                    {
                        int? promocao = null;
                        if (!isNull("PROMOCAO", reader))
                        {
                            promocao = getInt32("PROMOCAO", reader);
                        }

                        retorno.Add(new ProdutoVO
                        {
                            Id = getInt32("ID", reader),
                            Nome = getString("NOME", reader),
                            Descricao = getString("DESCRICAO", reader),
                            Valor = getFloat("VALOR", reader),
                            Broto = getFloat("BROTO", reader),
                            ValorUnitario = getFloat("VALOR", reader).ToString("C"),
                            ValorBroto = getFloat("BROTO", reader).ToString("C"),
                            Promocao = promocao
                        });
                    }
                    reader.Close();
                }

                total = retorno.Count();
                var ret = retorno.AsQueryable();
                ret = AplicarPaginacaoOrdenacaoDataTable<ProdutoVO, CardapioModel>(ret, tableQuery);

                return ret.ToList();
            }
            catch (Exception ex)
            {
                log.Error(ex, "Query: {0}", sql);
                throw ex;
            }
        }

        public bool AtualizarProduto(ProdutoVO vo)
        {
            try
            {
                var sql = string.Format(
                    "UPDATE PRODUTO SET NOME = '{0}', DESCRICAO = '{1}', VALOR = '{2}', PROMOCAO = {3} WHERE ID = {4}", 
                    vo.Nome, 
                    vo.Descricao, 
                    vo.Valor.ToString().Replace(",", "."),
                    vo.Promocao.HasValue ? vo.Promocao.Value.ToString() : "NULL",
                    vo.Id);
                ExecuteSQLCommandNonQuery(sql);
                MySqlConnection.Close();
                MySqlConnection.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                log.Error(string.Format("AtualizarProduto => {0}", string.Format("Exception: {0} ==> {1} ==> {2}", ex.Message, (ex.InnerException == null ? "NULL" : ex.InnerException.ToString()), ex.StackTrace)));
                MySqlConnection.Close();
                MySqlConnection.Dispose();
                return false;
            }
        }

        public IList<PromocaoVO> ListarPromocoes(IDataTablesFilterRequest<PromocaoModel> tableQuery, out int total)
        {
            var sql = "SELECT * FROM PROMOCAO ORDER BY DESCRICAO";
            try
            {
                var filtros = tableQuery.FilterModel;
                var retorno = new List<PromocaoVO>();

                using (var reader = ExecuteSQLCommandDataReader(sql))
                {
                    while (reader.Read())
                    {
                        retorno.Add(new PromocaoVO
                        {
                            Id = getInt32("ID", reader),
                            Descricao = getString("DESCRICAO", reader),
                            Domingo = getInt32("DOMINGO", reader) == 1,
                            Segunda = getInt32("SEGUNDA", reader) == 1,
                            Terca = getInt32("TERCA", reader) == 1,
                            Quarta = getInt32("QUARTA", reader) == 1,
                            Quinta = getInt32("QUINTA", reader) == 1,
                            Sexta = getInt32("SEXTA", reader) == 1,
                            Sabado = getInt32("SABADO", reader) == 1,
                            Desconto = getFloat("DESCONTO", reader)
                        });
                    }
                    reader.Close();
                }

                total = retorno.Count();
                var ret = retorno.AsQueryable();
                ret = AplicarPaginacaoOrdenacaoDataTable<PromocaoVO, PromocaoModel>(ret, tableQuery);

                return ret.ToList();
            }
            catch (Exception ex)
            {
                log.Error(ex, "Query: {0}", sql);
                throw ex;
            }
        }

        public PromocaoVO ObterPromocao(int id)
        {
            var sql = string.Format(
                    @"SELECT *
                        FROM PROMOCAO 
                       WHERE ID = {0}", id);
            try
            {
                PromocaoVO retorno = null;

                using (var reader = ExecuteSQLCommandDataReader(sql))
                {
                    if (reader.Read())
                    {
                        int? brindeId = null;
                        if (!isNull("BRINDE", reader))
                            brindeId = getInt32("BRINDE", reader);

                        retorno = new PromocaoVO
                        {
                            Id = getInt32("ID", reader),
                            Descricao = getString("DESCRICAO", reader),
                            Domingo = getInt32("DOMINGO", reader) == 1,
                            Segunda = getInt32("SEGUNDA", reader) == 1,
                            Terca = getInt32("TERCA", reader) == 1,
                            Quarta = getInt32("QUARTA", reader) == 1,
                            Quinta = getInt32("QUINTA", reader) == 1,
                            Sexta = getInt32("SEXTA", reader) == 1,
                            Sabado = getInt32("SABADO", reader) == 1,
                            Desconto = getFloat("DESCONTO", reader),
                            PossuiBrinde = getInt32("POSSUIBRINDE", reader) == 1,
                            BrindeId = brindeId
                        };
                    }
                    reader.Close();
                }

                return retorno;
            }
            catch (Exception ex)
            {
                log.Error(ex, "Query: {0}", sql);
                throw ex;
            }
        }

        public bool GravarPromocao(PromocaoVO vo)
        {
            try
            {
                var sql = string.Format(
                    @"UPDATE PROMOCAO 
                         SET DESCRICAO = '{0}', 
                             DOMINGO = {1}, 
                             SEGUNDA = {2}, 
                             TERCA = {3}, 
                             QUARTA = {4}, 
                             QUINTA = {5}, 
                             SEXTA = {6}, 
                             SABADO = {7}, 
                             DESCONTO = {8}, 
                             POSSUIBRINDE = {9}, 
                             BRINDE = {10} 
                       WHERE ID = {11}",
                    vo.Descricao,
                    Convert.ToInt32(vo.Domingo),
                    Convert.ToInt32(vo.Segunda),
                    Convert.ToInt32(vo.Terca),
                    Convert.ToInt32(vo.Quarta),
                    Convert.ToInt32(vo.Quinta),
                    Convert.ToInt32(vo.Sexta),
                    Convert.ToInt32(vo.Sabado),
                    vo.Desconto.ToString().Replace(",", "."),
                    Convert.ToInt32(vo.PossuiBrinde),
                    vo.BrindeId.HasValue ? vo.BrindeId.Value : 0,
                    vo.Id);

                if (vo.Id == 0)
                {
                    sql = string.Format(
                        @"INSERT INTO PROMOCAO
                                       (DESCRICAO
                                       ,DOMINGO
                                       ,SEGUNDA
                                       ,TERCA
                                       ,QUARTA
                                       ,QUINTA
                                       ,SEXTA
                                       ,SABADO
                                       ,DESCONTO)
                                VALUES ('{0}'
                                       ,{1}
                                       ,{2}
                                       ,{3}
                                       ,{4}
                                       ,{5}
                                       ,{6}
                                       ,{7}
                                       ,{8})",
                        vo.Descricao,
                        Convert.ToInt32(vo.Domingo),
                        Convert.ToInt32(vo.Segunda),
                        Convert.ToInt32(vo.Terca),
                        Convert.ToInt32(vo.Quarta),
                        Convert.ToInt32(vo.Quinta),
                        Convert.ToInt32(vo.Sexta),
                        Convert.ToInt32(vo.Sabado),
                        vo.Desconto.ToString().Replace(",", "."));
                }

                ExecuteSQLCommandNonQuery(sql);
                MySqlConnection.Close();
                MySqlConnection.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                log.Error(string.Format("GravarPromocao => {0}", string.Format("Exception: {0} ==> {1} ==> {2}", ex.Message, (ex.InnerException == null ? "NULL" : ex.InnerException.ToString()), ex.StackTrace)));
                MySqlConnection.Close();
                MySqlConnection.Dispose();
                return false;
            }
        }

        public bool PossuiBrinde(List<OrderItemModel> carrinho, out string brinde)
        {
            var qtdPizza = 0;
            brinde = string.Empty;
            var promocoes = new List<int>();
            var tipos = ListarTipos(string.Empty, true);

            foreach (var item in carrinho)
            {
                var tipo = tipos.FirstOrDefault(t => t.Id == item.IdTipo);
                if (tipo != null)
                {
                    using (var reader = ListarProdutos(item.SubTipos))
                    {
                        while (reader.Read())
                        {
                            int? promocao = null;
                            if (!isNull("PROMOCAO", reader))
                                promocao = getInt32("PROMOCAO", reader);

                            if (promocao.HasValue)
                            {
                                if (!promocoes.Contains(promocao.Value))
                                    promocoes.Add(promocao.Value);

                                qtdPizza += item.Quantidade;
                                break;
                            }
                        }
                        reader.Close();
                    }
                }
            }

            var validacaoQtdPizza = qtdPizza > 1;
            if (!validacaoQtdPizza)
            { return false; }

            if (promocoes.Count == 1)
            {
                var promocaoId = promocoes.FirstOrDefault();
                var vo = ObterPromocao(promocaoId);
                if (vo != null)
                {
                    if (!vo.PossuiBrinde)
                    { return false; }

                    var validacaoDiaSemana = false;
                    switch (DateTime.Today.DayOfWeek)
                    {
                        case DayOfWeek.Sunday:
                            validacaoDiaSemana = vo.Domingo;
                            break;
                        case DayOfWeek.Monday:
                            validacaoDiaSemana = vo.Segunda;
                            break;
                        case DayOfWeek.Tuesday:
                            validacaoDiaSemana = vo.Terca;
                            break;
                        case DayOfWeek.Wednesday:
                            validacaoDiaSemana = vo.Quarta;
                            break;
                        case DayOfWeek.Thursday:
                            validacaoDiaSemana = vo.Quinta;
                            break;
                        case DayOfWeek.Friday:
                            validacaoDiaSemana = vo.Sexta;
                            break;
                        case DayOfWeek.Saturday:
                            validacaoDiaSemana = vo.Sabado;
                            break;
                    }
                    if (!validacaoDiaSemana)
                    { return false; }

                    var produto = ObterProduto(vo.BrindeId.Value);
                    brinde = string.Format("1 {0}: {1}", produto.Nome, produto.Descricao);
                }
                else
                { return false; }
            }

            return true;
        }

        public List<int> ListarProdutosCondicionais()
        {
            var retorno = new List<int>();
            var sql = 
                string.Format(@"
                    SELECT ID 
                      FROM PRODUTO 
                     WHERE DESCRICAO LIKE '%mussarela ou catupiry%' 
                        OR DESCRICAO LIKE '%catupiry ou mussarela%'
                        OR DESCRICAO LIKE '%parmesão ou rúcula%'
                        OR DESCRICAO LIKE '%bufala ou cream%'");

            using (var reader = ExecuteSQLCommandDataReader(sql))
            {
                while (reader.Read())
                {
                    retorno.Add(getInt32("ID", reader));
                }
                reader.Close();
            }
            return retorno.ToList();
        }
    }
}
