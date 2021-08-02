using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzaria.Data
{
    public static class DataMixins
    {
        #region [ para IDataReader ]

        /// <summary>
        /// Obtem, a partir de um datareader, um valor, ja no tipo correto. Caso o valor seja dbnull, retorna o default desse tipo
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="self"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(this IDataReader self, string key)
        {
            var val = self[key];

            var type4Convertion =
                (typeof(T).IsGenericType && typeof(T).GetGenericTypeDefinition().IsAssignableFrom(typeof(Nullable<>))) ?
                typeof(T).GetGenericArguments()[0] :
                typeof(T);

            return val != DBNull.Value ?
                (T)Convert.ChangeType(val, type4Convertion) :
                default(T);
        }

        /// <summary>
        /// Usado para obter, a partir de um 'S' ou 'N', um true ou false
        /// </summary>
        /// <param name="self"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool SOrN(this IDataReader self, string key)
        {
            return self[key] != DBNull.Value && (self[key].ToString() == "S" || self[key].ToString() == "s");
        }

        #endregion
    }
}
