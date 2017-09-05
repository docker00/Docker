using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MySql.AspNet.Identity.Repositories
{
    public class UserConfirmCodeRepository<TUserConfirmCode> where TUserConfirmCode : UserConfirmCode
    {
        private readonly string _connectionString;
        public UserConfirmCodeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public TUserConfirmCode Get(string userId, string code)
        {
            TUserConfirmCode userConfirmCode = null;
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@UserId", userId },
                    { "@Code", code }
                };
                IDataReader reader = MySqlHelper.ExecuteReader(conn,
                    CommandType.Text,
                    @"SELECT Id, UserId, Code, TimeEnd FROM user_confirm_codes 
                        WHERE UserId = @UserId AND Code = @Code AND TimeEnd > NOW() LIMIT 1",
                    parameters);
                while (reader.Read())
                {
                    userConfirmCode = (TUserConfirmCode)Activator.CreateInstance(typeof(TUserConfirmCode));
                    userConfirmCode.Id = reader[0].ToString();
                    userConfirmCode.UserId = reader[1].ToString();
                    userConfirmCode.Code = reader[2].ToString();
                    userConfirmCode.TimeEnd = Convert.ToDateTime(reader[3]);
                }
            }
            return userConfirmCode;
        }

        public void Insert(TUserConfirmCode userConfirmCode)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", userConfirmCode.Id },
                    { "@UserId", userConfirmCode.UserId },
                    { "@Code", userConfirmCode.Code },
                    { "@TimeEnd", userConfirmCode.TimeEnd }
                };
                MySqlHelper.ExecuteNonQuery(conn,
                    CommandType.Text,
                    @"INSERT INTO user_confirm_codes (Id, UserId, Code, TimeEnd) 
                        VALUES (@Id, @UserId, @Code, @TimeEnd)",
                    parameters);
            }
        }

        public void Delete(string userId, string codeId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@UserId", userId },
                    { "@Code", codeId },
                };
                MySqlHelper.ExecuteNonQuery(conn,
                    CommandType.Text,
                    @"DELETE FROM user_confirm_codes WHERE UserId = @UserId AND Code = @Code LIMIT 1",
                    parameters);
            }
        }
    }
}
