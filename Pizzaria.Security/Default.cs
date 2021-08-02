using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pizzaria.Security
{
    public static class Default
    {
        public const string OwnerRoleName = "Owner";
        public const string AdminRoleName = "Administrador";
        public const string AdminClaimName = "SystemAdmin";

        public const string adminUsername = "ADNeder";
        public const string adminName = "Neder Nasser";
        public const string adminEmail = "nedernasser@gmail.com";

        public static string ApplicationName
        {
            get
            {
                string appName = string.Empty;
                if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("ApplicationName")))
                    appName = ConfigurationManager.AppSettings.Get("ApplicationName");

                return appName;
            }
        }
    }
}
