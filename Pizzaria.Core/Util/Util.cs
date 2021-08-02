using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzaria.Core.Util
{
    public static class Util
    {
        public static string RemoverAcentos(string texto)
        {
            var bytes = Encoding.GetEncoding("iso-8859-8").GetBytes(texto);
            return Encoding.UTF8.GetString(bytes);
        }

        public static string RemoverMascaraTelefone(string telefone)
        {
            if (string.IsNullOrEmpty(telefone))
            { return telefone; }

            return telefone
                .Replace("(", string.Empty)
                .Replace(")", string.Empty)
                .Replace(" ", string.Empty)
                .Replace("-", string.Empty);
        }

        public static string FormatarTelefone(string telefone)
        {
            if (string.IsNullOrEmpty(telefone))
            { return telefone; }

            var retorno = new StringBuilder();
            retorno.Append("(");
            retorno.Append(telefone.Substring(0, 2));
            retorno.Append(") ");

            var prefixo = telefone.Substring(2, 4);
            var final = telefone.Substring(6, 4);
            if (telefone.Length == 11)
            {
                prefixo = telefone.Substring(2, 5);
                final = telefone.Substring(7, 4);
            }
            retorno.Append(prefixo);
            retorno.Append("-");
            retorno.Append(final);

            return retorno.ToString();
        }
    }
}
