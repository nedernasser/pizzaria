using Pizzaria.Core.VO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzaria.Core.Model
{
    public class OrderModel
    {
        public int IdCliente { get; set; }
        public int IdEndereco { get; set; }

        public List<OrderItemModel> Itens { get; set; }
    }

    public class OrderItemModel
    {
        public int Quantidade { get; set; }
        public int IdTipo { get; set; }
        public string Tamanho { get; set; }
        public List<DetalhePedidoVO> SubTipos { get; set; }
        public List<int> ItensAdicionais { get; set; }
        public string Observacao { get; set; }
        public float Valor { get; set; }
    }
}
