using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace MySql.AspNet.Identity.Repositories
{
    public class UserTestRepository
    {
        private readonly string _connectionString;
        public UserTestRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void InsertTestUsers()
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                for (int i = 3228321; i < 10000000; ++i)
                {
                    string id = Guid.NewGuid().ToString();
                    string email = "test" + i + "@gmail.com";

                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        {"@Id", id},
                        {"@Email", (object) email },
                        {"@EmailConfirmed", true },
                        {"@PasswordHash", CalculateMD5Hash(email) },
                        {"@SecurityStamp", (object) "" },
                        {"@PhoneNumber", (object) "" },
                        {"@PhoneNumberConfirmed", true },
                        {"@TwoFactorAuthEnabled", false },
                        {"@LockoutEndDate", (object) null },
                        {"@LockoutEnabled", true },
                        {"@AccessFailedCount", 0 },
                        {"@UserName", email }
                    };

                    MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO users (Id, Email, EmailConfirmed,
                        PasswordHash, SecurityStamp, PhoneNumber, PhoneNumberConfirmed, TwoFactorEnabled,
                        LockoutEndDateUtc, LockoutEnabled, AccessFailedCount, UserName) VALUES(@Id, @Email, @EmailConfirmed, @PasswordHash, @SecurityStamp, @PhoneNumber, @PhoneNumberConfirmed,
                        @TwoFactorAuthEnabled, @LockoutEndDate, @LockoutEnabled, @AccessFailedCount, @UserName)", parameters);
                }
            }
        }

        public string CalculateMD5Hash(string input)
        {
            StringBuilder sb = new StringBuilder();

            input += "--(!)--(.)--_!_--";

            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }
    }
}
