using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Pizzaria.Core.Model.ManageViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Campo obrigatório")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha Atual")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(100, ErrorMessage = "O camop {0} deve ter ao menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nova Senha")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Nova Senha")]
        [Compare("NewPassword", ErrorMessage = "A nova senha e a confirmação devem ser iguais.")]
        public string ConfirmPassword { get; set; }
    }
}