using System.Web;
using System.Web.Mvc;

namespace Pizzaria.Site
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new Pizzaria.Security.CustomAttributes.ClaimsAuthorizeAttribute(Configuration.ApplicationName));
        }
    }
}
