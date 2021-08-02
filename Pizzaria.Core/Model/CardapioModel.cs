using Pizzaria.Core.VO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzaria.Core.Model
{
    public class CardapioModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [Display(Name = "Valor")]
        public string Valor { get; set; }

        [Required(ErrorMessage = "Campo obrigatório!")]
        [Display(Name = "Broto")]
        public string Broto { get; set; }

        [Display(Name = "Promoção")]
        public int? Promocao { get; set; }

        public List<ProdutoVO> ListaProdutos { get; set; }
    }
}
