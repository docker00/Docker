using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Identity;

namespace MySql.AspNet.Identity.Repositories
{
    public class ObjectEndpointPermissionsRepository
    {
        private readonly string _connectionString;
        private readonly PermissionRepository _permissionRepository;
        public ObjectEndpointPermissionsRepository(string connectionString)
        {
            _connectionString = connectionString;

            _permissionRepository = new PermissionRepository(_connectionString);
        }

        public void Insert(string objectEndpointId, string permissionId)
        {
            string id = Guid.NewGuid().ToString();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", id },
                    { "@ObjectEndpointId", objectEndpointId },
                    { "@PermissionId", permissionId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO object_endpoint_permissions (Id, ObjectEndpointId, PermissionId) VALUES(@Id, @ObjectEndpointId, @PermissionId)", parameters);
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

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM object_endpoint_permissions WHERE Id = @Id", parameters);
            }
        }

        public void Delete(string objectEndpointId, string permissionId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ObjectEndpointId", objectEndpointId },
                    { "@PermissionId", permissionId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM object_endpoint_permissions WHERE ObjectEndpointId = @ObjectEndpointId AND PermissionId = @PermissionId", parameters);
            }
        }

        public void DeletePermissions(string objectEndpointId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ObjectEndpointId", objectEndpointId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM object_endpoint_permissions WHERE ObjectEndpointId = @ObjectEndpointId", parameters);
            }
        }

        public List<Permission> PopulateObjectEndpointPermissions(string objectEndpointId)
        {
            List<Permission> permissions = new List<Permission>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ObjectEndpointId", objectEndpointId }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                 @"SELECT p.Id, p.Name
                    FROM permissions p
                      JOIN object_endpoint_permissions oep
                        ON oep.ObjectEndpointId = @ObjectEndpointId
                        AND oep.PermissionId = p.Id", parameters);
                while (reader.Read())
                {
                    Permission permission = (Permission)Activator.CreateInstance(typeof(Permission));

                    permission.Id = reader[0].ToString();
                    permission.Name = reader[1].ToString();

                    permissions.Add(permission);
                }
            }

            return permissions;

        }

        public List<ObjectEndpointPermition> GetByObjectEndpointId(string objectEndpointId)
        {
            List<ObjectEndpointPermition> objectEndpointPermitions = new List<ObjectEndpointPermition>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ObjectEndpointId", objectEndpointId }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                 @"SELECT oep.Id, oep.PermissionId, oep.ObjectEndpointId FROM object_endpoint_permissions oep WHERE oep.ObjectEndpointId = @ObjectEndpointId", parameters);
                while (reader.Read())
                {
                    ObjectEndpointPermition permission = (ObjectEndpointPermition)Activator.CreateInstance(typeof(ObjectEndpointPermition));

                    permission.Id = reader[0].ToString();
                    permission.PermissionId = reader[1].ToString();
                    permission.ObjectEndpointId = reader[2].ToString();

                    objectEndpointPermitions.Add(permission);
                }
            }

            return objectEndpointPermitions;
        }

        public List<ObjectEndpointPermition> PopulateObjectEndpointPermissionByRole(string roleId)
        {
            List<ObjectEndpointPermition> objectEndpointPermitions = new List<ObjectEndpointPermition>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@RoleId", roleId }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                 @"SELECT oep.Id, oep.ObjectEndpointId, PermissionId
                        FROM role_permissions rp
                          JOIN object_endpoint_permissions oep
                            ON oep.Id = rp.ObjectEndpointPermissionId
                        WHERE rp.RoleId = @RoleId", parameters);
                while (reader.Read())
                {
                    ObjectEndpointPermition permission = (ObjectEndpointPermition)Activator.CreateInstance(typeof(ObjectEndpointPermition));

                    permission.Id = reader[0].ToString();
                    permission.ObjectEndpointId = reader[1].ToString();
                    permission.PermissionId = reader[2].ToString();

                    objectEndpointPermitions.Add(permission);
                }
            }

            return objectEndpointPermitions;
        }
    }
}
