using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzaria.Core.VO
{
    public class ResumoPedidoVO
    {
        public int Id { get; set; }
        public string DataInicial { get; set; }
        public string DataFinal { get; set; }
        public string TotalPizza { get; set; }
        public string TotalBebida { get; set; }
        public string TotalDinheiro { get; set; }
        public string TotalDebito { get; set; }
        public string TotalCredito { get; set; }
        public string TotalValeRefeicao { get; set; }
        public string TotalGeral { get; set; }
    }
}
