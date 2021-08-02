using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Pizzaria.Core.Model.AccountViewModels
{
    public class VerifyCodeViewModel
    {
        [Required(ErrorMessage = "Campo obrigatório")]
        public string Provider { get; set; }

        [Required(ErrorMessage = "Campo obrigatório")]
        [Display(Name = "Código")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Lembrar neste navegador?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }
}