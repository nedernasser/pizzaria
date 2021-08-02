using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Pizzaria.Security.Extensions.Controller;
using Pizzaria.Security.IdentityConfig;
using Pizzaria.Security.Data;
using Pizzaria.Core.Model.RoleViewModels;
using Pizzaria.Components.Web.DataTablesBinder;
using Pizzaria.Components.Web.DataTablesBinder.Interfaces;
using Pizzaria.Data;

namespace Pizzaria.Site.Controllers
{
    public class RoleController : BaseController
    {
        #region [ Atributos e Propriedades ]

        private UsuarioDao usuarioDao;
        private ApplicationDbContext _context;

        public ApplicationDbContext Context
        {
            get
            {
                return _context ?? HttpContext.GetOwinContext().Get<ApplicationDbContext>();
            }
            private set
            {
                _context = value;
            }
        }

        #endregion

        #region [ Construtores e Dispose ]

        public RoleController()
        {
        }

        public RoleController(ApplicationRoleManager roleManager, ApplicationDbContext context)
        {
            RoleManager = roleManager;
            Context = context;
        }

        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            base.Initialize(requestContext);
            usuarioDao = new UsuarioDao();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_roleManager != null)
                {
                    _roleManager.Dispose();
                    _roleManager = null;
                }

                if (_context != null)
                {
                    _context.Dispose();
                    _context = null;
                }

                if (null != usuarioDao)
                {
                    usuarioDao.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        #endregion

        #region [ Index ]

        public ActionResult Index()
        {
            var model = new IndexViewModel
            {
                StatusMessage = StatusMessage
            };

            ViewBag.HasCreatePermission = this.VerifyPermission("Role_Create");
            ViewBag.HasDetailsPermission = this.VerifyPermission("Role_Details");
            ViewBag.HasDeletePermission = this.VerifyPermission("Role_Delete");

            return View(model);
        }

        [HttpPost]
        public JsonResult CarregarDataTable([ModelBinder(typeof(DataTablesFilterBinder<IndexViewModel>))] IDataTablesFilterRequest<IndexViewModel> tableQuery)
        {
            try
            {
                var roles = RoleManager.Roles.OrderBy(x => x.Name).ToList();

                int total = 0;
                var retorno = usuarioDao.ListarRoles(roles, tableQuery, out total);
                return Json(new DataTablesResponse(tableQuery.Draw, retorno, total, total));
            }
            catch
            {
                return Json(new DataTablesResponse(tableQuery.Draw, new List<DetailsViewModel>(), 0, 0));
            }
        }

        #endregion

        #region [ Create ]

        public ActionResult Create(int? id)
        {
            var model = new CreateViewModel()
            {
                StatusMessage = StatusMessage
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = new ApplicationRole(model.RoleName);

                await RoleManager.CreateAsync(role);

                StatusMessage = "Dados gravados com sucesso!";
                return RedirectToAction("Index");
            }

            return View(model);
                
        }

        #endregion

        #region [ Delete ]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string roleName)
        {
            var role = await RoleManager.FindByNameAsync(roleName);

            await RoleManager.DeleteAsync(role);

            StatusMessage = "Perfil excluído com sucesso!";

            return RedirectToAction("Index");
        }

        #endregion

        #region [ Métodos Privados ]

        private List<SelectListItem> LoadApplications()
        {
            var applicationList = from e in Context.Applications
                                  orderby e.ApplicationName
                                  select new SelectListItem
                                  {
                                      Text = e.ApplicationName,
                                      Value = e.Id.ToString()
                                  };
            return applicationList.ToList();
        }

        private List<SelectListItem> LoadClaims(int applicationId)
        {
            var claimsList = from e in Context.ApplicationPermissions
                             where e.ApplicationId == applicationId
                             select new SelectListItem
                             {
                                 Text = e.PermissionName,
                                 Value = e.PermissionCode != null ? e.PermissionCode : e.Controller + "_" + e.Action,
                                 Group = new SelectListGroup { Name = e.PermissionGroupName },
                                 Selected = false
                             };

            return claimsList.ToList();
        }

        private async Task<DetailsViewModel> LoadDetails(string roleName)
        {
            var role = await RoleManager.FindByNameAsync(roleName);

            //var selectedClaimsList = await RoleManager.GetClaimsAsync(role);
            //foreach (var item in claimsList)
            //{
            //    item.Selected = selectedClaimsList.Any(x => x.Value == item.Value);
            //}

            var model = new DetailsViewModel
            {
                //ApplicationId = role.ApplicationId,
                //Applications = LoadApplications(),
                RoleName = role.Name,
                RoleNameWithoutApplication = role.NameWithoutApplication,
                //Claims = claimsList,
                StatusMessage = StatusMessage
            };

            return model;
        }

        #endregion
    }
}