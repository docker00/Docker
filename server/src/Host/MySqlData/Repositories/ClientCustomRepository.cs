using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using IdentityServer4.Models;

namespace MySql.AspNet.Identity.Repositories
{
    public class ClientCustomRepository
    {
        private readonly string _connectionString;
        
        private readonly ClientAllowedCorsOriginRepository _clientAllowedCorsOriginsCustomRepository;
        private readonly ClientAllowedGrantTypeRepository _clientAllowedGrantTypesCustomRepository;
        private readonly ClientAllowedScopeRepository _clientAllowedScopeRepository;
        private readonly ClientIdentityProviderRestrictionRepository<IdentityProviderRestriction> _clientIdentityProviderRestrictionsCustomRepository;
        private readonly ClientPostLogoutRedirectUriRepository _clientPostLogoutRedirectUrisCustomRepository;
        private readonly ClientRedirectUriRepository _clientRedirectUrisCustomRepository;
        private readonly ClientSecretRepository _clientSecretRepository;

        public ClientCustomRepository(string connectionString)
        {
            _connectionString = connectionString;

            _clientAllowedCorsOriginsCustomRepository = new ClientAllowedCorsOriginRepository(_connectionString);
            _clientAllowedGrantTypesCustomRepository = new ClientAllowedGrantTypeRepository(_connectionString);
            _clientAllowedScopeRepository = new ClientAllowedScopeRepository(_connectionString);
            _clientIdentityProviderRestrictionsCustomRepository = new ClientIdentityProviderRestrictionRepository<IdentityProviderRestriction>(_connectionString);
            _clientPostLogoutRedirectUrisCustomRepository = new ClientPostLogoutRedirectUriRepository(_connectionString);
            _clientRedirectUrisCustomRepository = new ClientRedirectUriRepository(_connectionString);
            _clientSecretRepository = new ClientSecretRepository(_connectionString);
        }

