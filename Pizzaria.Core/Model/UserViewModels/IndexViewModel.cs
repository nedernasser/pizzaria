using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Pizzaria.Core.Model.UserViewModels
{
    public class IndexViewModel
    {
        [Display(Name = "Filtro")]
        public string Filtro { get; set; }

        public IList<DetailsViewModel> Users { get; set; }

        public string StatusMessage { get; set; }
    }
}