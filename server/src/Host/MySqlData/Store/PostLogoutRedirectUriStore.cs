using Microsoft.AspNetCore.Identity;
using MySql.AspNet.Identity.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace MySql.AspNet.Identity
{
    public class PostLogoutRedirectUriStore
    {
        private readonly string _connectionString;
        private readonly PostLogoutRedirectUriRepository _postLogoutRedirectUriRepository;
        public PostLogoutRedirectUriStore()
            : this("DefaultConnection")
        {

        }

        public PostLogoutRedirectUriStore(string connectionString)
        {
            _connectionString = connectionString;
            _postLogoutRedirectUriRepository = new PostLogoutRedirectUriRepository(_connectionString);
        }

        public IQueryable<PostLogoutRedirectUri> PostLogoutRedirectUris
        {
            get
            {
                return _postLogoutRedirectUriRepository.GetPostLogoutRedirectUris();
            }
        }

        public void Dispose()
        {
            // connection is automatically disposed
        }

        public Task<IdentityResult> CreateAsync(PostLogoutRedirectUri postLogoutRedirectUrisCustom, CancellationToken cancellationToken)
        {
            if (postLogoutRedirectUrisCustom == null)
            {
                throw new ArgumentNullException("postLogoutRedirectUrisCustom");
            }

            _postLogoutRedirectUriRepository.Insert(postLogoutRedirectUrisCustom);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(PostLogoutRedirectUri postLogoutRedirectUrisCustom, CancellationToken cancellationToken)
        {
            if (postLogoutRedirectUrisCustom == null)
            {
                throw new ArgumentNullException("postLogoutRedirectUrisCustom");
            }

            _postLogoutRedirectUriRepository.Update(postLogoutRedirectUrisCustom);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(PostLogoutRedirectUri postLogoutRedirectUrisCustom, CancellationToken cancellationToken)
        {
            if (postLogoutRedirectUrisCustom == null)
            {
                throw new ArgumentNullException("postLogoutRedirectUrisCustom");
            }

            _postLogoutRedirectUriRepository.Delete(postLogoutRedirectUrisCustom.Id);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<PostLogoutRedirectUri> FindByIdAsync(string postLogoutRedirectUrisCustomId, CancellationToken cancellationToken)
        {
            PostLogoutRedirectUri result = _postLogoutRedirectUriRepository.GetPostLogoutRedirectUriById(postLogoutRedirectUrisCustomId);

            return Task.FromResult<PostLogoutRedirectUri>(result);
        }

        public Task<PostLogoutRedirectUri> FindByNameAsync(string postLogoutRedirectUrisCustomName, CancellationToken cancellationToken)
        {
            PostLogoutRedirectUri result = _postLogoutRedirectUriRepository.GetPostLogoutRedirectUriByName(postLogoutRedirectUrisCustomName);
            return Task.FromResult(result);
        }

        public Task<KeyValuePair<int, IQueryable<PostLogoutRedirectUri>>> GetRedirectQueryFormatter(string order, int limit, int offset, string sort, string search)
        {
            KeyValuePair<int, IQueryable<PostLogoutRedirectUri>> redirects = _postLogoutRedirectUriRepository.GetRedirectsQueryFormatter(order, limit, offset, sort, search);
            return Task.FromResult(redirects);
        }
    }
}