        public IQueryable<ClientCustom> GetClientCustoms()
        {
            List<ClientCustom> clientCustoms = new List<ClientCustom>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT
                        Id, ClientId, ProtocolType, RequireClientSecret, ClientName, ClientUri, LogoUri, RequireConsent, AllowRememberConsent, RequirePkce,
                        AllowPlainTextPkce, AllowAccessTokensViaBrowser, LogoutUri, LogoutSessionRequired, AllowOfflineAccess, AlwaysIncludeUserClaimsInIdToken,
                        IdentityTokenLifetime, AccessTokenLifetime, AuthorizationCodeLifetime, AbsoluteRefreshTokenLifetime, SlidingRefreshTokenLifetime,
                        RefreshTokenUsage, UpdateAccessTokenClaimsOnRefresh, RefreshTokenExpiration, AccessTokenType, EnableLocalLogin, IncludeJwtId,
                        AlwaysSendClientClaims, PrefixClientClaims, Enabled FROM clients", null);

                while (reader.Read())
                {
                    ClientCustom clientCustom = (ClientCustom)Activator.CreateInstance(typeof(ClientCustom));

                    clientCustom.Id = reader[0].ToString();
                    clientCustom.ClientId = reader[1].ToString();
                    clientCustom.ProtocolType = reader[2].ToString();
                    clientCustom.RequireClientSecret = (bool)reader[3];
                    clientCustom.ClientName = reader[4].ToString();
                    clientCustom.ClientUri = reader[5].ToString();
                    clientCustom.LogoUri = reader[6].ToString();
                    clientCustom.RequireConsent = (bool)reader[7];
                    clientCustom.AllowRememberConsent = (bool)reader[8];
                    clientCustom.RequirePkce = (bool)reader[9];
                    clientCustom.AllowPlainTextPkce = (bool)reader[10];
                    clientCustom.AllowAccessTokensViaBrowser = (bool)reader[11];
                    clientCustom.LogoutUri = reader[12].ToString();
                    clientCustom.LogoutSessionRequired = (bool)reader[13];
                    clientCustom.AllowOfflineAccess = (bool)reader[14];
                    clientCustom.AlwaysIncludeUserClaimsInIdToken = (bool)reader[15];
                    clientCustom.IdentityTokenLifetime = (int)reader[16];
                    clientCustom.AccessTokenLifetime = (int)reader[17];
                    clientCustom.AuthorizationCodeLifetime = (int)reader[18];
                    clientCustom.AbsoluteRefreshTokenLifetime = (int)reader[19];
                    clientCustom.SlidingRefreshTokenLifetime = (int)reader[20];
                    clientCustom.RefreshTokenUsage = (TokenUsage)reader[21];
                    clientCustom.UpdateAccessTokenClaimsOnRefresh = (bool)reader[22];
                    clientCustom.RefreshTokenExpiration = (TokenExpiration)reader[23];
                    clientCustom.AccessTokenType = (AccessTokenType)reader[24];
                    clientCustom.EnableLocalLogin = (bool)reader[25];
                    clientCustom.IncludeJwtId = (bool)reader[26];
                    clientCustom.AlwaysSendClientClaims = (bool)reader[27];
                    clientCustom.PrefixClientClaims = (bool)reader[28];
                    clientCustom.Enabled = (bool)reader[29];

                    clientCustom.AllowedCorsOrigins = _clientAllowedCorsOriginsCustomRepository.PopulateClientAllowedCorsOrigins(clientCustom.Id).Select(c => c.Name).ToList();
                    clientCustom.AllowedGrantTypes = _clientAllowedGrantTypesCustomRepository.PopulateClientAllowedGrantTypes(clientCustom.Id).Select(c => c.Name).ToList();
                    clientCustom.AllowedScopes = _clientAllowedScopeRepository.PopulateClientIdentityResourceScopes(clientCustom.Id);
                    clientCustom.IdentityProviderRestrictions = _clientIdentityProviderRestrictionsCustomRepository.PopulateClientIdentityProviderRestrictions(clientCustom.Id).Select(c => c.Name).ToList();
                    clientCustom.PostLogoutRedirectUris = _clientPostLogoutRedirectUrisCustomRepository.PopulateClientPostLogoutRedirectUris(clientCustom.Id).Select(c => c.Name).ToList();
                    clientCustom.RedirectUris = _clientRedirectUrisCustomRepository.PopulateClientRedirectUris(clientCustom.Id).Select(c => c.Name).ToList();
                    clientCustom.ClientSecrets = _clientSecretRepository.PopulateClientSecretByClientId(clientCustom.Id);
                    //Claims

                    clientCustoms.Add(clientCustom);
                }

            }
            return clientCustoms.AsQueryable();
        }

