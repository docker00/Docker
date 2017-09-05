using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;

namespace MySql.AspNet.Identity.Repositories
{
    public class RoleRepository<TRole> where TRole : Role
    {
        private readonly string _connectionString;
        public RoleRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IQueryable<TRole> GetRoles()
        {
            List<TRole> roles = new List<TRole>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Name FROM roles", null);

                while (reader.Read())
                {
                    TRole role = (TRole)Activator.CreateInstance(typeof(TRole));

                    role.Id = reader[0].ToString();
                    role.Name = reader[1].ToString();

                    roles.Add(role);
                }

            }
            return roles.AsQueryable();
        }

        public void Insert(Role role)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@name", role.Name },
                    { "@id", role.Id }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO roles (Id, Name) VALUES (@id, @name)", parameters);
            }
        }

        public void Delete(string roleId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@id", roleId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM roles WHERE Id = @id", parameters);
            }
        }

        public TRole GetRoleById(string roleId)
        {
            TRole role = null;
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", roleId }
                };
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Name FROM roles WHERE Id = @Id", parameters);
                while (reader.Read())
                {
                    role = (TRole)Activator.CreateInstance(typeof(TRole));
                    role.Id = reader[0].ToString();
                    role.Name = reader[1].ToString();
                }
            }
            return role;
        }

        public Role GetRoleByName(string roleName)
        {
            Role role = null;

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Name", roleName }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Name FROM roles WHERE Name = @Name", parameters);

                while (reader.Read())
                {
                    role = (TRole)Activator.CreateInstance(typeof(TRole));

                    role.Id = reader[0].ToString();
                    role.Name = reader[1].ToString();
                }
            }
            return role;
        }

        public IQueryable<TRole> GetByGroupId(string groupId)
        {
            List<TRole> roles = (List<TRole>)Activator.CreateInstance(typeof(List<TRole>));
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@GroupId", groupId }
                };
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT r.Id, r.Name FROM roles r, group_roles gr WHERE gr.GroupId = @GroupId AND r.Id = gr.RoleId",
                    parameters);
                while (reader.Read())
                {
                    TRole role = (TRole)Activator.CreateInstance(typeof(TRole));
                    role.Id = reader[0].ToString();
                    role.Name = reader[1].ToString();
                    roles.Add(role);
                }
            }
            return roles.AsQueryable();
        }

        public KeyValuePair<int, IQueryable<TRole>> GetQueryFormatter(string order, int limit, int offset, string sort, string search, string groupId = null, bool inGroup = false)
        {
            List<TRole> roles = new List<TRole>();
            int total = 0;
            string query = "SELECT Id, Name FROM roles";
            string count_query = "SELECT COUNT(Id) FROM roles";
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
                if(search.Length > 0)
                {
                    query += " WHERE Name LIKE '%" + search + "%'";
                    count_query += " WHERE Name LIKE '%" + search + "%'";
                }
            }
            if (!string.IsNullOrEmpty(groupId))
            {
                if (string.IsNullOrEmpty(search))
                {
                    query += " WHERE";
                    count_query += " WHERE";
                }
                query += " Id " + (inGroup ? "IN" : "NOT IN") + " (SELECT RoleId FROM group_roles WHERE GroupId = '" + groupId + "')";
                count_query += " Id " + (inGroup ? "IN" : "NOT IN") + " (SELECT RoleId FROM group_roles WHERE GroupId = '" + groupId + "')";
            }
            if (!string.IsNullOrEmpty(sort))
            {
                query += " ORDER BY " + sort + " " + order;
            }
            query += " LIMIT " + offset + ", " + limit;
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, query, null);
                while (reader.Read())
                {
                    TRole role = (TRole)Activator.CreateInstance(typeof(TRole));
                    role.Id = reader[0].ToString();
                    role.Name = reader[1].ToString();
                    roles.Add(role);
                }
            }
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, count_query, null);
                while (reader.Read())
                {
                    total = Convert.ToInt32(reader[0].ToString());
                }
            }
            return new KeyValuePair<int, IQueryable<TRole>>(total, roles.AsQueryable());
        }

        public void Update(Role role)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@name", role.Name },
                    { "@id", role.Id }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"UPDATE roles SET Name = @name WHERE Id = @id", parameters);
            }
        }

        public List<TRole> GetRolesByObjectEndpointId(string objectEndpointId)
        {
            List<TRole> roles = new List<TRole>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ObjectEndpointId", objectEndpointId }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT distinct
                      r.Id, r.Name
                    FROM object_endpoint_permissions oep
                      JOIN role_permissions rp
                        ON rp.ObjectEndpointPermissionId = oep.Id
                      JOIN roles r
                        ON r.Id = rp.RoleId
                    WHERE oep.ObjectEndpointId = @ObjectEndpointId", parameters);
                while (reader.Read())
                {
                    TRole role = (TRole)Activator.CreateInstance(typeof(TRole));

                    role.Id = reader[0].ToString();
                    role.Name = reader[1].ToString();

                    roles.Add(role);
                }
            }
            return roles;
        }
    }
}
