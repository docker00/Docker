using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;

namespace MySql.AspNet.Identity.Repositories
{
    public class UserRepository<TUser> where TUser : User
    {
        private readonly string _connectionString;
        private readonly UserRoleRepository<TUser> _userRoleRepository;
        private readonly UserClaimRepository<TUser> _userClaimRepository;
        private readonly UserLoginRepository _userLoginRepository;
        public UserRepository(string connectionString)
        {
            _connectionString = connectionString;
            _userRoleRepository = new UserRoleRepository<TUser>(_connectionString);
            _userClaimRepository = new UserClaimRepository<TUser>(_connectionString);
            _userLoginRepository = new UserLoginRepository(_connectionString);
        }

        public void Insert(TUser user)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", user.Id},
                    { "@Email", (object) user.Email ?? "" },
                    { "@EmailConfirmed", user.EmailConfirmed },
                    { "@PasswordHash", (object) user.PasswordHash ?? "" },
                    { "@SecurityStamp", (object) user.SecurityStamp ?? "" },
                    { "@PhoneNumber", (object) user.PhoneNumber ?? "" },
                    { "@PhoneNumberConfirmed", user.PhoneNumberConfirmed },
                    { "@TwoFactorAuthEnabled", user.TwoFactorAuthEnabled },
                    { "@LockoutEndDate", (object) user.LockoutEndDate ?? DateTime.Now },
                    { "@LockoutEnabled", user.LockoutEnabled },
                    { "@AccessFailedCount", user.AccessFailedCount },
                    { "@UserName", user.UserName },
                    { "@Activated", user.Activated },
                    { "@AttributesValidated", user.AttributesValidated }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO users (Id, Email, EmailConfirmed,
                PasswordHash, SecurityStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled,
                LockoutEndDateUtc, LockoutEnabled, AccessFailedCount, UserName, Activated, AttributesValidated) VALUES(@Id, @Email, @EmailConfirmed, @PasswordHash, @SecurityStamp, @PhoneNumber, @PhoneNumberConfirmed,
                @TwoFactorAuthEnabled, @LockoutEndDate, @LockoutEnabled, @AccessFailedCount, @UserName, @Activated, @AttributesValidated)", parameters);
            }
        }

        public void Delete(TUser user)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    {"@Id", user.Id}
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM users WHERE Id=@Id", parameters);
            }
        }

        public IQueryable<TUser> GetAll()
        {
            List<TUser> users = new List<TUser>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,

                    @"SELECT Id, Email, EmailConfirmed,
                PasswordHash, SecurityStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled,
                LockoutEndDateUtc, LockoutEnabled, AccessFailedCount, UserName, Activated, AttributesValidated FROM users", null);

                while (reader.Read())
                {
                    TUser user = (TUser)Activator.CreateInstance(typeof(TUser));
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
                    user.AttributesValidated = (bool)reader[13];
                    
                    user.Roles = _userRoleRepository.PopulateRoles(user.Id).Select(r => r.Name).ToList();
                    user.Claims = _userClaimRepository.PopulateUserClaims(user);
                    user.Logins = _userLoginRepository.PopulateUserLogins(user.Id);

                    users.Add(user);
                }

            }
            return users.AsQueryable<TUser>();
        }

        public TUser GetById(string userId)
        {
            TUser user = null;
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    {"@Id", userId}
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,

                    @"SELECT Id, Email, EmailConfirmed,
                PasswordHash, SecurityStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled,
                LockoutEndDateUtc, LockoutEnabled, AccessFailedCount, UserName, Activated, AttributesValidated FROM users WHERE Id = @Id", parameters);
                while (reader.Read())
                {
                    user = (TUser)Activator.CreateInstance(typeof(TUser));

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
                    user.AttributesValidated = (bool)reader[13];

                    user.Roles = _userRoleRepository.PopulateRoles(user.Id).Select(r => r.Name).ToList();
                    user.Claims = _userClaimRepository.PopulateUserClaims(user);
                    user.Logins = _userLoginRepository.PopulateUserLogins(user.Id);
                }

            }
            return user;
        }

        public TUser GetByName(string userName)
        {
            TUser user = (TUser)Activator.CreateInstance(typeof(TUser));
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    {"@UserName", userName}
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT Id,Email,EmailConfirmed,
                        PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,
                        LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName, Activated, AttributesValidated FROM users WHERE UserName=@UserName", parameters);
                while (reader.Read())
                {
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
                    user.AttributesValidated = (bool)reader[13];

                    user.Roles = _userRoleRepository.PopulateRoles(user.Id).Select(r => r.Name).ToList();
                    user.Claims = _userClaimRepository.PopulateUserClaims(user);
                    user.Logins = _userLoginRepository.PopulateUserLogins(user.Id);
                }

            }
            return user;
        }

        public TUser GetByEmail(string email)
        {
            TUser user = (TUser)Activator.CreateInstance(typeof(TUser));
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    {"@Email", email}
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,

                    @"SELECT Id, Email, EmailConfirmed,
                        PasswordHash, SecurityStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled,
                        LockoutEndDateUtc, LockoutEnabled, AccessFailedCount, UserName, Activated, AttributesValidated FROM users WHERE Email = @Email", parameters);
                while (reader.Read())
                {
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
                    user.AttributesValidated = (bool)reader[13];

                    user.Roles = _userRoleRepository.PopulateRoles(user.Id).Select(r => r.Name).ToList();
                    user.Claims = _userClaimRepository.PopulateUserClaims(user);
                    user.Logins = _userLoginRepository.PopulateUserLogins(user.Id);
                }

            }
            return user;
        }

        public IQueryable<TUser> GetQueryFormatter(string order, int limit, int offset, string sort, string search, string groupId, bool inGroup)
        {
            List<TUser> _users = new List<TUser>();
            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Limit", limit },
                    { "@Offset", offset }
                };
            string query = @"SELECT Id, Email, EmailConfirmed,
                PasswordHash, SecurityStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled,
                LockoutEndDateUtc, LockoutEnabled, AccessFailedCount, UserName, Activated, AttributesValidated FROM users";
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
                if (search.Length > 0)
                {
                    query += $" WHERE (UserName LIKE '{search}%' OR Email LIKE '{search}%')";
                }
            }
            if (!string.IsNullOrEmpty(groupId))
            {
                if (string.IsNullOrEmpty(search))
                {
                    query += " WHERE";
                }
                else
                {
                    query += " AND";
                }
                query += " Id " + (inGroup ? "IN" : "NOT IN") + " (SELECT UserId FROM group_users WHERE GroupId = '" + groupId + "')";
            }
            if (!string.IsNullOrEmpty(sort))
            {
                sort = sort.First().ToString().ToUpper() + sort.Substring(1);
                query += " ORDER BY " + sort + " " + order;
            }
            query += " LIMIT @Offset, @Limit";
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, query, parameters);
                while (reader.Read())
                {
                    TUser user = (TUser)Activator.CreateInstance(typeof(TUser));
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
                    user.AttributesValidated = (bool)reader[13];

                    user.Roles = _userRoleRepository.PopulateRoles(user.Id).Select(r => r.Name).ToList();
                    user.Claims = _userClaimRepository.PopulateUserClaims(user);
                    user.Logins = _userLoginRepository.PopulateUserLogins(user.Id);

                    _users.Add(user);
                }
            }
            return _users.AsQueryable();
        }

        public IQueryable<TUser> GetQueryFormatterByRoleId(string order, int limit, int offset, string sort, string search, string roleId, bool inRole)
        {
            List<TUser> _users = new List<TUser>();
            Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Limit", limit },
                    { "@Offset", offset }
                };
            string query = @"SELECT Id, Email, EmailConfirmed,
                PasswordHash, SecurityStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled,
                LockoutEndDateUtc, LockoutEnabled, AccessFailedCount, UserName, Activated, AttributesValidated FROM users";
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
                if (search.Length > 0)
                {
                    query += $" WHERE (UserName LIKE '{search}%' OR Email LIKE '{search}%')";
                }
            }
            if (!string.IsNullOrEmpty(roleId))
            {
                if (string.IsNullOrEmpty(search))
                {
                    query += " WHERE";
                }
                else
                {
                    query += " AND";
                }
                query += " Id " + (inRole ? "IN" : "NOT IN") + " (SELECT UserId FROM user_roles WHERE RoleId = '" + roleId + "')";
            }
            if (!string.IsNullOrEmpty(sort))
            {
                sort = sort.First().ToString().ToUpper() + sort.Substring(1);
                query += " ORDER BY " + sort + " " + order;
            }
            query += " LIMIT @Offset, @Limit";
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, query, parameters);
                while (reader.Read())
                {
                    TUser user = (TUser)Activator.CreateInstance(typeof(TUser));
                    user.Id = reader[0].ToString();
                    user.Email = reader[1].ToString();
                    user.EmailConfirmed = (bool)reader[2];
                    user.PasswordHash = reader[3].ToString();
                    user.SecurityStamp = reader[4].ToString();
                    user.PhoneNumber = reader[5].ToString();
                    user.PhoneNumberConfirmed = (bool)reader[6];
                    user.TwoFactorAuthEnabled = (bool)reader[7];
                    user.LockoutEndDate = reader[8] == DBNull.Value ? null : (DateTime?)reader[8];
                    user.LockoutEnabled = (bool)reader[9];
                    user.AccessFailedCount = (int)reader[10];
                    user.UserName = reader[11].ToString();
                    user.Activated = (bool)reader[12];
                    user.AttributesValidated = (bool)reader[13];

                    user.Roles = _userRoleRepository.PopulateRoles(user.Id).Select(r => r.Name).ToList();
                    user.Claims = _userClaimRepository.PopulateUserClaims(user);
                    user.Logins = _userLoginRepository.PopulateUserLogins(user.Id);

                    _users.Add(user);
                }
            }
            return _users.AsQueryable();
        }

        public int GetQueryFormatterCount(string search, string groupId, bool inGroup)
        {
            string query = @"SELECT COUNT(Id) FROM users";
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
                if (search.Length > 0)
                {
                    query += " WHERE UserName LIKE '" + search + "%' OR Email LIKE '" + search + "%'";
                }
            }
            if (!string.IsNullOrEmpty(groupId))
            {
                if (string.IsNullOrEmpty(search))
                {
                    query += " WHERE";
                }
                else
                {
                    query += " AND";
                }
                query += $" Id " + (inGroup ? "IN" : "NOT IN") + " (SELECT UserId FROM group_users WHERE GroupId = '" + groupId + "')";
            }
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, query, null);
                while (reader.Read())
                {
                    return Convert.ToInt32(reader[0]);
                }
            }
            return 0;
        }

        public int GetQueryFormatterCountByRoleId(string search, string roleId, bool inGroup)
        {
            string query = @"SELECT COUNT(Id) FROM users";
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
                if (search.Length > 0)
                {
                    query += " WHERE UserName LIKE '" + search + "%' OR Email LIKE '" + search + "%'";
                }
            }
            if (!string.IsNullOrEmpty(roleId))
            {
                if (string.IsNullOrEmpty(search))
                {
                    query += " WHERE";
                }
                else
                {
                    query += " AND";
                }
                query += $" Id " + (inGroup ? "IN" : "NOT IN") + " (SELECT UserId FROM user_roles WHERE RoleId = '" + roleId + "')";
            }
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, query, null);
                while (reader.Read())
                {
                    return Convert.ToInt32(reader[0]);
                }
            }
            return 0;
        }

        public List<User> GetUsersByObjectEndpointId(string objectEndpointId)
        {
            List<User> _users = new List<User>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ObjectEndpointId", objectEndpointId }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT u.Id, u.Email, u.EmailConfirmed,
                    u.PasswordHash, u.SecurityStamp, u.PhoneNumber, u.PhoneNumberConfirmed, u.TwoFactorEnabled,
                    u.LockoutEndDateUtc, u.LockoutEnabled, u.AccessFailedCount, u.UserName, u.Activated, u.AttributesValidated 
                    FROM user_roles ur, users u WHERE ur.RoleId IN (SELECT Id FROM roles WHERE Id IN 
                    (SELECT rp.RoleId FROM object_endpoint_permissions op, role_permissions rp 
                     WHERE op.ObjectEndpointId = @ObjectEndpointId AND op.Id = rp.ObjectEndpointPermissionId)) AND u.Id = ur.UserId", parameters);
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
                    user.LockoutEndDate = reader[8] == DBNull.Value ? null : (DateTime?)reader[8];
                    user.LockoutEnabled = (bool)reader[9];
                    user.AccessFailedCount = (int)reader[10];
                    user.UserName = reader[11].ToString();
                    user.Activated = (bool)reader[12];
                    user.AttributesValidated = (bool)reader[13];

                    user.Roles = _userRoleRepository.PopulateRoles(user.Id).Select(r => r.Name).ToList();
                    user.Claims = _userClaimRepository.PopulateUserClaims(user);
                    user.Logins = _userLoginRepository.PopulateUserLogins(user.Id);

                    _users.Add(user);
                }
            }
            return _users;
        }

        public void Update(TUser user)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@NewId", user.Id },
                    { "@Email", (object) user.Email ?? "" },
                    { "@EmailConfirmed", user.EmailConfirmed },
                    { "@PasswordHash", (object) user.PasswordHash ?? "" },
                    { "@SecurityStamp", (object) user.SecurityStamp ?? "" },
                    { "@PhoneNumber", (object) user.PhoneNumber ?? "" },
                    { "@PhoneNumberConfirmed", user.PhoneNumberConfirmed },
                    { "@TwoFactorAuthEnabled", user.TwoFactorAuthEnabled },
                    { "@LockoutEndDate", (object) user.LockoutEndDate ?? DateTime.Now },
                    { "@LockoutEnabled", user.LockoutEnabled },
                    { "@AccessFailedCount", user.AccessFailedCount },
                    { "@UserName", !string.IsNullOrEmpty(user.UserName) ? user.UserName : user.Email },
                    { "@Id", user.Id },
                    { "@Activated", user.Activated },
                    { "@AttributesValidated", user.AttributesValidated }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"UPDATE users 
                SET Id = @NewId, Email = @Email, EmailConfirmed = @EmailConfirmed, PasswordHash = COALESCE(@PasswordHash, PasswordHash), SecurityStamp = @SecurityStamp, PhoneNumber = @PhoneNumber,
                PhoneNumberConfirmed = @PhoneNumberConfirmed, TwoFactorEnabled = @TwoFactorAuthEnabled, LockoutEndDateUtc = @LockoutEndDate, LockoutEnabled = @LockoutEnabled,
                AccessFailedCount = @AccessFailedCount, UserName = @UserName, Activated = @Activated, AttributesValidated = @AttributesValidated
                WHERE Id = @Id", parameters);
            }
        }

        public void UpdatePassword(string userId, string new_password)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@UserId", userId },
                    { "@PasswordHash", new_password }
                };
                MySqlHelper.ExecuteNonQuery(conn, @"UPDATE users SET PasswordHash=@PasswordHash WHERE Id=@UserId", parameters);
            }
        }

        public void UpdateActivated(string userId, bool activated)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", userId },
                    { "@Activated", activated }
                };
                MySqlHelper.ExecuteNonQuery(conn, @"UPDATE users SET Activated = @Activated WHERE Id = @Id LIMIT 1", parameters);
            }
        }

        public void UpdateAttributesValidated(bool attributesValidated)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@AttributesValidated", attributesValidated }
                };
                MySqlHelper.ExecuteNonQuery(conn, @"UPDATE users SET AttributesValidated = @AttributesValidated WHERE AttributesValidated != @AttributesValidated", parameters);
            }
        }

        public void UpdateAttributesValidated(string userId, bool attributesValidated)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", userId },
                    { "@AttributesValidated", attributesValidated }
                };
                MySqlHelper.ExecuteNonQuery(conn, @"UPDATE users SET AttributesValidated = @AttributesValidated WHERE Id = @Id LIMIT 1", parameters);
            }
        }

        public void UpdateEmailConfirmed(string userId, bool emailConfirmed)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", userId },
                    { "@EmailConfirmed", emailConfirmed }
                };
                MySqlHelper.ExecuteNonQuery(conn, @"UPDATE users SET EmailConfirmed = @EmailConfirmed WHERE Id = @Id LIMIT 1", parameters);
            }
        }

        public bool ValidatePermissionOnUrl(string userId, string objectUrl, string endpointValue, string permissionName)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@ObjectUrl", objectUrl },
                { "@ObjectEndpointValue", endpointValue },
                { "@UserId", userId },
                { "@PermissionName", permissionName }
            };

            string query = @"
                SELECT
                  COUNT(*)
                FROM objects o
                  JOIN user_roles ur
                    ON ur.UserId = @UserId
                  JOIN role_permissions rp
                    ON rp.RoleId = ur.RoleId
                  JOIN object_endpoints oe
                    ON oe.ObjectId = o.Id
                    AND oe.Value = @ObjectEndpointValue
                  JOIN object_client oc
                    ON oc.ObjectId = o.Id
                  JOIN clients c
                    ON c.Id = oc.ClientId
                    AND c.Enabled = 1
                  JOIN object_endpoint_permissions oep
                    ON oep.Id = rp.ObjectEndpointPermissionId
                    AND oep.ObjectEndpointId = oe.Id
                  JOIN permissions p
                    ON p.Id = oep.PermissionId
                    AND p.Name = @PermissionName
                    AND p.Id = oep.PermissionId
                WHERE o.Url = @ObjectUrl";


            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, query, parameters);
                while (reader.Read())
                {
                    return Convert.ToInt32(reader[0]) > 0;
                }
            }

            return false;
        }

        public bool GetRequiredAttributesOnUser(string userId)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "@UserId", userId }
            };

            string query = @"
                SELECT
                  COUNT(c.ClaimValue) cnt
                FROM (SELECT
                    uc.ClaimValue AS ClaimValue
                  FROM profile_attribute_claim pac
                    JOIN profile_attributes pa
                      ON pa.Id = pac.ProfileAttributeId
                    LEFT JOIN user_claims uc
                      ON uc.ClaimId = pac.ClaimId
                      AND uc.UserId = @UserId) c
                WHERE ClaimValue = ''";


            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, query, parameters);
                while (reader.Read())
                {
                    return Convert.ToInt32(reader[0]) > 0;
                }
            }

            return false;
        }
    }
}
