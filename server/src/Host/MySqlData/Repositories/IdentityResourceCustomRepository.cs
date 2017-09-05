using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using IdentityServer4.Models;

namespace MySql.AspNet.Identity.Repositories
{
    public class IdentityResourceCustomRepository
    {
        private readonly string _connectionString;
        private readonly IdentityResourceCustomClaimRepository _identityResourceCustomClaimRepository;
        public IdentityResourceCustomRepository(string connectionString)
        {
            _connectionString = connectionString;
            _identityResourceCustomClaimRepository = new IdentityResourceCustomClaimRepository(_connectionString);
        }

        public IQueryable<IdentityResourceCustom> GetIdentityResources()
        {
            List<IdentityResourceCustom> identityResourceCustoms = new List<IdentityResourceCustom>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Name, DisplayName, Description, Enabled, Required, Emphasize, ShowInDiscoveryDocument FROM identity_resources", null);

                while (reader.Read())
                {
                    IdentityResourceCustom identityResourceCustom = (IdentityResourceCustom)Activator.CreateInstance(typeof(IdentityResourceCustom));

                    identityResourceCustom.Id = reader[0].ToString();
                    identityResourceCustom.Name = reader[1].ToString();
                    identityResourceCustom.DisplayName = reader[2].ToString();
                    identityResourceCustom.Description = reader[3].ToString();
                    identityResourceCustom.Enabled = (bool)reader[4];
                    identityResourceCustom.Required = (bool)reader[5];
                    identityResourceCustom.Emphasize = (bool)reader[6];
                    identityResourceCustom.ShowInDiscoveryDocument = (bool)reader[7];

                    identityResourceCustom.UserClaims = _identityResourceCustomClaimRepository.PopulateIdentityResourceCustomClaims(identityResourceCustom.Id).Select(c => c.Type).ToList();

                    identityResourceCustoms.Add(identityResourceCustom);
                }

            }
            return identityResourceCustoms.AsQueryable();
        }

        public void Insert(IdentityResourceCustom identityResourceCustom)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", identityResourceCustom.Id },
                    { "@Name", identityResourceCustom.Name },
                    { "@DisplayName", identityResourceCustom.DisplayName ?? string.Empty },
                    { "@Description", identityResourceCustom.Description ?? string.Empty },
                    { "@Enabled", identityResourceCustom.Enabled },
                    { "@Required", identityResourceCustom.Required },
                    { "@Emphasize", identityResourceCustom.Emphasize },
                    { "@ShowInDiscoveryDocument", identityResourceCustom.ShowInDiscoveryDocument }

                };

                MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO identity_resources (Id, Name, DisplayName, Description, Enabled, Required, Emphasize, ShowInDiscoveryDocument) 
                    VALUES (@Id, @Name, @DisplayName, @Description, @Enabled, @Required, @Emphasize, @ShowInDiscoveryDocument)", parameters);
            }
        }

        public void Delete(string identityResourceCustomId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", identityResourceCustomId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM identity_resources WHERE Id = @Id", parameters);
            }
        }

        public IdentityResourceCustom GetIdentityResourceCustomById(string identityResourceCustomId)
        {
            IdentityResourceCustom identityResourceCustom = null;

            if (!string.IsNullOrEmpty(identityResourceCustomId))
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "@Id", identityResourceCustomId }
                    };

                    IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Name, DisplayName, Description, Enabled, Required, Emphasize, ShowInDiscoveryDocument FROM identity_resources WHERE Id = @Id", parameters);
                    while (reader.Read())
                    {
                        identityResourceCustom = (IdentityResourceCustom)Activator.CreateInstance(typeof(IdentityResourceCustom));

                        identityResourceCustom.Id = reader[0].ToString();
                        identityResourceCustom.Name = reader[1].ToString();
                        identityResourceCustom.DisplayName = reader[2].ToString();
                        identityResourceCustom.Description = reader[3].ToString();
                        identityResourceCustom.Enabled = (bool)reader[4];
                        identityResourceCustom.Required = (bool)reader[5];
                        identityResourceCustom.Emphasize = (bool)reader[6];
                        identityResourceCustom.ShowInDiscoveryDocument = (bool)reader[7];

                        identityResourceCustom.UserClaims = _identityResourceCustomClaimRepository.PopulateIdentityResourceCustomClaims(identityResourceCustom.Id).Select(c => c.Type).ToList();
                    }
                }
            }

            return identityResourceCustom;
        }

        public IdentityResourceCustom GetIdentityResourceCustomByName(string identityResourceCustomName)
        {
            IdentityResourceCustom identityResourceCustom = null;

            if (!string.IsNullOrEmpty(identityResourceCustomName))
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "@Name", identityResourceCustomName }
                    };

                    IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Name, DisplayName, Description, Enabled, Required, Emphasize, ShowInDiscoveryDocument FROM identity_resources WHERE Name = @Name", parameters);
                    while (reader.Read())
                    {
                        identityResourceCustom = (IdentityResourceCustom)Activator.CreateInstance(typeof(IdentityResourceCustom));

                        identityResourceCustom.Id = reader[0].ToString();
                        identityResourceCustom.Name = reader[1].ToString();
                        identityResourceCustom.DisplayName = reader[2].ToString();
                        identityResourceCustom.Description = reader[3].ToString();
                        identityResourceCustom.Enabled = (bool)reader[4];
                        identityResourceCustom.Required = (bool)reader[5];
                        identityResourceCustom.Emphasize = (bool)reader[6];
                        identityResourceCustom.ShowInDiscoveryDocument = (bool)reader[7];

                        identityResourceCustom.UserClaims = _identityResourceCustomClaimRepository.PopulateIdentityResourceCustomClaims(identityResourceCustom.Id).Select(c => c.Type).ToList();
                    }
                }
            }

            return identityResourceCustom;
        }

        public void Update(IdentityResourceCustom identityResourceCustom)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", identityResourceCustom.Id },
                    { "@Name", identityResourceCustom.Name },
                    { "@DisplayName", identityResourceCustom.DisplayName ?? string.Empty },
                    { "@Description", identityResourceCustom.Description ?? string.Empty },
                    { "@Enabled", identityResourceCustom.Enabled },
                    { "@Required", identityResourceCustom.Required },
                    { "@Emphasize", identityResourceCustom.Emphasize },
                    { "@ShowInDiscoveryDocument", identityResourceCustom.ShowInDiscoveryDocument }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"UPDATE identity_resources SET Name = @Name, DisplayName = @DisplayName, Description = @Description,
                        Enabled = @Enabled, Required = @Required, Emphasize = @Emphasize, ShowInDiscoveryDocument = @ShowInDiscoveryDocument WHERE Id = @Id", parameters);
            }
        }
    }
}
