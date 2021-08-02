using Microsoft.AspNet.Identity.EntityFramework;
using Pizzaria.Security.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Pizzaria.Security.IdentityConfig
{
    public class ApplicationRoleStore : RoleStore<ApplicationRole>
    {
        public ApplicationRoleStore(DbContext context) : base(context) { }

        /// <summary>
        /// Get the claims associated with the specified <paramref name="role"/> as an asynchronous operation.
        /// </summary>
        /// <param name="role">The role whose claims should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that contains the claims granted to a role.</returns>
        public async virtual Task<IList<Claim>> GetClaimsAsync(ApplicationRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            var result = await RoleClaims.Where(rc => rc.RoleId.Equals(role.Id)).ToListAsync(cancellationToken);
            return result.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
        }

        /// <summary>
        /// Adds the <paramref name="claim"/> given to the specified <paramref name="role"/>.
        /// </summary>
        /// <param name="role">The role to add the claim to.</param>
        /// <param name="claim">The claim to add to the role.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual async Task AddClaimAsync(ApplicationRole role, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }

            RoleClaims.Add(CreateRoleClaim(role, claim));

            await Context.SaveChangesAsync();
        }

        /// <summary>
        /// Removes the <paramref name="claim"/> given from the specified <paramref name="role"/>.
        /// </summary>
        /// <param name="role">The role to remove the claim from.</param>
        /// <param name="claim">The claim to remove from the role.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public async virtual Task RemoveClaimAsync(ApplicationRole role, Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }
            var claims = await RoleClaims.Where(rc => rc.RoleId.Equals(role.Id) && rc.ClaimValue == claim.Value && rc.ClaimType == claim.Type).ToListAsync(cancellationToken);
            foreach (var c in claims)
            {
                RoleClaims.Remove(c);
            }

            await Context.SaveChangesAsync();
        }

        public async virtual Task ClearClaimsAsync(ApplicationRole role, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            var claims = await RoleClaims.Where(rc => rc.RoleId.Equals(role.Id)).ToListAsync(cancellationToken);
            foreach (var c in claims)
            {
                RoleClaims.Remove(c);
            }

            await Context.SaveChangesAsync();
        }

        private DbSet<IdentityRoleClaim> RoleClaims { get { return Context.Set<IdentityRoleClaim>(); } }

        /// <summary>
        /// Creates a entity representing a role claim.
        /// </summary>
        /// <param name="role">The associated role.</param>
        /// <param name="claim">The associated claim.</param>
        /// <returns>The role claim entity.</returns>
        protected virtual IdentityRoleClaim CreateRoleClaim(ApplicationRole role, Claim claim)
            => new IdentityRoleClaim { RoleId = role.Id, ClaimType = claim.Type, ClaimValue = claim.Value };
    }
}