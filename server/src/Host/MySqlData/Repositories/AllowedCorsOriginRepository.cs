using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using IdentityServer4.Models;

namespace MySql.AspNet.Identity.Repositories
{
    public class AllowedCorsOriginRepository
    {
        private readonly string _connectionString;
        public AllowedCorsOriginRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IQueryable<AllowedCorsOrigin> GetAllowedCorsOrigins()
        {
            List<AllowedCorsOrigin> allowedCorsOrigins = new List<AllowedCorsOrigin>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Name FROM allowed_cors_origins", null);

                while (reader.Read())
                {
                    AllowedCorsOrigin allowedCorsOrigin = (AllowedCorsOrigin)Activator.CreateInstance(typeof(AllowedCorsOrigin));

                    allowedCorsOrigin.Id = reader[0].ToString();
                    allowedCorsOrigin.Name = reader[1].ToString();

                    allowedCorsOrigins.Add(allowedCorsOrigin);
                }

            }
            return allowedCorsOrigins.AsQueryable();
        }

        public void Insert(AllowedCorsOrigin allowedCorsOrigin)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", allowedCorsOrigin.Id },
                    { "@Name", allowedCorsOrigin.Name }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO allowed_cors_origins (Id, Name) VALUES (@Id, @Name)", parameters);
            }
        }

        public void Delete(string allowedCorsOriginId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", allowedCorsOriginId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM allowed_cors_origins WHERE Id = @Id", parameters);
            }
        }

        public AllowedCorsOrigin GetAllowedCorsOriginsById(string allowedCorsOriginId)
        {
            AllowedCorsOrigin allowedCorsOrigin = null;

            if (!string.IsNullOrEmpty(allowedCorsOriginId))
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "@Id", allowedCorsOriginId }
                    };

                    IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Name FROM allowed_cors_origins WHERE Id = @Id", parameters);
                    while (reader.Read())
                    {
                        allowedCorsOrigin = (AllowedCorsOrigin)Activator.CreateInstance(typeof(AllowedCorsOrigin));

                        allowedCorsOrigin.Id = reader[0].ToString();
                        allowedCorsOrigin.Name = reader[1].ToString();
                    }
                }
            }

            return allowedCorsOrigin;
        }

        public AllowedCorsOrigin GetAllowedCorsOriginsByName(string allowedCorsOriginName)
        {
            AllowedCorsOrigin allowedCorsOrigin = null;

            if (!string.IsNullOrEmpty(allowedCorsOriginName))
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "@Name", allowedCorsOriginName }
                    };

                    IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Name FROM allowed_cors_origins WHERE Name = @Name", parameters);
                    while (reader.Read())
                    {
                        allowedCorsOrigin = (AllowedCorsOrigin)Activator.CreateInstance(typeof(AllowedCorsOrigin));

                        allowedCorsOrigin.Id = reader[0].ToString();
                        allowedCorsOrigin.Name = reader[1].ToString();
                    }
                }
            }

            return allowedCorsOrigin;
        }

        public void Update(AllowedCorsOrigin allowedCorsOrigin)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", allowedCorsOrigin.Id },
                    { "@Name", allowedCorsOrigin.Name }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"UPDATE allowed_cors_origins SET Name = @Name WHERE Id = @Id", parameters);
            }
        }
    }
}
