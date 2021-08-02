using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Pizzaria.Security.Data;

namespace Pizzaria.Security.IdentityConfig
{
    // Configure the application sign-in manager which is used in this application.
    public class ApplicationSignInManager : SignInManager<ApplicationUser, string>
    {
        private ApplicationRoleManager _roleManager;

        public ApplicationSignInManager(ApplicationUserManager userManager, ApplicationRoleManager roleManager, IAuthenticationManager authenticationManager)
            : base(userManager, authenticationManager)
        {
            _roleManager = roleManager;
        }

        public async override Task<ClaimsIdentity> CreateUserIdentityAsync(ApplicationUser user)
        {
            var userIdentity = await user.GenerateUserIdentityAsync((ApplicationUserManager)UserManager);

            //var userRoles = user.Roles;
            //foreach (var item in userRoles)
            //{
            //    var role = await _roleManager.FindByIdAsync(item.RoleId);
            //    var claims = await _roleManager.GetClaimsAsync(role);

            //    foreach (var itemClaim in claims)
            //    {
            //        var appAndClaim = role.Application.ApplicationName + "_" + itemClaim.Value;
            //        userIdentity.AddClaim(new Claim(appAndClaim, appAndClaim));
            //    }
            //}

            return userIdentity;
        }

        public static ApplicationSignInManager Create(IdentityFactoryOptions<ApplicationSignInManager> options, IOwinContext context)
        {
            return new ApplicationSignInManager(context.GetUserManager<ApplicationUserManager>(), context.Get<ApplicationRoleManager>(), context.Authentication);
        }
    }
}