using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzaria.Core.VO
{
    public class ProdutoVO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public float Valor { get; set; }
        public string ValorUnitario { get; set; }
        public float Broto { get; set; }
        public string ValorBroto { get; set; }
        public int? Promocao { get; set; }
    }

    public class TipoVO
    {
        public string value
        {
            get
            {
                return Id.ToString();
            }
        }
        public string label
        {
            get
            {
                return Nome;
            }
        }

        public string IdNome
        {
            get
            {
                return string.Format(
                    "{0} - {1}",
                    Id.ToString(),
                    Nome);
            }
        }

        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public float Valor { get; set; }
        public float Broto { get; set; }
    }
}
