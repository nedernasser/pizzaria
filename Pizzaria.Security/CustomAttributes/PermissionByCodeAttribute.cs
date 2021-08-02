using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pizzaria.Security.CustomAttributes
{
    public class PermissionByCodeAttribute : Attribute
    {
        public string PermissionCode { get; set; }

        public PermissionByCodeAttribute(string PermissionCode)
        {
            this.PermissionCode = PermissionCode;
        }
    }
}