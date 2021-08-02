using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace Pizzaria.Security.CustomAttributes
{
    public class HttpClaimsAuthorizeAttribute : System.Web.Http.AuthorizeAttribute
    {
        private string portalName;

        public HttpClaimsAuthorizeAttribute(string portal) : base()
        {
            this.portalName = portal;
        }

        public override void OnAuthorization(HttpActionContext actionContext)
        {
            base.OnAuthorization(actionContext);
            var isAuthorized = actionContext.RequestContext.Principal.Identity.IsAuthenticated;

            var skipAuthorization = actionContext.ActionDescriptor.GetCustomAttributes<SkipAuthorizationAttribute>().Count > 0;

            if (isAuthorized && !skipAuthorization && actionContext.RequestContext.Principal != null)
            {
                var controller = actionContext.ControllerContext.ControllerDescriptor.ControllerName;
                var action = actionContext.ActionDescriptor.ActionName;
                var claimName = string.Format("{0}_{1}_{2}", portalName, controller, action);
                var adminClaimName = string.Format("{0}_{1}", portalName, Security.Default.AdminClaimName);

                if (actionContext.ActionDescriptor.GetCustomAttributes<PermissionByCodeAttribute>().Count > 0)
                    claimName = string.Format("{0}_{1}", portalName, actionContext.ActionDescriptor.GetCustomAttributes<PermissionByCodeAttribute>()[0].PermissionCode);

                var user = actionContext.RequestContext.Principal as ClaimsPrincipal;

                //if (!user.HasClaim(adminClaimName, adminClaimName) &&
                //    !user.HasClaim(claimName, claimName))
                //{
                //    actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Forbidden);
                //}
            }
        }
    }
}
