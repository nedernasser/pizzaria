using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzaria.Core.VO
{
    public class EnderecoVO
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string Logradouro { get; set; }
        public int Numero { get; set; }
        public string Complemento { get; set; }
        public string Bairro { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public string CEP { get; set; }
        public string EnderecoCompleto
        {
            get
            {
                return string.Format("{0}, {1}{2}<br />{3} - {4}-{5}",
                    Logradouro,
                    Numero,
                    (string.IsNullOrEmpty(Complemento) ? string.Empty : string.Format(" {0} ", Complemento)),
                    Bairro,
                    Cidade,
                    Estado
                );
            }
        }
        public string EnderecoDataTable
        {
            get
            {
                return string.Format("{0}, {1}{2} | {3} - {4}-{5}",
                    Logradouro,
                    Numero,
                    (string.IsNullOrEmpty(Complemento) ? string.Empty : string.Format(" {0} ", Complemento)),
                    Bairro,
                    Cidade,
                    Estado
                );
            }
        }
        public bool EnderecoPadrao { get; set; }
    }
}
