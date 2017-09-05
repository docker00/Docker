using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;

namespace MySql.AspNet.Identity.Repositories
{
    public class PermissionRepository
    {
        private readonly string _connectionString;
        public PermissionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IQueryable<Permission> GetPermissions()
        {
            List<Permission> permissions = new List<Permission>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Name FROM permissions", null);

                while (reader.Read())
                {
                    Permission permission = (Permission)Activator.CreateInstance(typeof(Permission));

                    permission.Id = reader[0].ToString();
                    permission.Name = reader[1].ToString();

                    permissions.Add(permission);
                }

            }
            return permissions.AsQueryable();
        }

        public void Insert(Permission permission)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@id", permission.Id },
                    { "@name", permission.Name }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO permissions (Id, Name) VALUES (@id, @name)", parameters);
            }
        }

        public void Update(Permission permission)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@id", permission.Id },
                    { "@name", permission.Name }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"UPDATE permissions SET Name = @name WHERE Id = @id", parameters);
            }
        }

        public void Delete(string permissionId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@id", permissionId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM permissions WHERE Id = @id", parameters);
            }
        }

        public Permission GetPermissionById(string permissionId)
        {
            Permission permission = null;

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@id", permissionId }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Name FROM permissions WHERE Id = @id", parameters);

                while (reader.Read())
                {
                    permission = (Permission)Activator.CreateInstance(typeof(Permission));

                    permission.Id = reader[0].ToString();
                    permission.Name = reader[1].ToString();
                }
            }

            return permission;
        }
        
        public Permission GetPermissionByName(string permissionName)
        {
            Permission permission = null;

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Name", permissionName }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Name FROM permissions WHERE Name = @Name", parameters);

                while (reader.Read())
                {
                    permission = (Permission)Activator.CreateInstance(typeof(Permission));

                    permission.Id = reader[0].ToString();
                    permission.Name = reader[1].ToString();
                }
            }

            return permission;
        }
    }
}
