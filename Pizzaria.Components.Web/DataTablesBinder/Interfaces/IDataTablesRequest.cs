using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzaria.Components.Web.DataTablesBinder.Interfaces
{
    public interface IDataTablesRequest
    {

        int Draw { get; set; }
        int Start { get; set; }
        int Length { get; set; }
        IDataTablesSearch Search { get; set; }
        IEnumerable<IDataTablesColumn> Columns { get; set; }

        IEnumerable<IDataTablesColumn> GetSortedColumns();
        IEnumerable<IDataTablesColumn> GetSearchableColumns();
    }
}
