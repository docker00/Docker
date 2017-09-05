using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using IdentityServer4.Models;

namespace MySql.AspNet.Identity.Repositories
{
    public class AllowedGrantTypeRepository
    {
        private readonly string _connectionString;
        public AllowedGrantTypeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IQueryable<AllowedGrantType> GetAllowedGrantTypes()
        {
            List<AllowedGrantType> allowedGrantTypes = new List<AllowedGrantType>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Name FROM allowed_grant_types", null);

                while (reader.Read())
                {
                    AllowedGrantType allowedGrantType = (AllowedGrantType)Activator.CreateInstance(typeof(AllowedGrantType));

                    allowedGrantType.Id = reader[0].ToString();
                    allowedGrantType.Name = reader[1].ToString();

                    allowedGrantTypes.Add(allowedGrantType);
                }

            }
            return allowedGrantTypes.AsQueryable();
        }

        public void Insert(AllowedGrantType allowedGrantType)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", allowedGrantType.Id },
                    { "@Name", allowedGrantType.Name }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO allowed_grant_types (Id, Name) VALUES (@Id, @Name)", parameters);
            }
        }

        public void Delete(string allowedGrantTypeId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", allowedGrantTypeId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM allowed_grant_types WHERE Id = @Id", parameters);
            }
        }

        public AllowedGrantType GetAllowedGrantTypeById(string allowedGrantTypeId)
        {
            AllowedGrantType allowedGrantType = null;

            if (!string.IsNullOrEmpty(allowedGrantTypeId))
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "@Id", allowedGrantTypeId }
                    };

                    IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Name FROM allowed_grant_types WHERE Id = @Id", parameters);
                    while (reader.Read())
                    {
                        allowedGrantType = (AllowedGrantType)Activator.CreateInstance(typeof(AllowedGrantType));

                        allowedGrantType.Id = reader[0].ToString();
                        allowedGrantType.Name = reader[1].ToString();
                    }
                }
            }

            return allowedGrantType;
        }

        public AllowedGrantType GetAllowedGrantTypeByName(string allowedGrantTypeName)
        {
            AllowedGrantType allowedGrantType = null;

            if (!string.IsNullOrEmpty(allowedGrantTypeName))
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "@Name", allowedGrantTypeName }
                    };

                    IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Name FROM allowed_grant_types WHERE Name = @Name", parameters);
                    while (reader.Read())
                    {
                        allowedGrantType = (AllowedGrantType)Activator.CreateInstance(typeof(AllowedGrantType));

                        allowedGrantType.Id = reader[0].ToString();
                        allowedGrantType.Name = reader[1].ToString();
                    }
                }
            }

            return allowedGrantType;
        }

        public void Update(AllowedGrantType allowedGrantType)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", allowedGrantType.Id },
                    { "@Name", allowedGrantType.Name }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"UPDATE allowed_grant_types SET Name = @Name WHERE Id = @Id", parameters);
            }
        }
    }
}
