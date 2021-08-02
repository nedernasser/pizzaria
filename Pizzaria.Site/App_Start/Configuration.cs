using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Pizzaria.Site
{
    public static class Configuration
    {
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