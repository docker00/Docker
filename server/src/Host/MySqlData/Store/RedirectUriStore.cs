using Microsoft.AspNetCore.Identity;
using MySql.AspNet.Identity.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace MySql.AspNet.Identity
{
    public class RedirectUriStore
    {
        private readonly string _connectionString;
        private readonly RedirectUriRepository _redirectUriRepository;
        public RedirectUriStore()
            : this("DefaultConnection")
        {

        }

        public RedirectUriStore(string connectionString)
        {
            _connectionString = connectionString;
            _redirectUriRepository = new RedirectUriRepository(_connectionString);
        }

        public IQueryable<RedirectUri> RedirectUris
        {
            get
            {
                return _redirectUriRepository.GetRedirectUris();
            }
        }

        public void Dispose()
        {
            // connection is automatically disposed
        }

        public Task<IdentityResult> CreateAsync(RedirectUri redirectUrisCustom, CancellationToken cancellationToken)
        {
            if (redirectUrisCustom == null)
            {
                throw new ArgumentNullException("redirectUrisCustom");
            }

            _redirectUriRepository.Insert(redirectUrisCustom);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(RedirectUri redirectUrisCustom, CancellationToken cancellationToken)
        {
            if (redirectUrisCustom == null)
            {
                throw new ArgumentNullException("redirectUrisCustom");
            }

            _redirectUriRepository.Update(redirectUrisCustom);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(RedirectUri redirectUrisCustom, CancellationToken cancellationToken)
        {
            if (redirectUrisCustom == null)
            {
                throw new ArgumentNullException("redirectUrisCustom");
            }

            _redirectUriRepository.Delete(redirectUrisCustom.Id);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<RedirectUri> FindByIdAsync(string redirectUrisCustomId, CancellationToken cancellationToken)
        {
            RedirectUri result = _redirectUriRepository.GetRedirectUriById(redirectUrisCustomId);

            return Task.FromResult<RedirectUri>(result);
        }

        public Task<RedirectUri> FindByNameAsync(string redirectUrisCustomName, CancellationToken cancellationToken)
        {
            RedirectUri result = _redirectUriRepository.GetRedirectUriByName(redirectUrisCustomName);
            return Task.FromResult(result);
        }

        public Task<KeyValuePair<int, IQueryable<RedirectUri>>> GetRedirectQueryFormatter(string order, int limit, int offset, string sort, string search)
        {
            KeyValuePair<int, IQueryable<RedirectUri>> redirects = _redirectUriRepository.GetRedirectsQueryFormatter(order, limit, offset, sort, search);
            return Task.FromResult(redirects);
        }

    }
}
