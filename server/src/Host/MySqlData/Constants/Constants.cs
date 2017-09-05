using System.Collections.Generic;

namespace MySql.AspNet.Identity
{
    public class Constants
    {
        public static IReadOnlyDictionary<string, string> ProtocolTypes
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { "OpenIdConnect", "oidc" },
                    { "WsFederation", "wsfed" },
                    { "Saml2p", "saml2p" }
                };
            }
        }
        public static IReadOnlyDictionary<string, string> TokenTypes
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { "IdentityToken", "id_token" },
                    { "AccessToken", "access_token" }
                };
            }
        }
        public static IReadOnlyDictionary<string, string> ParsedSecretTypes
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { "NoSecret", "NoSecret" },
                    { "SharedSecret", "SharedSecret" },
                    { "X509Certificate", "X509Certificate" },
                    { "JwtBearer", "urn:ietf:params:oauth:grant-type:jwt-bearer" }
                };
            }
        }
        public static IReadOnlyDictionary<string, string> SecretTypes
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { "SharedSecret", "SharedSecret" },
                    { "X509CertificateThumbprint", "X509Thumbprint" },
                    { "X509CertificateName", "X509Name" },
                    { "X509CertificateBase64", "X509CertificateBase64" }
                };
            }
        }
        public static IReadOnlyDictionary<string, string> ProfileDataCallers
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { "UserInfoEndpoint", "UserInfoEndpoint" },
                    { "ClaimsProviderIdentityToken", "ClaimsProviderIdentityToken" },
                    { "ClaimsProviderAccessToken", "ClaimsProviderAccessToken" }
                };
            }
        }
        public static IReadOnlyDictionary<string, string> ProfileIsActiveCallers
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { "AuthorizeEndpoint", "AuthorizeEndpoint" },
                    { "IdentityTokenValidation", "IdentityTokenValidation" },
                    { "AccessTokenValidation", "AccessTokenValidation" },
                    { "ResourceOwnerValidation", "ResourceOwnerValidation" },
                    { "RefreshTokenValidation", "RefreshTokenValidation" },
                    { "AuthorizationCodeValidation", "AuthorizationCodeValidation" }
                };
            }
        }
        public static IReadOnlyDictionary<string, string> StandardScopes
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { "OpenId", "openid" },
                    { "Profile", "profile" },
                    { "Email", "email" },
                    { "Address", "address" },
                    { "Phone", "phone" },
                    { "OfflineAccess", "offline_access" }
                };
            }
        }
    }
}

