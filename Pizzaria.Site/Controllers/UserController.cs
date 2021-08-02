using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Pizzaria.Security.IdentityConfig;
using Pizzaria.Core.Model.UserViewModels;
using Pizzaria.Security.Data;
using Pizzaria.Security.Extensions.Controller;
using Pizzaria.Components.Web.DataTablesBinder;
using Pizzaria.Components.Web.DataTablesBinder.Interfaces;
using Pizzaria.Data;

namespace Pizzaria.Site.Controllers
{
    public class UserController : BaseController
    {
        #region [ Atributos e Propriedades ]

        private UsuarioDao usuarioDao;

        #endregion

        #region [ Construtores e Dispose ]

        public UserController()
        {
        }

        public UserController(ApplicationUserManager userManager, ApplicationRoleManager roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
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
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_roleManager != null)
                {
                    _roleManager.Dispose();
                    _roleManager = null;
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

            ViewBag.HasDeletePermission = this.VerifyPermission("User_Delete");

            return View(model);
        }

        [HttpPost]
        public JsonResult CarregarDataTable([ModelBinder(typeof(DataTablesFilterBinder<IndexViewModel>))] IDataTablesFilterRequest<IndexViewModel> tableQuery)
        {
            try
            {
                var userQuery = UserManager.Users;
                var roleQuery = RoleManager.Roles;

                var query = from e in userQuery
                            orderby e.Nome
                            select new DetailsViewModel
                            {
                                Nome = e.Nome,
                                Username = e.UserName,
                                Email = e.Email,
                                Perfil = (from f in e.Roles
                                          from g in roleQuery.Where(x => x.Id == f.RoleId)
                                          select g.Name).FirstOrDefault()
                            };

                var users = query.ToList();

                int total = 0;
                IList<DetailsViewModel> listaUsuarios = usuarioDao.ListarUsuarios(users, tableQuery, out total);
                return Json(new DataTablesResponse(tableQuery.Draw, listaUsuarios, total, total));
            }
            catch
            {
                return Json(new DataTablesResponse(tableQuery.Draw, new List<DetailsViewModel>(), 0, 0));
            }
        }

        #endregion

        #region [ Create ]

        public ActionResult Create()
        {
            var model = new CreateViewModel()
            {
                Roles = LoadRoles(),
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
                var user = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.Email,
                    Nome = model.Nome
                };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    var userWithId = await UserManager.FindByNameAsync(model.Username);
                    var roles = RoleManager.Roles.Where(x => model.Perfil.Contains(x.Id)).ToList();
                    foreach (var perfil in roles)
                        await UserManager.AddToRoleAsync(userWithId.Id, perfil.Name);

                    StatusMessage = "Dados gravados com sucesso!";
                    return RedirectToAction("Details", new { userName = model.Username });
                }
                else
                    AddErrors(result);
            }

            model.StatusMessage = StatusMessage;
            model.Roles = LoadRoles();
            return View(model);
        }

        #endregion

        #region [ Details ]

        public async Task<ActionResult> Details(string userName)
        {
            var model = await LoadDetails(userName);

            model.StatusMessage = StatusMessage;

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Details(DetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Username);
                if (user != null)
                {
                    user.Nome = model.Nome;
                    user.Email = model.Email;

                    var result = await UserManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        model.StatusMessage = "Dados gravados com sucesso!";

                        var dbRoles = RoleManager.Roles.Select(x => x.Name).ToArray();
                        await UserManager.RemoveFromRolesAsync(user.Id, dbRoles);

                        var selectedRoles = RoleManager.Roles.Where(x => model.Perfil.Contains(x.Id)).ToList();
                        foreach (var perfil in selectedRoles)
                            await UserManager.AddToRoleAsync(user.Id, perfil.Name);
                    }
                    else
                        AddErrors(result);
                }
            }

            model.Roles = LoadRoles();
            return View(model);
        }

        #endregion

        #region [ Delete ]

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(string userName)
        {
            var user = await UserManager.FindByNameAsync(userName);

            await UserManager.DeleteAsync(user);

            StatusMessage = "Usuário excluído com sucesso!";

            return RedirectToAction("Index");
        }

        #endregion

        #region [ Métodos Privados ]

        private List<SelectListItem> LoadRoles()
        {
            var roleList = from e in RoleManager.Roles
                           orderby e.Name
                           select new
                           {
                               Name = e.Name,
                               Id = e.Id
                           };

           var roles = from e in roleList.ToList()
                       select new SelectListItem
                       {
                           Text = e.Name.Substring(e.Name.IndexOf("_") + 1),
                           Value = e.Id
                       };

            return roles.ToList();
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private async Task<DetailsViewModel> LoadDetails(string userName)
        {
            var user = await UserManager.FindByNameAsync(userName);

            var model = new DetailsViewModel
            {
                Nome = user.Nome,
                Email = user.Email,
                Username = userName
            };

            if (user.Roles.Count > 0)
            {
                model.Perfil = user.Roles.Select(e => e.RoleId).FirstOrDefault();
            }

            model.Roles = LoadRoles();

            return model;
        }

        #endregion
    }
}