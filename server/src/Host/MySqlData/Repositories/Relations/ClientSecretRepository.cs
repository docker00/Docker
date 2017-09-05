using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Identity;
using IdentityServer4.Models;

namespace MySql.AspNet.Identity.Repositories
{
    public class ClientSecretRepository
    {
        private readonly string _connectionString;
        private readonly SecretCustomRepository _secretCustomRepository;
        public ClientSecretRepository(string connectionString)
        {
            _connectionString = connectionString;

            _secretCustomRepository = new SecretCustomRepository(_connectionString);
        }

        public void Insert(string clientId, string allowedCorsOriginId)
        {
            string id = Guid.NewGuid().ToString();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", id },
                    { "@ClientId", clientId },
                    { "@SecretId", allowedCorsOriginId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO client_secrets (Id, ClientId, SecretId) VALUES(@Id, @ClientId, @SecretId)", parameters);
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

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM client_secrets WHERE Id = @Id", parameters);
            }
        }

        public void Delete(string clientId, string allowedCorsOriginId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ClientId", clientId },
                    { "@SecretId", allowedCorsOriginId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM client_secrets WHERE ClientId = @ClientId AND SecretId = @SecretId", parameters);
            }
        }

        public List<SecretCustom> PopulateClientSecretCustomsById(string id)
        {
            List<SecretCustom> secretCustoms = new List<SecretCustom>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", id }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT s.Id, s.Value, s.Type 
                    FROM secret s
                      JOIN client_secrets cs
                        ON cs.Id = @Id
                        AND cs.SecretId = s.Id", parameters);
                while (reader.Read())
                {
                    SecretCustom secretCustom = (SecretCustom)Activator.CreateInstance(typeof(SecretCustom));

                    secretCustom.Id = reader[0].ToString();
                    secretCustom.Value = reader[1].ToString();
                    secretCustom.Type = reader[2].ToString();

                    secretCustoms.Add(secretCustom);
                }
            }

            return secretCustoms;
        }

        public List<SecretCustom> PopulateClientSecretCustomsByClientId(string clientId)
        {
            List<SecretCustom> secretCustoms = new List<SecretCustom>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ClientId", clientId }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT s.Id, s.Value, s.Type 
                    FROM secret s
                      JOIN client_secrets cs
                        ON cs.ClientId = @ClientId
                        AND cs.SecretId = s.Id", parameters);
                while (reader.Read())
                {
                    SecretCustom secretCustom = (SecretCustom)Activator.CreateInstance(typeof(SecretCustom));

                    secretCustom.Id = reader[0].ToString();
                    secretCustom.Value = reader[1].ToString();
                    secretCustom.Type = reader[2].ToString();

                    secretCustoms.Add(secretCustom);
                }
            }

            return secretCustoms;
        }

        public List<Secret> PopulateClientSecretById(string id)
        {
            List<Secret> secretCustoms = new List<Secret>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", id }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT s.Id, s.Value, s.Type 
                    FROM secret s
                      JOIN client_secrets cs
                        ON cs.Id = @Id
                        AND cs.SecretId = s.Id", parameters);
                while (reader.Read())
                {
                    SecretCustom secretCustom = (SecretCustom)Activator.CreateInstance(typeof(SecretCustom));

                    secretCustom.Id = reader[0].ToString();
                    secretCustom.Value = reader[1].ToString();
                    secretCustom.Type = reader[2].ToString();

                    secretCustoms.Add(secretCustom);
                }
            }

            return secretCustoms;
        }

        public List<Secret> PopulateClientSecretByClientId(string clientId)
        {
            List<Secret> secretCustoms = new List<Secret>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ClientId", clientId }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT s.Id, s.Value, s.Type 
                    FROM secret s
                      JOIN client_secrets cs
                        ON cs.ClientId = @ClientId
                        AND cs.SecretId = s.Id", parameters);
                while (reader.Read())
                {
                    SecretCustom secretCustom = (SecretCustom)Activator.CreateInstance(typeof(SecretCustom));

                    secretCustom.Id = reader[0].ToString();
                    secretCustom.Value = reader[1].ToString();
                    secretCustom.Type = reader[2].ToString();

                    secretCustoms.Add(secretCustom);
                }
            }

            return secretCustoms;
        }
    }
}
