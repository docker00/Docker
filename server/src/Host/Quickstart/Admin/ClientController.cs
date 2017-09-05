using Host.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySql.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityServer4.Quickstart.UI
{
    [Menu(Name = "Клиенты", Description = "Управление клиентами", GroupName = "Client", ShowInMenu = true, Weight = 5)]
    public class ClientController : Controller
    {
        private readonly ClientCustomStore _clients;
        private readonly AllowedCorsOriginsCustomStore _cors;
        private readonly AllowedGrantTypeStore _grants;
        private readonly PostLogoutRedirectUriStore _logoutRedirectUris;
        private readonly RedirectUriStore _redirectUris;
        private readonly SecretCustomStore _secrets;
        private readonly ObjectStore _objects;
        private readonly IdentityResourceCustomStore _identityResources;
        private readonly IdentityProviderRestrictionStore _providerRestructions;

        public ClientController(
            ClientCustomStore clients = null,
            AllowedCorsOriginsCustomStore cors = null,
            AllowedGrantTypeStore grants = null,
            PostLogoutRedirectUriStore logoutRedirectUris = null,
            RedirectUriStore redirectUris = null,
            SecretCustomStore secrets = null,
            ObjectStore objects = null,
            IdentityResourceCustomStore identityResources = null)
        {
            _clients = clients ?? new ClientCustomStore(Host.Startup.ConnectionString);
            _cors = cors ?? new AllowedCorsOriginsCustomStore(Host.Startup.ConnectionString);
            _grants = grants ?? new AllowedGrantTypeStore(Host.Startup.ConnectionString);
            _logoutRedirectUris = logoutRedirectUris ?? new PostLogoutRedirectUriStore(Host.Startup.ConnectionString);
            _redirectUris = redirectUris ?? new RedirectUriStore(Host.Startup.ConnectionString);
            _secrets = secrets ?? new SecretCustomStore(Host.Startup.ConnectionString);
            _objects = objects ?? new ObjectStore(Host.Startup.ConnectionString);
            _identityResources = identityResources ?? new IdentityResourceCustomStore(Host.Startup.ConnectionString);
            _providerRestructions = new IdentityProviderRestrictionStore(Host.Startup.ConnectionString);
        }

        #region Client

        [Menu(Name = "Список клиентов", Description = "Список клиентов", GroupName = "Client", ShowInMenu = true, Weight = 1)]
        [HttpGet]
        public IActionResult List()
        {
            return View();
        }

        [Menu(Relation = "List")]
        [HttpGet]
        public async Task<IActionResult> ClientsData(string order, int limit, int offset, string sort, string search)
        {
            KeyValuePair<int, IQueryable<ClientCustom>> clients = await _clients.GetClientsQueryFormatterAsync(order, limit, offset, sort, search);
            List<ClientViewModel> secretsView = new List<ClientViewModel>();
            foreach (ClientCustom client in clients.Value)
            {
                secretsView.Add(new ClientViewModel(client));

            }
            return Json(new { total = clients.Key, rows = secretsView });
        }

        [Menu(Name = "Подробнее об клиенте", Description = "Детальная информация клиента", GroupName = "Client")]
        [HttpGet]
        public async Task<IActionResult> Details(string clientId)
        {
            ClientCustom client = await _clients.FindByIdAsync(clientId, CancellationToken.None);
            if (client != null)
            {
                ClientViewModel clientView = new ClientViewModel(client);
                IQueryable<IdentityProviderRestriction> restrictions = await _clients.FindIdentityProviderRestrictionAsync(clientId, CancellationToken.None);
                foreach (IdentityProviderRestriction restiction in restrictions)
                {
                    clientView.IdentityProviderRestrictions.Add(new IdentityProviderRestrictionViewModel(restiction.Id, restiction.Name));
                }
                IQueryable<AllowedCorsOrigin> cors = await _clients.FindAllowerCorsOriginAsync(clientId, CancellationToken.None);
                foreach (AllowedCorsOrigin cor in cors)
                {
                    clientView.AllowedCorsOrigins.Add(new AllowedCorsOriginViewModel(cor.Id, cor.Name));
                }
                IQueryable<AllowedGrantType> grants = await _clients.FindAllowedGrantTypeAsync(clientId, CancellationToken.None);
                foreach (AllowedGrantType grant in grants)
                {
                    clientView.AllowedGrantTypes.Add(new AllowedGrantTypeViewModel(grant.Id, grant.Name));
                }
                clientView.AllowedScopes = client.AllowedScopes.Where(s => !s.Equals("access_token_api")).ToList();
                IQueryable <PostLogoutRedirectUri> logoutRedirectUris = await _clients.FindPostLogoutRedirectUrisAsync(clientId, CancellationToken.None);
                foreach (PostLogoutRedirectUri logoutRedirectUri in logoutRedirectUris)
                {
                    clientView.PostLogoutRedirectUris.Add(new PostLogoutRedirectUriViewModel()
                    {
                        Id = logoutRedirectUri.Id,
                        Name = logoutRedirectUri.Name
                    });
                }
                IQueryable<RedirectUri> redirectUris = await _clients.FindRedirectUrisAsync(clientId, CancellationToken.None);
                foreach (RedirectUri redirectUri in redirectUris)
                {
                    clientView.RedirectUris.Add(new RedirectUriViewModel()
                    {
                        Id = redirectUri.Id,
                        Name = redirectUri.Name
                    });
                }
                foreach (SecretCustom secret in client.ClientSecrets)
                {
                    clientView.Secrets.Add(new SecretViewModel()
                    {
                        Id = secret.Id,
                        Type = secret.Type,
                        Value = secret.Value
                    });
                }
                Object _clientObject = await _objects.FindByClientIdAsync(clientId, CancellationToken.None);
                if (_clientObject != null)
                {
                    clientView.ObjectId = _clientObject.Id;
                }

                return View(clientView);
            }
            return RedirectToAction("List");
        }

        [Menu(Relation = "Details")]
        public async Task<IActionResult> _DetailsPartial(string clientId)
        {
            ClientCustom client = await _clients.FindByIdAsync(clientId, CancellationToken.None);
            ClientViewModel clientView = new ClientViewModel(client);
            return PartialView(clientView);
        }

        [Menu(Name = "Добавление клиента", Description = "Добавить клиента", GroupName = "Client", ShowInMenu = true, Weight = 0)]
        [HttpGet]
        public async Task<IActionResult> Add(string objectId = null)
        {
            ClientInputModel client = new ClientInputModel();
            if (!string.IsNullOrEmpty(objectId))
            {
                Object _object = await _objects.FindByIdAsync(objectId, CancellationToken.None);
                if (_object != null)
                {
                    client.ClientName = _object.Name;
                    client.ClientUri = _object.Url;
                    client.ObjectId = objectId;
                }
            }
            ViewBag.ProtocolType = Constants.ProtocolTypes;
            return View(client);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ClientInputModel model)
        {
            if (ModelState.IsValid)
            {
                ClientCustom clientByName = await _clients.FindByNameAsync(model.ClientId, CancellationToken.None);
                if (clientByName == null)
                {
                    ClientCustom client = new ClientCustom()
                    {
                        Id = model.Id,
                        ClientId = model.ClientId,
                        ProtocolType = model.ProtocolType,
                        RequireClientSecret = model.RequireClientSecret,
                        ClientName = model.ClientName,
                        ClientUri = model.ClientUri,
                        LogoUri = model.LogoUri,
                        RequireConsent = model.RequireConsent,
                        AllowRememberConsent = model.AllowRememberConsent,
                        RequirePkce = model.RequirePkce,
                        AllowPlainTextPkce = model.AllowPlainTextPkce,
                        AllowAccessTokensViaBrowser = model.AllowAccessTokensViaBrowser,
                        LogoutUri = model.LogoutUri,
                        LogoutSessionRequired = model.LogoutSessionRequired,
                        AllowOfflineAccess = model.AllowOfflineAccess,
                        AlwaysIncludeUserClaimsInIdToken = model.AlwaysIncludeUserClaimsInIdToken,
                        IdentityTokenLifetime = model.IdentityTokenLifetime,
                        AccessTokenLifetime = model.AccessTokenLifetime,
                        AuthorizationCodeLifetime = model.AuthorizationCodeLifetime,
                        AbsoluteRefreshTokenLifetime = model.AbsoluteRefreshTokenLifetime,
                        SlidingRefreshTokenLifetime = model.SlidingRefreshTokenLifetime,
                        RefreshTokenUsage = model.RefreshTokenUsage,
                        UpdateAccessTokenClaimsOnRefresh = model.UpdateAccessTokenClaimsOnRefresh,
                        RefreshTokenExpiration = model.RefreshTokenExpiration,
                        AccessTokenType = model.AccessTokenType,
                        EnableLocalLogin = model.EnableLocalLogin,
                        IncludeJwtId = model.IncludeJwtId,
                        AlwaysSendClientClaims = model.AlwaysSendClientClaims,
                        PrefixClientClaims = model.PrefixClientClaims,
                        Enabled = model.Enabled
                    };
                    IdentityResult result = await _clients.CreateAsync(client, CancellationToken.None);
                    if (result.Succeeded)
                    {
                        if (!string.IsNullOrEmpty(model.ObjectId))
                        {
                            result = await _clients.AddToObjectAsync(client, model.ObjectId, CancellationToken.None);
                        }
                        return RedirectToAction("Details", new { clientId = client.Id });
                    }
                    AddErrors(result);
                }
                else
                {
                    ModelState.AddModelError("ClientId", "Такое название уже существует");
                }
            }
            ViewBag.ProtocolType = Constants.ProtocolTypes;
            return View(model);
        }

        [Menu(Name = "Редактирование клиента", Description = "Редактировать клиента", GroupName = "Client")]
        [HttpGet]
        public async Task<IActionResult> _EditPartial(string clientId)
        {
            ClientCustom client = await _clients.FindByIdAsync(clientId, CancellationToken.None);
            ClientInputModel clientView = new ClientInputModel(client);
            ViewBag.ProtocolType = Constants.ProtocolTypes;
            return PartialView(clientView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _EditPartial(ClientInputModel model)
        {
            if (ModelState.IsValid)
            {
                ClientCustom client = new ClientCustom()
                {
                    Id = model.Id,
                    ClientId = model.ClientId,
                    ProtocolType = model.ProtocolType,
                    RequireClientSecret = model.RequireClientSecret,
                    ClientName = model.ClientName,
                    ClientUri = model.ClientUri,
                    LogoUri = model.LogoUri,
                    RequireConsent = model.RequireConsent,
                    AllowRememberConsent = model.AllowRememberConsent,
                    RequirePkce = model.RequirePkce,
                    AllowPlainTextPkce = model.AllowPlainTextPkce,
                    AllowAccessTokensViaBrowser = model.AllowAccessTokensViaBrowser,
                    LogoutUri = model.LogoutUri,
                    LogoutSessionRequired = model.LogoutSessionRequired,
                    AllowOfflineAccess = model.AllowOfflineAccess,
                    AlwaysIncludeUserClaimsInIdToken = model.AlwaysIncludeUserClaimsInIdToken,
                    IdentityTokenLifetime = model.IdentityTokenLifetime,
                    AccessTokenLifetime = model.AccessTokenLifetime,
                    AuthorizationCodeLifetime = model.AuthorizationCodeLifetime,
                    AbsoluteRefreshTokenLifetime = model.AbsoluteRefreshTokenLifetime,
                    SlidingRefreshTokenLifetime = model.SlidingRefreshTokenLifetime,
                    RefreshTokenUsage = model.RefreshTokenUsage,
                    UpdateAccessTokenClaimsOnRefresh = model.UpdateAccessTokenClaimsOnRefresh,
                    RefreshTokenExpiration = model.RefreshTokenExpiration,
                    AccessTokenType = model.AccessTokenType,
                    EnableLocalLogin = model.EnableLocalLogin,
                    IncludeJwtId = model.IncludeJwtId,
                    AlwaysSendClientClaims = model.AlwaysSendClientClaims,
                    PrefixClientClaims = model.PrefixClientClaims,
                    Enabled = model.Enabled
                };
                IdentityResult result = await _clients.UpdateAsync(client, CancellationToken.None);
                if (result.Succeeded)
                {
                    return RedirectToAction("_DetailsPartial", new { clientId = model.Id });
                }
                AddErrors(result);
            }
            ViewBag.ProtocolType = Constants.ProtocolTypes;
            return PartialView(model);
        }

        [Menu(Name = "Удаление клиента", Description = "Удаление клиента", GroupName = "Client")]
        [HttpGet]
        public async Task<IActionResult> Delete(string clientId)
        {
            ClientCustom client = await _clients.FindByIdAsync(clientId, CancellationToken.None);
            if (client != null)
            {
                IdentityResult result = await _clients.DeleteAsync(client, CancellationToken.None);
                return Json(result.Succeeded);
            }
            return Json(false);
        }
        #endregion

        #region AllowedCorsOrigins
        [Menu(Relation = "_EditPartial")]
        [HttpGet]
        public IActionResult _AddCorOriginPartial(string clientId)
        {
            return PartialView(new ClientAllowedCorsOriginInputModel(clientId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _AddCorOriginPartial(ClientAllowedCorsOriginInputModel model)
        {
            if (ModelState.IsValid)
            {
                AllowedCorsOrigin allowedCorsOrigin = new AllowedCorsOrigin()
                {
                    Id = model.AllowedCorsOriginId,
                    Name = model.Name
                };
                IdentityResult result = await _cors.CreateAsync(allowedCorsOrigin, CancellationToken.None);
                if (result.Succeeded)
                {
                    result = await _clients.AddCorOriginAsync(model.ClientId, model.AllowedCorsOriginId, CancellationToken.None);
                    return Json(result.Succeeded);
                }
            }
            return PartialView(model);
        }

        [Menu(Relation = "_EditPartial")]
        [HttpGet]
        public async Task<IActionResult> DeleteCorOrigin(string clientId, string allowedCorsOriginId)
        {
            IdentityResult result = await _clients.DeleteCorOriginAsync(clientId, allowedCorsOriginId, CancellationToken.None);
            if (result.Succeeded)
            {
                AllowedCorsOrigin cor = await _cors.FindByIdAsync(allowedCorsOriginId, CancellationToken.None);
                if (cor != null)
                {
                    result = await _cors.DeleteAsync(cor, CancellationToken.None);
                }
                return Json(result.Succeeded);
            }
            return Json(false);
        }
        #endregion

        #region GrantTypes
        [Menu(Relation = "_EditPartial")]
        [HttpGet]
        public async Task<IActionResult> _AddGrantTypePartial(string clientId)
        {
            IQueryable<AllowedGrantType> grants = await _clients.FindAllowedGrantTypeAsync(clientId, CancellationToken.None);
            List<AllowedGrantTypeViewModel> grantView = new List<AllowedGrantTypeViewModel>();
            foreach (AllowedGrantType grant in _grants.AllowedGrantTypes)
            {
                AllowedGrantType type = grants.FirstOrDefault(g => g.Id == grant.Id);
                if (type == null)
                {
                    grantView.Add(new AllowedGrantTypeViewModel(grant.Id, grant.Name));
                }
            }
            if (grants.FirstOrDefault(g => g.Name == "hibrid") != null)
            {
                grantView.RemoveAll(g => g.Name == "authorization_code" || g.Name == "implicit");
            }
            if (grants.Where(g => g.Name.Contains("authorization_code") || g.Name.Contains("implicit")).Count() > 0)
            {
                grantView.RemoveAll(g => g.Name.Contains("hybrid"));
            }

            ViewData["GrantTypes"] = grantView;
            return PartialView(new ClientAllowedGrantTypeInputModel(clientId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _AddGrantTypePartial(ClientAllowedGrantTypeInputModel model)
        {
            if (ModelState.IsValid)
            {
                AllowedGrantType grant = await _grants.FindByIdAsync(model.AllowedGrantTypeId, CancellationToken.None);
                if (grant != null)
                {
                    IdentityResult result = await _clients.AddAllowedGrantTypeAsync(model.ClientId, grant, CancellationToken.None);
                    return Json(result.Succeeded);
                }
            }
            IQueryable<AllowedGrantType> grants = await _clients.FindAllowedGrantTypeAsync(model.ClientId, CancellationToken.None);
            List<AllowedGrantTypeViewModel> grantView = new List<AllowedGrantTypeViewModel>();
            foreach (AllowedGrantType grant in _grants.AllowedGrantTypes)
            {
                if (grants.FirstOrDefault(c => c.Id == grant.Id) == null)
                {
                    grantView.Add(new AllowedGrantTypeViewModel(grant.Id, grant.Name));
                }
            }
            if (grants.FirstOrDefault(g => g.Name == "hibrid") != null)
            {
                grantView.RemoveAll(g => g.Name == "authorization_code" || g.Name == "implicit");
            }
            if (grants.Where(g => g.Name.Contains("authorization_code") || g.Name.Contains("implicit")).Count() > 0)
            {
                grantView.RemoveAll(g => g.Name.Contains("hybrid"));
            }
            ViewData["GrantTypes"] = grantView;
            return PartialView(model);
        }

        [Menu(Relation = "_EditPartial")]
        [HttpGet]
        public async Task<IActionResult> DeleteGrantType(string clientId, string allowedGrantTypeId)
        {
            IdentityResult result = await _clients.DeleteAllowedGrantTypeAsync(clientId, allowedGrantTypeId, CancellationToken.None);
            return Json(result.Succeeded);
        }
        #endregion

        #region Scope
        [Menu(Relation = "_EditPartial")]
        [HttpGet]
        public async Task<IActionResult> _AddScopePartial(string clientId)
        {
            IQueryable<string> scopes = await _clients.FindAllowedScopesAsync(clientId, CancellationToken.None);
            List<string> scopeView = new List<string>();
            foreach (IdentityResourceCustom resourse in _identityResources.IdentityResourceCustoms)
            {
                if (!scopes.Contains(resourse.Name))
                {
                    scopeView.Add(resourse.Name);
                }
            }
            ViewData["Scopes"] = scopeView;
            return PartialView(new ClientAllowedScopeInputModel(clientId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _AddScopePartial(ClientAllowedScopeInputModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _clients.AddAllowedScopeAsync(model.ClientId, model.ScopeName, CancellationToken.None);
                return Json(result.Succeeded);
            }
            IQueryable<string> scopes = await _clients.FindAllowedScopesAsync(model.ClientId, CancellationToken.None);
            List<string> scopeView = new List<string>();
            foreach (IdentityResourceCustom resourse in _identityResources.IdentityResourceCustoms)
            {
                if (!scopes.Contains(resourse.Name))
                {
                    scopeView.Add(resourse.Name);
                }
            }
            ViewData["Scopes"] = scopeView;
            return PartialView(model);
        }

        [Menu(Relation = "_EditPartial")]
        [HttpGet]
        public async Task<IActionResult> DeleteScope(string clientId, string scopeName)
        {
            IdentityResult result = await _clients.DeleteAllowedScopeAsync(clientId, scopeName, CancellationToken.None);
            return Json(result.Succeeded);
        }
        #endregion

        #region PostLogoutRedirectUri
        [Menu(Relation = "_EditPartial")]
        [HttpGet]
        public IActionResult _AddPostLogoutRedirectUriPartial(string clientId)
        {
            return PartialView(new ClientPostLogoutRedirectUriInputModel(clientId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _AddPostLogoutRedirectUriPartial(ClientPostLogoutRedirectUriInputModel model)
        {
            if (ModelState.IsValid)
            {
                PostLogoutRedirectUri redirect = new PostLogoutRedirectUri()
                {
                    Id = model.PostLogoutRedirectUriId,
                    Name = model.Name
                };
                IdentityResult result = await _logoutRedirectUris.CreateAsync(redirect, CancellationToken.None);
                if (result != null)
                {
                    result = await _clients.AddPostLogoutRedirectUriAsync(model.ClientId, redirect.Id, CancellationToken.None);
                    return Json(result.Succeeded);
                }
            }
            return PartialView(model);
        }

        [Menu(Relation = "_EditPartial")]
        [HttpGet]
        public async Task<IActionResult> DeletePostLogoutRedirectUri(string clientId, string postLogoutRedirectUriId)
        {
            IdentityResult result = await _clients.DeletePostLogoutRedirectUriAsync(clientId, postLogoutRedirectUriId, CancellationToken.None);
            if (result.Succeeded)
            {
                PostLogoutRedirectUri redirect = await _logoutRedirectUris.FindByIdAsync(postLogoutRedirectUriId, CancellationToken.None);
                if (redirect != null)
                {
                    result = await _logoutRedirectUris.DeleteAsync(redirect, CancellationToken.None);
                    return Json(result.Succeeded);
                }
            }

            return Json(false);
        }
        #endregion

        #region RedirectUri
        [Menu(Relation = "_EditPartial")]
        [HttpGet]
        public IActionResult _AddRedirectUriPartial(string clientId)
        {
            return PartialView(new ClientRedirectUriInputModel(clientId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _AddRedirectUriPartial(ClientRedirectUriInputModel model)
        {
            if (ModelState.IsValid)
            {
                RedirectUri redirectUri = new RedirectUri()
                {
                    Id = model.RedirectUriId,
                    Name = model.Name
                };
                IdentityResult result = await _redirectUris.CreateAsync(redirectUri, CancellationToken.None);
                if (result.Succeeded)
                {
                    result = await _clients.AddRedirectUriAsync(model.ClientId, model.RedirectUriId, CancellationToken.None);
                    return Json(result.Succeeded);
                }
            }
            return PartialView(model);
        }

        [Menu(Relation = "_EditPartial")]
        [HttpGet]
        public async Task<IActionResult> DeleteRedirectUri(string clientId, string redirectUriId)
        {
            IdentityResult result = await _clients.DeleteRedirectUriAsync(clientId, redirectUriId, CancellationToken.None);
            if (result.Succeeded)
            {
                RedirectUri redirectUri = await _redirectUris.FindByIdAsync(redirectUriId, CancellationToken.None);
                if (redirectUri != null)
                {
                    result = await _redirectUris.DeleteAsync(redirectUri, CancellationToken.None);
                }
                return Json(result.Succeeded);
            }
            return Json(false);
        }
        #endregion

        #region Secrets
        [Menu(Relation = "_EditPartial")]
        [HttpGet]
        public IActionResult _AddSecretsPartial(string clientId)
        {
            ViewBag.SecretTypes = Constants.SecretTypes;
            return PartialView(new ClientSecretInputModel(clientId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _AddSecretsPartial(ClientSecretInputModel model)
        {
            if (ModelState.IsValid)
            {
                SecretCustom secret = new SecretCustom()
                {
                    Id = model.SecretId,
                    Value = model.Value,
                    Type = model.Type
                };
                IdentityResult result = await _secrets.CreateAsync(secret, CancellationToken.None);
                if (result.Succeeded)
                {
                    result = await _clients.AddSecretAsync(model.ClientId, model.SecretId, CancellationToken.None);
                    return Json(result.Succeeded);
                }
            }
            ViewBag.SecretTypes = Constants.SecretTypes;
            return PartialView(model);
        }

        [Menu(Relation = "_EditPartial")]
        [HttpGet]
        public async Task<IActionResult> DeleteSecrets(string clientId, string secretId)
        {
            IdentityResult result = await _clients.DeleteSecretAsync(clientId, secretId, CancellationToken.None);
            if (result.Succeeded)
            {
                SecretCustom secret = await _secrets.FindByIdAsync(secretId, CancellationToken.None);
                if (secret != null)
                {
                    result = await _secrets.DeleteAsync(secret, CancellationToken.None);
                }
                return Json(result.Succeeded);
            }
            return Json(false);
        }
        #endregion Secrets

        #region ClientRelations

        [Menu(Name = "Связи клиентов", Description = "Связи клиентов", GroupName = "Client")]
        public async Task<IActionResult> ClientRelationList(string clientId)
        {
            ViewBag.clientId = clientId;

            IQueryable<ClientCustom> clientRelationsByFromClientId = await _clients.FindClientRelationsByFromClientIdAsync(clientId, CancellationToken.None);
            List<ClientCustom> clientCustomView = new List<ClientCustom>();

            foreach (ClientCustom clientCustom in clientRelationsByFromClientId)
            {
                ClientCustom client = await _clients.FindByIdAsync(clientCustom.Id, CancellationToken.None);
                clientCustomView.Add(new ClientCustom() { Id = clientCustom.Id, ClientName = client.ClientId, ClientId = clientId });
            }

            return View(clientCustomView);
        }

        [Menu(Relation = "ClientRelationList")]
        public async Task<JsonResult> ClientRelationData(string clientId, string order, int limit, int offset, string sort, string search)
        {
            IQueryable<ClientCustom> clientRelationsByFromClientId = await _clients.FindClientRelationsByFromClientIdAsync(clientId, CancellationToken.None);
            List<ClientCustom> clientCustomView = new List<ClientCustom>();

            foreach (ClientCustom clientCustom in clientRelationsByFromClientId)
            {
                ClientCustom client = await _clients.FindByIdAsync(clientCustom.Id, CancellationToken.None);
                clientCustomView.Add(new ClientCustom() { Id = clientCustom.Id, ClientName = client.ClientId, ClientId = clientId });
            }

            return Json(clientCustomView);
        }

        [Menu(Name = "Добавить связь", Description = "Добавить связь клиентов", GroupName = "Client")]
        [HttpGet]
        public async Task<IActionResult> _AddClientRelationPartial(string clientId)
        {
            IQueryable<ClientCustom> clientRelationsByFromClientId = await _clients.FindClientRelationsByFromClientIdAsync(clientId, CancellationToken.None);
            IQueryable clients = _clients.ClientCustoms;
            List<ClientCustom> clientCustomView = new List<ClientCustom>();

            foreach (ClientCustom clientCustom in clients)
            {
                ClientCustom type = clientRelationsByFromClientId.FirstOrDefault(g => g.Id == clientCustom.Id);
                if (type == null)
                {
                    clientCustomView.Add(new ClientCustom()
                    {
                        Id = clientCustom.Id,
                        ClientId = clientCustom.ClientId
                    });
                }
            }

            ViewBag.clientCustomView = clientCustomView;
            ViewBag.clientId = clientId;
            return PartialView(new ClientRelationInputModel(clientId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _AddClientRelationPartial(ClientRelationInputModel model)
        {

            if (ModelState.IsValid)
            {
                IdentityResult result = await _clients.AddClientRelationAsync(model.FromClientId, model.ToClientId, CancellationToken.None);
                return Json(result.Succeeded);
            }

            IQueryable<ClientCustom> clientRelationsByFromClientId = await _clients.FindClientRelationsByFromClientIdAsync(model.FromClientId, CancellationToken.None);
            IQueryable clients = _clients.ClientCustoms;
            List<ClientCustom> clientCustomView = new List<ClientCustom>();

            foreach (ClientCustom clientCustom in clients)
            {
                ClientCustom type = clientRelationsByFromClientId.FirstOrDefault(g => g.Id == clientCustom.Id);
                if (type == null)
                {
                    clientCustomView.Add(new ClientCustom()
                    {
                        Id = clientCustom.Id,
                        ClientId = clientCustom.ClientId
                    });
                }
            }

            ViewBag.clientCustomView = clientCustomView;
            ViewBag.clientId = model.FromClientId;

            return PartialView(model);
        }

        /*[HttpGet]
        public async Task<IActionResult> AddClientRelation(string clientId)
        {
            IQueryable<ClientCustom> clientRelationsByFromClientId = await _clients.FindClientRelationsByFromClientIdAsync(clientId, CancellationToken.None);
            IQueryable clients = _clients.ClientCustoms;
            List<ClientCustom> clientCustomView = new List<ClientCustom>();

            foreach (ClientCustom clientCustom in clients)
            {
                ClientCustom type = clientRelationsByFromClientId.FirstOrDefault(g => g.Id == clientCustom.Id);
                if (type == null)
                {
                    clientCustomView.Add(new ClientCustom()
                    {
                        Id = clientCustom.Id,
                        ClientId = clientCustom.ClientId
                    });
                }
            }

            ViewBag.clientCustomView = clientCustomView;
            ViewBag.clientId = clientId;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddClientRelation(ClientRelationInputModel model)
        {
            if (ModelState.IsValid)
            {

                IdentityResult result = await _clients.AddClientRelationAsync(model.FromClientId, model.ToClientId, CancellationToken.None);
                if (result != null && !result.Succeeded)
                {
                    return View(model);
                }
            }

            return RedirectToAction("ClientRelationList", new { clientId = model.FromClientId });
        }*/

        [Menu(Name = "Удалить связь", Description = "Удалить связь клиентов", GroupName = "Client")]
        [HttpGet]
        public async Task<IActionResult> DeleteClientRelation(string fromClientId, string toClientId)
        {
            IdentityResult result = await _clients.DeleteClientRelationAsync(fromClientId, toClientId, CancellationToken.None);

            if (result.Succeeded)
            {
                return Json(true);
            }
            AddErrors(result);

            return Json(false);
        }
        #endregion ClientRelations

        #region ClientIdentityProviderRestrictions
        [Menu(Relation = "Details")]
        public async Task<IActionResult> _ClientIdentityProviderRestrictionListPartial(string clientId)
        {
            IQueryable<IdentityProviderRestriction> clientRestruction = await _clients.FindIdentityProviderRestrictionAsync(clientId, CancellationToken.None);
            ViewBag.ClientId = clientId;
            List<IdentityProviderRestrictionViewModel> clientRestructionView = new List<IdentityProviderRestrictionViewModel>();
            foreach (IdentityProviderRestriction restruction in clientRestruction)
            {
                clientRestructionView.Add(new IdentityProviderRestrictionViewModel(restruction.Id, restruction.Name));
            }
            return PartialView(clientRestructionView);
        }

        [Menu(Relation = "_EditPartial")]
        [HttpGet]
        public async Task<IActionResult> _AddClientIdentityProviderRestrictionPartial(string clientId)
        {
            ViewBag.Restrictions = await GetClientAllowRestrictions(clientId);
            return PartialView(new ClientProviderRestrictionInputModel(clientId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _AddClientIdentityProviderRestrictionPartial(ClientProviderRestrictionInputModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result =
                    await _clients.AddIdentityProviderRestrictionAsync(model.ClientId, model.IdentityProviderRestrictionId, CancellationToken.None);
                return Json(result.Succeeded);
            }
            ViewBag.Restrictions = await GetClientAllowRestrictions(model.ClientId);
            return View(model);
        }

        [Menu(Relation = "_EditPartial")]
        [HttpGet]
        public async Task<IActionResult> DeleteClientIdentityProviderRestriction(string clientId, string identityProviderRestrictionId)
        {
            IdentityResult result = await _clients.DeleteIdentityProviderRestrictionAsync(clientId, identityProviderRestrictionId, CancellationToken.None);
            return Json(result.Succeeded);
        }

        private async Task<IEnumerable<IdentityProviderRestrictionViewModel>> GetClientAllowRestrictions(string clientId)
        {
            IQueryable<IdentityProviderRestriction> restrictions = await _clients.FindIdentityProviderRestrictionAsync(clientId, CancellationToken.None);
            List<IdentityProviderRestrictionViewModel> clientRestructionView = new List<IdentityProviderRestrictionViewModel>();
            foreach (IdentityProviderRestriction restruction in _providerRestructions.IdentityProviderRestrictions)
            {
                if (restrictions.FirstOrDefault(r => r.Id == restruction.Id) == null)
                {
                    clientRestructionView.Add(new IdentityProviderRestrictionViewModel(restruction.Id, restruction.Name));
                }
            }
            return clientRestructionView.AsEnumerable();
        }
        #endregion ClientIdentityProviderRestrictions

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
