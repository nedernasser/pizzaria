using Pizzaria.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzaria.Core.VO
{
    public class ClienteVO
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public DateTime? DataNascimento { get; set; }
        public string DataNascimentoFormatada
        {
            get
            {
                return DataNascimento.Value != DateTime.MinValue ? DataNascimento.Value.ToString("dd/MM/yyyy") : string.Empty;
            }
        }

        public string Celular { get; set; }

        public string Telefone { get; set; }

        public EnderecoVO EnderecoPadrao { get; set; }

        public List<EnderecoVO> ListaEnderecos { get; set; }

        public PedidoModel UltimoPedido { get; set; }
    }
}
