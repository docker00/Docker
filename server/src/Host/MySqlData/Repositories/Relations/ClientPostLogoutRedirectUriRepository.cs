using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Identity;

namespace MySql.AspNet.Identity.Repositories
{
    public class ClientPostLogoutRedirectUriRepository
    {
        private readonly string _connectionString;
        private readonly PostLogoutRedirectUriRepository _postLogoutRedirectUriRepository;
        public ClientPostLogoutRedirectUriRepository(string connectionString)
        {
            _connectionString = connectionString;

            _postLogoutRedirectUriRepository = new PostLogoutRedirectUriRepository(_connectionString);
        }

        public void Insert(string clientId, string postLogoutRedirectUriId)
        {
            string id = Guid.NewGuid().ToString();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", id },
                    { "@ClientId", clientId },
                    { "@PostLogoutRedirectUriId", postLogoutRedirectUriId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO client_post_logout_redirect_uris (Id, ClientId, PostLogoutRedirectUriId) VALUES(@Id, @ClientId, @PostLogoutRedirectUriId)", parameters);
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

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM client_post_logout_redirect_uris WHERE Id = @Id", parameters);
            }
        }

        public void Delete(string clientId, string postLogoutRedirectUriId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ClientId", clientId },
                    { "@PostLogoutRedirectUriId", postLogoutRedirectUriId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM client_post_logout_redirect_uris WHERE ClientId = @ClientId AND PostLogoutRedirectUriId = @PostLogoutRedirectUriId", parameters);
            }
        }

        public List<PostLogoutRedirectUri> PopulateClientPostLogoutRedirectUris(string clientId)
        {
            List<PostLogoutRedirectUri> postLogoutRedirectUris = new List<PostLogoutRedirectUri>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ClientId", clientId }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT plru.Id, plru.Name 
                    FROM post_logout_redirect_uris plru
                      JOIN client_post_logout_redirect_uris cplru
                        ON cplru.ClientId = @ClientId
                        AND cplru.PostLogoutRedirectUriId = plru.Id", parameters);
                while (reader.Read())
                {
                    PostLogoutRedirectUri postLogoutRedirectUri = (PostLogoutRedirectUri)Activator.CreateInstance(typeof(PostLogoutRedirectUri));

                    postLogoutRedirectUri.Id = reader[0].ToString();
                    postLogoutRedirectUri.Name = reader[1].ToString();

                    postLogoutRedirectUris.Add(postLogoutRedirectUri);
                }
            }

            return postLogoutRedirectUris;
        }
    }
}
