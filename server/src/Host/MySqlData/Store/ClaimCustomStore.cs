using Microsoft.AspNetCore.Identity;
using MySql.AspNet.Identity.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace MySql.AspNet.Identity
{
    public class ClaimCustomStore
    {
        private readonly string _connectionString;
        private readonly ClaimCustomRepository _claimCustomRepository;
        public ClaimCustomStore()
            : this("DefaultConnection")
        {

        }

        public ClaimCustomStore(string connectionString)
        {
            _connectionString = connectionString;
            _claimCustomRepository = new ClaimCustomRepository(_connectionString);
        }

        public IQueryable<ClaimCustom> ClaimCustoms
        {
            get
            {
                return _claimCustomRepository.GetClaimCustoms();
            }
        }

        public void Dispose()
        {
            // connection is automatically disposed
        }

        public Task<IdentityResult> CreateAsync(ClaimCustom claimCustom, CancellationToken cancellationToken)
        {
            if (claimCustom == null)
            {
                throw new ArgumentNullException("claimCustom");
            }
            ClaimCustom claimByName = _claimCustomRepository.GetClaimCustomByType(claimCustom.Type);
            if(claimByName != null)
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError() { Description = "Такое требование уже есть" }));
            }
            _claimCustomRepository.Insert(claimCustom);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(ClaimCustom claimCustom, CancellationToken cancellationToken)
        {
            if (claimCustom == null)
            {
                throw new ArgumentNullException("claimCustom");
            }
            ClaimCustom claim = _claimCustomRepository.GetClaimCustomByType(claimCustom.Type);
            if(claim != null && !claimCustom.Id.Equals(claim.Id))
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError() { Description = "Такое требование уже существует" }));
            }

            _claimCustomRepository.Update(claimCustom);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(ClaimCustom claimCustom, CancellationToken cancellationToken)
        {
            if (claimCustom == null)
            {
                throw new ArgumentNullException("claimCustom");
            }

            _claimCustomRepository.Delete(claimCustom.Id);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<ClaimCustom> FindByIdAsync(string claimCustomId, CancellationToken cancellationToken)
        {
            ClaimCustom result = _claimCustomRepository.GetClaimCustomById(claimCustomId);

            return Task.FromResult<ClaimCustom>(result);
        }

        public Task<ClaimCustom> FindByNameAsync(string claimCustomType, CancellationToken cancellationToken)
        {
            ClaimCustom result = _claimCustomRepository.GetClaimCustomByType(claimCustomType);
            return Task.FromResult(result);
        }
    }
}
