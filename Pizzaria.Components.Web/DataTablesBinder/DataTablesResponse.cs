using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pizzaria.Components.Web.DataTablesBinder.Interfaces;

namespace Pizzaria.Components.Web.DataTablesBinder
{
    public class DataTablesResponse : IDataTablesResponse
    {
        public int draw { get; private set; }
        public IEnumerable data { get; private set; }
        public int recordsTotal { get; private set; }
        public int recordsFiltered { get; private set; }
        public string extraData { get; set; }

        public DataTablesResponse(int draw, IEnumerable data, int recordsFiltered, int recordsTotal, string extraData = null)
        {
            this.draw = draw;
            this.data = data;
            this.recordsFiltered = recordsFiltered;
            this.recordsTotal = recordsTotal;
            this.extraData = extraData;
        }
    }
}
