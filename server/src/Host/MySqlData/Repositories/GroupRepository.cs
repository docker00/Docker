using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;

namespace MySql.AspNet.Identity.Repositories
{

    public class GroupRepository<TGroup> where TGroup : Group
    {
        private readonly string _connectionString;

        public GroupRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IQueryable<TGroup> GetGroups()
        {

            List<TGroup> groups = new List<TGroup>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Name FROM groups", null);

                while (reader.Read())
                {

                    TGroup group = (TGroup)Activator.CreateInstance(typeof(TGroup));

                    group.Id = reader[0].ToString();
                    group.Name = reader[1].ToString();

                    groups.Add(group);
                }

            }
            return groups.AsQueryable();
        }

        public void Insert(TGroup group)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", group.Id },
                    { "@Name", group.Name }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO groups (Id, Name) VALUES (@Id, @Name)", parameters);
            }
        }

        public void Update(TGroup group)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", group.Id },
                    { "@Name", group.Name }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"UPDATE groups SET Name = @Name WHERE Id = @Id", parameters);
            }
        }

        public void Delete(string groupId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", groupId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM groups WHERE Id = @Id", parameters);
            }
        }

        public TGroup GetGroupById(string groupId)
        {

            TGroup group = null;

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@id", groupId }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Name FROM groups WHERE Id = @id", parameters);

                while (reader.Read())
                {
                    group = (TGroup)Activator.CreateInstance(typeof(TGroup));

                    group.Id = reader[0].ToString();
                    group.Name = reader[1].ToString();
                }
            }

            return group;
        }

        public TGroup GetGroupByName(string groupName)
        {

            TGroup group = null;

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Name", groupName }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Name FROM groups WHERE Name = @Name", parameters);

                while (reader.Read())
                {
                    group = (TGroup)Activator.CreateInstance(typeof(TGroup));

                    group.Id = reader[0].ToString();
                    group.Name = reader[1].ToString();
                }
            }
            return group;
        }

     

        public IQueryable<TGroup> GetGroupsByObjectId(string parentId)
        {
            List<TGroup> groups = (List<TGroup>)Activator.CreateInstance(typeof(List<TGroup>));
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ParentId", parentId }
                };
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT g.Id, g.Name FROM groups g, group_parents gp WHERE gp.ParentGroupId = @ParentId AND g.Id = gp.GroupId",
                    parameters);
                while (reader.Read())
                {
                    TGroup group = (TGroup)Activator.CreateInstance(typeof(TGroup));
                    group.Id = reader[0].ToString();
                    group.Name = reader[1].ToString();
                    groups.Add(group);
                }
            }
            return groups.AsQueryable();
        }

        public KeyValuePair<int, IQueryable<TGroup>> GetQueryFromatter(string order, int limit, int offset, string sort, string search)
        {
            string query = "SELECT Id, Name FROM groups";
            string count_query = "SELECT COUNT(Id) FROM groups";
            int total = 0;
            List<TGroup> groups = new List<TGroup>();
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
                if (search.Length > 0)
                {
                    query += " WHERE Name LIKE '%" + search + "%'";
                    count_query += " WHERE Name LIKE '%" + search + "%'";
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
                    TGroup group = (TGroup)Activator.CreateInstance(typeof(TGroup));
                    group.Id = reader[0].ToString();
                    group.Name = reader[1].ToString();
                    groups.Add(group);
                }
            }
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, count_query, null);
                while (reader.Read())
                {
                    total = Convert.ToInt32(reader[0]);
                }
            }
            return new KeyValuePair<int, IQueryable<TGroup>>(total, groups.AsQueryable());
        }

        public List<Group> GetGroupsByObjectEndpointId(string objectEndpointId)
        {
            List<Group> groups = new List<Group>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ObjectEndpointId", objectEndpointId }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT distinct
                      g.ID,
                      g.Name
                    FROM group_roles gr,
                         groups g
                    WHERE gr.RoleId IN (SELECT
                        ID
                      FROM roles
                      WHERE ID IN (SELECT
                          rp.RoleId
                        FROM object_endpoint_permissions op,
                             role_permissions rp
                        WHERE op.ObjectEndpointId = @ObjectEndpointId
                        AND op.Id = rp.ObjectEndpointPermissionId))
                    AND g.Id = gr.GroupId", parameters);
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
