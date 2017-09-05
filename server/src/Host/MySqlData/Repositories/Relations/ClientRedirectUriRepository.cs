using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Identity;

namespace MySql.AspNet.Identity.Repositories
{
    public class ClientRedirectUriRepository
    {
        private readonly string _connectionString;
        private readonly RedirectUriRepository _redirectUriRepository;
        public ClientRedirectUriRepository(string connectionString)
        {
            _connectionString = connectionString;

            _redirectUriRepository = new RedirectUriRepository(_connectionString);
        }

        public void Insert(string clientId, string redirectUriId)
        {
            string id = Guid.NewGuid().ToString();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", id },
                    { "@ClientId", clientId },
                    { "@RedirectUriId", redirectUriId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO client_redirect_uris (Id, ClientId, RedirectUriId) VALUES(@Id, @ClientId, @RedirectUriId)", parameters);
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

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM client_redirect_uris WHERE Id = @Id", parameters);
            }
        }

        public void Delete(string clientId, string redirectUriId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ClientId", clientId },
                    { "@RedirectUriId", redirectUriId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM client_redirect_uris WHERE ClientId = @ClientId AND RedirectUriId = @RedirectUriId", parameters);
            }
        }

        public List<RedirectUri> PopulateClientRedirectUris(string clientId)
        {
            List<RedirectUri> redirectUris = new List<RedirectUri>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ClientId", clientId }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT ru.Id, ru.Name 
                    FROM redirect_uris ru
                      JOIN client_redirect_uris cru
                        ON cru.ClientId = @ClientId
                        AND cru.RedirectUriId = ru.Id", parameters);
                while (reader.Read())
                {
                    RedirectUri redirectUri = (RedirectUri)Activator.CreateInstance(typeof(RedirectUri));

                    redirectUri.Id = reader[0].ToString();
                    redirectUri.Name = reader[1].ToString();

                    redirectUris.Add(redirectUri);
                }
            }

            return redirectUris;
        }
    }
}
