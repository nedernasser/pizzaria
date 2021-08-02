using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Pizzaria.Core.Model.ManageViewModels
{
    public class SetPasswordViewModel
    {
        [Required(ErrorMessage = "Campo obrigatório")]
        [StringLength(100, ErrorMessage = "O campo {0} deve ter ao menos {2} caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nova Senha")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Nova Senha")]
        [Compare("NewPassword", ErrorMessage = "A nova senha e a confirmação devem ser iguais.")]
        public string ConfirmPassword { get; set; }
    }
}