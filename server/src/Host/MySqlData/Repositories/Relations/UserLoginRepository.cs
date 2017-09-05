using Microsoft.AspNetCore.Identity;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace MySql.AspNet.Identity.Repositories
{
    public class UserLoginRepository
    {
        private readonly string _connectionString;
        public UserLoginRepository(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void Insert(string userId, IdentityUserLogin login)
        {
            string id = Guid.NewGuid().ToString();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", id },
                    { "@UserId", userId },
                    { "@LoginProvider", login.LoginProvider },
                    { "@ProviderKey", login.ProviderKey },
                    { "@ProviderDisplayName",  login.ProviderDisplayName }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO user_logins (Id, UserId, LoginProvider, ProviderKey, ProviderDisplayName)
                                        VALUES(@Id, @UserId, @LoginProvider, @ProviderKey, @ProviderDisplayName)", parameters);
            }
        }

        public void Delete(string userId, IdentityUserLogin login)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@UserId", userId },
                    { "@LoginProvider", login.LoginProvider },
                    { "@ProviderKey", login.ProviderKey }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM user_logins WHERE UserId = @UserId AND LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey", parameters);
            }
        }

        public string GetByUserLoginInfo(IdentityUserLogin login)
        {
            string userId;
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@LoginProvider", login.LoginProvider },
                    { "@ProviderKey", login.ProviderKey }
                };
                object userIdObject = MySqlHelper.ExecuteScalar(conn, CommandType.Text,
                    @"SELECT UserId FROM user_logins WHERE LoginProvider = @LoginProvider AND ProviderKey = @ProviderKey", parameters);
                userId = userIdObject == null
                    ? null
                    : userIdObject.ToString();
            }
            return userId;
        }

        public List<IdentityUserLogin> PopulateUserLogins(string userId)
        {
            List<IdentityUserLogin> listLogins = new List<IdentityUserLogin>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@UserId",userId }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                   @"SELECT LoginProvider, ProviderKey, ProviderDisplayName FROM user_logins WHERE UserId = @UserId", parameters);
                while (reader.Read())
                {
                    listLogins.Add(new IdentityUserLogin(reader[0].ToString(), reader[1].ToString(), reader[2].ToString()));
                }
            }
            return listLogins;
        }


    }
}
