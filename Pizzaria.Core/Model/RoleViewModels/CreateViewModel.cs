using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Pizzaria.Core.Model.RoleViewModels
{
    public class CreateViewModel
    {
        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public string RoleName { get; set; }

        public string StatusMessage { get; set; }
    }
}
