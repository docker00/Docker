using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using IdentityServer4.Quickstart.UI;

namespace MySql.AspNet.Identity.Repositories
{
    public class RolePermissionRepository
    {
        private readonly string _connectionString;
        private readonly PermissionRepository _permissionRepository;
        public RolePermissionRepository(string connectionString)
        {
            _connectionString = connectionString;

            _permissionRepository = new PermissionRepository(_connectionString);
        }

        public void Insert(string roleId, string objectEndpointPermissionId)
        {
            string id = Guid.NewGuid().ToString();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", id },
                    { "@RoleId", roleId },
                    { "@ObjectEndpointPermissionId", objectEndpointPermissionId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO role_permissions (Id, RoleId, ObjectEndpointPermissionId) VALUES(@Id, @RoleId, @ObjectEndpointPermissionId)", parameters);
            }
        }

        public void Delete(string roleId, string objectEndpointPermissionId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                        {
                            { "@RoleId", roleId },
                            { "@ObjectEndpointPermissionId", objectEndpointPermissionId }
                        };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM role_permissions WHERE RoleId = @RoleId and ObjectEndpointPermissionId = @ObjectEndpointPermissionId", parameters);
            }
        }

        public List<Permission> PopulateRolePermissionsOnRoleId(string roleId)
        {
            List<Permission> permissions = new List<Permission>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@RoleId", roleId }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT
                          p.Id,
                          p.Name
                        FROM role_permissions rp
                          JOIN object_endpoint_permissions oep
                            ON oep.Id = rp.ObjectEndpointPermissionId
                          JOIN permissions p
                            ON p.Id = oep.PermissionId
                        WHERE rp.RoleId = @RoleId", parameters);
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

        public RolePermissionModel Get(string roleId, string objectEndpointPermissionId)
        {
            RolePermissionModel rolePermission = null;
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@RoleId", roleId },
                    { "@ObjectEndpointPermissionId", objectEndpointPermissionId }
                };
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT Id, RoleId, ObjectEndpointPermissionId FROM role_permissions WHERE RoleId = @RoleId AND ObjectEndpointPermissionId = @objectEndpointPermissionId LIMIT 1",
                    parameters);
                while (reader.Read())
                {
                    rolePermission = (RolePermissionModel)Activator.CreateInstance(typeof(RolePermissionModel));

                    rolePermission.Id = reader[0].ToString();
                    rolePermission.RoleId = reader[1].ToString();
                    rolePermission.PermissionId = reader[2].ToString();
                }
            }
            return rolePermission;
        }
    }
}
