using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Pizzaria.Core.Util;
using Pizzaria.Components.Web.DataTablesBinder.Interfaces;

namespace Pizzaria.Data
{
    public partial class BaseDao 
    {
        public MySqlConnection DB { get; set; }

        #region [ Atributos ]

        private MySqlConnection mysqlConnection;
        private MySqlCommand mysqlObject;

        #endregion

        #region [ Propriedades ]

        public MySqlConnection MySqlConnection
        {
            get { return mysqlConnection; }
            set { mysqlConnection = value; }
        }

        public MySqlCommand MySqlObject
        {
            get { return mysqlObject; }
            set { mysqlObject = value; }
        }

        #endregion

        #region [ Construtores ]

        public BaseDao()
        {
            mysqlConnection = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            mysqlObject = mysqlConnection.CreateCommand();
        }

        #endregion

        #region [ IBaseDao Members ]

        private void CheckConnection()
        {
            if (this.mysqlConnection.State != ConnectionState.Open)
                this.mysqlConnection.Open();
        }

        public MySqlParameter MontarOracleParameter<T>(string nome, MySqlDbType tipo, T valor, ParameterDirection direcao)
        {
            var retorno = new MySqlParameter();

            retorno.ParameterName = nome;
            retorno.MySqlDbType = tipo;
            retorno.Value = valor;
            retorno.Direction = direcao;

            return retorno;
        }

        public IDataReader ExecuteProcedureCommandDataReader(string textCommand)
        {
            return ExecuteProcedureCommandDataReader(textCommand, null);
        }

        public IDataReader ExecuteProcedureCommandDataReader(string textCommand, params MySqlParameter[] dbParameters)
        {
            if (null != dbParameters)
            {
                this.mysqlObject.Parameters.AddRange(dbParameters);
            }

            CheckConnection();

            this.mysqlObject.CommandText = textCommand;
            this.mysqlObject.CommandType = CommandType.StoredProcedure;

            return this.mysqlObject.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public bool ExecuteProcedureCommandNonQuery(string textCommand)
        {
            return ExecuteProcedureCommandNonQuery(textCommand, null);
        }

        public bool ExecuteProcedureCommandNonQuery(string textCommand, params MySqlParameter[] dbParameters)
        {
            CheckConnection();

            this.mysqlObject.CommandText = textCommand;
            this.mysqlObject.CommandType = CommandType.StoredProcedure;

            if (null != dbParameters)
            {
                this.mysqlObject.Parameters.Clear();
                this.mysqlObject.Parameters.AddRange(dbParameters);
            }

            return this.mysqlObject.ExecuteNonQuery() > 0;
        }

        public long ExecuteSQLCommandNonQuery(string textCommand)
        {
            return ExecuteSQLCommandNonQuery(textCommand, null);
        }

        public long ExecuteSQLCommandNonQuery(string textCommand, params MySqlParameter[] dbParameters)
        {
            this.mysqlObject.CommandText = textCommand;
            this.mysqlObject.CommandType = CommandType.Text;

            if (null != dbParameters)
            {
                this.mysqlObject.Parameters.AddRange(dbParameters);
            }

            CheckConnection();

            var rowsAfected = this.mysqlObject.ExecuteNonQuery();
            var id = this.mysqlObject.LastInsertedId;

            return id > 0 ? id : rowsAfected;
        }

        public IDataReader ExecuteSQLCommandDataReader(string textCommand)
        {
            return ExecuteSQLCommandDataReader(textCommand, null);
        }

        public IDataReader ExecuteSQLCommandDataReader(string textCommand, params MySqlParameter[] dbParameters)
        {
            if (null != dbParameters)
            {
                this.mysqlObject.Parameters.AddRange(dbParameters);
            }

            CheckConnection();

            this.mysqlObject.CommandText = textCommand;
            this.mysqlObject.CommandType = CommandType.Text;

            return this.mysqlObject.ExecuteReader(CommandBehavior.CloseConnection);
        }

        public int GetLastIdInserted()
        {
            var retorno = 0;
            try
            {
                var sql = "SELECT last_insert_id() ID";
                using (var reader = ExecuteSQLCommandDataReader(sql))
                {

                    if (reader.Read())
                    {
                        retorno = getInt32("ID", reader);
                    }
                    reader.Close();
                }

                return retorno;
            }
            catch
            {
                return 0;
            }
        }

        #endregion

        #region [ IDisposable Members ]

        public void Dispose()
        {
            try
            {
                if (null != mysqlObject)
                    mysqlObject.Dispose();

                if (null != mysqlConnection)
                {
                    if (mysqlConnection.State == ConnectionState.Open)
                        mysqlConnection.Close();

                    mysqlConnection.Dispose();
                }
            }
            catch { }
        }

        #endregion

        #region [ Util ]

        public Func<string, IDataReader, object> getObject = (name, rs) => rs.GetValue(rs.GetOrdinal(name));

        public Func<string, IDataReader, bool> isNull = (name, rs) => rs.IsDBNull(rs.GetOrdinal(name));

        public Func<string, IDataReader, string> getString = (name, rs) => rs.Get<string>(name);

        public Func<string, IDataReader, Int32> getInt32 = (name, rs) => rs.Get<int>(name);

        public Func<string, IDataReader, Int64> getInt64 = (name, rs) => rs.Get<long>(name);

        public Func<string, IDataReader, DateTime> getDate = (name, rs) => rs.Get<DateTime>(name);

        public Func<string, IDataReader, float> getFloat = (name, rs) => rs.Get<float>(name);

        protected IQueryable<T> AplicarPaginacaoOrdenacaoDataTable<T>(IQueryable<T> query, IDataTablesRequest tableQuery)
        {
            // Ordenação
            if (tableQuery.GetSortedColumns().Any())
            {
                foreach (var sortItem in tableQuery.GetSortedColumns().OrderBy(e => e.OrderIndex))
                {
                    if (sortItem.SortDirection.HasValue && sortItem.SortDirection.Value == Components.Web.DataTablesBinder.SortDirection.Descending)
                        query = query.OrderByDescending(sortItem.Data);
                    else
                        query = query.OrderBy(sortItem.Data);
                }
            }

            // Paginação
            if (tableQuery.Length != -1)
                query = query.Skip(tableQuery.Start).Take(tableQuery.Length);

            return query;
        }

        protected IQueryable<T> AplicarPaginacaoOrdenacaoDataTable<T, TFilter>(IQueryable<T> query, IDataTablesFilterRequest<TFilter> tableQuery)
        {
            // Ordenação
            if (tableQuery.GetSortedColumns().Any())
            {
                foreach (var sortItem in tableQuery.GetSortedColumns().OrderBy(e => e.OrderIndex))
                {
                    if (sortItem.SortDirection.HasValue && sortItem.SortDirection.Value == Components.Web.DataTablesBinder.SortDirection.Descending)
                        query = query.OrderByDescending(sortItem.Data);
                    else
                        query = query.OrderBy(sortItem.Data);
                }
            }

            // Paginação
            if (tableQuery.Length != -1)
                query = query.Skip(tableQuery.Start).Take(tableQuery.Length);

            return query;
        }

        #endregion
    }
}
