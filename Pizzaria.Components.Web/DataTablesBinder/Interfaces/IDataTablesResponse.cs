using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzaria.Components.Web.DataTablesBinder.Interfaces
{
    public interface IDataTablesResponse
    {

        int draw { get; }
        IEnumerable data { get; }
        int recordsTotal { get; }
        int recordsFiltered { get; }
    }
}
