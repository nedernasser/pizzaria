using Microsoft.AspNet.Identity.EntityFramework;
using Pizzaria.Security.Data;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pizzaria.Core.Model.RoleViewModels
{
    public class IndexViewModel
    {
        [Display(Name = "Filtro")]
        public string Filtro { get; set; }

        public IList<ApplicationRole> Roles { get; set; }

        public string StatusMessage { get; set; }
    }
}
