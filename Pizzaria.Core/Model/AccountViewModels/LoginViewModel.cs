using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Pizzaria.Core.Model.AccountViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Campo Obrigatório")]
        [Display(Name = "Login")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Campo Obrigatório")]
        [DataType(DataType.Password)]
        [Display(Name = "Senha")]
        public string Password { get; set; }

        [Display(Name = "Lembrar Usuário?")]
        public bool RememberMe { get; set; }
    }
}