using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pizzaria.Core.VO;

namespace Pizzaria.Core.Model
{
    public class HomeModel
    {
        [Required(ErrorMessage = "Campo obrigatório!")]
        [Display(Name = "Fixo ou Celular")]
        public string TelefonePesquisa { get; set; }

        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [Display(Name = "Nome")]
        public string Nome { get; set; }
        
        [Display(Name = "Data de Nascimento")]
        public string DataNascimento { get; set; }
        
        [Display(Name = "Celular")]
        public string Celular { get; set; }

        [Display(Name = "Fixo")]
        public string Telefone { get; set; }

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

        public Dictionary<int, string> ListaEnderecos { get; set; }

        public PedidoModel UltimoPedido { get; set; }

        [Required(ErrorMessage = "Logradouro obrigatório!")]
        [Display(Name = "Logradouro")]
        public string NovoLogradouro { get; set; }

        [Required(ErrorMessage = "Número obrigatório!")]
        [Display(Name = "Número")]
        public int NovoNumero { get; set; }

        [Display(Name = "Complemento")]
        public string NovoComplemento { get; set; }

        [Required(ErrorMessage = "Bairro obrigatório!")]
        [Display(Name = "Bairro")]
        public string NovoBairro { get; set; }

        [Required(ErrorMessage = "Cidade obrigatória!")]
        [Display(Name = "Cidade")]
        public string NovoCidade { get; set; }

        [Required(ErrorMessage = "Estado obrigatório!")]
        [Display(Name = "Estado")]
        public string NovoEstado { get; set; }

        [Required(ErrorMessage = "CEP obrigatório!")]
        [Display(Name = "CEP")]
        public string NovoCEP { get; set; }
    }
}
