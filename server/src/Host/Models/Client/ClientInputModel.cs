using IdentityServer4.Models;
using MySql.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.Quickstart.UI
{
    public class ClientInputModel
    {
        public string Id { get; set; }
        [Required]
        public string ClientId { get; set; }
        [Required]
        public string ProtocolType { get; set; } = "oidc";
        public bool RequireClientSecret { get; set; } = true;
        public string ClientName { get; set; }
        public string ClientUri { get; set; }
        public string LogoUri { get; set; }
        public bool RequireConsent { get; set; } = true;
        public bool AllowRememberConsent { get; set; } = true;
        public bool RequirePkce { get; set; }
        public bool AllowPlainTextPkce { get; set; }
        public bool AllowAccessTokensViaBrowser { get; set; }
        public string LogoutUri { get; set; }
        public bool LogoutSessionRequired { get; set; } = true;
        public bool AllowOfflineAccess { get; set; }
        public bool AlwaysIncludeUserClaimsInIdToken { get; set; }
        [Required]
        public int IdentityTokenLifetime { get; set; } = 300;
        [Required]
        public int AccessTokenLifetime { get; set; } = 3600;
        [Required]
        public int AuthorizationCodeLifetime { get; set; } = 300;
        [Required]
        public int AbsoluteRefreshTokenLifetime { get; set; } = 2592000;
        [Required]
        public int SlidingRefreshTokenLifetime { get; set; } = 1296000;
        [Required]
        public TokenUsage RefreshTokenUsage { get; set; } = TokenUsage.OneTimeOnly;
        public bool UpdateAccessTokenClaimsOnRefresh { get; set; }
        [Required]
        public TokenExpiration RefreshTokenExpiration { get; set; } = TokenExpiration.Absolute;
        [Required]
        public AccessTokenType AccessTokenType { get; set; } = AccessTokenType.Jwt;
        public bool EnableLocalLogin { get; set; } = true;
        public bool IncludeJwtId { get; set; }
        public bool AlwaysSendClientClaims { get; set; }
        public bool PrefixClientClaims { get; set; } = true;
        public bool Enabled { get; set; } = true;

        public string ObjectId { get; set; }

        public ClientInputModel()
        {
            Id = Guid.NewGuid().ToString();
        }

        public ClientInputModel(ClientCustom client)
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
        }
    }
}
