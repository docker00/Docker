using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MySql.AspNet.Identity.Repositories
{
    public class UserDeviceRepository<TUserDevice> where TUserDevice : UserDevice
    {
        private readonly string _connectionString;
        public UserDeviceRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public TUserDevice Get(string id)
        {
            TUserDevice userDevice = null;
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", id }
                };
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT Id, UserId, Name, Type, Confirmed FROM user_devices WHERE Id = @Id LIMIT 1", parameters);
                while (reader.Read())
                {
                    userDevice = (TUserDevice)Activator.CreateInstance(typeof(TUserDevice));
                    userDevice.Id = reader[0].ToString();
                    userDevice.UserId = reader[1].ToString();
                    userDevice.Name = reader[2].ToString();
                    userDevice.Type = reader[3].ToString();
                    userDevice.Confirmed = (bool)reader[4];
                }
            }
            return userDevice;
        }

        public void Create(TUserDevice userDevice)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", userDevice.Id },
                    { "@UserId", userDevice.UserId },
                    { "@Name", userDevice.Name },
                    { "@Type", userDevice.Type },
                    { "@Confirmed", userDevice.Confirmed }
                };
                MySqlHelper.ExecuteNonQuery(conn, CommandType.Text,
                    @"INSERT INTO user_devices (Id, UserId, Name, Type, Confirmed) VALUES(@Id, @UserId, @Name, @Type, @Confirmed)",
                    parameters);
            }
        }

        public void Update(TUserDevice userDevice)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                string id = Guid.NewGuid().ToString();
                string apiKey = Guid.NewGuid().ToString();
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", userDevice.Id },
                    { "@UserId", userDevice.UserId },
                    { "@Name", userDevice.Name },
                    { "@Type", userDevice.Type },
                    { "@Confirmed", userDevice.Confirmed }
                };
                MySqlHelper.ExecuteNonQuery(conn, CommandType.Text,
                    @"UPDATE user_devices SET Id = @Id, UserId = @UserId, Name = @Name, Type = @Type, Confirmed = @Confirmed", parameters);
            }
        }

        public IQueryable<TUserDevice> PopulateUserDevices(string userId)
        {
            List<TUserDevice> userDevices = new List<TUserDevice>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@UserId", userId }
                };
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT Id, UserId, Name, Type, Confirmed FROM user_devices WHERE UserId = @UserId", parameters);
                while (reader.Read())
                {
                    TUserDevice userDevice = (TUserDevice)Activator.CreateInstance(typeof(TUserDevice));
                    userDevice.Id = reader[0].ToString();
                    userDevice.UserId = reader[1].ToString();
                    userDevice.Name = reader[2].ToString();
                    userDevice.Type = reader[3].ToString();
                    userDevice.Confirmed = (bool)reader[4];
                    userDevices.Add(userDevice);
                }
            }
            return userDevices.AsQueryable();
        }
    }
}
