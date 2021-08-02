using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Pizzaria.Security.Data
{
    [Table("AspNetApplications")]
    public class Application
    {
        [Key]
        public virtual int Id { get; set; }

        [StringLength(500)]
        [Required]
        public virtual string ApplicationName { get; set; }

        public List<ApplicationPermission> Permissions { get; set; }
    }
}