using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Pizzaria.Security.Extensions.Controller
{
    public static class ControllerExtensions
    {
        public static bool VerifyPermission(this ControllerBase controllerBase, string permissionCode)
        {
            bool hasPermission = false;
            if (controllerBase.ControllerContext.HttpContext.User.Identity.IsAuthenticated)
            {
                string portal = Default.ApplicationName;
                var adminClaim = string.Format("{0}_{1}", portal, Security.Default.AdminClaimName);
                var permissionCodeClaim = string.Format("{0}_{1}", portal, permissionCode);
                var user = controllerBase.ControllerContext.HttpContext.User as ClaimsPrincipal;
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