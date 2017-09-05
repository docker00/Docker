using Microsoft.AspNetCore.Identity;
using MySql.AspNet.Identity.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace MySql.AspNet.Identity
{
    public class ProfileAttributeStore<TProfileAttribute> where TProfileAttribute : ProfileAttribute
    {
        private readonly string _connectionString;
        private readonly ProfileAttributeRepository _profileAttributeRepository;
        private readonly ProfileAttributeClaimRepository _profileAttributeClaimRepository;
        public ProfileAttributeStore()
            : this("DefaultConnection")
        {

        }

        public ProfileAttributeStore(string connectionString)
        {
            _connectionString = connectionString;
            _profileAttributeRepository = new ProfileAttributeRepository(_connectionString);
            _profileAttributeClaimRepository = new ProfileAttributeClaimRepository(_connectionString);
        }

        public IQueryable<TProfileAttribute> ProfileAttributes
        {
            get
            {
                return _profileAttributeRepository.GetProfileAttributes() as IQueryable<TProfileAttribute>;
            }
        }

        public IQueryable<ProfileAttributeClaim> ProfileAttributesClaims
        {
            get
            {
                return _profileAttributeClaimRepository.GetAll();
            }
        }

        public void Dispose()
        {
            // connection is automatically disposed
        }

        public Task<IdentityResult> CreateAsync(TProfileAttribute profileAttribute, CancellationToken cancellationToken)
        {
            if (profileAttribute == null)
            {
                throw new ArgumentNullException("profileAttribute");
            }

            _profileAttributeRepository.Insert(profileAttribute);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(TProfileAttribute profileAttribute, CancellationToken cancellationToken)
        {
            if (profileAttribute == null)
            {
                throw new ArgumentNullException("profileAttribute");
            }

            _profileAttributeRepository.Update(profileAttribute);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(TProfileAttribute profileAttribute, CancellationToken cancellationToken)
        {
            if (profileAttribute == null)
            {
                throw new ArgumentNullException("profileAttribute");
            }

            _profileAttributeRepository.Delete(profileAttribute.Id);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<ProfileAttribute> GetProfileAttributeIdAsync(TProfileAttribute profileAttribute, CancellationToken cancellationToken)
        {
            if (profileAttribute == null)
            {
                return Task.FromResult<ProfileAttribute>(null);
            }

            ProfileAttribute result = _profileAttributeRepository.GetProfileAttributeById(profileAttribute.Id);

            return Task.FromResult<ProfileAttribute>(result);
        }

        public Task<ProfileAttribute> GetProfileAttributeNameAsync(TProfileAttribute profileAttribute, CancellationToken cancellationToken)
        {
            if (profileAttribute == null)
            {
                return Task.FromResult<ProfileAttribute>(null);
            }

            ProfileAttribute result = _profileAttributeRepository.GetProfileAttributeByName(profileAttribute.Name);
            return Task.FromResult(result);
        }

        public Task<IQueryable<ProfileAttributeClaim>> GetUserProfileAttributeClaim(string userId, CancellationToken cancellationToken)
        {
            IQueryable<ProfileAttributeClaim> profileAttributes = _profileAttributeClaimRepository.PopulateUserProfileAttributeClaim(userId);
            return Task.FromResult(profileAttributes);
        }

        public Task<ProfileAttribute> FindByIdAsync(string profileAttributeId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(profileAttributeId))
            {
                return Task.FromResult<ProfileAttribute>(null);
            }

            ProfileAttribute result = _profileAttributeRepository.GetProfileAttributeById(profileAttributeId);

            return Task.FromResult(result);
        }

        public Task<ProfileAttribute> FindByNameAsync(string profileAttributeName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(profileAttributeName))
            {
                return Task.FromResult<ProfileAttribute>(null);
            }

            ProfileAttribute result = _profileAttributeRepository.GetProfileAttributeByName(profileAttributeName);
            return Task.FromResult(result);
        }

        public Task<IdentityResult> CreateClaimAsync(string profileAttributeId, string claimId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(profileAttributeId))
            {
                throw new ArgumentNullException("profileAttributeId");
            }

            if (string.IsNullOrEmpty(claimId))
            {
                throw new ArgumentNullException("claimId");
            }

            _profileAttributeClaimRepository.Insert(profileAttributeId, claimId);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateClaimAsync(string profileAttributeId, string claimId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(profileAttributeId))
            {
                throw new ArgumentNullException("profileAttributeId");
            }

            if (string.IsNullOrEmpty(claimId))
            {
                throw new ArgumentNullException("claimId");
            }

            _profileAttributeClaimRepository.Update(profileAttributeId, claimId);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<ProfileAttributeClaim> GetClaimAsync(string profileAttributeId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(profileAttributeId))
            {
                throw new ArgumentNullException("profileAttributeId");
            }

            ProfileAttributeClaim profileAttributeClaim = _profileAttributeClaimRepository.GetByProfileAttributeId(profileAttributeId);
            return Task.FromResult(profileAttributeClaim);
        }

        public Task<IdentityResult> DeleteClaimByProfileAttributeIdAsync(string profileAttributeId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(profileAttributeId))
            {
                throw new ArgumentNullException("profileAttributeId");
            }

            _profileAttributeClaimRepository.DeleteByProfileAttributeId(profileAttributeId);

            return Task.FromResult(IdentityResult.Success);
        }
    }
}
