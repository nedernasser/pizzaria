using Pizzaria.Core.VO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzaria.Core.Model
{
    public class ClienteModel
    {
        [Display(Name = "Filtro")]
        public string Filtro { get; set; }

        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [Display(Name = "Celular")]
        public string Celular { get; set; }

        [Display(Name = "Fixo")]
        public string Telefone { get; set; }

        [Display(Name = "Data Nascimento")]
        public string DataNascimento { get; set; }

        public int EnderecoId { get; set; }

        [Required(ErrorMessage = "Logradouro obrigatório!")]
        [Display(Name = "Logradouro")]
        public string Logradouro { get; set; }

        [Required(ErrorMessage = "Número obrigatório!")]
        [Display(Name = "Número")]
        public int Numero { get; set; }

        [Display(Name = "Complemento")]
        public string Complemento { get; set; }

        [Required(ErrorMessage = "Bairro obrigatório!")]
        [Display(Name = "Bairro")]
        public string Bairro { get; set; }

        [Required(ErrorMessage = "Cidade obrigatória!")]
        [Display(Name = "Cidade")]
        public string Cidade { get; set; }

        [Required(ErrorMessage = "Estado obrigatório!")]
        [Display(Name = "Estado")]
        public string Estado { get; set; }

        [Required(ErrorMessage = "CEP obrigatório!")]
        [Display(Name = "CEP")]
        public string CEP { get; set; }
    }
}
