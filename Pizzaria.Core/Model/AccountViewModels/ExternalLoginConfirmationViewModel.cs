using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Pizzaria.Core.Model.AccountViewModels
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required(ErrorMessage = "Campo obrigatório")]
        [Display(Name = "E-mail")]
        [EmailAddress]
        public string Email { get; set; }
    }
}