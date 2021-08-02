using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Pizzaria.Security.Data
{
    [Table("AspNetPermissions")]
    public class ApplicationPermission
    {
        [Key]
        public virtual int Id { get; set; }

        [Required]
        public virtual int ApplicationId { get; set; }

        [StringLength(500)]
        [Required]
        public virtual string PermissionGroupName { get; set; }

        [StringLength(500)]
        [Required]
        public virtual string PermissionName { get; set; }

        [StringLength(500)]
        public virtual string Controller { get; set; }

        [StringLength(500)]
        public virtual string Action { get; set; }

        [StringLength(100)]
        public virtual string PermissionCode { get; set; }

        [ForeignKey("ApplicationId")]
        public virtual Application Application { get; set; }
    }
}