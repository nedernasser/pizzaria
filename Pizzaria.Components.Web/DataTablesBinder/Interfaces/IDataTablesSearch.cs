using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzaria.Components.Web.DataTablesBinder.Interfaces
{
    public interface IDataTablesSearch
    {
        bool IsRegexValue { get; set; }
        string Value { get; set; }
    }
}
