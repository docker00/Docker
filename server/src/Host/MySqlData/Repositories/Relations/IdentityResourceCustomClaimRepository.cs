using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace MySql.AspNet.Identity.Repositories
{
    public class IdentityResourceCustomClaimRepository
    {
        private readonly string _connectionString;
        private readonly ClaimCustomRepository _claimCustomRepository;
        public IdentityResourceCustomClaimRepository(string connectionString)
        {
            _connectionString = connectionString;

            _claimCustomRepository = new ClaimCustomRepository(_connectionString);
        }

        public void Insert(string identityResourceId, string claimId)
        {
            string id = Guid.NewGuid().ToString();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", id },
                    { "@IdentityResourceId", identityResourceId },
                    { "@ClaimId", claimId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO identity_resource_claims (Id, IdentityResourceId, ClaimId) VALUES(@Id, @IdentityResourceId, @ClaimId)", parameters);
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

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM identity_resource_claims WHERE Id = @Id", parameters);
            }
        }

        public void Delete(string identityResourceId, string claimId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@IdentityResourceId", identityResourceId },
                    { "@ClaimId", claimId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM identity_resource_claims WHERE IdentityResourceId = @IdentityResourceId AND ClaimId = @ClaimId", parameters);
            }
        }

        public List<ClaimCustom> PopulateIdentityResourceCustomClaims(string identityResourceId)
        {
            List<ClaimCustom> claims = new List<ClaimCustom>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@IdentityResourceId", identityResourceId }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                 @"SELECT c.Id, c.Type
                    FROM claims c
                      JOIN identity_resource_claims irc
                        ON irc.IdentityResourceId = @IdentityResourceId
                        AND irc.ClaimId = c.Id", parameters);
                while (reader.Read())
                {
                    ClaimCustom _claimCustom = new ClaimCustom(reader[1].ToString(), "");
                    _claimCustom.Id = reader[0].ToString();

                    claims.Add(_claimCustom);
                }
            }

            return claims;
        }
    }
}
