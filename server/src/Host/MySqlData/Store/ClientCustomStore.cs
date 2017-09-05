using Microsoft.AspNetCore.Identity;
using MySql.AspNet.Identity.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace MySql.AspNet.Identity
{
    public class ClientCustomStore
    {
        private readonly string _connectionString;
        private readonly ClientCustomRepository _clientCustomRepository;
        private readonly ClientIdentityProviderRestrictionRepository<IdentityProviderRestriction> _clientIdentityProviderRestrictionRepository;
        private readonly ClientAllowedCorsOriginRepository _clientAllowedCorsOriginRepository;
        private readonly ClientAllowedGrantTypeRepository _clientAllowedGrantTypeRepository;
        private readonly ClientAllowedScopeRepository _clientAllowedScopeRepository;
        private readonly ClientPostLogoutRedirectUriRepository _clientPostLogoutRedirectUriRepository;
        private readonly ClientRedirectUriRepository _clientRedirectUriRepository;
        private readonly ClientSecretRepository _clientSecretRepository;
        private readonly ObjectClientRepository _objectClientRepository;
        private readonly IdentityResourceCustomRepository _identityResourceCustom;
        private readonly ClientRelationRepository _clientRelationRepository;
        
        public ClientCustomStore()
            : this("DefaultConnection")
        {

        }

        public ClientCustomStore(string connectionString)
        {
            _connectionString = connectionString;
            _clientCustomRepository = new ClientCustomRepository(_connectionString);
            _clientIdentityProviderRestrictionRepository = new ClientIdentityProviderRestrictionRepository<IdentityProviderRestriction>(_connectionString);
            _clientAllowedCorsOriginRepository = new ClientAllowedCorsOriginRepository(_connectionString);
            _clientAllowedGrantTypeRepository = new ClientAllowedGrantTypeRepository(_connectionString);
            _clientAllowedScopeRepository = new ClientAllowedScopeRepository(_connectionString);
            _clientPostLogoutRedirectUriRepository = new ClientPostLogoutRedirectUriRepository(_connectionString);
            _clientRedirectUriRepository = new ClientRedirectUriRepository(_connectionString);
            _clientSecretRepository = new ClientSecretRepository(_connectionString);
            _objectClientRepository = new ObjectClientRepository(_connectionString);
            _identityResourceCustom = new IdentityResourceCustomRepository(_connectionString);
            _clientRelationRepository = new ClientRelationRepository(_connectionString);
        }

        public IQueryable<ClientCustom> ClientCustoms
        {
            get
            {
                return _clientCustomRepository.GetClientCustoms();
            }
        }

        public void Dispose()
        {
            // connection is automatically disposed
        }

        private void SaveLocalConf(ClientCustom clientCustom)
        {
            string fileText = Host.Startup.GetLocalConf();

            string redirectUri = clientCustom.RedirectUris.FirstOrDefault(r => r.Contains(clientCustom.ClientUri));
            if (!string.IsNullOrEmpty(redirectUri))
            {
                redirectUri = "/" + redirectUri.Replace(clientCustom.ClientUri, "").TrimStart('/');
            }

            fileText = fileText.Replace("@client_id_header@", clientCustom.ClientId);
            fileText = fileText.Replace("@client_id@", clientCustom.ClientId);
            fileText = fileText.Replace("@client_id_redirect@", redirectUri);
            fileText = fileText.Replace("@secret@", clientCustom.ClientSecrets.FirstOrDefault()?.Value ?? "");
            fileText = fileText.Replace("@proxy_pass@", clientCustom.ClientUri);

            Host.Startup.SaveClientLocalConf(clientCustom.ClientId, fileText);
        }

        public Task<IdentityResult> CreateAsync(ClientCustom clientCustom, CancellationToken cancellationToken)
        {
            if (clientCustom == null)
            {
                throw new ArgumentNullException("clientCustom");
            }
            if (Constants.ProtocolTypes.Where(p => p.Value == clientCustom.ProtocolType).Select(p => new { Key = p.Key, Value = p.Value })
                   .FirstOrDefault() == null)
            {
                throw new ArgumentNullException("protocolType");
            }

            _clientCustomRepository.Insert(clientCustom);
            _clientAllowedGrantTypeRepository.Insert(clientCustom.Id, "6");

            //SaveLocalConf(clientCustom);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> AddToObjectAsync(ClientCustom client, string objectId, CancellationToken cancellationToken)
        {
            if (client == null)
            {
                throw new ArgumentException("client");
            }
            if (string.IsNullOrEmpty(objectId))
            {
                throw new ArgumentException("objectId");
            }
            _objectClientRepository.Insert(objectId, client.Id);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> AddIdentityProviderRestrictionAsync(string clientId, string identityProviderRestrictionId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException("clientId");
            }
            if (string.IsNullOrEmpty(identityProviderRestrictionId))
            {
                throw new ArgumentException("identityProviderRestrictionId");
            }
            List<IdentityProviderRestriction> clientRestructions =
                _clientIdentityProviderRestrictionRepository.PopulateClientIdentityProviderRestrictions(clientId);
            if (clientRestructions.FirstOrDefault(r => r.Id == identityProviderRestrictionId) != null)
            {
                throw new ArgumentException("this clientIdentityProviderRestriction already exist");
            }
            _clientIdentityProviderRestrictionRepository.Insert(clientId, identityProviderRestrictionId);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> AddCorOriginAsync(string clientId, string corOriginId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException("clientId");
            }
            if (string.IsNullOrEmpty(corOriginId))
            {
                throw new ArgumentException("corOriginId");
            }
            _clientAllowedCorsOriginRepository.Insert(clientId, corOriginId);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> AddAllowedGrantTypeAsync(string clientId, AllowedGrantType allowedGrantType, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException("clientId");
            }
            if (allowedGrantType == null)
            {
                throw new ArgumentException("allowedGrantType");
            }
            List<AllowedGrantType> grants = _clientAllowedGrantTypeRepository.PopulateClientAllowedGrantTypes(clientId);
            if (grants.FirstOrDefault(g => g.Id == allowedGrantType.Id) != null)
            {
                throw new ArgumentException("this grant type already exist in client");
            }
            if (allowedGrantType.Name.ToLower().Contains("hybrid") &&
                grants.Where(g => g.Name.Contains("authorization_code") || g.Name.Contains("implicit")).Count() > 0)
            {
                throw new ArgumentException("Нельзя добавить hibrid при наличии implicit или authorization_code");
            }
            if (new string[] { "authorization_code", "implicit" }.Contains(allowedGrantType.Name.ToLower()) &&
                grants.FirstOrDefault(g => g.Name.Contains("hybrid")) != null)
            {
                throw new ArgumentException("Нельзя добавить authorization_code или implicit при наличии hybrid");
            }
            _clientAllowedGrantTypeRepository.Insert(clientId, allowedGrantType.Id);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> AddAllowedScopeAsync(string clientId, string scopeName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException("clientId");
            }
            if (string.IsNullOrEmpty(scopeName))
            {
                throw new ArgumentException("scopeName");
            }
            IdentityResourceCustom resource = _identityResourceCustom.GetIdentityResourceCustomByName(scopeName);
            if (resource == null)
            {
                throw new ArgumentException("resource");
            }
            _clientAllowedScopeRepository.Insert(clientId, resource.Id);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> AddPostLogoutRedirectUriAsync(string clientId, string postLogoutRedirectUriId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException("clientId");
            }
            if (string.IsNullOrEmpty(postLogoutRedirectUriId))
            {
                throw new ArgumentException("PostLogoutRedirectUriId");
            }
            _clientPostLogoutRedirectUriRepository.Insert(clientId, postLogoutRedirectUriId);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> AddRedirectUriAsync(string clientId, string redirectUriId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException("clientId");
            }
            if (string.IsNullOrEmpty(redirectUriId))
            {
                throw new ArgumentException("redirectUriId");
            }
            _clientRedirectUriRepository.Insert(clientId, redirectUriId);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> AddSecretAsync(string clientId, string secretId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException("clientId");
            }
            if (string.IsNullOrEmpty(secretId))
            {
                throw new ArgumentException("redirectUriId");
            }
            _clientSecretRepository.Insert(clientId, secretId);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> AddClientRelationAsync(string fromClientId, string toClientId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(fromClientId))
            {
                throw new ArgumentException("fromClientId");
            }
            if (string.IsNullOrEmpty(toClientId))
            {
                throw new ArgumentException("toClientId");
            }
            if (_clientRelationRepository.Get(fromClientId, toClientId) != null){
                throw new ArgumentException("Join is exist");
            }

            _clientRelationRepository.Insert(fromClientId, toClientId);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(ClientCustom clientCustom, CancellationToken cancellationToken)
        {
            if (clientCustom == null)
            {
                throw new ArgumentNullException("clientCustom");
            }
            if (Constants.ProtocolTypes.Where(p => p.Value == clientCustom.ProtocolType).Select(p => new { Key = p.Key, Value = p.Value })
                 .FirstOrDefault() == null)
            {
                throw new ArgumentNullException("protocolType");
            }
            _clientCustomRepository.Update(clientCustom);

            //TODO: доделать и раскомментировать
            //SaveLocalConf(clientCustom);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(ClientCustom clientCustom, CancellationToken cancellationToken)
        {
            if (clientCustom == null)
            {
                throw new ArgumentNullException("clientCustom");
            }

            _clientCustomRepository.Delete(clientCustom.Id);

            Host.Startup.DeleteClientLocalConf(clientCustom.ClientId);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteIdentityProviderRestrictionAsync(string clientId, string identityProviderRestrictionId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException("clientId");
            }
            if (string.IsNullOrEmpty(identityProviderRestrictionId))
            {
                throw new ArgumentException("identityProviderRestrictionId");
            }
            _clientIdentityProviderRestrictionRepository.Delete(clientId, identityProviderRestrictionId);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteCorOriginAsync(string clientId, string corOriginId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException("clientId");
            }
            if (string.IsNullOrEmpty(corOriginId))
            {
                throw new ArgumentException("CorOriginId");
            }
            _clientAllowedCorsOriginRepository.Delete(clientId, corOriginId);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAllowedGrantTypeAsync(string clientId, string allowedGrantTypeId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException("clientId");
            }
            if (string.IsNullOrEmpty(allowedGrantTypeId))
            {
                throw new ArgumentException("allowedGrantTypeId");
            }
            _clientAllowedGrantTypeRepository.Delete(clientId, allowedGrantTypeId);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAllowedScopeAsync(string clientId, string scopeName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException("clientId");
            }
            if (string.IsNullOrEmpty(scopeName))
            {
                throw new ArgumentException("scopeName");
            }
            IdentityResourceCustom resource = _identityResourceCustom.GetIdentityResourceCustomByName(scopeName);
            if (resource == null)
            {
                throw new ArgumentException("scopeName");
            }
            _clientAllowedScopeRepository.Delete(clientId, resource.Id);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeletePostLogoutRedirectUriAsync(string clientId, string postLogoutRedirectUriId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException("clientId");
            }
            if (string.IsNullOrEmpty(postLogoutRedirectUriId))
            {
                throw new ArgumentException("CorOriginId");
            }
            _clientPostLogoutRedirectUriRepository.Delete(clientId, postLogoutRedirectUriId);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteRedirectUriAsync(string clientId, string redirectUriId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException("clientId");
            }
            if (string.IsNullOrEmpty(redirectUriId))
            {
                throw new ArgumentException("redirectUriId");
            }
            _clientRedirectUriRepository.Delete(clientId, redirectUriId);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteSecretAsync(string clientId, string secretId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException("clientId");
            }
            if (string.IsNullOrEmpty(secretId))
            {
                throw new ArgumentException("secretId");
            }
            _clientSecretRepository.Delete(clientId, secretId);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteClientRelationAsync(string fromClientId, string toClientId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(fromClientId))
            {
                throw new ArgumentException("fromClientId");
            }
            if (string.IsNullOrEmpty(toClientId))
            {
                throw new ArgumentException("toClientId");
            }
            _clientRelationRepository.Delete(fromClientId, toClientId);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<ClientCustom> FindByIdAsync(string clientCustomId, CancellationToken cancellationToken)
        {
            ClientCustom result = _clientCustomRepository.GetClientCustomById(clientCustomId);

            return Task.FromResult<ClientCustom>(result);
        }

        public Task<ClientCustom> FindByNameAsync(string clientCustomClientId, CancellationToken cancellationToken)
        {
            ClientCustom result = _clientCustomRepository.GetClientCustomByClientId(clientCustomClientId);
            return Task.FromResult(result);
        }

        public Task<ClientCustom> FindByObjectIdAsync(string objectId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(objectId))
            {
                throw new ArgumentException("objectId");
            }
            ClientCustom client = _objectClientRepository.GetClient(objectId);
            return Task.FromResult(client);
        }

        public Task<IQueryable<IdentityProviderRestriction>> FindIdentityProviderRestrictionAsync(string clientId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException("clientId");
            }
            IQueryable<IdentityProviderRestriction> clientRestructions =
                 _clientIdentityProviderRestrictionRepository.PopulateClientIdentityProviderRestrictions(clientId).AsQueryable();
            return Task.FromResult(clientRestructions);
        }

        public Task<IQueryable<AllowedCorsOrigin>> FindAllowerCorsOriginAsync(string clientId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(clientId))
            {
                throw new ArgumentException("clientId");
            }
            IQueryable<AllowedCorsOrigin> cors = _clientAllowedCorsOriginRepository.PopulateClientAllowedCorsOrigins(clientId).AsQueryable();
            return Task.FromResult(cors);
        }

        public Task<IQueryable<AllowedGrantType>> FindAllowedGrantTypeAsync(string clientId, CancellationToken cancellationToken)
        {
            IQueryable<AllowedGrantType> grants = _clientAllowedGrantTypeRepository.PopulateClientAllowedGrantTypes(clientId).AsQueryable();
            return Task.FromResult(grants);
        }

        public Task<IQueryable<string>> FindAllowedScopesAsync(string clientId, CancellationToken cancellationToken)
        {
            IQueryable<string> allowedScopes = _clientAllowedScopeRepository.PopulateClientIdentityResourceScopes(clientId).AsQueryable();
            return Task.FromResult(allowedScopes);
        }

        public Task<IQueryable<PostLogoutRedirectUri>> FindPostLogoutRedirectUrisAsync(string clientId, CancellationToken cancellationToken)
        {
            List<PostLogoutRedirectUri> logoutRedirect = _clientPostLogoutRedirectUriRepository.PopulateClientPostLogoutRedirectUris(clientId);
            return Task.FromResult(logoutRedirect.AsQueryable());
        }

        public Task<IQueryable<RedirectUri>> FindRedirectUrisAsync(string clientId, CancellationToken cancellationToken)
        {
            List<RedirectUri> Redirects = _clientRedirectUriRepository.PopulateClientRedirectUris(clientId);
            return Task.FromResult(Redirects.AsQueryable());
        }

        public Task<IQueryable<SecretCustom>> FindSecretsAsync(string clientId, CancellationToken cancellationToken)
        {
            List<SecretCustom> Secrets = _clientSecretRepository.PopulateClientSecretCustomsByClientId(clientId);
            return Task.FromResult(Secrets.AsQueryable());
        }

        public Task<IQueryable<ClientCustom>> FindClientRelationsByFromClientIdAsync(string fromClientId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(fromClientId))
            {
                throw new ArgumentException("fromClientId");
            }
            IQueryable<ClientCustom> clients = _clientRelationRepository.PopulateClientsByFromClientId(fromClientId).AsQueryable();
            return Task.FromResult(clients);
        }

        public Task<KeyValuePair<int, IQueryable<ClientCustom>>> GetClientsQueryFormatterAsync(string order, int limit, int offset, string sort, string search)
        {
            KeyValuePair<int, IQueryable<ClientCustom>> clients = _clientCustomRepository.GetClientsQueryFormatter(order, limit, offset, sort, search);
            return Task.FromResult(clients);
        }
    }
}
