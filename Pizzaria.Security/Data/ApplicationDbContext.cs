using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Pizzaria.Security.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext() : base("DefaultConnection", throwIfV1Schema: false)
        {
            
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.HasDefaultSchema("mysqldatabase1186");
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>().Property(x => x.PasswordHash).HasMaxLength(500);
            modelBuilder.Entity<ApplicationUser>().Property(x => x.SecurityStamp).HasMaxLength(500);
            modelBuilder.Entity<ApplicationUser>().Property(x => x.PhoneNumber).HasMaxLength(500);

            modelBuilder.Entity<IdentityUserClaim>().Property(x => x.ClaimType).HasMaxLength(500);
            modelBuilder.Entity<IdentityUserClaim>().Property(x => x.ClaimValue).HasMaxLength(500);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<IdentityRoleClaim> RoleClaims { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<ApplicationPermission> ApplicationPermissions { get; set; }
    }
}