using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Identity;
using IdentityServer4.Models;
using System.Linq;

namespace MySql.AspNet.Identity.Repositories
{
    public class ClientRelationRepository
    {
        private readonly string _connectionString;
        public ClientRelationRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Insert(string fromClientId, string toClientId)
        {
            string id = Guid.NewGuid().ToString();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", id },
                    { "@FromClientId", fromClientId },
                    { "@ToClientId", toClientId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO client_relations (Id, FromClientId, ToClientId) VALUES(@Id, @FromClientId, @ToClientId)", parameters);
            }
        }

        public void Delete(string id)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", id }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM client_relations WHERE Id = @Id", parameters);
            }
        }

        public void Delete(string fromClientId, string toClientId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@FromClientId", fromClientId },
                    { "@ToClientId", toClientId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM client_relations WHERE FromClientId = @FromClientId AND ToClientId = @ToClientId", parameters);
            }
        }

        public List<ClientCustom> PopulateClientsByFromClientId(string fromClientId)
        {
            List<ClientCustom> clients = new List<ClientCustom>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@FromClientId", fromClientId }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT
                        c.Id, c.ClientId, c.ProtocolType, c.RequireClientSecret, c.ClientName, c.ClientUri, c.LogoUri, c.RequireConsent, c.AllowRememberConsent, c.RequirePkce,
                        AllowPlainTextPkce, c.AllowAccessTokensViaBrowser, c.LogoutUri, c.LogoutSessionRequired, c.AllowOfflineAccess, c.AlwaysIncludeUserClaimsInIdToken,
                        IdentityTokenLifetime, c.AccessTokenLifetime, c.AuthorizationCodeLifetime, c.AbsoluteRefreshTokenLifetime, c.SlidingRefreshTokenLifetime,
                        RefreshTokenUsage, c.UpdateAccessTokenClaimsOnRefresh, c.RefreshTokenExpiration, c.AccessTokenType, c.EnableLocalLogin, c.IncludeJwtId,
                        AlwaysSendClientClaims, c.PrefixClientClaims, c.Enabled FROM clients c JOIN client_relations cr ON cr.FromClientId = @FromClientId AND c.Id = cr.ToClientId",
                        parameters);
                while (reader.Read())
                {
                    ClientCustom client = (ClientCustom)Activator.CreateInstance(typeof(ClientCustom));

                    client.Id = reader[0].ToString();
                    client.ClientId = reader[1].ToString();
                    client.ProtocolType = reader[2].ToString();
                    client.RequireClientSecret = (bool)reader[3];
                    client.ClientName = reader[4].ToString();
                    client.ClientUri = reader[5].ToString();
                    client.LogoUri = reader[6].ToString();
                    client.RequireConsent = (bool)reader[7];
                    client.AllowRememberConsent = (bool)reader[8];
                    client.RequirePkce = (bool)reader[9];
                    client.AllowPlainTextPkce = (bool)reader[10];
                    client.AllowAccessTokensViaBrowser = (bool)reader[11];
                    client.LogoutUri = reader[12].ToString();
                    client.LogoutSessionRequired = (bool)reader[13];
                    client.AllowOfflineAccess = (bool)reader[14];
                    client.AlwaysIncludeUserClaimsInIdToken = (bool)reader[15];
                    client.IdentityTokenLifetime = (int)reader[16];
                    client.AccessTokenLifetime = (int)reader[17];
                    client.AuthorizationCodeLifetime = (int)reader[18];
                    client.AbsoluteRefreshTokenLifetime = (int)reader[19];
                    client.SlidingRefreshTokenLifetime = (int)reader[20];
                    client.RefreshTokenUsage = (TokenUsage)reader[21];
                    client.UpdateAccessTokenClaimsOnRefresh = (bool)reader[22];
                    client.RefreshTokenExpiration = (TokenExpiration)reader[23];
                    client.AccessTokenType = (AccessTokenType)reader[24];
                    client.EnableLocalLogin = (bool)reader[25];
                    client.IncludeJwtId = (bool)reader[26];
                    client.AlwaysSendClientClaims = (bool)reader[27];
                    client.PrefixClientClaims = (bool)reader[28];
                    client.Enabled = (bool)reader[29];

                    clients.Add(client);
                }
            }

            return clients;
        }

        public bool ClientRelationsCheck(string fromClientId, string toClientId)
        {
            List<ClientCustom> clients = new List<ClientCustom>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@FromClientId", fromClientId },
                    { "@ToClientId", toClientId }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"
                    SELECT
                      COUNT(*)
                    FROM client_relations cr
                      JOIN clients c
                        ON c.ClientId = @FromClientId
                        AND c.Id = cr.FromClientId
                      JOIN clients c1
                        ON c1.ClientId = @ToClientId
                        AND c1.Id = cr.ToClientId", parameters);
                while (reader.Read())
                {
                    return Convert.ToInt32(reader[0]) > 0;
                }
            }

            return false;
        }

        public ClientCustom Get(string fromClientId, string toClientId)
        {
            ClientCustom clientCustom = null;
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@FromClientId", fromClientId },
                    { "@ToClientId", toClientId }
                };
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT
                        c.Id, c.ClientId, c.ProtocolType, c.RequireClientSecret, c.ClientName, c.ClientUri, c.LogoUri, c.RequireConsent, c.AllowRememberConsent, c.RequirePkce,
                        AllowPlainTextPkce, c.AllowAccessTokensViaBrowser, c.LogoutUri, c.LogoutSessionRequired, c.AllowOfflineAccess, c.AlwaysIncludeUserClaimsInIdToken,
                        IdentityTokenLifetime, c.AccessTokenLifetime, c.AuthorizationCodeLifetime, c.AbsoluteRefreshTokenLifetime, c.SlidingRefreshTokenLifetime,
                        RefreshTokenUsage, c.UpdateAccessTokenClaimsOnRefresh, c.RefreshTokenExpiration, c.AccessTokenType, c.EnableLocalLogin, c.IncludeJwtId,
                        AlwaysSendClientClaims, c.PrefixClientClaims, c.Enabled FROM clients c JOIN client_relations cr ON cr.FromClientId = @FromClientId AND cr.ToClientId = @ToClientId AND c.Id = cr.ToClientId", 
                        parameters);

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
                }

            }
            return clientCustom;
        }
    }
}
