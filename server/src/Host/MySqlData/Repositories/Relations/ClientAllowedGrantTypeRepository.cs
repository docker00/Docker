using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Identity;

namespace MySql.AspNet.Identity.Repositories
{
    public class ClientAllowedGrantTypeRepository
    {
        private readonly string _connectionString;
        private readonly AllowedGrantTypeRepository _allowedGrantTypeRepository;
        public ClientAllowedGrantTypeRepository(string connectionString)
        {
            _connectionString = connectionString;

            _allowedGrantTypeRepository = new AllowedGrantTypeRepository(_connectionString);
        }

        public void Insert(string clientId, string allowedGrantTypeId)
        {
            string id = Guid.NewGuid().ToString();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", id },
                    { "@ClientId", clientId },
                    { "@AllowedGrantTypeId", allowedGrantTypeId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO client_allowed_grant_types (Id, ClientId, AllowedGrantTypeId) VALUES(@Id, @ClientId, @AllowedGrantTypeId)", parameters);
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

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM client_allowed_grant_types WHERE Id = @Id", parameters);
            }
        }

        public void Delete(string clientId, string allowedGrantTypeId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ClientId", clientId },
                    { "@AllowedGrantTypeId", allowedGrantTypeId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM client_allowed_grant_types WHERE ClientId = @ClientId AND AllowedGrantTypeId = @AllowedGrantTypeId", parameters);
            }
        }

        public List<AllowedGrantType> PopulateClientAllowedGrantTypes(string clientId)
        {
            List<AllowedGrantType> allowedGrantTypes = new List<AllowedGrantType>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ClientId", clientId }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT agt.Id, agt.Name 
                    FROM allowed_grant_types agt
                      JOIN client_allowed_grant_types cagt
                        ON cagt.ClientId = @ClientId
                        AND cagt.AllowedGrantTypeId = agt.Id", parameters);
                while (reader.Read())
                {
                    AllowedGrantType allowedGrantType = (AllowedGrantType)Activator.CreateInstance(typeof(AllowedGrantType));

                    allowedGrantType.Id = reader[0].ToString();
                    allowedGrantType.Name = reader[1].ToString();

                    allowedGrantTypes.Add(allowedGrantType);
                }
            }

            return allowedGrantTypes;
        }
    }
}
