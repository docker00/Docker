using Microsoft.AspNetCore.Identity;
using MySql.AspNet.Identity.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace MySql.AspNet.Identity
{
    public class AllowedGrantTypeStore
    {
        private readonly string _connectionString;
        private readonly AllowedGrantTypeRepository _allowedGrantTypeRepository;
        public AllowedGrantTypeStore()
            : this("DefaultConnection")
        {

        }

        public AllowedGrantTypeStore(string connectionString)
        {
            _connectionString = connectionString;
            _allowedGrantTypeRepository = new AllowedGrantTypeRepository(_connectionString);
        }

        public IQueryable<AllowedGrantType> AllowedGrantTypes
        {
            get
            {
                return _allowedGrantTypeRepository.GetAllowedGrantTypes();
            }
        }

        public void Dispose()
        {
            // connection is automatically disposed
        }

        public Task<IdentityResult> CreateAsync(AllowedGrantType allowedCorsOriginsCustom, CancellationToken cancellationToken)
        {
            if (allowedCorsOriginsCustom == null)
            {
                throw new ArgumentNullException("allowedCorsOriginsCustom");
            }

            _allowedGrantTypeRepository.Insert(allowedCorsOriginsCustom);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(AllowedGrantType allowedCorsOriginsCustom, CancellationToken cancellationToken)
        {
            if (allowedCorsOriginsCustom == null)
            {
                throw new ArgumentNullException("allowedCorsOriginsCustom");
            }

            _allowedGrantTypeRepository.Update(allowedCorsOriginsCustom);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(AllowedGrantType allowedCorsOriginsCustom, CancellationToken cancellationToken)
        {
            if (allowedCorsOriginsCustom == null)
            {
                throw new ArgumentNullException("allowedCorsOriginsCustom");
            }

            _allowedGrantTypeRepository.Delete(allowedCorsOriginsCustom.Id);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<AllowedGrantType> FindByIdAsync(string allowedCorsOriginsCustomId, CancellationToken cancellationToken)
        {
            AllowedGrantType result = _allowedGrantTypeRepository.GetAllowedGrantTypeById(allowedCorsOriginsCustomId);

            return Task.FromResult<AllowedGrantType>(result);
        }

        public Task<AllowedGrantType> FindByNameAsync(string allowedCorsOriginsCustomName, CancellationToken cancellationToken)
        {
            AllowedGrantType result = _allowedGrantTypeRepository.GetAllowedGrantTypeByName(allowedCorsOriginsCustomName);
            return Task.FromResult(result);
        }
    }
}
