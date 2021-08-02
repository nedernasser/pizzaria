using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Pizzaria.Security.Extensions.Api
{
    public static class ApiExtensions
    {
        public static bool VerifyPermission(this System.Web.Http.ApiController controllerBase, string permissionCode)
        {
            bool hasPermission = false;
            if (controllerBase.RequestContext.Principal.Identity.IsAuthenticated)
            {
                string portal = Default.ApplicationName;
                var adminClaim = string.Format("{0}_{1}", portal, Security.Default.AdminClaimName);
                var permissionCodeClaim = string.Format("{0}_{1}", portal, permissionCode);
                var user = controllerBase.RequestContext.Principal as ClaimsPrincipal;
                hasPermission = true;
                //if (user.HasClaim(adminClaim, adminClaim) ||
                //    user.HasClaim(permissionCodeClaim, permissionCodeClaim))
                //{
                //    hasPermission = true;
                //}
            }

            return hasPermission;
        }
    }
}
