using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Pizzaria.Security.CustomAttributes
{
    public class ClaimsAuthorizeAttribute : AuthorizeAttribute
    {
        private string portalName;

        public ClaimsAuthorizeAttribute(string portal) : base()
        {
            this.portalName = portal;
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            var isAuthorized = filterContext.HttpContext.User.Identity.IsAuthenticated;

            var skipAuthorization = filterContext.ActionDescriptor.GetCustomAttributes(typeof(SkipAuthorizationAttribute), true).Length > 0;

            if (isAuthorized && 
                !skipAuthorization &&
                filterContext.HttpContext.User != null &&
                !filterContext.HttpContext.SkipAuthorization &&
                filterContext.HttpContext.Request.Cookies.AllKeys.Contains("UsuarioLogado"))
            {
                var controller = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                var action = filterContext.ActionDescriptor.ActionName;
                var claimName = string.Format("{0}_{1}_{2}", portalName, controller, action);
                var adminClaimName = string.Format("{0}_{1}", portalName, Security.Default.AdminClaimName);

                if (filterContext.ActionDescriptor.GetCustomAttributes(typeof(PermissionByCodeAttribute), true).Length > 0)
                    claimName = string.Format("{0}_{1}", portalName, ((PermissionByCodeAttribute)filterContext.ActionDescriptor.GetCustomAttributes(typeof(PermissionByCodeAttribute), true)[0]).PermissionCode);
                
                var user = HttpContext.Current.User as ClaimsPrincipal;

                //if (!user.HasClaim(adminClaimName, adminClaimName) &&
                //    !user.HasClaim(claimName, claimName))
                //{
                //    filterContext.Result = new RedirectToRouteResult(
                //        new System.Web.Routing.RouteValueDictionary
                //        {
                //            { "action", "AccessDenied" },
                //            { "controller", "Account" }
                //        });
                //}
            }
            else
            {
                //filterContext.Result = new RedirectToRouteResult(
                //    new System.Web.Routing.RouteValueDictionary
                //    { 
                //        { "action", "LogOff" },
                //        { "controller", "Account" }
                //    });
            }
        }
    }
}