        public void Insert(ClientCustom clientCustom)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", clientCustom.Id },
                    { "@ClientId", clientCustom.ClientId },
                    { "@ProtocolType", clientCustom.ProtocolType },
                    { "@RequireClientSecret", clientCustom.RequireClientSecret },
                    { "@ClientName", clientCustom.ClientName ?? string.Empty },
                    { "@ClientUri", clientCustom.ClientUri ?? string.Empty },
                    { "@LogoUri", clientCustom.LogoUri ?? string.Empty },
                    { "@RequireConsent", clientCustom.RequireConsent },
                    { "@AllowRememberConsent", clientCustom.AllowRememberConsent },
                    { "@RequirePkce", clientCustom.RequirePkce },
                    { "@AllowPlainTextPkce", clientCustom.AllowPlainTextPkce },
                    { "@AllowAccessTokensViaBrowser", clientCustom.AllowAccessTokensViaBrowser },
                    { "@LogoutUri", clientCustom.LogoutUri ?? string.Empty },
                    { "@LogoutSessionRequired", clientCustom.LogoutSessionRequired },
                    { "@AllowOfflineAccess", clientCustom.AllowOfflineAccess },
                    { "@AlwaysIncludeUserClaimsInIdToken", clientCustom.AlwaysIncludeUserClaimsInIdToken },
                    { "@IdentityTokenLifetime", clientCustom.IdentityTokenLifetime },
                    { "@AccessTokenLifetime", clientCustom.AccessTokenLifetime },
                    { "@AuthorizationCodeLifetime", clientCustom.AuthorizationCodeLifetime },
                    { "@AbsoluteRefreshTokenLifetime", clientCustom.AbsoluteRefreshTokenLifetime },
                    { "@SlidingRefreshTokenLifetime", clientCustom.SlidingRefreshTokenLifetime },
                    { "@RefreshTokenUsage", clientCustom.RefreshTokenUsage },
                    { "@UpdateAccessTokenClaimsOnRefresh", clientCustom.UpdateAccessTokenClaimsOnRefresh },
                    { "@RefreshTokenExpiration", clientCustom.RefreshTokenExpiration },
                    { "@AccessTokenType", clientCustom.AccessTokenType },
                    { "@EnableLocalLogin", clientCustom.EnableLocalLogin },
                    { "@IncludeJwtId", clientCustom.IncludeJwtId },
                    { "@AlwaysSendClientClaims", clientCustom.AlwaysSendClientClaims },
                    { "@PrefixClientClaims", clientCustom.PrefixClientClaims },
                    { "@Enabled", clientCustom.Enabled }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO clients (Id, ClientId, ProtocolType, RequireClientSecret, ClientName, ClientUri, LogoUri, RequireConsent, AllowRememberConsent, RequirePkce,
                        AllowPlainTextPkce, AllowAccessTokensViaBrowser, LogoutUri, LogoutSessionRequired, AllowOfflineAccess, AlwaysIncludeUserClaimsInIdToken,
                        IdentityTokenLifetime, AccessTokenLifetime, AuthorizationCodeLifetime, AbsoluteRefreshTokenLifetime, SlidingRefreshTokenLifetime,
                        RefreshTokenUsage, UpdateAccessTokenClaimsOnRefresh, RefreshTokenExpiration, AccessTokenType, EnableLocalLogin, IncludeJwtId,
                        AlwaysSendClientClaims, PrefixClientClaims, Enabled) 
                    VALUES (@Id, @ClientId, @ProtocolType, @RequireClientSecret, @ClientName, @ClientUri, @LogoUri, @RequireConsent, @AllowRememberConsent, @RequirePkce,
                        AllowPlainTextPkce, @AllowAccessTokensViaBrowser, @LogoutUri, @LogoutSessionRequired, @AllowOfflineAccess, @AlwaysIncludeUserClaimsInIdToken,
                        IdentityTokenLifetime, @AccessTokenLifetime, @AuthorizationCodeLifetime, @AbsoluteRefreshTokenLifetime, @SlidingRefreshTokenLifetime,
                        RefreshTokenUsage, @UpdateAccessTokenClaimsOnRefresh, @RefreshTokenExpiration, @AccessTokenType, @EnableLocalLogin, @IncludeJwtId,
                        AlwaysSendClientClaims, @PrefixClientClaims, @Enabled)", parameters);
            }
        }

        public void Delete(string clientCustomId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", clientCustomId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM clients WHERE Id = @Id", parameters);
            }
        }

        public ClientCustom GetClientCustomById(string clientCustomId)
        {
            ClientCustom clientCustom = null;

            if (!string.IsNullOrEmpty(clientCustomId))
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "@Id", clientCustomId }
                    };

                    IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT
                        Id, ClientId, ProtocolType, RequireClientSecret, ClientName, ClientUri, LogoUri, RequireConsent, AllowRememberConsent, RequirePkce,
                        AllowPlainTextPkce, AllowAccessTokensViaBrowser, LogoutUri, LogoutSessionRequired, AllowOfflineAccess, AlwaysIncludeUserClaimsInIdToken,
                        IdentityTokenLifetime, AccessTokenLifetime, AuthorizationCodeLifetime, AbsoluteRefreshTokenLifetime, SlidingRefreshTokenLifetime,
                        RefreshTokenUsage, UpdateAccessTokenClaimsOnRefresh, RefreshTokenExpiration, AccessTokenType, EnableLocalLogin, IncludeJwtId,
                        AlwaysSendClientClaims, PrefixClientClaims, Enabled FROM clients WHERE Id = @Id", parameters);
                    while (reader.Read())
                    {
                        clientCustom = (ClientCustom)Activator.CreateInstance(typeof(ClientCustom));

                        clientCustom.Id = reader[0].ToString();
                        clientCustom.ClientId = reader[1].ToString();
                        clientCustom.ProtocolType = reader[2].ToString();
                        clientCustom.RequireClientSecret = (bool)reader[3];
                        clientCustom.ClientName = reader[4].ToString();
                        clientCustom.ClientUri = reader[5].ToString();
                        clientCustom.LogoUri = reader[6].ToString();
                        clientCustom.RequireConsent = (bool)reader[7];
                        clientCustom.AllowRememberConsent = (bool)reader[8];
                        clientCustom.RequirePkce = (bool)reader[9];
                        clientCustom.AllowPlainTextPkce = (bool)reader[10];
                        clientCustom.AllowAccessTokensViaBrowser = (bool)reader[11];
                        clientCustom.LogoutUri = reader[12].ToString();
                        clientCustom.LogoutSessionRequired = (bool)reader[13];
                        clientCustom.AllowOfflineAccess = (bool)reader[14];
                        clientCustom.AlwaysIncludeUserClaimsInIdToken = (bool)reader[15];
                        clientCustom.IdentityTokenLifetime = (int)reader[16];
                        clientCustom.AccessTokenLifetime = (int)reader[17];
                        clientCustom.AuthorizationCodeLifetime = (int)reader[18];
                        clientCustom.AbsoluteRefreshTokenLifetime = (int)reader[19];
                        clientCustom.SlidingRefreshTokenLifetime = (int)reader[20];
                        clientCustom.RefreshTokenUsage = (TokenUsage)reader[21];
                        clientCustom.UpdateAccessTokenClaimsOnRefresh = (bool)reader[22];
                        clientCustom.RefreshTokenExpiration = (TokenExpiration)reader[23];
                        clientCustom.AccessTokenType = (AccessTokenType)reader[24];
                        clientCustom.EnableLocalLogin = (bool)reader[25];
                        clientCustom.IncludeJwtId = (bool)reader[26];
                        clientCustom.AlwaysSendClientClaims = (bool)reader[27];
                        clientCustom.PrefixClientClaims = (bool)reader[28];
                        clientCustom.Enabled = (bool)reader[29];

                        clientCustom.AllowedCorsOrigins = _clientAllowedCorsOriginsCustomRepository.PopulateClientAllowedCorsOrigins(clientCustom.Id).Select(c => c.Name).ToList();
                        clientCustom.AllowedGrantTypes = _clientAllowedGrantTypesCustomRepository.PopulateClientAllowedGrantTypes(clientCustom.Id).Select(c => c.Name).ToList();
                        clientCustom.AllowedScopes = _clientAllowedScopeRepository.PopulateClientIdentityResourceScopes(clientCustom.Id);
                        clientCustom.IdentityProviderRestrictions = _clientIdentityProviderRestrictionsCustomRepository.PopulateClientIdentityProviderRestrictions(clientCustom.Id).Select(c => c.Name).ToList();
                        clientCustom.PostLogoutRedirectUris = _clientPostLogoutRedirectUrisCustomRepository.PopulateClientPostLogoutRedirectUris(clientCustom.Id).Select(c => c.Name).ToList();
                        clientCustom.RedirectUris = _clientRedirectUrisCustomRepository.PopulateClientRedirectUris(clientCustom.Id).Select(c => c.Name).ToList();
                        clientCustom.ClientSecrets = _clientSecretRepository.PopulateClientSecretByClientId(clientCustom.Id);
                    }
                }
            }

            return clientCustom;
        }

        public ClientCustom GetClientCustomByClientId(string clientCustomClientId)
        {
            ClientCustom clientCustom = null;

            if (!string.IsNullOrEmpty(clientCustomClientId))
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "@ClientId", clientCustomClientId }
                    };

                    IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT
                        Id, ClientId, ProtocolType, RequireClientSecret, ClientName, ClientUri, LogoUri, RequireConsent, AllowRememberConsent, RequirePkce,
                        AllowPlainTextPkce, AllowAccessTokensViaBrowser, LogoutUri, LogoutSessionRequired, AllowOfflineAccess, AlwaysIncludeUserClaimsInIdToken,
                        IdentityTokenLifetime, AccessTokenLifetime, AuthorizationCodeLifetime, AbsoluteRefreshTokenLifetime, SlidingRefreshTokenLifetime,
                        RefreshTokenUsage, UpdateAccessTokenClaimsOnRefresh, RefreshTokenExpiration, AccessTokenType, EnableLocalLogin, IncludeJwtId,
                        AlwaysSendClientClaims, PrefixClientClaims, Enabled FROM clients WHERE ClientId = @ClientId", parameters);
                    while (reader.Read())
                    {
                        clientCustom = (ClientCustom)Activator.CreateInstance(typeof(ClientCustom));

                        clientCustom.Id = reader[0].ToString();
                        clientCustom.ClientId = reader[1].ToString();
                        clientCustom.ProtocolType = reader[2].ToString();
                        clientCustom.RequireClientSecret = (bool)reader[3];
                        clientCustom.ClientName = reader[4].ToString();
                        clientCustom.ClientUri = reader[5].ToString();
                        clientCustom.LogoUri = reader[6].ToString();
                        clientCustom.RequireConsent = (bool)reader[7];
                        clientCustom.AllowRememberConsent = (bool)reader[8];
                        clientCustom.RequirePkce = (bool)reader[9];
                        clientCustom.AllowPlainTextPkce = (bool)reader[10];
                        clientCustom.AllowAccessTokensViaBrowser = (bool)reader[11];
                        clientCustom.LogoutUri = reader[12].ToString();
                        clientCustom.LogoutSessionRequired = (bool)reader[13];
                        clientCustom.AllowOfflineAccess = (bool)reader[14];
                        clientCustom.AlwaysIncludeUserClaimsInIdToken = (bool)reader[15];
                        clientCustom.IdentityTokenLifetime = (int)reader[16];
                        clientCustom.AccessTokenLifetime = (int)reader[17];
                        clientCustom.AuthorizationCodeLifetime = (int)reader[18];
                        clientCustom.AbsoluteRefreshTokenLifetime = (int)reader[19];
                        clientCustom.SlidingRefreshTokenLifetime = (int)reader[20];
                        clientCustom.RefreshTokenUsage = (TokenUsage)reader[21];
                        clientCustom.UpdateAccessTokenClaimsOnRefresh = (bool)reader[22];
                        clientCustom.RefreshTokenExpiration = (TokenExpiration)reader[23];
                        clientCustom.AccessTokenType = (AccessTokenType)reader[24];
                        clientCustom.EnableLocalLogin = (bool)reader[25];
                        clientCustom.IncludeJwtId = (bool)reader[26];
                        clientCustom.AlwaysSendClientClaims = (bool)reader[27];
                        clientCustom.PrefixClientClaims = (bool)reader[28];
                        clientCustom.Enabled = (bool)reader[29];

                        clientCustom.AllowedCorsOrigins = _clientAllowedCorsOriginsCustomRepository.PopulateClientAllowedCorsOrigins(clientCustom.Id).Select(c => c.Name).ToList();
                        clientCustom.AllowedGrantTypes = _clientAllowedGrantTypesCustomRepository.PopulateClientAllowedGrantTypes(clientCustom.Id).Select(c => c.Name).ToList();
                        clientCustom.AllowedScopes = _clientAllowedScopeRepository.PopulateClientIdentityResourceScopes(clientCustom.Id);
                        clientCustom.IdentityProviderRestrictions = _clientIdentityProviderRestrictionsCustomRepository.PopulateClientIdentityProviderRestrictions(clientCustom.Id).Select(c => c.Name).ToList();
                        clientCustom.PostLogoutRedirectUris = _clientPostLogoutRedirectUrisCustomRepository.PopulateClientPostLogoutRedirectUris(clientCustom.Id).Select(c => c.Name).ToList();
                        clientCustom.RedirectUris = _clientRedirectUrisCustomRepository.PopulateClientRedirectUris(clientCustom.Id).Select(c => c.Name).ToList();
                        clientCustom.ClientSecrets = _clientSecretRepository.PopulateClientSecretByClientId(clientCustom.Id);
                    }
                }
            }

            return clientCustom;
        }

        public KeyValuePair<int, IQueryable<ClientCustom>> GetClientsQueryFormatter(string order, int limit, int offset, string sort, string search)
        {
            string query = @"SELECT
                        Id, ClientId, ProtocolType, RequireClientSecret, ClientName, ClientUri, LogoUri, RequireConsent, AllowRememberConsent, RequirePkce,
                        AllowPlainTextPkce, AllowAccessTokensViaBrowser, LogoutUri, LogoutSessionRequired, AllowOfflineAccess, AlwaysIncludeUserClaimsInIdToken,
                        IdentityTokenLifetime, AccessTokenLifetime, AuthorizationCodeLifetime, AbsoluteRefreshTokenLifetime, SlidingRefreshTokenLifetime,
                        RefreshTokenUsage, UpdateAccessTokenClaimsOnRefresh, RefreshTokenExpiration, AccessTokenType, EnableLocalLogin, IncludeJwtId,
                        AlwaysSendClientClaims, PrefixClientClaims, Enabled FROM clients";
            List<ClientCustom> clients = new List<ClientCustom>();
            int total = 0;
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
                if (search.Trim().Length > 0)
                {
                    query += " WHERE ClientId LIKE '" + search + "%' OR ClientName LIKE '" + search + "%'";
                }
            }
            if (!string.IsNullOrEmpty(sort))
            {
                query += " ORDER BY " + sort + " " + order.ToUpper();
            }
            query += " LIMIT " + offset + "," + limit;
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, query, null);
                while (reader.Read())
                {
                    ClientCustom clientCustom = (ClientCustom)Activator.CreateInstance(typeof(ClientCustom));
                    clientCustom.Id = reader[0].ToString();
                    clientCustom.ClientId = reader[1].ToString();
                    clientCustom.ProtocolType = reader[2].ToString();
                    clientCustom.RequireClientSecret = (bool)reader[3];
                    clientCustom.ClientName = reader[4].ToString();
                    clientCustom.ClientUri = reader[5].ToString();
                    clientCustom.LogoUri = reader[6].ToString();
                    clientCustom.RequireConsent = (bool)reader[7];
                    clientCustom.AllowRememberConsent = (bool)reader[8];
                    clientCustom.RequirePkce = (bool)reader[9];
                    clientCustom.AllowPlainTextPkce = (bool)reader[10];
                    clientCustom.AllowAccessTokensViaBrowser = (bool)reader[11];
                    clientCustom.LogoutUri = reader[12].ToString();
                    clientCustom.LogoutSessionRequired = (bool)reader[13];
                    clientCustom.AllowOfflineAccess = (bool)reader[14];
                    clientCustom.AlwaysIncludeUserClaimsInIdToken = (bool)reader[15];
                    clientCustom.IdentityTokenLifetime = (int)reader[16];
                    clientCustom.AccessTokenLifetime = (int)reader[17];
                    clientCustom.AuthorizationCodeLifetime = (int)reader[18];
                    clientCustom.AbsoluteRefreshTokenLifetime = (int)reader[19];
                    clientCustom.SlidingRefreshTokenLifetime = (int)reader[20];
                    clientCustom.RefreshTokenUsage = (TokenUsage)reader[21];
                    clientCustom.UpdateAccessTokenClaimsOnRefresh = (bool)reader[22];
                    clientCustom.RefreshTokenExpiration = (TokenExpiration)reader[23];
                    clientCustom.AccessTokenType = (AccessTokenType)reader[24];
                    clientCustom.EnableLocalLogin = (bool)reader[25];
                    clientCustom.IncludeJwtId = (bool)reader[26];
                    clientCustom.AlwaysSendClientClaims = (bool)reader[27];
                    clientCustom.PrefixClientClaims = (bool)reader[28];
                    clientCustom.Enabled = (bool)reader[29];
                    clients.Add(clientCustom);
                }
            }
            total = GetClientsQueryFormatterCount(search);
            return new KeyValuePair<int, IQueryable<ClientCustom>>(total, clients.AsQueryable());
        }

        private int GetClientsQueryFormatterCount(string search)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                string query = "SELECT COUNT(Id) FROM clients";
                if (!string.IsNullOrEmpty(search))
                {
                    search = search.Trim();
                    if (search.Trim().Length > 0)
                    {
                        query += " WHERE ClientId LIKE '" + search + "%' OR ClientName LIKE '" + search + "%'";
                    }
                }
                query += " LIMIT 1";
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, query, null);
                while (reader.Read())
                {
                    return Convert.ToInt32(reader[0]);
                }
            }
            return 0;
        }

        public void Update(ClientCustom clientCustom)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                     { "@Id", clientCustom.Id },
                    { "@ClientId", clientCustom.ClientId },
                    { "@ProtocolType", clientCustom.ProtocolType },
                    { "@RequireClientSecret", clientCustom.RequireClientSecret },
                    { "@ClientName", clientCustom.ClientName ?? string.Empty },
                    { "@ClientUri", clientCustom.ClientUri ?? string.Empty },
                    { "@LogoUri", clientCustom.LogoUri ?? string.Empty },
                    { "@RequireConsent", clientCustom.RequireConsent },
                    { "@AllowRememberConsent", clientCustom.AllowRememberConsent },
                    { "@RequirePkce", clientCustom.RequirePkce },
                    { "@AllowPlainTextPkce", clientCustom.AllowPlainTextPkce },
                    { "@AllowAccessTokensViaBrowser", clientCustom.AllowAccessTokensViaBrowser },
                    { "@LogoutUri", clientCustom.LogoutUri ?? string.Empty },
                    { "@LogoutSessionRequired", clientCustom.LogoutSessionRequired },
                    { "@AllowOfflineAccess", clientCustom.AllowOfflineAccess },
                    { "@AlwaysIncludeUserClaimsInIdToken", clientCustom.AlwaysIncludeUserClaimsInIdToken },
                    { "@IdentityTokenLifetime", clientCustom.IdentityTokenLifetime },
                    { "@AccessTokenLifetime", clientCustom.AccessTokenLifetime },
                    { "@AuthorizationCodeLifetime", clientCustom.AuthorizationCodeLifetime },
                    { "@AbsoluteRefreshTokenLifetime", clientCustom.AbsoluteRefreshTokenLifetime },
                    { "@SlidingRefreshTokenLifetime", clientCustom.SlidingRefreshTokenLifetime },
                    { "@RefreshTokenUsage", clientCustom.RefreshTokenUsage },
                    { "@UpdateAccessTokenClaimsOnRefresh", clientCustom.UpdateAccessTokenClaimsOnRefresh },
                    { "@RefreshTokenExpiration", clientCustom.RefreshTokenExpiration },
                    { "@AccessTokenType", clientCustom.AccessTokenType },
                    { "@EnableLocalLogin", clientCustom.EnableLocalLogin },
                    { "@IncludeJwtId", clientCustom.IncludeJwtId },
                    { "@AlwaysSendClientClaims", clientCustom.AlwaysSendClientClaims },
                    { "@PrefixClientClaims", clientCustom.PrefixClientClaims },
                    { "@Enabled", clientCustom.Enabled }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"UPDATE clients SET Id = @Id, ClientId = @ClientId, ProtocolType = @ProtocolType, RequireClientSecret = RequireClientSecret,
                        ClientName = @ClientName, ClientUri = @ClientUri, LogoUri = @LogoUri, RequireConsent = @RequireConsent, AllowRememberConsent = @AllowRememberConsent,
                        RequirePkce = @RequirePkce, AllowPlainTextPkce = @AllowPlainTextPkce, AllowAccessTokensViaBrowser = @AllowAccessTokensViaBrowser, LogoutUri = @LogoutUri,
                        LogoutSessionRequired = @LogoutSessionRequired, AllowOfflineAccess = @AllowOfflineAccess, AlwaysIncludeUserClaimsInIdToken = @AlwaysIncludeUserClaimsInIdToken,
                        IdentityTokenLifetime = @IdentityTokenLifetime, AccessTokenLifetime = @AccessTokenLifetime, AuthorizationCodeLifetime = @AuthorizationCodeLifetime,
                        AbsoluteRefreshTokenLifetime = @AbsoluteRefreshTokenLifetime, SlidingRefreshTokenLifetime = @SlidingRefreshTokenLifetime,
                        RefreshTokenUsage = @RefreshTokenUsage, UpdateAccessTokenClaimsOnRefresh = @UpdateAccessTokenClaimsOnRefresh, RefreshTokenExpiration = @RefreshTokenExpiration,
                        AccessTokenType = @AccessTokenType, EnableLocalLogin = @EnableLocalLogin, IncludeJwtId = @IncludeJwtId,
                        AlwaysSendClientClaims = @AlwaysSendClientClaims, PrefixClientClaims = @PrefixClientClaims, Enabled = @Enabled WHERE Id = @Id", parameters);
            }
        }
    }
}