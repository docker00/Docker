using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySql.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using System.Security.Claims;
using Host.Extensions;
using MySql.AspNet.Identity.Repositories;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;

namespace IdentityServer4.Quickstart.UI
{
    [Authorize]
    [Menu(Name = "Профиль", Description = "Управление профилем", GroupName = "Manage", Weight = -1)]
    [SecurityHeaders]
    public class ManageApiController : Controller
    {
        private readonly UserStore<User> _users;
        private readonly ClientCustomStore _clients;
        public ManageApiController(UserStore<User> users = null, ClientCustomStore clients = null)
        {
            _users = users ?? new UserStore<User>(Host.Startup.ConnectionString);
            _clients = clients ?? new ClientCustomStore(Host.Startup.ConnectionString);
        }

        #region ApiKey
        [Menu(Relation = "_ApiKeysJoinPartial")]
        public async Task<IActionResult> ApiKeyData()
        {
            string userId = "";
            ClaimsPrincipal claimsPrincipal = await HttpContext.GetIdentityServerUserAsync();
            if (claimsPrincipal != null)
            {
                userId = claimsPrincipal.GetSubjectId();
            }

            List<UserApiKeyViewModel> apiKeysViewList = new List<UserApiKeyViewModel>();
            if (string.IsNullOrEmpty(userId))
            {
                return Json(apiKeysViewList);
            }
            
            User user = await _users.FindByIdAsync(userId, CancellationToken.None);
            if (user != null)
            {
                IQueryable<UserApiKey> apiKeys = await _users.GetUserApiKeys(user, CancellationToken.None);
                foreach (UserApiKey key in apiKeys)
                {
                    ClientCustom client = await _clients.FindByIdAsync(key.ClientId, CancellationToken.None);
                    if (client != null)
                    {
                        apiKeysViewList.Add(new UserApiKeyViewModel(key.Id, key.UserId, client.ClientName, key.ApiKey, key.ExperienceTime));
                    }
                }
            }
            return Json(apiKeysViewList);
        }

        [Menu(Name = "Добавление ключей API", Description = "Добавление ключей API", GroupName = "Manage")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _ApiKeyAddPartial(UserApiKeyInputModel model)
        {
            string userId = "";
            ClaimsPrincipal claimsPrincipal = await HttpContext.GetIdentityServerUserAsync();
            if (claimsPrincipal != null)
            {
                userId = claimsPrincipal.GetSubjectId();
            }
            if (string.IsNullOrEmpty(userId))
            {
                return PartialView(model);
            }

            User user = await _users.FindByIdAsync(userId, CancellationToken.None);
            if (user == null)
            {
                ModelState.AddModelError("UserId", "Пользователь не найден");
                return PartialView(model);
            }
            ClientCustom client = await _clients.FindByIdAsync(model.ClientId, CancellationToken.None);
            if (client == null)
            {
                ModelState.AddModelError("ClientId", "Клиент не найден");
                return PartialView(model);
            }
            if (!string.IsNullOrEmpty(model.ClientId) && model.ExperienceTime != null)
            {
                UserApiKey apiKey = new UserApiKey(userId, model.ClientId, model.ExperienceTime);
                IdentityResult result = await _users.CreateApiKey(apiKey, CancellationToken.None);
                if (result != null && result.Succeeded)
                {
                    model.ClientId = string.Empty;
                    return PartialView(model);
                }
                AddErrors(result);
            }
            return PartialView(model);
        }

        [Menu(Name = "Обновление ключей API", Description = "Обновление ключей API", GroupName = "Manage")]
        [HttpGet]
        public async Task<IActionResult> ApiKeyUpdate(string apiKeyId)
        {
            string userId = "";
            ClaimsPrincipal claimsPrincipal = await HttpContext.GetIdentityServerUserAsync();
            if (claimsPrincipal != null)
            {
                userId = claimsPrincipal.GetSubjectId();
            }
            if (string.IsNullOrEmpty(userId))
            {
                return Json(false);
            }

            UserApiKey key = await _users.FindApiKeyAsync(apiKeyId, CancellationToken.None);
            if (key == null || (key != null && !key.UserId.Equals(userId)))
            {
                return Json(false);
            }
            IdentityResult result = await _users.UpdateApiKeyAsync(new UserApiKey(key.Id, key.UserId, key.ClientId, key.ExperienceTime), CancellationToken.None);
            if (result.Succeeded)
            {
                return Json(true);
            }
            return Json(false);
        }

        [Menu(Name = "Удаление ключей API", Description = "Удаление ключей API", GroupName = "Manage")]
        [HttpGet]
        public async Task<IActionResult> DeleteApiKey(string apiKeyId)
        {
            string userId = "";
            ClaimsPrincipal claimsPrincipal = await HttpContext.GetIdentityServerUserAsync();
            if (claimsPrincipal != null)
            {
                userId = claimsPrincipal.GetSubjectId();
            }
            if (string.IsNullOrEmpty(userId))
            {
                return Json(false);
            }
            UserApiKey key = await _users.FindApiKeyAsync(apiKeyId, CancellationToken.None);
            if (key == null || (key != null && !key.UserId.Equals(userId)))
            {
                return Json(false);
            }
            
            IdentityResult result = await _users.DeleteApiKeyAsync(key, CancellationToken.None);
            if (result.Succeeded)
            {
                return Json(true);
            }
            return Json(false);
        }

        [Menu(Relation = "_ApiKeysJoinPartial")]
        public async Task<IActionResult> ClientListData()
        {
            string userId = "";
            ClaimsPrincipal claimsPrincipal = await HttpContext.GetIdentityServerUserAsync();
            if (claimsPrincipal != null)
            {
                userId = claimsPrincipal.GetSubjectId();
            }

            List<ClientViewModel> clientViewList = new List<ClientViewModel>();
            if (string.IsNullOrEmpty(userId))
            {
                return Json(clientViewList);
            }

            List<UserApiKey> apiKeys = new List<UserApiKey>();
            User user = await _users.FindByIdAsync(userId, CancellationToken.None);
            if (user != null)
            {
                apiKeys = (await _users.GetUserApiKeys(user, CancellationToken.None)).ToList();
            }
            foreach (ClientCustom client in _clients.ClientCustoms)
            {
                if (apiKeys.FirstOrDefault(ak => ak.ClientId.ToLower().Equals(client.Id)) == null)
                {
                    clientViewList.Add(new ClientViewModel(client));
                }
            }
            return Json(clientViewList);
        }

        [Menu(Name = "Список ключей API", Description = "Список ключей API", GroupName = "Manage")]
        [HttpGet]
        public IActionResult _ApiKeysJoinPartial()//(string userId)
        {
            //ViewData["UserId"] = userId;
            return PartialView();
        }
        #endregion

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
