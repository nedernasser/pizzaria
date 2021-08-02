using Pizzaria.Core.VO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzaria.Core.Model
{
    public class PromocaoModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [Display(Name = "Nome")]
        public string Descricao { get; set; }

        public bool PossuiBrinde { get; set; }

        [Display(Name = "Selecione o Brinde")]
        public int? BrindeId { get; set; }

        [Display(Name = "Domingo")]
        public bool Domingo { get; set; }
        [Display(Name = "Segunda-feira")]
        public bool Segunda { get; set; }
        [Display(Name = "Terca-feira")]
        public bool Terca { get; set; }
        [Display(Name = "Quarta-feira")]
        public bool Quarta { get; set; }
        [Display(Name = "Quinta-feira")]
        public bool Quinta { get; set; }
        [Display(Name = "Sexta-feira")]
        public bool Sexta { get; set; }
        [Display(Name = "Sábado")]
        public bool Sabado { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [Display(Name = "Desconto")]
        public string Desconto { get; set; }

        public List<PromocaoVO> ListaPromocoes { get; set; }
    }
}
