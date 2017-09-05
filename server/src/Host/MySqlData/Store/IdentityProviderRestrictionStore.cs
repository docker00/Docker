using Microsoft.AspNetCore.Identity;
using MySql.AspNet.Identity.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace MySql.AspNet.Identity
{
    public class IdentityProviderRestrictionStore
    {
        private readonly string _connectionString;
        private readonly IdentityProviderRestrictionRepository _identityProviderRestrictionRepository;
        public IdentityProviderRestrictionStore()
            : this("DefaultConnection")
        {

        }

        public IdentityProviderRestrictionStore(string connectionString)
        {
            _connectionString = connectionString;
            _identityProviderRestrictionRepository = new IdentityProviderRestrictionRepository(_connectionString);
        }

        public IQueryable<IdentityProviderRestriction> IdentityProviderRestrictions
        {
            get
            {
                return _identityProviderRestrictionRepository.GetIdentityProviderRestrictions();
            }
        }

        public void Dispose()
        {
            // connection is automatically disposed
        }

        public Task<IdentityResult> CreateAsync(IdentityProviderRestriction allowedCorsOriginsCustom, CancellationToken cancellationToken)
        {
            if (allowedCorsOriginsCustom == null)
            {
                throw new ArgumentNullException("allowedCorsOriginsCustom");
            }

            _identityProviderRestrictionRepository.Insert(allowedCorsOriginsCustom);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(IdentityProviderRestriction allowedCorsOriginsCustom, CancellationToken cancellationToken)
        {
            if (allowedCorsOriginsCustom == null)
            {
                throw new ArgumentNullException("allowedCorsOriginsCustom");
            }

            _identityProviderRestrictionRepository.Update(allowedCorsOriginsCustom);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(IdentityProviderRestriction allowedCorsOriginsCustom, CancellationToken cancellationToken)
        {
            if (allowedCorsOriginsCustom == null)
            {
                throw new ArgumentNullException("allowedCorsOriginsCustom");
            }

            _identityProviderRestrictionRepository.Delete(allowedCorsOriginsCustom.Id);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityProviderRestriction> FindByIdAsync(string allowedCorsOriginsCustomId, CancellationToken cancellationToken)
        {
            IdentityProviderRestriction result = _identityProviderRestrictionRepository.GetIdentityProviderRestrictionById(allowedCorsOriginsCustomId);

            return Task.FromResult<IdentityProviderRestriction>(result);
        }

        public Task<IdentityProviderRestriction> FindByNameAsync(string allowedCorsOriginsCustomName, CancellationToken cancellationToken)
        {
            IdentityProviderRestriction result = _identityProviderRestrictionRepository.GetIdentityProviderRestrictionByName(allowedCorsOriginsCustomName);
            return Task.FromResult(result);
        }
    }
}
