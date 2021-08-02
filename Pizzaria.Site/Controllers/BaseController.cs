using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Pizzaria.Security.IdentityConfig;

namespace Pizzaria.Site.Controllers
{
    public class BaseController : Controller
    {
        #region [ Atributos ]

        public ApplicationSignInManager _signInManager;
        public ApplicationUserManager _userManager;
        public ApplicationRoleManager _roleManager;

        public IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
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
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
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
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            set
            {
                _roleManager = value;
            }
        }

        public string StatusMessage
        {
            get { return ViewData["StatusMessage"] == null ? string.Empty : ViewData["StatusMessage"].ToString(); }
            set { ViewData["StatusMessage"] = value; }
        }

        public string UserName
        {
            get
            {
                return HttpContext.Request.Cookies["UsuarioLogado"]["UserName"];
            }
        }

        public string UserId
        {
            get
            {
                return HttpContext.Request.Cookies["UsuarioLogado"]["UserId"];
            }
        }

        public string Perfil
        {
            get
            {
                return HttpContext.Request.Cookies["UsuarioLogado"]["Perfil"];
            }
        }

        #endregion

        #region [ Initialize e Dispose ]

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion

        #region [ Métodos Públicos ]

        public string LimparTelefone(string telefone)
        {
            return telefone
                .Replace("(", string.Empty)
                .Replace(")", string.Empty)
                .Replace("-", string.Empty)
                .Replace(" ", string.Empty);
        }

        #endregion
    }
}