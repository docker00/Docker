using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace MySql.AspNet.Identity.Repositories
{
    public class GroupParentRepository<TGroupParent> where TGroupParent : GroupParent
    {
        private readonly string _connectionString;
        private readonly GroupRepository<Group> _groupRepository;
        public GroupParentRepository(string connectionString)
        {
            _connectionString = connectionString;
            _groupRepository = new GroupRepository<Group>(_connectionString);
        }

        public TGroupParent Get(string groupId, string parentId)
        {
            TGroupParent groupParent = null;
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ParentGroupId", parentId },
                    { "@GroupId", groupId }
                };
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT Id, GroupId, ParentGroupId FROM group_parents WHERE GroupId = @GroupId AND ParentGroupId = @ParentGroupId LIMIT 1",
                    parameters);
                while (reader.Read())
                {
                    groupParent = (TGroupParent)Activator.CreateInstance(typeof(TGroupParent));
                    
                    groupParent.Id = reader[0].ToString();
                    groupParent.GroupId = reader[1].ToString();
                    groupParent.ParentGroupId = reader[2].ToString();
                }
            }
            return groupParent;
        }

        public void Insert(string groupId, string parentGroupId)
        {
            string id = Guid.NewGuid().ToString();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", id },
                    { "@GroupId", groupId },
                    { "@ParentGroupId", parentGroupId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO group_parents (Id, GroupId, ParentGroupId) VALUES(@Id, @GroupId, @ParentGroupId)", parameters);
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

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM group_parents WHERE Id = @Id", parameters);
            }
        }

        public void Delete(string groupId, string parentGroupId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@GroupId", groupId },
                    { "@ParentGroupId", parentGroupId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM group_parents WHERE GroupId = @GroupId AND ParentGroupId = @ParentGroupId", parameters);
            }
        }

        public void DeleteGroupJoins(string groupId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@GroupId", groupId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM group_parents WHERE GroupId = @GroupId OR ParentGroupId = @GroupId", parameters);
            }
        }

        /// <summary>
        /// Получает родителей группы. Если recursive = true, выведутся родители до верхнего уровня
        /// </summary>
        /// <param name="groupId">Идентификатор группы</param>
        /// <param name="recursive">Если true, то выведет паренты до верхнего уровня. По умочанию false</param>
        /// <returns>Возвращает список родителей группы</returns>
        public IQueryable<Group> PopulateGroupParents(string groupId, bool recursive = false)
        {
            List<Group> groupParents = new List<Group>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@GroupId", groupId }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT g.Id, g.Name 
                    FROM groups g
                      JOIN group_parents gp
                        ON gp.GroupId = @GroupId
                        AND gp.ParentGroupId = g.Id", parameters);
                while (reader.Read())
                {
                    Group group = (Group)Activator.CreateInstance(typeof(Group));
                    group.Id = reader[0].ToString();
                    group.Name = reader[1].ToString();
                    groupParents.Add(group);
                }
            }
            List<Group> totalGroups = groupParents;
            //Вызывает рекурсию до тех пор, пока не дойдет до последнего парента
            if (groupParents.Count > 0 && recursive)
            {
                foreach (Group groupParent in groupParents)
                {
                    totalGroups = totalGroups.Concat(PopulateGroupParents(groupParent.Id, recursive)).ToList();
                }
            }

            return totalGroups.AsQueryable();
        }

        /// <summary>
        /// Получает дочерние группы. Если recursive = true, выведутся дочерние группы до нижнего уровня
        /// </summary>
        /// <param name="groupId">Идентификатор группы</param>
        /// <param name="recursive">Если true, то выведет дочерние группы до нижнего уровня. По умочанию false</param>
        /// <returns>Возвращает список дочерних групп</returns>
        public IQueryable<Group> PoplulateGroupChildren(string parentId, bool recursive = false)
        {
            List<Group> groups = (List<Group>)Activator.CreateInstance(typeof(List<Group>));
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
                    Group group = (Group)Activator.CreateInstance(typeof(Group));
                    group.Id = reader[0].ToString();
                    group.Name = reader[1].ToString();
                    groups.Add(group);
                }
            }
            List<Group> totalGroups = groups;
            //Вызывает рекурсию до тех пор, пока не дойдет до последнего парента
            if (groups.Count > 0 && recursive)
            {
                foreach (Group group in groups)
                {
                    totalGroups = totalGroups.Concat(PoplulateGroupChildren(group.Id, recursive)).ToList();
                }
            }
            return totalGroups.AsQueryable();
        }
    }
}
