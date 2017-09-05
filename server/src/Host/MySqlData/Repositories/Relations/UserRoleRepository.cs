using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;

namespace MySql.AspNet.Identity.Repositories
{
    public class UserRoleRepository<TUser> where TUser : User
    {
        private readonly string _connectionString;

        public UserRoleRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IQueryable<Role> GetUserRoles(string userId)
        {
            List<Role> roles = new List<Role>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>()
                {
                    { "@UserId", userId }
                };
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT r.Id, r.Name FROM roles r, user_roles ur WHERE ur.UserId=@UserId AND r.Id = ur.RoleId", parameters);
                while (reader.Read())
                {
                    roles.Add(new Role(reader[1].ToString(), reader[0].ToString()));
                }
            }
            return roles.AsQueryable();
        }

        public void Insert(TUser user, string roleName)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@RoleName", roleName }
                };

                object idObject = MySqlHelper.ExecuteScalar(conn, CommandType.Text,
                    @"SELECT Id FROM roles WHERE Name=@RoleName", parameters);
                string roleId = idObject == null ? null : idObject.ToString();


                if (!string.IsNullOrEmpty(roleId))
                {
                    string id = Guid.NewGuid().ToString();
                    using (MySqlConnection conn1 = new MySqlConnection(_connectionString))
                    {
                        Dictionary<string, object> parameters1 = new Dictionary<string, object>
                        {
                            {"@Id", id},
                            {"@UserId", user.Id},
                            {"@RoleId", roleId}
                        };

                        MySqlHelper.ExecuteNonQuery(conn1, @"INSERT INTO user_roles (Id, UserId, RoleId) VALUES (@Id, @UserId, @RoleId)", parameters1);
                    }
                }
            }
        }

        public void Insert(string userId, string roleId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                string id = Guid.NewGuid().ToString();
                Dictionary<string, object> parameters = new Dictionary<string, object> {
                    { "@Id", id },
                    { "@UserId", userId },
                    { "@RoleId", roleId }
                };
                MySqlHelper.ExecuteNonQuery(conn, CommandType.Text, @"INSERT INTO user_roles (Id, UserId, RoleId) VALUES (@Id, @UserId, @RoleId)", parameters);
            }
        }

        public void Delete(TUser user, string roleName)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    {"@RoleName", roleName}
                };

                object idObject = MySqlHelper.ExecuteScalar(conn, CommandType.Text, @"SELECT Id FROM roles WHERE Name = @RoleName", parameters);
                string roleId = idObject == null ? null : idObject.ToString();

                if (!string.IsNullOrEmpty(roleId))
                {

                    using (MySqlConnection conn1 = new MySqlConnection(_connectionString))
                    {
                        Dictionary<string, object> parameters1 = new Dictionary<string, object>
                        {
                            { "@UserId", user.Id },
                            { "@RoleId", roleId }
                        };

                        MySqlHelper.ExecuteNonQuery(conn1, @"DELETE FROM user_roles WHERE UserId = @UserId AND RoleId = @RoleId", parameters1);
                    }
                }
            }
        }

        public void Delete(string userId, string roleId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@UserId", userId },
                    { "@RoleId", roleId}
                };
                MySqlHelper.ExecuteNonQuery(conn, @"Delete FROM user_roles WHERE UserId = @UserId AND RoleId = @RoleId", parameters);
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
                MySqlHelper.ExecuteNonQuery(conn, @"Delete FROM user_roles WHERE UserId = @UserId", parameters);
            }
        }

        public IQueryable<Role> PopulateRoles(string userId)
        {
            List<Role> listRoles = new List<Role>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@UserId", userId }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                @"SELECT
                      r.Id,
                      r.Name
                    FROM roles r
                      JOIN user_roles ur
                        ON ur.UserId = @UserId
                        AND ur.RoleId = r.Id", parameters);
                while (reader.Read())
                {
                    Role role = new Role(reader[1].ToString(), reader[0].ToString());
                    listRoles.Add(role);
                }
            }

            return listRoles.AsQueryable();
        }

        public List<User> PopulateRoleUsers(string roleId)
        {
            List<User> users = new List<User>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@RoleId", roleId }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                 @"SELECT u.Id, u.Email, u.EmailConfirmed,
                u.PasswordHash, u.SecurityStamp, u.PhoneNumber, u.PhoneNumberConfirmed, u.TwoFactorEnabled,
                u.LockoutEndDateUtc, u.LockoutEnabled, u.AccessFailedCount, u.UserName, u.Activated
                    FROM users u
                      JOIN user_roles ur
                        ON ur.RoleId = @RoleId
                        AND ur.UserId = u.Id", parameters);
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
    }
}
