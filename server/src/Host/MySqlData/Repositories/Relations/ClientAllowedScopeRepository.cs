using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Identity;

namespace MySql.AspNet.Identity.Repositories
{
    public class ClientAllowedScopeRepository
    {
        private readonly string _connectionString;
        public ClientAllowedScopeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Insert(string clientId, string identityResourceId)
        {
            string id = Guid.NewGuid().ToString();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", id },
                    { "@ClientId", clientId },
                    { "@IdentityResourceId", identityResourceId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO client_allowed_identity_resource_scopes (Id, ClientId, IdentityResourceId) VALUES(@Id, @ClientId, @IdentityResourceId)", parameters);
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

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM client_allowed_identity_resource_scopes WHERE Id = @Id", parameters);
            }
        }

        public void Delete(string clientId, string identityResourceId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ClientId", clientId },
                    { "@IdentityResourceId", identityResourceId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM client_allowed_identity_resource_scopes WHERE ClientId = @ClientId AND IdentityResourceId = @IdentityResourceId", parameters);
            }
        }

        public List<string> PopulateClientIdentityResourceScopes(string clientId)
        {
            List<string> clientIdentityResourceScopes = new List<string>() { "access_token_api" };
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ClientId", clientId }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT
                      ir.Name
                    FROM client_allowed_identity_resource_scopes cars
                      JOIN identity_resources ir
                        ON ir.Id = cars.IdentityResourceId
                    WHERE cars.ClientId = @ClientId", parameters);
                while (reader.Read())
                {
                    clientIdentityResourceScopes.Add(reader[0].ToString());
                }
            }

            return clientIdentityResourceScopes;
        }
    }
}
