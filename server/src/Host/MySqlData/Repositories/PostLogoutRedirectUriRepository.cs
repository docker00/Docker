using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using IdentityServer4.Models;

namespace MySql.AspNet.Identity.Repositories
{
    public class PostLogoutRedirectUriRepository
    {
        private readonly string _connectionString;
        public PostLogoutRedirectUriRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IQueryable<PostLogoutRedirectUri> GetPostLogoutRedirectUris()
        {
            List<PostLogoutRedirectUri> postLogoutRedirectUris = new List<PostLogoutRedirectUri>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Name FROM post_logout_redirect_uris", null);

                while (reader.Read())
                {
                    PostLogoutRedirectUri postLogoutRedirectUri = (PostLogoutRedirectUri)Activator.CreateInstance(typeof(PostLogoutRedirectUri));

                    postLogoutRedirectUri.Id = reader[0].ToString();
                    postLogoutRedirectUri.Name = reader[1].ToString();

                    postLogoutRedirectUris.Add(postLogoutRedirectUri);
                }

            }
            return postLogoutRedirectUris.AsQueryable();
        }

        public void Insert(PostLogoutRedirectUri postLogoutRedirectUri)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", postLogoutRedirectUri.Id },
                    { "@Name", postLogoutRedirectUri.Name }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO post_logout_redirect_uris (Id, Name) VALUES (@Id, @Name)", parameters);
            }
        }

        public void Delete(string postLogoutRedirectUriId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", postLogoutRedirectUriId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM post_logout_redirect_uris WHERE Id = @Id", parameters);
            }
        }

        public PostLogoutRedirectUri GetPostLogoutRedirectUriById(string postLogoutRedirectUriId)
        {
            PostLogoutRedirectUri postLogoutRedirectUri = null;

            if (!string.IsNullOrEmpty(postLogoutRedirectUriId))
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "@Id", postLogoutRedirectUriId }
                    };

                    IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Name FROM post_logout_redirect_uris WHERE Id = @Id", parameters);
                    while (reader.Read())
                    {
                        postLogoutRedirectUri = (PostLogoutRedirectUri)Activator.CreateInstance(typeof(PostLogoutRedirectUri));

                        postLogoutRedirectUri.Id = reader[0].ToString();
                        postLogoutRedirectUri.Name = reader[1].ToString();
                    }
                }
            }

            return postLogoutRedirectUri;
        }

        public PostLogoutRedirectUri GetPostLogoutRedirectUriByName(string postLogoutRedirectUriName)
        {
            PostLogoutRedirectUri postLogoutRedirectUri = null;

            if (!string.IsNullOrEmpty(postLogoutRedirectUriName))
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "@Name", postLogoutRedirectUriName }
                    };

                    IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Name FROM post_logout_redirect_uris WHERE Name = @Name", parameters);
                    while (reader.Read())
                    {
                        postLogoutRedirectUri = (PostLogoutRedirectUri)Activator.CreateInstance(typeof(PostLogoutRedirectUri));

                        postLogoutRedirectUri.Id = reader[0].ToString();
                        postLogoutRedirectUri.Name = reader[1].ToString();
                    }
                }
            }

            return postLogoutRedirectUri;
        }

        public KeyValuePair<int, IQueryable<PostLogoutRedirectUri>> GetRedirectsQueryFormatter(string order, int limit, int offset, string sort, string search)
        {
            string query = "SELECT Id, Name FROM post_logout_redirect_uris";
            int total = 0;
            List<PostLogoutRedirectUri> redirects = new List<PostLogoutRedirectUri>();
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
                if (search.Length > 0)
                {
                    query += " WHERE Name LIKE '" + search + "%'";
                }
            }
            if (!string.IsNullOrEmpty(sort))
            {
                query += " ORDER BY " + sort + " " + order;
            }
            query += " LIMIT " + offset + "," + limit;
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, query, null);
                while (reader.Read())
                {
                    PostLogoutRedirectUri redirect = (PostLogoutRedirectUri)Activator.CreateInstance(typeof(PostLogoutRedirectUri));
                    redirect.Id = reader[0].ToString();
                    redirect.Name = reader[1].ToString();
                    redirects.Add(redirect);
                }
            }
            total = GetRedirectsQueryFormatterCount(search);
            return new KeyValuePair<int, IQueryable<PostLogoutRedirectUri>>(total, redirects.AsQueryable());
        }

        private int GetRedirectsQueryFormatterCount(string search)
        {
            string query = "SELECT COUNT(Id) FROM post_logout_redirect_uris";
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
                if (search.Length > 0)
                {
                    query += " WHERE Name LIKE '" + search + "%'";
                }
            }
            query += " LIMIT 1";
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, query, null);
                while (reader.Read())
                {
                    return Convert.ToInt32(reader[0]);
                }
            }
            return 0;
        }

        public void Update(PostLogoutRedirectUri postLogoutRedirectUri)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", postLogoutRedirectUri.Id },
                    { "@Name", postLogoutRedirectUri.Name }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"UPDATE post_logout_redirect_uris SET Name = @Name WHERE Id = @Id", parameters);
            }
        }
    }
}
