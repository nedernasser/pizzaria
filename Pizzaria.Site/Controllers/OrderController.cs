using System.Web;
using System.Web.Http;
using Pizzaria.Security.IdentityConfig;
using System.Net.Http;
using Microsoft.AspNet.Identity.Owin;
using Pizzaria.Core.Model.UserViewModels;
using System.Linq;
using System.Collections.Generic;
using Pizzaria.Core.Model;

namespace Pizzaria.Site.Controllers
{
    [RoutePrefix("api/order")]
    public class OrderController : ApiController
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
        [Route("GetList")]
        public IHttpActionResult GetList(string perfil, string id)
        {
            var retorno = new List<PedidoModel>();

            var user = UserManager.FindByIdAsync(id).Result;
            if (user != null)
            {
                

            }
            return Ok(retorno);
        }
    }
}