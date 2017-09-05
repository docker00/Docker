using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Identity;
using IdentityServer4.Models;

namespace MySql.AspNet.Identity.Repositories
{
    public class ObjectClientRepository
    {
        private readonly string _connectionString;

        public ObjectClientRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Insert(string objectId, string clientId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                string id = Guid.NewGuid().ToString();
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", id },
                    { "@ObjectId", objectId },
                    { "@ClientId", clientId }
                };
                MySqlHelper.ExecuteNonQuery(conn,
                    @"INSERT INTO object_client (Id, ObjectId, ClientId) VALUES (@Id, @ObjectId, @ClientId)", parameters);
            }
        }

        public Object GetObject(string clientId)
        {
            Object _object = null;
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ClientId", clientId }
                };
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT o.Id, o.Name, o.Description, o.Url FROM objects o, object_client oc 
                        WHERE o.Id = oc.ObjectId AND oc.ClientId = @ClientId", parameters);
                while (reader.Read())
                {
                    _object = (Object)Activator.CreateInstance(typeof(Object));
                    _object.Id = reader[0].ToString();
                    _object.Name = reader[1].ToString();
                    _object.Description = reader[2].ToString();
                    _object.Url = reader[3].ToString();
                }
            }
            return _object;
        }

        public ClientCustom GetClient(string objectId)
        {
            ClientCustom clientCustom = null;
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ObjectId", objectId }
                };
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT c.Id, c.ClientId, c.ProtocolType, c.RequireClientSecret, c.ClientName, c.ClientUri, c.LogoUri, c.RequireConsent, c.AllowRememberConsent, c.RequirePkce,
                        c.AllowPlainTextPkce, c.AllowAccessTokensViaBrowser, c.LogoutUri, c.LogoutSessionRequired, c.AllowOfflineAccess, c.AlwaysIncludeUserClaimsInIdToken,
                        c.IdentityTokenLifetime, c.AccessTokenLifetime, c.AuthorizationCodeLifetime, c.AbsoluteRefreshTokenLifetime, c.SlidingRefreshTokenLifetime,
                        c.RefreshTokenUsage, c.UpdateAccessTokenClaimsOnRefresh, c.RefreshTokenExpiration, c.AccessTokenType, c.EnableLocalLogin, c.IncludeJwtId,
                        c.AlwaysSendClientClaims, c.PrefixClientClaims, c.Enabled FROM clients c, object_client oc 
                        WHERE c.Id = oc.ClientId AND oc.ObjectId = @ObjectId", parameters);
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
