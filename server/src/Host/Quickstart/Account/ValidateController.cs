using Host.Extensions;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySql.AspNet.Identity;
using MySql.AspNet.Identity.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityServer4.Quickstart.UI
{
    [SecurityHeaders]
    public class ValidateController : Controller
    {
        private readonly UserStore<User> _userStore;
        private readonly ClientRelationRepository _clientRelationRepository;

        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEventService _events;
        private readonly AccountService _account;

        public ValidateController(
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IHttpContextAccessor httpContextAccessor,
            IEventService events)
        {
            _userStore = new UserStore<User>(Host.Startup.ConnectionString);
            _clientRelationRepository = new ClientRelationRepository(Host.Startup.ConnectionString);

            _interaction = interaction;
            _events = events;
            _account = new AccountService(interaction, httpContextAccessor, clientStore);
        }

        [HttpPost]
        public async Task<IActionResult> ValidatePermissionOnUrl(string sub, string uri, string domain, string scheme, string method, string from, string to)
        {
            //return new StatusCodeResult(200); //TODO: test

            if (string.IsNullOrEmpty(sub) || string.IsNullOrEmpty(uri) || string.IsNullOrEmpty(domain) || 
                string.IsNullOrEmpty(scheme) || string.IsNullOrEmpty(method) || string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
            {
                return new StatusCodeResult(500);
            }

            if (!from.Equals(to) && !IsRelationExists(from, to))
            {
                return new StatusCodeResult(403);
            }

            User user = await _userStore.FindByIdAsync(sub, CancellationToken.None);
            if (user == null)
            {
                return new StatusCodeResult(500);
            }
            if(!user.Activated || user.LockoutEnabled)
            {
                return new StatusCodeResult(403);
            }

            string userId = sub;
            string objectUrl = scheme + "://" + (domain.TrimEnd('/') + "/").Replace(":80/", "/").Replace(":443/", "/").TrimEnd('/');
            string endpointValue = "/" + uri.TrimStart('/').TrimEnd('/');
            string permissionName = method.ToUpper();

            if (_userStore.ValidatePermissionOnUrl(userId, objectUrl, endpointValue, permissionName))
            {
                return new StatusCodeResult(200);
            }

            return new StatusCodeResult(403);
        }

        private bool IsRelationExists(string from, string to)
        {
            return _clientRelationRepository.ClientRelationsCheck(from, to);
        }

    }
}