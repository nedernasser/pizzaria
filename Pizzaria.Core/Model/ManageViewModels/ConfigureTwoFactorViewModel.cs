using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Pizzaria.Core.Model.ManageViewModels
{
    public class ConfigureTwoFactorViewModel
    {
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
    }
}