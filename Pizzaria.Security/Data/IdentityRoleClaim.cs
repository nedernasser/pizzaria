using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Pizzaria.Security.Data
{
    [Table("AspNetRoleClaims")]
    public class IdentityRoleClaim
    {
        [Key]
        public virtual int Id { get; set; }

        [StringLength(500)]
        public virtual string RoleId { get; set; }

        [StringLength(500)]
        public virtual string ClaimType { get; set; }

        [StringLength(500)]
        public virtual string ClaimValue { get; set; }

        [ForeignKey("RoleId")]
        [Required]
        public virtual ApplicationRole Role { get; set; }
    }
}