using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MySql.AspNet.Identity.Repositories
{
    public class UserApiKeyRepository<TUserApiKey> where TUserApiKey : UserApiKey
    {
        private readonly string _connectionString;
        public UserApiKeyRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public TUserApiKey Get(string id)
        {
            TUserApiKey apiKey = null;
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", id}
                };
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT Id, UserId, ClientId, ApiKey, ExperienceTime FROM user_api_keys WHERE Id = @Id LIMIT 1", parameters);
                while (reader.Read())
                {
                    apiKey = (TUserApiKey)Activator.CreateInstance(typeof(TUserApiKey));
                    apiKey.Id = reader[0].ToString();
                    apiKey.UserId = reader[1].ToString();
                    apiKey.ClientId = reader[2].ToString();
                    apiKey.ApiKey = reader[3].ToString();
                    apiKey.ExperienceTime = Convert.ToDateTime(reader[4]);
                };
            }
            return apiKey;
        }

        public TUserApiKey Get(string userId, string clientId)
        {
            TUserApiKey apiKey = null;
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@UserId", userId},
                    { "@ClientId", clientId }
                };
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT Id, UserId, ClientId, ApiKey, ExperienceTime FROM user_api_keys WHERE UserId = @UserId AND ClientId = @ClientId LIMIT 1",
                    parameters);
                while (reader.Read())
                {
                    apiKey = (TUserApiKey)Activator.CreateInstance(typeof(TUserApiKey));
                    apiKey.Id = reader[0].ToString();
                    apiKey.UserId = reader[1].ToString();
                    apiKey.ClientId = reader[2].ToString();
                    apiKey.ApiKey = reader[3].ToString();
                    apiKey.ExperienceTime = Convert.ToDateTime(reader[4]);
                };
            }
            return apiKey;
        }

        public void Create(TUserApiKey apiKey)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", apiKey.Id },
                    { "@UserId", apiKey.UserId },
                    { "@ClientId", apiKey.ClientId },
                    { "@ApiKey", apiKey.ApiKey },
                    { "@ExperienceTime", apiKey.ExperienceTime }
                };
                MySqlHelper.ExecuteNonQuery(conn, CommandType.Text,
                    @"INSERT INTO user_api_keys (Id, UserId, ClientId, ApiKey, ExperienceTime) 
                        VALUES(@Id, @UserId, @ClientId, @ApiKey, @ExperienceTime)", parameters);
            }
        }

        public void Update(TUserApiKey apiKey)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", apiKey.Id },
                    { "@UserId", apiKey.UserId },
                    { "@ClientId", apiKey.ClientId},
                    { "@ApiKey", apiKey.ApiKey }
                };
                MySqlHelper.ExecuteNonQuery(conn, CommandType.Text,
                    @"UPDATE user_api_keys SET ApiKey = @ApiKey WHERE Id = @Id OR (UserId = @UserId AND ClientId = @ClientId) LIMIT 1",
                    parameters);
            }
        }

        public void Delete(TUserApiKey apiKey)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", apiKey.Id }
                };
                MySqlHelper.ExecuteNonQuery(conn, CommandType.Text,
                    @"DELETE FROM user_api_keys WHERE Id = @Id LIMIT 1",
                    parameters);
            }
        }

        public IQueryable<TUserApiKey> PopulateUserApiKeys(string userId)
        {
            List<TUserApiKey> userApiKeys = new List<TUserApiKey>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@UserId", userId}
                };
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT Id, UserId, ClientId, ApiKey, ExperienceTime FROM user_api_keys WHERE UserId = @UserId", parameters);
                while (reader.Read())
                {
                    TUserApiKey apiKey = (TUserApiKey)Activator.CreateInstance(typeof(TUserApiKey));
                    apiKey.Id = reader[0].ToString();
                    apiKey.UserId = reader[1].ToString();
                    apiKey.ClientId = reader[2].ToString();
                    apiKey.ApiKey = reader[3].ToString();
                    apiKey.ExperienceTime = Convert.ToDateTime(reader[4]);
                    userApiKeys.Add(apiKey);
                };
            }
            return userApiKeys.AsQueryable();
        }
    }
}
