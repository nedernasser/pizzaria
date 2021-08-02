using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzaria.Core.VO
{
    public class DetalhePedidoVO
    {
        public int IdProduto { get; set; }
        public string Produto { get; set; }
        public int IdSubtipo { get; set; }
        public string Subtipo { get; set; }
        public string DescricaoProduto { get; set; }
        public string Observacao { get; set; }
    }
}
