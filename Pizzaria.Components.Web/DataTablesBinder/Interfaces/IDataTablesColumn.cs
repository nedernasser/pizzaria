using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzaria.Components.Web.DataTablesBinder.Interfaces
{
    public interface IDataTablesColumn
    {
        string Data { get; set; }
        string Name { get; set; }
        bool Searchable { get; set; }
        bool Orderable { get; set; }
        IDataTablesSearch Search { get; set; }
        int? OrderIndex { get; set; }
        SortDirection? SortDirection { get; set; }
    }
}
