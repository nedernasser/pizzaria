using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pizzaria.Core.Model;
using Pizzaria.Core.Util;
using Pizzaria.Core.VO;
using System.Diagnostics;
using Pizzaria.Components.Web.DataTablesBinder.Interfaces;

namespace Pizzaria.Data
{
    public class ClienteDao : BaseDao
    {
        Logger<ClienteDao> log;

        public ClienteDao()
        {
            log = new Logger<ClienteDao>();
            log.init();
        }

        public ClienteVO PesquisarTelefone(string telefone)
        {
            Logar("Pesquisando..", EventLogEntryType.Information);
            ClienteVO model = null;
            var sql = string.Format(@"SELECT * FROM CLIENTE WHERE TELEFONE LIKE ""%{0}%"" OR CELULAR LIKE ""%{1}%""", telefone, telefone);

            using (var reader = ExecuteSQLCommandDataReader(sql))
            {
                if (reader.Read())
                {
                    model = new ClienteVO
                    {
                        Id = getInt32("ID", reader),
                        Nome = getString("NOME", reader),
                        Celular = Util.FormatarTelefone(getString("CELULAR", reader)),
                        Telefone = Util.FormatarTelefone(getString("TELEFONE", reader)),
                        DataNascimento = !isNull("DATANASCIMENTO", reader) ? getDate("DATANASCIMENTO", reader) : DateTime.MinValue
                    };
                }
                reader.Close();
            }
            if (model != null)
            {
                model.ListaEnderecos = ListarEnderecos(model.Id);
                model.UltimoPedido = new PedidoDao().ObterUltimoPedido(model.Id);
                if (model.UltimoPedido != null)
                {
                    model.EnderecoPadrao = model.UltimoPedido.Endereco;
                }
                else
                {
                    model.EnderecoPadrao = model.ListaEnderecos.FirstOrDefault(e => e.EnderecoPadrao);
                }
            }
            return model;
        }

        public ClienteVO Obter(int idCliente)
        {
            ClienteVO model = null;
            var sql = string.Format(@"SELECT * FROM CLIENTE WHERE ID = {0}", idCliente);

            using (var reader = ExecuteSQLCommandDataReader(sql))
            {
                if (reader.Read())
                {
                    model = new ClienteVO
                    {
                        Id = getInt32("ID", reader),
                        Nome = getString("NOME", reader),
                        Celular = Util.FormatarTelefone(getString("CELULAR", reader)),
                        Telefone = Util.FormatarTelefone(getString("TELEFONE", reader)),
                        DataNascimento = !isNull("DATANASCIMENTO", reader) ? getDate("DATANASCIMENTO", reader) : DateTime.MinValue
                    };
                }
                reader.Close();
            }
            if (model != null)
            {
                model.ListaEnderecos = ListarEnderecos(idCliente);
                model.UltimoPedido = new PedidoDao().ObterUltimoPedido(idCliente);
            }
            return model;
        }

