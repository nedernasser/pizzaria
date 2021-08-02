using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzaria.Components.Web.DataTablesBinder
{
    internal class KeyValueWork
    {
        internal string ObjIndex { get; set; }
        internal string ParentName { get; set; }
        internal string Key { get; set; }
        internal string Value { get; set; }
        internal KeyValuePair<string, string> SourceKvp { get; set; }
    }
}
