using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Pizzaria.Core.Model.ManageViewModels
{
    public class VerifyPhoneNumberViewModel
    {
        [Required(ErrorMessage = "Campo obrigatório")]
        [Display(Name = "Código")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [Phone]
        [Display(Name = "Número de Telefone")]
        public string PhoneNumber { get; set; }
    }
}