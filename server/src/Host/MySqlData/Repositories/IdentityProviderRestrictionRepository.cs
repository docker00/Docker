using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using IdentityServer4.Models;

namespace MySql.AspNet.Identity.Repositories
{
    public class IdentityProviderRestrictionRepository
    {
        private readonly string _connectionString;
        public IdentityProviderRestrictionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IQueryable<IdentityProviderRestriction> GetIdentityProviderRestrictions()
        {
            List<IdentityProviderRestriction> identityProviderRestrictions = new List<IdentityProviderRestriction>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Name FROM identity_provider_restrictions", null);

                while (reader.Read())
                {
                    IdentityProviderRestriction identityProviderRestriction = (IdentityProviderRestriction)Activator.CreateInstance(typeof(IdentityProviderRestriction));

                    identityProviderRestriction.Id = reader[0].ToString();
                    identityProviderRestriction.Name = reader[1].ToString();

                    identityProviderRestrictions.Add(identityProviderRestriction);
                }

            }
            return identityProviderRestrictions.AsQueryable();
        }

        public void Insert(IdentityProviderRestriction identityProviderRestriction)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", identityProviderRestriction.Id },
                    { "@Name", identityProviderRestriction.Name }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO identity_provider_restrictions (Id, Name) VALUES (@Id, @Name)", parameters);
            }
        }

        public void Delete(string identityProviderRestrictionId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", identityProviderRestrictionId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM identity_provider_restrictions WHERE Id = @Id", parameters);
            }
        }

        public IdentityProviderRestriction GetIdentityProviderRestrictionById(string identityProviderRestrictionId)
        {
            IdentityProviderRestriction identityProviderRestriction = null;

            if (!string.IsNullOrEmpty(identityProviderRestrictionId))
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "@Id", identityProviderRestrictionId }
                    };

                    IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Name FROM identity_provider_restrictions WHERE Id = @Id", parameters);
                    while (reader.Read())
                    {
                        identityProviderRestriction = (IdentityProviderRestriction)Activator.CreateInstance(typeof(IdentityProviderRestriction));

                        identityProviderRestriction.Id = reader[0].ToString();
                        identityProviderRestriction.Name = reader[1].ToString();
                    }
                }
            }

            return identityProviderRestriction;
        }

        public IdentityProviderRestriction GetIdentityProviderRestrictionByName(string identityProviderRestrictionName)
        {
            IdentityProviderRestriction identityProviderRestriction = null;

            if (!string.IsNullOrEmpty(identityProviderRestrictionName))
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "@Name", identityProviderRestrictionName }
                    };

                    IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Name FROM identity_provider_restrictions WHERE Name = @Name", parameters);
                    while (reader.Read())
                    {
                        identityProviderRestriction = (IdentityProviderRestriction)Activator.CreateInstance(typeof(IdentityProviderRestriction));

                        identityProviderRestriction.Id = reader[0].ToString();
                        identityProviderRestriction.Name = reader[1].ToString();
                    }
                }
            }

            return identityProviderRestriction;
        }

        public void Update(IdentityProviderRestriction identityProviderRestriction)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", identityProviderRestriction.Id },
                    { "@Name", identityProviderRestriction.Name }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"UPDATE identity_provider_restrictions SET Name = @Name WHERE Id = @Id", parameters);
            }
        }
    }
}
