using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Pizzaria.Core.Model.UserViewModels
{
    public class DetailsViewModel
    {
        public string Id { get; set; }

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

        [Display(Name = "Perfil de Acesso")]
        [Required(ErrorMessage = "Campo obrigatório")]
        public string Perfil { get; set; }

        public string StatusMessage { get; set; }

        public List<SelectListItem> Roles { get; set; }
    }
}