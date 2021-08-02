using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzaria.Core.VO
{
    public class BasicoVO<TKey, TValue>
    {
        public BasicoVO() { }

        public BasicoVO(TKey codigo, TValue valor)
        {
            this.Codigo = codigo;
            this.Valor = valor;
        }

        public TKey Codigo { get; set; }

        public TValue Valor { get; set; }

        public TValue label
        {
            get { return Valor; }
        }

        public TValue value
        {
            get { return Valor; }
        }
    }

    public class KeyValueVO
    {
        public int Key { get; set; }
        public string Value { get; set; }
    }
}