        public EnderecoVO ObterEndereco(int id)
        {
            EnderecoVO retorno = null;
            var sql = string.Format(@"SELECT ID, LOGRADOURO, NUMERO, COMPLEMENTO, BAIRRO, CIDADE, ESTADO, CEP, PADRAO
                                        FROM ENDERECO 
                                       WHERE ID = {0}", id);

            using (var reader = ExecuteSQLCommandDataReader(sql))
            {
                while (reader.Read())
                {
                    retorno = new EnderecoVO
                    {
                        Id = getInt32("ID", reader),
                        Logradouro = getString("LOGRADOURO", reader),
                        Numero = getInt32("NUMERO", reader),
                        Complemento = getString("COMPLEMENTO", reader),
                        Bairro = getString("BAIRRO", reader),
                        Cidade = getString("CIDADE", reader),
                        Estado = getString("ESTADO", reader),
                        CEP = getString("CEP", reader),
                        EnderecoPadrao = getInt32("PADRAO", reader) == 1
                    };
                }
                reader.Close();
            }
            return retorno;
        }

        public List<EnderecoVO> ListarEnderecos(int idCliente)
        {
            var retorno = new List<EnderecoVO>();
            var sql = string.Format(@"SELECT ID, LOGRADOURO, NUMERO, COMPLEMENTO, BAIRRO, CIDADE, ESTADO, CEP, PADRAO
                                        FROM ENDERECO 
                                       WHERE CLIENTE = {0}
                                    ORDER BY PADRAO DESC, LOGRADOURO", idCliente);

            using (var reader = ExecuteSQLCommandDataReader(sql))
            {
                while (reader.Read())
                {
                    retorno.Add(new EnderecoVO
                    {
                        Id = getInt32("ID", reader),
                        Logradouro = getString("LOGRADOURO", reader),
                        Numero = getInt32("NUMERO", reader),
                        Complemento = getString("COMPLEMENTO", reader),
                        Bairro = getString("BAIRRO", reader),
                        Cidade = getString("CIDADE", reader),
                        Estado = getString("ESTADO", reader),
                        CEP = getString("CEP", reader),
                        EnderecoPadrao = getInt32("PADRAO", reader) == 1
                    });
                }
                reader.Close();
            }
            return retorno.ToList();
        }

        public IList<EnderecoVO> ListarEnderecos(IDataTablesFilterRequest<ClienteModel> tableQuery, out int total)
        {
            var sql = string.Empty;
            try
            {
                var filtros = tableQuery.FilterModel;
                var retorno = new List<EnderecoVO>();

                sql = string.Format(@"SELECT ID, LOGRADOURO, NUMERO, COMPLEMENTO, BAIRRO, CIDADE, ESTADO, CEP, PADRAO
                                        FROM ENDERECO 
                                       WHERE CLIENTE = {0}
                                    ORDER BY PADRAO DESC, LOGRADOURO", filtros.Id);
                using (var reader = ExecuteSQLCommandDataReader(sql))
                {
                    while (reader.Read())
                    {
                        retorno.Add(new EnderecoVO
                        {
                            Id = getInt32("ID", reader),
                            Logradouro = getString("LOGRADOURO", reader),
                            Numero = getInt32("NUMERO", reader),
                            Complemento = getString("COMPLEMENTO", reader),
                            Bairro = getString("BAIRRO", reader),
                            Cidade = getString("CIDADE", reader),
                            Estado = getString("ESTADO", reader),
                            CEP = getString("CEP", reader),
                            EnderecoPadrao = getInt32("PADRAO", reader) == 1
                        });
                    }
                    reader.Close();
                }

                total = retorno.Count();
                var ret = retorno.AsQueryable();
                ret = AplicarPaginacaoOrdenacaoDataTable<EnderecoVO, ClienteModel>(ret, tableQuery);

                return ret.ToList();
            }
            catch (Exception ex)
            {
                log.Error(ex, "Query: {0}", sql);
                throw ex;
            }
        }

        public long InserirCliente(ClienteVO vo)
        {
            long result = 0;
            var sql = string.Format(
                @"INSERT INTO
                CLIENTE
                (NOME, DATANASCIMENTO, CELULAR, TELEFONE, DATACADASTRO)
                VALUES
                ('{0}', {1}, '{2}', '{3}', NOW())",
                vo.Nome,
                vo.DataNascimento.HasValue ? string.Format("'{0}'", vo.DataNascimento.Value.ToString("yyyy-MM-dd")) : "NULL",
                Util.RemoverMascaraTelefone(vo.Celular),
                Util.RemoverMascaraTelefone(vo.Telefone));
            try
            {

                result = ExecuteSQLCommandNonQuery(sql);
                MySqlConnection.Close();
                MySqlConnection.Dispose();
            }
            catch (Exception ex)
            {
                Logar(string.Format("InserirCliente => {0}", string.Format("Exception: {0} ==> {1} ==> {2}", ex.Message, (ex.InnerException == null ? "NULL" : ex.InnerException.ToString()), ex.StackTrace)), EventLogEntryType.Error);
                MySqlConnection.Close();
                MySqlConnection.Dispose();
            }
            return result;
        }

        public long InserirEndereco(EnderecoVO vo)
        {
            long result = 0;
            var sql = string.Format(
                @"INSERT INTO
                ENDERECO
                (CLIENTE, LOGRADOURO, NUMERO, COMPLEMENTO, BAIRRO, CIDADE, ESTADO, CEP, PADRAO)
                VALUES
                ({0}, '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', {8})",
                vo.ClienteId,
                vo.Logradouro,
                vo.Numero,
                vo.Complemento,
                vo.Bairro,
                vo.Cidade,
                vo.Estado,
                vo.CEP,
                vo.EnderecoPadrao ? 1 : 0);
            try
            {
                result = ExecuteSQLCommandNonQuery(sql);
                MySqlConnection.Close();
                MySqlConnection.Dispose();
            }
            catch (Exception ex)
            {
                Logar(string.Format("InserirEndereco => {0}", string.Format("Exception: {0} ==> {1} ==> {2}", ex.Message, (ex.InnerException == null ? "NULL" : ex.InnerException.ToString()), ex.StackTrace)), EventLogEntryType.Error);
                MySqlConnection.Close();
                MySqlConnection.Dispose();
            }
            return result;
        }

        public bool AlterarEndereco(EnderecoVO vo)
        {
            long result = 0;
            var sql = string.Format(
                @"UPDATE
                ENDERECO
                SET 
                LOGRADOURO = '{0}', 
                NUMERO = '{1}', 
                COMPLEMENTO = '{2}',  
                BAIRRO = '{3}', 
                CIDADE = '{4}', 
                ESTADO = '{5}' 
                WHERE ID = {6}",
                vo.Logradouro,
                vo.Numero,
                vo.Complemento,
                vo.Bairro,
                vo.Cidade,
                vo.Estado,
                vo.Id);
            try
            {
                result = ExecuteSQLCommandNonQuery(sql);
                MySqlConnection.Close();
                MySqlConnection.Dispose();
            }
            catch (Exception ex)
            {
                Logar(string.Format("InserirEndereco => {0}", string.Format("Exception: {0} ==> {1} ==> {2}", ex.Message, (ex.InnerException == null ? "NULL" : ex.InnerException.ToString()), ex.StackTrace)), EventLogEntryType.Error);
                MySqlConnection.Close();
                MySqlConnection.Dispose();
            }
            return result > 0;
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

        public IList<ClienteVO> ListarClientes(IDataTablesFilterRequest<ClienteModel> tableQuery, out int total)
        {
            var sql = string.Empty;
            try
            {
                var filtros = tableQuery.FilterModel;
                var retorno = new List<ClienteVO>();

                sql = string.Format("SELECT * FROM CLIENTE WHERE NOME LIKE '%{0}%' OR CELULAR LIKE '%{1}%' OR TELEFONE LIKE '%{2}%' ORDER BY NOME", filtros.Filtro, filtros.Filtro, filtros.Filtro);
                using (var reader = ExecuteSQLCommandDataReader(sql))
                {
                    while (reader.Read())
                    {
                        retorno.Add(new ClienteVO
                        {
                            Id = getInt32("ID", reader),
                            Nome = getString("NOME", reader),
                            Celular = Util.FormatarTelefone(getString("CELULAR", reader)),
                            Telefone = Util.FormatarTelefone(getString("TELEFONE", reader)),
                            DataNascimento = !isNull("DATANASCIMENTO", reader) ? getDate("DATANASCIMENTO", reader) : DateTime.MinValue
                        });
                    }
                    reader.Close();
                }

                total = retorno.Count();
                var ret = retorno.AsQueryable();
                ret = AplicarPaginacaoOrdenacaoDataTable<ClienteVO, ClienteModel>(ret, tableQuery);

                return ret.ToList();
            }
            catch (Exception ex)
            {
                log.Error(ex, "Query: {0}", sql);
                throw ex;
            }
        }

        public bool AtualizarCliente(ClienteVO vo)
        {
            try
            {
                var sql = string.Format(
                    "UPDATE CLIENTE SET NOME = '{0}', CELULAR = '{1}', TELEFONE = '{2}', DATANASCIMENTO = '{3}' WHERE ID = {4}",
                    vo.Nome,
                    vo.Celular,
                    vo.Telefone,
                    vo.DataNascimento.HasValue ? vo.DataNascimento.Value.ToString("yyyy-MM-dd") : string.Empty,
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
    }
}
