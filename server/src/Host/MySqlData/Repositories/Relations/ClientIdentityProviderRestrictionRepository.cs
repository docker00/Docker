using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace MySql.AspNet.Identity.Repositories
{
    public class ClientIdentityProviderRestrictionRepository<TIdentityProviderRestriction> where TIdentityProviderRestriction : IdentityProviderRestriction
    {
        private readonly string _connectionString;
        private readonly IdentityProviderRestrictionRepository _identityProviderRestrictionRepository;
        public ClientIdentityProviderRestrictionRepository(string connectionString)
        {
            _connectionString = connectionString;

            _identityProviderRestrictionRepository = new IdentityProviderRestrictionRepository(_connectionString);
        }

        public void Insert(string clientId, string identityProviderRestrictionId)
        {
            string id = Guid.NewGuid().ToString();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", id },
                    { "@ClientId", clientId },
                    { "@IdentityProviderRestrictionId", identityProviderRestrictionId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO client_identity_provider_restrictions (Id, ClientId, IdentityProviderRestrictionId) VALUES(@Id, @ClientId, @IdentityProviderRestrictionId)", parameters);
            }
        }

        public void Update(ClientIdentityProviderRestriction clentIdentityProviderRestriction)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", clentIdentityProviderRestriction.Id },
                    { "@ClientId", clentIdentityProviderRestriction.ClientId },
                    { "@IdentityProviderRestrictionId", clentIdentityProviderRestriction.IdentityProviderRestrictionId }
                };
                MySqlHelper.ExecuteNonQuery(conn,
                    @"UPDATE client_identity_provider_restrictions 
                    SET ClientId = @ClientId, IdentityProviderRestrictionId = @IdentityProviderRestrictionId WHERE Id = @Id",
                    parameters);
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

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM client_identity_provider_restrictions WHERE Id = @Id", parameters);
            }
        }

        public void Delete(string clientId, string identityProviderRestrictionId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ClientId", clientId },
                    { "@IdentityProviderRestrictionId", identityProviderRestrictionId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM client_identity_provider_restrictions WHERE ClientId = @ClientId AND IdentityProviderRestrictionId = @IdentityProviderRestrictionId", parameters);
            }
        }

        public List<IdentityProviderRestriction> PopulateClientIdentityProviderRestrictions(string clientId)
        {
            List<IdentityProviderRestriction> identityProviderRestrictions = new List<IdentityProviderRestriction>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ClientId", clientId }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT ipr.Id, ipr.Name 
                    FROM identity_provider_restrictions ipr
                      JOIN client_identity_provider_restrictions cipr
                        ON cipr.ClientId = @ClientId
                        AND cipr.IdentityProviderRestrictionId = ipr.Id", parameters);
                while (reader.Read())
                {
                    IdentityProviderRestriction identityProviderRestriction = (IdentityProviderRestriction)Activator.CreateInstance(typeof(IdentityProviderRestriction));

                    identityProviderRestriction.Id = reader[0].ToString();
                    identityProviderRestriction.Name = reader[1].ToString();

                    identityProviderRestrictions.Add(identityProviderRestriction);
                }
            }

            return identityProviderRestrictions;
        }
    }
}
