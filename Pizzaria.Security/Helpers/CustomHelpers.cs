using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Pizzaria.Security.Helpers
{
    public static class CustomHelpers
    {
        private static bool VerificarPermissao(IPrincipal user, string controller, string action, string permissionCode = null)
        {
            return user.IsInRole("Administrador") || user.IsInRole("Owner");
        }

        public static MvcHtmlString SecurityButtonHelper(this HtmlHelper helper, string id, string name, string type, string className, string imageName, string controller, string action, string permissionCode = null, object parameters = null)
        {
            bool temPermissao = VerificarPermissao(helper.ViewContext.HttpContext.User, controller, action, permissionCode);
            if (temPermissao)
            {
                TagBuilder builder = new TagBuilder("button");
                builder.MergeAttribute("id", id);
                builder.MergeAttribute("type", type);
                builder.MergeAttribute("class", className);
                if (string.IsNullOrEmpty(imageName))
                    builder.InnerHtml = name;
                else
                    builder.InnerHtml = string.Format("<i class=\"{0}\"></i> {1}", imageName, name);

                if (type.ToLower().Equals("button"))
                {
                    UrlHelper urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
                    string url = urlHelper.Action(action, controller, parameters);
                    builder.MergeAttribute("onclick", string.Format("location.href = '{0}'", url));
                }
                return MvcHtmlString.Create(builder.ToString());
            }
            else
                return MvcHtmlString.Empty;
        }

        public static MvcHtmlString SecurityMenuItemHelper(this HtmlHelper helper, string name, string className, string imageName, string controller, string action, string permissionCode = null, object parameters = null)
        {
            bool temPermissao = VerificarPermissao(helper.ViewContext.HttpContext.User, controller, action, permissionCode);

            if (temPermissao)
            {
                TagBuilder builderLi = new TagBuilder("li");
                if (!string.IsNullOrEmpty(className))
                    builderLi.MergeAttribute("class", className);

                TagBuilder builder = new TagBuilder("a");
                if (string.IsNullOrEmpty(imageName))
                    builder.InnerHtml = name;
                else
                    builder.InnerHtml = string.Format("<i class=\"{0}\"></i> {1}", imageName, name);

                UrlHelper urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
                string url = urlHelper.Action(action, controller, parameters);
                builder.MergeAttribute("href", url);
                
                builderLi.InnerHtml = builder.ToString();

                return MvcHtmlString.Create(builderLi.ToString());
            }
            else
                return MvcHtmlString.Empty;
        }

        public static MvcHtmlString SecurityLinkHelper(this HtmlHelper helper, string name, string className, string imageName, string controller, string action, string permissionCode = null, object parameters = null)
        {
            bool temPermissao = VerificarPermissao(helper.ViewContext.HttpContext.User, controller, action, permissionCode);

            if (temPermissao)
            {
                TagBuilder builder = new TagBuilder("a");
                builder.MergeAttribute("class", className);

                if (string.IsNullOrEmpty(imageName))
                    builder.InnerHtml = name;
                else
                    builder.InnerHtml = string.Format("<i class=\"{0}\"></i> {1}", imageName, name);

                UrlHelper urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
                string url = urlHelper.Action(action, controller, parameters);
                builder.MergeAttribute("href", url);
                return MvcHtmlString.Create(builder.ToString());
            }
            else
                return MvcHtmlString.Empty;
        }

        public static MvcHtmlString SecurityLinkJavascriptHelper(this HtmlHelper helper, string name, string className, string imageName, string controller, string action, string functionName, string permissionCode = null)
        {
            bool temPermissao = VerificarPermissao(helper.ViewContext.HttpContext.User, controller, action, permissionCode);

            if (temPermissao)
            {
                TagBuilder builder = new TagBuilder("a");
                builder.MergeAttribute("class", className);

                if (string.IsNullOrEmpty(imageName))
                    builder.InnerHtml = name;
                else
                    builder.InnerHtml = string.Format("<i class=\"{0}\"></i> {1}", imageName, name);

                builder.MergeAttribute("onclick", string.Format("{0}; return false;", functionName));
                return MvcHtmlString.Create(builder.ToString());
            }
            else
                return MvcHtmlString.Empty;
        }

        public static MvcHtmlString JavascriptUrlHelper(this HtmlHelper helper, string functionName, string controller, string action, string permissionCode = null, object parameters = null)
        {
            bool temPermissao = VerificarPermissao(helper.ViewContext.HttpContext.User, controller, action, permissionCode);

            if (temPermissao)
            {
                UrlHelper urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
                string url = urlHelper.Action(action, controller, parameters);
                return MvcHtmlString.Create(url);
            }
            else
                return MvcHtmlString.Empty;
        }
    }
}
