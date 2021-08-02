using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Pizzaria.Security.Data
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }

        public ApplicationRole(string roleName) : base(roleName) { }

        //[Required]
        //public virtual int ApplicationId { get; set; }

        //[ForeignKey("ApplicationId")]
        //public virtual Application Application { get; set; }

        [NotMapped]
        public string NameWithoutApplication
        {
            get { return this.Name.Substring(this.Name.IndexOf("_") + 1); }
        }
    }
}