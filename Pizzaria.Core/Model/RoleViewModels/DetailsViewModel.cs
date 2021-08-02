using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Pizzaria.Core.Model.RoleViewModels
{
    public class DetailsViewModel
    {
        [Display(Name = "Nome")]
        public string RoleName { get; set; }
        public string RoleNameWithoutApplication { get; set; }

        [Display(Name = "Permissões")]
        public IEnumerable<string> SelectedClaims { get; set; }
        public IEnumerable<SelectListItem> Claims { get; set; }

        public string StatusMessage { get; set; }
    }
}
