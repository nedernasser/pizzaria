namespace Pizzaria.Security.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Security.Claims;

    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;

    using Pizzaria.Security.Data;
    using Pizzaria.Security.IdentityConfig;
    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
            SetSqlGenerator("MySql.Data.MySqlClient", new MySql.Data.Entity.MySqlMigrationSqlGenerator());
        }

        protected override void Seed(ApplicationDbContext context)
        {
            //var application = context.Applications.Where(x => x.ApplicationName == Default.SSOApplicationName).FirstOrDefault();
            //if (application == null)
            //{
            //    application = new Application() { ApplicationName = Default.SSOApplicationName };
            //    context.Applications.Add(application);
            //    context.SaveChanges();

            //    context.ApplicationPermissions.Add(new ApplicationPermission { ApplicationId = application.Id, PermissionGroupName = Default.AdminClaimName, PermissionName = Default.AdminClaimDescription, PermissionCode = Default.AdminClaimName });

            //    context.ApplicationPermissions.Add(new ApplicationPermission { ApplicationId = application.Id, PermissionGroupName = "Perfis de Acesso", PermissionName = "Listar", Controller = "Role", Action = "Index" });
            //    context.ApplicationPermissions.Add(new ApplicationPermission { ApplicationId = application.Id, PermissionGroupName = "Perfis de Acesso", PermissionName = "Visualizar", Controller = "Role", Action = "Details" });
            //    context.ApplicationPermissions.Add(new ApplicationPermission { ApplicationId = application.Id, PermissionGroupName = "Perfis de Acesso", PermissionName = "Inserir", Controller = "Role", Action = "Create" });
            //    context.ApplicationPermissions.Add(new ApplicationPermission { ApplicationId = application.Id, PermissionGroupName = "Perfis de Acesso", PermissionName = "Editar", PermissionCode = "Permission_Edit" });
            //    context.ApplicationPermissions.Add(new ApplicationPermission { ApplicationId = application.Id, PermissionGroupName = "Perfis de Acesso", PermissionName = "Excluir", Controller = "Role", Action = "Delete" });

            //    context.ApplicationPermissions.Add(new ApplicationPermission { ApplicationId = application.Id, PermissionGroupName = "Usuários", PermissionName = "Listar", Controller = "User", Action = "Index" });
            //    context.ApplicationPermissions.Add(new ApplicationPermission { ApplicationId = application.Id, PermissionGroupName = "Usuários", PermissionName = "Visualizar", Controller = "User", Action = "Details" });
            //    context.ApplicationPermissions.Add(new ApplicationPermission { ApplicationId = application.Id, PermissionGroupName = "Usuários", PermissionName = "Inserir", Controller = "User", Action = "Create" });
            //    context.ApplicationPermissions.Add(new ApplicationPermission { ApplicationId = application.Id, PermissionGroupName = "Usuários", PermissionName = "Editar", PermissionCode = "User_Edit" });
            //    context.ApplicationPermissions.Add(new ApplicationPermission { ApplicationId = application.Id, PermissionGroupName = "Usuários", PermissionName = "Excluir", Controller = "User", Action = "Delete" });

            //    context.ApplicationPermissions.Add(new ApplicationPermission { ApplicationId = application.Id, PermissionGroupName = "Aplicações", PermissionName = "Listar", Controller = "Application", Action = "Index" });
            //    context.ApplicationPermissions.Add(new ApplicationPermission { ApplicationId = application.Id, PermissionGroupName = "Aplicações", PermissionName = "Visualizar", Controller = "Application", Action = "Details" });
            //    context.ApplicationPermissions.Add(new ApplicationPermission { ApplicationId = application.Id, PermissionGroupName = "Aplicações", PermissionName = "Inserir", Controller = "Application", Action = "Create" });
            //    context.ApplicationPermissions.Add(new ApplicationPermission { ApplicationId = application.Id, PermissionGroupName = "Aplicações", PermissionName = "Editar", Controller = "Application", Action = "Edit", PermissionCode = "Application_Edit" });
            //    context.ApplicationPermissions.Add(new ApplicationPermission { ApplicationId = application.Id, PermissionGroupName = "Aplicações", PermissionName = "Excluir", Controller = "Application", Action = "Delete" });

            //    context.SaveChanges();
            //}

            var roleManager = new ApplicationRoleManager(new ApplicationRoleStore(context));
            if (!roleManager.RoleExists(Default.AdminRoleName))
            {
                ApplicationRole adminRole = new ApplicationRole(Default.AdminRoleName);
                //adminRole.ApplicationId = application.Id;
                roleManager.Create(adminRole);
                roleManager.AddClaimAsync(adminRole, new Claim(Default.AdminClaimName, Default.AdminClaimName)).Wait();
            }

            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));
            var user = userManager.FindByName(Default.adminUsername);
            if (user == null)
            {
                user = new ApplicationUser { UserName = Default.adminUsername, Email = Default.adminEmail, Nome = Default.adminName };
                userManager.Create(user, "alah2882");
                userManager.AddToRole(user.Id, Default.AdminRoleName);
            };
        }
    }
}
