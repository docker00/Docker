using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using IdentityServer4.Models;

namespace MySql.AspNet.Identity.Repositories
{
    public class RedirectUriRepository
    {
        private readonly string _connectionString;
        public RedirectUriRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IQueryable<RedirectUri> GetRedirectUris()
        {
            List<RedirectUri> redirectUris = new List<RedirectUri>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Name FROM redirect_uris", null);

                while (reader.Read())
                {
                    RedirectUri redirectUri = (RedirectUri)Activator.CreateInstance(typeof(RedirectUri));

                    redirectUri.Id = reader[0].ToString();
                    redirectUri.Name = reader[1].ToString();

                    redirectUris.Add(redirectUri);
                }

            }
            return redirectUris.AsQueryable();
        }

        public void Insert(RedirectUri redirectUri)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", redirectUri.Id },
                    { "@Name", redirectUri.Name }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO redirect_uris (Id, Name) VALUES (@Id, @Name)", parameters);
            }
        }

        public void Delete(string redirectUriId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", redirectUriId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM redirect_uris WHERE Id = @Id", parameters);
            }
        }

        public RedirectUri GetRedirectUriById(string redirectUriId)
        {
            RedirectUri redirectUri = null;

            if (!string.IsNullOrEmpty(redirectUriId))
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "@Id", redirectUriId }
                    };

                    IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Name FROM redirect_uris WHERE Id = @Id", parameters);
                    while (reader.Read())
                    {
                        redirectUri = (RedirectUri)Activator.CreateInstance(typeof(RedirectUri));

                        redirectUri.Id = reader[0].ToString();
                        redirectUri.Name = reader[1].ToString();
                    }
                }
            }

            return redirectUri;
        }

        public RedirectUri GetRedirectUriByName(string redirectUriName)
        {
            RedirectUri redirectUri = null;

            if (!string.IsNullOrEmpty(redirectUriName))
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "@Name", redirectUriName }
                    };

                    IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Name FROM redirect_uris WHERE Name = @Name", parameters);
                    while (reader.Read())
                    {
                        redirectUri = (RedirectUri)Activator.CreateInstance(typeof(RedirectUri));

                        redirectUri.Id = reader[0].ToString();
                        redirectUri.Name = reader[1].ToString();
                    }
                }
            }

            return redirectUri;
        }

        public KeyValuePair<int, IQueryable<RedirectUri>> GetRedirectsQueryFormatter(string order, int limit, int offset, string sort, string search)
        {
            string query = "SELECT Id, Name FROM redirect_uris";
            int total = 0;
            List<RedirectUri> redirects = new List<RedirectUri>();
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
                    RedirectUri redirect = (RedirectUri)Activator.CreateInstance(typeof(RedirectUri));
                    redirect.Id = reader[0].ToString();
                    redirect.Name = reader[1].ToString();
                    redirects.Add(redirect);
                }
            }
            total = GetRedirectsQueryFormatterCount(search);
            return new KeyValuePair<int, IQueryable<RedirectUri>>(total, redirects.AsQueryable());
        }

        private int GetRedirectsQueryFormatterCount(string search)
        {
            string query = "SELECT COUNT(Id) FROM redirect_uris";
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

        public void Update(RedirectUri redirectUri)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", redirectUri.Id },
                    { "@Name", redirectUri.Name }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"UPDATE redirect_uris SET Name = @Name WHERE Id = @Id", parameters);
            }
        }
    }
}
