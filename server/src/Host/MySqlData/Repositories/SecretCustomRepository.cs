using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using IdentityServer4.Models;

namespace MySql.AspNet.Identity.Repositories
{
    public class SecretCustomRepository
    {
        private readonly string _connectionString;
        public SecretCustomRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IQueryable<SecretCustom> GetSecretCustoms()
        {
            List<SecretCustom> secretCustoms = new List<SecretCustom>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Value, Type FROM secret", null);

                while (reader.Read())
                {
                    SecretCustom secretCustom = (SecretCustom)Activator.CreateInstance(typeof(SecretCustom));

                    secretCustom.Id = reader[0].ToString();
                    secretCustom.Value = reader[1].ToString();
                    secretCustom.Type = reader[2].ToString();

                    secretCustoms.Add(secretCustom);
                }

            }
            return secretCustoms.AsQueryable();
        }

        public void Insert(SecretCustom secretCustom)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", secretCustom.Id },
                    { "@Value", secretCustom.Value },
                    { "@Type", secretCustom.Type ?? "SharedSecret" }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO secret (Id, Value, Type) VALUES (@Id, @Value, @Type)", parameters);
            }
        }

        public void Delete(string secretCustomId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", secretCustomId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM secret WHERE Id = @Id", parameters);
            }
        }

        public SecretCustom GetSecretCustomById(string secretCustomId)
        {
            SecretCustom secretCustom = null;

            if (!string.IsNullOrEmpty(secretCustomId))
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "@Id", secretCustomId }
                    };

                    IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Value, Type FROM secret WHERE Id = @Id", parameters);
                    while (reader.Read())
                    {
                        secretCustom = (SecretCustom)Activator.CreateInstance(typeof(SecretCustom));

                        secretCustom.Id = reader[0].ToString();
                        secretCustom.Value = reader[1].ToString();
                        secretCustom.Type = reader[2].ToString();
                    }
                }
            }

            return secretCustom;
        }

        public SecretCustom GetSecretCustomByValue(string secretCustomValue)
        {
            SecretCustom secretCustom = null;

            if (!string.IsNullOrEmpty(secretCustomValue))
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "@Value", secretCustomValue }
                    };

                    IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Value, Type FROM secret WHERE Value = @Value", parameters);
                    while (reader.Read())
                    {
                        secretCustom = (SecretCustom)Activator.CreateInstance(typeof(SecretCustom));

                        secretCustom.Id = reader[0].ToString();
                        secretCustom.Value = reader[1].ToString();
                        secretCustom.Type = reader[2].ToString();
                    }
                }
            }

            return secretCustom;
        }

        public KeyValuePair<int, IQueryable<SecretCustom>> GetSecretQueryFormatter(string order, int limit, int offset, string sort, string search)
        {
            string query = "SELECT Id, Value, Type FROM secret";
            List<SecretCustom> secrets = new List<SecretCustom>();
            int total = 0;
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
                if (search.Trim().Length > 0)
                {
                    query += " WHERE Value LIKE '" + search + "%' OR Type LIKE '" + search + "%'";
                }
            }
            if (!string.IsNullOrEmpty(sort))
            {
                query += " ORDER BY " + sort + " " + order.ToUpper();
            }
            query += " LIMIT " + offset + "," + limit;
            using(MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, query, null);
                while (reader.Read())
                {
                    SecretCustom secret = (SecretCustom)Activator.CreateInstance(typeof(SecretCustom));
                    secret.Id = reader[0].ToString();
                    secret.Value = reader[1].ToString();
                    secret.Type = reader[2].ToString();
                    secrets.Add(secret);
                }
            }
            total = GetSecretQueryFormatterCount(search);
            return new KeyValuePair<int, IQueryable<SecretCustom>>(total, secrets.AsQueryable());
        }

        private int GetSecretQueryFormatterCount(string search)
        {
            using(MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                string query = "SELECT COUNT(Id) FROM secret";
                if (!string.IsNullOrEmpty(search))
                {
                    search = search.Trim();
                    if (search.Trim().Length > 0)
                    {
                        query += " WHERE Value LIKE '" + search + "%' OR Type LIKE '" + search + "%'";
                    }
                }
                query += " LIMIT 1";
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, query, null);
                while (reader.Read())
                {
                    return Convert.ToInt32(reader[0]);
                }
            }
            return 0;
        }

        public void Update(SecretCustom secretCustom)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", secretCustom.Id },
                    { "@Value", secretCustom.Value },
                    { "@Type", secretCustom.Type }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"UPDATE secret SET Value = @Value, Type = @Type WHERE Id = @Id", parameters);
            }
        }
    }
}
