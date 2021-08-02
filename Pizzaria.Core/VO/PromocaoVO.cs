using Pizzaria.Core.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzaria.Core.VO
{
    public class PromocaoVO
    {
        public int Id { get; set; }

        public string Descricao { get; set; }

        public bool PossuiBrinde { get; set; }
        public int? BrindeId { get; set; }

        public bool Domingo { get; set; }
        public bool Segunda { get; set; }
        public bool Terca { get; set; }
        public bool Quarta { get; set; }
        public bool Quinta { get; set; }
        public bool Sexta { get; set; }
        public bool Sabado { get; set; }

        public float Desconto { get; set; }
        public string DescontoFormatado
        {
            get
            {
                return string.Format("{0} %", Desconto.ToString("#.00", CultureInfo.InvariantCulture));
            }
        }
    }
}
