using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Identity;

namespace MySql.AspNet.Identity.Repositories
{
    public class ClientAllowedCorsOriginRepository
    {
        private readonly string _connectionString;
        private readonly AllowedCorsOriginRepository _allowedCorsOriginRepository;
        public ClientAllowedCorsOriginRepository(string connectionString)
        {
            _connectionString = connectionString;

            _allowedCorsOriginRepository = new AllowedCorsOriginRepository(_connectionString);
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
                    { "@AllowedCorsOriginId", allowedCorsOriginId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO client_allowed_cors_origins (Id, ClientId, AllowedCorsOriginId) VALUES(@Id, @ClientId, @AllowedCorsOriginId)", parameters);
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

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM client_allowed_cors_origins WHERE Id = @Id", parameters);
            }
        }

        public void Delete(string clientId, string allowedCorsOriginId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ClientId", clientId },
                    { "@AllowedCorsOriginId", allowedCorsOriginId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM client_allowed_cors_origins WHERE ClientId = @ClientId AND AllowedCorsOriginId = @AllowedCorsOriginId", parameters);
            }
        }

        public List<AllowedCorsOrigin> PopulateClientAllowedCorsOrigins(string clientId)
        {
            List<AllowedCorsOrigin> allowedCorsOrigins = new List<AllowedCorsOrigin>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ClientId", clientId }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT aco.Id, aco.Name 
                    FROM allowed_cors_origins aco
                      JOIN client_allowed_cors_origins caco
                        ON caco.ClientId = @ClientId
                        AND caco.AllowedCorsOriginId = aco.Id", parameters);
                while (reader.Read())
                {
                    AllowedCorsOrigin allowedCorsOrigin = (AllowedCorsOrigin)Activator.CreateInstance(typeof(AllowedCorsOrigin));

                    allowedCorsOrigin.Id = reader[0].ToString();
                    allowedCorsOrigin.Name = reader[1].ToString();

                    allowedCorsOrigins.Add(allowedCorsOrigin);
                }
            }

            return allowedCorsOrigins;
        }
    }
}
