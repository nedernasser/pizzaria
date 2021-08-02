using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Pizzaria.Components.Web.DataTablesBinder.Interfaces;

namespace Pizzaria.Components.Web.DataTablesBinder
{
    public class DataTablesSearch : IDataTablesSearch
    {
        public bool IsRegexValue { get; set; }
        public string Value { get; set; }

        public DataTablesSearch(string value, bool isRegexValue)
        {
            Value = value;
            IsRegexValue = isRegexValue;
        }
    }
}
