using MySql.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.Quickstart.UI
{
    public class ClientViewModel : ClientInputModel
    {
        public List<IdentityProviderRestrictionViewModel> IdentityProviderRestrictions { get; set; }
        public List<AllowedCorsOriginViewModel> AllowedCorsOrigins { get;set; }
        public List<AllowedGrantTypeViewModel> AllowedGrantTypes { get; set; }
        public ICollection<string> AllowedScopes { get; set; }
        public List<PostLogoutRedirectUriViewModel> PostLogoutRedirectUris { get; set; }
        public List<RedirectUriViewModel> RedirectUris { get; set; }
        public List<SecretViewModel> Secrets { get; set; }

        public ClientViewModel()
        {
            IdentityProviderRestrictions = new List<IdentityProviderRestrictionViewModel>();
            AllowedCorsOrigins = new List<AllowedCorsOriginViewModel>();
            AllowedGrantTypes = new List<AllowedGrantTypeViewModel>();
            AllowedScopes = new List<string>();
            PostLogoutRedirectUris = new List<PostLogoutRedirectUriViewModel>();
            RedirectUris = new List<RedirectUriViewModel>();
            Secrets = new List<SecretViewModel>();
        }

        public ClientViewModel(ClientCustom client)
        {
            Id = client.Id;
            ClientId = client.ClientId;
            ProtocolType = client.ProtocolType;
            RequireClientSecret = client.RequireClientSecret;
            ClientName = client.ClientName;
            ClientUri = client.ClientUri;
            LogoUri = client.LogoUri;
            RequireConsent = client.RequireConsent;
            AllowRememberConsent = client.AllowRememberConsent;
            RequirePkce = client.RequirePkce;
            AllowPlainTextPkce = client.AllowPlainTextPkce;
            AllowAccessTokensViaBrowser = client.AllowAccessTokensViaBrowser;
            LogoutUri = client.LogoutUri;
            LogoutSessionRequired = client.LogoutSessionRequired;
            AllowOfflineAccess = client.AllowOfflineAccess;
            AlwaysIncludeUserClaimsInIdToken = client.AlwaysIncludeUserClaimsInIdToken;
            IdentityTokenLifetime = client.IdentityTokenLifetime;
            AccessTokenLifetime = client.AccessTokenLifetime;
            AuthorizationCodeLifetime = client.AuthorizationCodeLifetime;
            AbsoluteRefreshTokenLifetime = client.AbsoluteRefreshTokenLifetime;
            SlidingRefreshTokenLifetime = client.SlidingRefreshTokenLifetime;
            RefreshTokenUsage = client.RefreshTokenUsage;
            UpdateAccessTokenClaimsOnRefresh = client.UpdateAccessTokenClaimsOnRefresh;
            RefreshTokenExpiration = client.RefreshTokenExpiration;
            AccessTokenType = client.AccessTokenType;
            EnableLocalLogin = client.EnableLocalLogin;
            IncludeJwtId = client.IncludeJwtId;
            AlwaysSendClientClaims = client.AlwaysSendClientClaims;
            PrefixClientClaims = client.PrefixClientClaims;
            Enabled = client.Enabled;
            IdentityProviderRestrictions = new List<IdentityProviderRestrictionViewModel>();
            AllowedCorsOrigins = new List<AllowedCorsOriginViewModel>();
            AllowedGrantTypes = new List<AllowedGrantTypeViewModel>();
            AllowedScopes = new List<string>();
            PostLogoutRedirectUris = new List<PostLogoutRedirectUriViewModel>();
            RedirectUris = new List<RedirectUriViewModel>();
            Secrets = new List<SecretViewModel>();
        }
    }
}
