using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Pizzaria.Security.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Pizzaria.Security.IdentityConfig
{
    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        protected virtual CancellationToken CancellationToken => CancellationToken.None;

        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new ApplicationRoleStore(context.Get<ApplicationDbContext>()));
        }

        public virtual async Task AddClaimAsync(ApplicationRole role, Claim claim)
        {
            var claimStore = GetClaimStore();
            if (claim == null)
            {
                throw new ArgumentNullException(nameof(claim));
            }
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            await claimStore.AddClaimAsync(role, claim, CancellationToken);
        }

        /// <summary>
        /// Removes a claim from a role.
        /// </summary>
        /// <param name="role">The role to remove the claim from.</param>
        /// <param name="claim">The claim to remove.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/>
        /// of the operation.
        /// </returns>
        public virtual async Task RemoveClaimAsync(ApplicationRole role, Claim claim)
        {
            var claimStore = GetClaimStore();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            await claimStore.RemoveClaimAsync(role, claim, CancellationToken);
        }

        public virtual async Task ClearClaimsAsync(ApplicationRole role)
        {
            var claimStore = GetClaimStore();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }

            await claimStore.ClearClaimsAsync(role, CancellationToken);
        }

        /// <summary>
        /// Gets a list of claims associated with the specified <paramref name="role"/>.
        /// </summary>
        /// <param name="role">The role whose claims should be returned.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the list of <see cref="Claim"/>s
        /// associated with the specified <paramref name="role"/>.
        /// </returns>
        public virtual async Task<IList<Claim>> GetClaimsAsync(ApplicationRole role)
        {
            var claimStore = GetClaimStore();
            if (role == null)
            {
                throw new ArgumentNullException(nameof(role));
            }
            return await claimStore.GetClaimsAsync(role, CancellationToken);
        }

        // IRoleClaimStore methods
        private ApplicationRoleStore GetClaimStore()
        {
            var cast = Store as ApplicationRoleStore;
            if (cast == null)
            {
                throw new NotSupportedException("Tipo Inválido ao obter Claims");
            }
            return cast;
        }
    }
}