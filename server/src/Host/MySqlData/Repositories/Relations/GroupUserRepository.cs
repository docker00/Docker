using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Identity;

namespace MySql.AspNet.Identity.Repositories
{
    public class GroupUserRepository
    {
        private readonly string _connectionString;
        private readonly UserRepository<User> _userRepository;
        private readonly GroupRepository<Group> _groupRepository;
        public GroupUserRepository(string connectionString)
        {
            _connectionString = connectionString;

            _userRepository = new UserRepository<User>(_connectionString);
            _groupRepository = new GroupRepository<Group>(_connectionString);
        }

        public void Insert(string groupId, string userId)
        {
            string id = Guid.NewGuid().ToString();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", id },
                    { "@GroupId", groupId },
                    { "@UserId", userId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO group_users (Id, GroupId, UserId) VALUES(@Id, @GroupId, @UserId)", parameters);
            }
        }

        public void Delete(string groupId, string userId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@GroupId", groupId },
                    { "@UserId", userId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM group_users WHERE GroupId = @GroupId AND UserId = @UserId", parameters);
            }
        }

        public void Delete(string userId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@UserId", userId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM group_users WHERE UserId = @UserId", parameters);
            }
        }

        public void DeleteUserRoles(string roleId, string groupId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@GroupId", groupId },
                    { "@RoleId", roleId }
                };
                MySqlHelper.ExecuteNonQuery(
                    conn,
                    @"DELETE ur
                        FROM user_roles ur
                        JOIN group_users gu
                        ON ur.UserId = gu.UserId
                        AND gu.GroupId = @GroupId
                      WHERE ur.RoleId = @RoleId",
                    parameters);
            }
        }

        public void DeleteGroupUsers(string groupId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@GroupId", groupId }
                };
                MySqlHelper.ExecuteNonQuery(
                    conn,
                    @"DELETE gu FROM group_users gu
                      WHERE gu.GroupId = @GroupId",
                    parameters);
            }
        }

        public List<User> PopulateGroupUsers(string groupId)
        {
            List<User> users = new List<User>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@GroupId", groupId }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                 @"SELECT u.Id, u.Email, u.EmailConfirmed,
                        u.PasswordHash, u.SecurityStamp, u.PhoneNumber, u.PhoneNumberConfirmed, u.TwoFactorEnabled,
                        u.LockoutEndDateUtc, u.LockoutEnabled, u.AccessFailedCount, u.UserName, u.Activated 
                    FROM users u
                      JOIN group_users gu
                        ON gu.GroupId = @GroupId
                        AND gu.UserId = u.Id", parameters);
                while (reader.Read())
                {
                    User user = (User)Activator.CreateInstance(typeof(User));

                    user.Id = reader[0].ToString();
                    user.Email = reader[1].ToString();
                    user.EmailConfirmed = (bool)reader[2];
                    user.PasswordHash = reader[3].ToString();
                    user.SecurityStamp = reader[4].ToString();
                    user.PhoneNumber = reader[5].ToString();
                    user.PhoneNumberConfirmed = (bool)reader[6];
                    user.TwoFactorAuthEnabled = (bool)reader[7];
                    user.LockoutEndDate = (DateTime)reader[8];
                    user.LockoutEnabled = (bool)reader[9];
                    user.AccessFailedCount = (int)reader[10];
                    user.UserName = reader[11].ToString();
                    user.Activated = (bool)reader[12];

                    users.Add(user);
                }
            }

            return users;
        }

        public List<Group> PopulateUserGroups(string userId)
        {
            List<Group> groups = new List<Group>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@UserId", userId }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT g.Id, g.Name 
                    FROM groups g
                      JOIN group_users gu
                        ON gu.UserId = @UserId
                        AND gu.GroupId = g.Id", parameters);
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
