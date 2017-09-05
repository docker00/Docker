using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;

namespace MySql.AspNet.Identity.Repositories
{
    public class ProfileAttributeRepository
    {
        private readonly string _connectionString;
        public ProfileAttributeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IQueryable<ProfileAttribute> GetProfileAttributes()
        {
            List<ProfileAttribute> profileAttributes = new List<ProfileAttribute>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Name, RequiredRegister, RequiredAdditional, Disabled, Weight, Deleted FROM profile_attributes", null);

                while (reader.Read())
                {
                    ProfileAttribute profileAttribute = (ProfileAttribute)Activator.CreateInstance(typeof(ProfileAttribute));

                    profileAttribute.Id = reader[0].ToString();
                    profileAttribute.Name = reader[1].ToString();
                    profileAttribute.RequiredRegister = (bool)reader[2];
                    profileAttribute.RequiredAdditional = (bool)reader[3];
                    profileAttribute.Disabled = (bool)reader[4];
                    profileAttribute.Weight = (int)reader[5];
                    profileAttribute.Deleted = (bool)reader[6];

                    profileAttributes.Add(profileAttribute);
                }

            }
            return profileAttributes.AsQueryable();
        }

        public void Insert(ProfileAttribute profileAttribute)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@id", profileAttribute.Id },
                    { "@name", profileAttribute.Name },
                    { "@requiredRegister", profileAttribute.RequiredRegister },
                    { "@requiredAdditional", profileAttribute.RequiredAdditional },
                    { "@disabled", profileAttribute.Disabled },
                    { "@weight", profileAttribute.Weight },
                    { "@deleted", profileAttribute.Deleted }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO profile_attributes (Id, Name, RequiredRegister, RequiredAdditional, Disabled, Weight, Deleted) 
                                                               VALUES (@id, @name, @requiredRegister, @requiredAdditional, @disabled, @weight, @deleted)", parameters);
            }
        }

        public void Update(ProfileAttribute profileAttribute)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    {"@id", profileAttribute.Id },
                    {"@name", profileAttribute.Name },
                    {"@requiredRegister", profileAttribute.RequiredRegister },
                    {"@requiredAdditional", profileAttribute.RequiredAdditional },
                    {"@disabled", profileAttribute.Disabled },
                    {"@weight", profileAttribute.Weight },
                    {"@deleted", profileAttribute.Deleted }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"UPDATE profile_attributes SET Name = @name, RequiredRegister = @requiredRegister,
                                                   RequiredAdditional = @requiredAdditional, Disabled = @disabled, Weight = @weight, Deleted = @deleted WHERE Id = @id", parameters);
            }
        }

        public void Delete(string profileAttributeId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@id", profileAttributeId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM profile_attributes WHERE Id = @id", parameters);
            }
        }

        public ProfileAttribute GetProfileAttributeById(string profileAttributeId)
        {
            ProfileAttribute profileAttribute = (ProfileAttribute)Activator.CreateInstance(typeof(ProfileAttribute));

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    {"@Id", profileAttributeId}
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Name, RequiredRegister, RequiredAdditional, Disabled, Weight, Deleted FROM profile_attributes WHERE Id = @Id", parameters);

                while (reader.Read())
                {
                    profileAttribute.Id = reader[0].ToString();
                    profileAttribute.Name = reader[1].ToString();
                    profileAttribute.RequiredRegister = (bool)reader[2];
                    profileAttribute.RequiredAdditional = (bool)reader[3];
                    profileAttribute.Disabled = (bool)reader[4];
                    profileAttribute.Weight = (int)reader[5];
                    profileAttribute.Deleted = (bool)reader[6];
                }
            }
            return profileAttribute;

        }

        public ProfileAttribute GetProfileAttributeByName(string profileAttributeName)
        {
            ProfileAttribute profileAttribute = null;

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    {"@Name", profileAttributeName}
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Name, RequiredRegister, RequiredAdditional, Disabled, Weight, Deleted FROM profile_attributes WHERE Name = @Name", parameters);

                while (reader.Read())
                {
                    profileAttribute = (ProfileAttribute)Activator.CreateInstance(typeof(ProfileAttribute));

                    profileAttribute.Id = reader[0].ToString();
                    profileAttribute.Name = reader[1].ToString();
                    profileAttribute.RequiredRegister = (bool)reader[2];
                    profileAttribute.RequiredAdditional = (bool)reader[3];
                    profileAttribute.Disabled = (bool)reader[4];
                    profileAttribute.Weight = (int)reader[5];
                    profileAttribute.Deleted = (bool)reader[6];
                }
            }

            return profileAttribute;
        }
    }
}
