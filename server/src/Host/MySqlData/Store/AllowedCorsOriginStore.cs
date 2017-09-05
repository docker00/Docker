using Microsoft.AspNetCore.Identity;
using MySql.AspNet.Identity.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace MySql.AspNet.Identity
{
    public class AllowedCorsOriginsCustomStore
    {
        private readonly string _connectionString;
        private readonly AllowedCorsOriginRepository _allowedCorsOriginRepository;
        public AllowedCorsOriginsCustomStore()
            : this("DefaultConnection")
        {

        }

        public AllowedCorsOriginsCustomStore(string connectionString)
        {
            _connectionString = connectionString;
            _allowedCorsOriginRepository = new AllowedCorsOriginRepository(_connectionString);
        }

        public IQueryable<AllowedCorsOrigin> AllowedCorsOrigins
        {
            get
            {
                return _allowedCorsOriginRepository.GetAllowedCorsOrigins();
            }
        }

        public void Dispose()
        {
            // connection is automatically disposed
        }

        public Task<IdentityResult> CreateAsync(AllowedCorsOrigin allowedCorsOriginsCustom, CancellationToken cancellationToken)
        {
            if (allowedCorsOriginsCustom == null)
            {
                throw new ArgumentNullException("allowedCorsOriginsCustom");
            }

            _allowedCorsOriginRepository.Insert(allowedCorsOriginsCustom);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(AllowedCorsOrigin allowedCorsOriginsCustom, CancellationToken cancellationToken)
        {
            if (allowedCorsOriginsCustom == null)
            {
                throw new ArgumentNullException("allowedCorsOriginsCustom");
            }

            _allowedCorsOriginRepository.Update(allowedCorsOriginsCustom);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(AllowedCorsOrigin allowedCorsOriginsCustom, CancellationToken cancellationToken)
        {
            if (allowedCorsOriginsCustom == null)
            {
                throw new ArgumentNullException("allowedCorsOriginsCustom");
            }

            _allowedCorsOriginRepository.Delete(allowedCorsOriginsCustom.Id);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<AllowedCorsOrigin> FindByIdAsync(string allowedCorsOriginsCustomId, CancellationToken cancellationToken)
        {
            AllowedCorsOrigin result = _allowedCorsOriginRepository.GetAllowedCorsOriginsById(allowedCorsOriginsCustomId);

            return Task.FromResult<AllowedCorsOrigin>(result);
        }

        public Task<AllowedCorsOrigin> FindByNameAsync(string allowedCorsOriginsCustomName, CancellationToken cancellationToken)
        {
            AllowedCorsOrigin result = _allowedCorsOriginRepository.GetAllowedCorsOriginsByName(allowedCorsOriginsCustomName);
            return Task.FromResult(result);
        }
    }
}
