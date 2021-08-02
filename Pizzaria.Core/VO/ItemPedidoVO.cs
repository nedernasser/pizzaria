using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzaria.Core.VO
{
    public class ItemPedidoVO
    {
        public int Id { get; set; }
        public int IdPedido { get; set; }
        public int IdTipo { get; set; }
        public bool Broto { get; set; }
        public string Tipo { get; set; }
        public float Valor { get; set; }
        public int Quantidade { get; set; }

        public List<DetalhePedidoVO> ListaDetalhes { get; set; }
        public List<ItemAdicionalVO> ListaItensAdicionais { get; set; }
    }
}
