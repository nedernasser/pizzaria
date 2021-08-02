using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Pizzaria.Core.Model.ManageViewModels
{
    public class AddPhoneNumberViewModel
    {
        [Required(ErrorMessage = "Campo obrigatório")]
        [Phone]
        [Display(Name = "Número de Telefone")]
        public string Number { get; set; }
    }
}