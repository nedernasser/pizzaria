using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Pizzaria.Core.Model.UserViewModels
{
    public class CreateViewModel
    {
        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public string Nome { get; set; }

        [Display(Name = "E-mail")]
        [Required(ErrorMessage = "Campo obrigatório")]
        [EmailAddress(ErrorMessage = "Endereço de e-mail inválido")]
        public string Email { get; set; }

        [Display(Name = "Login")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public string Username { get; set; }

        [Display(Name = "Senha")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public string Password { get; set; }

        [Display(Name = "Perfis de Acesso")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public List<string> Perfil { get; set; }

        public string StatusMessage { get; set; }

        public List<SelectListItem> Roles { get; set; }
    }
}