using Microsoft.AspNetCore.Identity;
using MySql.AspNet.Identity.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace MySql.AspNet.Identity
{
    public class IdentityResourceCustomStore
    {
        private readonly string _connectionString;
        private readonly IdentityResourceCustomRepository _identityResourceCustomRepository;
        private readonly IdentityResourceCustomClaimRepository _identityResourceCustomClaimRepository;

        public IdentityResourceCustomStore()
            : this("DefaultConnection")
        {

        }

        public IdentityResourceCustomStore(string connectionString)
        {
            _connectionString = connectionString;
            _identityResourceCustomRepository = new IdentityResourceCustomRepository(_connectionString);
            _identityResourceCustomClaimRepository = new IdentityResourceCustomClaimRepository(_connectionString);
        }

        public IQueryable<IdentityResourceCustom> IdentityResourceCustoms
        {
            get
            {
                return _identityResourceCustomRepository.GetIdentityResources();
            }
        }

        public void Dispose()
        {
            // connection is automatically disposed
        }

        public Task<IdentityResult> CreateAsync(IdentityResourceCustom identityResourceCustom, CancellationToken cancellationToken)
        {
            if (identityResourceCustom == null)
            {
                throw new ArgumentNullException("IdentityResourceCustom");
            }

            _identityResourceCustomRepository.Insert(identityResourceCustom);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> CreateClaimAsync(string identityResourceId, string claimId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(identityResourceId) || string.IsNullOrEmpty(claimId))
            {
                throw new ArgumentNullException("identityResourceCustomClaim");
            }

            _identityResourceCustomClaimRepository.Insert(identityResourceId, claimId);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(IdentityResourceCustom identityResourceCustom, CancellationToken cancellationToken)
        {
            if (identityResourceCustom == null)
            {
                throw new ArgumentNullException("identityResourceCustom");
            }

            _identityResourceCustomRepository.Update(identityResourceCustom);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateClaimsAsync(string identityResourceCustomId, List<string> claims_ids, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(identityResourceCustomId))
            {
                throw new ArgumentException("identityResourceCustomId");
            }
            if(claims_ids == null)
            {
                claims_ids = new List<string>();
            }
            List<ClaimCustom> old_claims =
                _identityResourceCustomClaimRepository.PopulateIdentityResourceCustomClaims(identityResourceCustomId);
            foreach(ClaimCustom claim in old_claims)
            {
                if (!claims_ids.Contains(claim.Id))
                {
                    _identityResourceCustomClaimRepository.Delete(identityResourceCustomId, claim.Id);
                }
                else
                {
                    claims_ids.Remove(claim.Id);
                }
            }
            foreach(string claim_id in claims_ids)
            {
                _identityResourceCustomClaimRepository.Insert(identityResourceCustomId, claim_id);
            }
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(IdentityResourceCustom identityResourceCustom, CancellationToken cancellationToken)
        {
            if (identityResourceCustom == null)
            {
                throw new ArgumentNullException("identityResourceCustom");
            }

            _identityResourceCustomRepository.Delete(identityResourceCustom.Id);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteClaimAsync(IdentityResourceCustomClaim identityResourceCustom, CancellationToken cancellationToken)
        {
            if (identityResourceCustom == null)
            {
                throw new ArgumentNullException("identityResourceCustom");
            }

            _identityResourceCustomClaimRepository.Delete(identityResourceCustom.IdentityResourceId, identityResourceCustom.ClaimId);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResourceCustom> FindByIdAsync(string identityResourceCustomId, CancellationToken cancellationToken)
        {
            IdentityResourceCustom result = _identityResourceCustomRepository.GetIdentityResourceCustomById(identityResourceCustomId);

            return Task.FromResult<IdentityResourceCustom>(result);
        }

        public Task<IdentityResourceCustom> FindByNameAsync(string identityResourceCustomName, CancellationToken cancellationToken)
        {
            IdentityResourceCustom result = _identityResourceCustomRepository.GetIdentityResourceCustomByName(identityResourceCustomName);
            return Task.FromResult(result);
        }

        public Task<IQueryable<ClaimCustom>> FindClaimsAsync(string identityResourceId, CancellationToken cancellationToken)
        {
            IQueryable<ClaimCustom> claims =
                _identityResourceCustomClaimRepository.PopulateIdentityResourceCustomClaims(identityResourceId).AsQueryable();
            return Task.FromResult(claims);
        }
    }
}
