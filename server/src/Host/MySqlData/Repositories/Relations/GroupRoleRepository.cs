using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Identity;

namespace MySql.AspNet.Identity.Repositories
{
    public class GroupRoleRepository<TRole> : Role
    {
        private readonly string _connectionString;
        private readonly RoleRepository<Role> _roleRepository;
        public GroupRoleRepository(string connectionString)
        {
            _connectionString = connectionString;

            _roleRepository = new RoleRepository<Role>(_connectionString);
        }

        public void Insert(string groupId, string roleId)
        {
            string id = Guid.NewGuid().ToString();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", id },
                    { "@GroupId", groupId },
                    { "@RoleId", roleId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO group_roles (Id, GroupId, RoleId) VALUES(@Id, @GroupId, @RoleId)", parameters);
            }
        }

        public void Delete(string groupId, string roleId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@GroupId", groupId },
                    { "@RoleId", roleId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM group_roles WHERE GroupId = @GroupId AND RoleId = @RoleId", parameters);
            }
        }

        public List<Role> PopulateGroupRoles(string groupId)
        {
            List<Role> roles = new List<Role>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@GroupId", groupId }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                 @"SELECT r.Id, r.Name 
                    FROM roles r
                      JOIN group_roles gr
                        ON gr.GroupId = @GroupId
                        AND gr.RoleId = r.Id", parameters);
                while (reader.Read())
                {
                    Role role = (Role)Activator.CreateInstance(typeof(Role));
                    role.Id = reader[0].ToString();
                    role.Name = reader[1].ToString();

                    roles.Add(role);
                }
            }
            
            return roles;
        }

        public List<Group> PopulateRoleGroups(string roleId)
        {
            List<Group> groups = new List<Group>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@RoleId", roleId }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                 @"SELECT g.Id, g.Name 
                    FROM groups g
                      JOIN group_roles gr
                        ON gr.RoleId = @RoleId
                        AND gr.GroupId = g.Id", parameters);
                while (reader.Read())
                {
                    Group group = (Group)Activator.CreateInstance(typeof(Group));
                    group.Id = reader[0].ToString();
                    group.Name = reader[1].ToString();

                    groups.Add(group);
                }
            }

            return groups;
        }
    }
}
