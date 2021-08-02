using System.Web;
using System.Web.Http;
using Pizzaria.Security.IdentityConfig;
using System.Net.Http;
using Microsoft.AspNet.Identity.Owin;
using Pizzaria.Core.Model.UserViewModels;
using System.Linq;
using Pizzaria.Security.Data;

namespace Pizzaria.Site.Controllers
{
    [RoutePrefix("api/authentication")]
    public class AuthenticationController : ApiController
    {
        #region [ Atributos ]

        public ApplicationSignInManager _signInManager;
        public ApplicationUserManager _userManager;
        public ApplicationRoleManager _roleManager;

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? Request.GetOwinContext().Get<ApplicationSignInManager>();
            }
            set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? Request.GetOwinContext().Get<ApplicationRoleManager>();
            }
            set
            {
                _roleManager = value;
            }
        }

        #endregion

        [HttpPost]
        [Route("Login")]
        public IHttpActionResult Login(string userName, string password)
        {
            ApplicationUser user = null;
            DetailsViewModel retorno = null;

            user = UserManager.FindByNameAsync(userName).Result;
            if (user == null)
            {
                user = UserManager.FindByEmailAsync(userName).Result;
            }

            if (user != null)
            {
                var status = SignInManager.PasswordSignInAsync(user.UserName, password, false, shouldLockout: false).Result;
                if (status == SignInStatus.Success)
                {
                    retorno = new DetailsViewModel
                    {
                        Id = user.Id,
                        Nome = user.Nome,
                        Username = user.UserName,
                        Email = user.Email
                    };
                    var roleId = user.Roles.FirstOrDefault().RoleId;
                    retorno.Perfil = RoleManager.Roles.Where(x => x.Id == roleId).FirstOrDefault().NameWithoutApplication;
                }
            }
            return Ok(retorno);
        }
    }
}