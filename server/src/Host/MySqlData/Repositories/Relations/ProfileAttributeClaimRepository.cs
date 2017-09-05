using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Linq;

namespace MySql.AspNet.Identity.Repositories
{
    public class ProfileAttributeClaimRepository
    {
        private readonly string _connectionString;
        public ProfileAttributeClaimRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IQueryable<ProfileAttributeClaim> GetAll()
        {
            List<ProfileAttributeClaim> profileAttributeClaims = new List<ProfileAttributeClaim>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                IDataReader reader = MySqlHelper.ExecuteReader(conn,
                    CommandType.Text,
                    @"SELECT
                        pac.Id, pac.ProfileAttributeId, pa.Name, pac.ClaimId, pa.RequiredRegister, pa.RequiredAdditional, pa.Disabled, pa.Weight, pa.Deleted
                      FROM profile_attribute_claim pac
                        JOIN profile_attributes pa
                        ON pa.Id = pac.ProfileAttributeId",
                    null);
                while (reader.Read())
                {
                    ProfileAttributeClaim profileAttributeClaim = (ProfileAttributeClaim)Activator.CreateInstance(typeof(ProfileAttributeClaim));
                    profileAttributeClaim.Id = reader[0].ToString();
                    profileAttributeClaim.ProfileAttributeId = reader[1].ToString();
                    profileAttributeClaim.ProfileAttributeName = reader[2].ToString();
                    profileAttributeClaim.ClaimId = reader[3].ToString();

                    profileAttributeClaim.RequiredRegister = (bool)reader[4];
                    profileAttributeClaim.RequiredAdditional = (bool)reader[5];
                    profileAttributeClaim.Disabled = (bool)reader[6];
                    profileAttributeClaim.Weight = Convert.ToInt32(reader[7]);
                    profileAttributeClaim.Deleted = (bool)reader[8];

                    profileAttributeClaims.Add(profileAttributeClaim);
                }
            }

            return profileAttributeClaims.AsQueryable();
        }

        public void Insert(string profileAttributeId, string claimId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                string id = Guid.NewGuid().ToString();
                Dictionary<string, object> parameters = new Dictionary<string, object> {
                    { "@Id", id },
                    { "@ProfileAttributeId", profileAttributeId },
                    { "@ClaimId", claimId }
                };
                MySqlHelper.ExecuteNonQuery(conn, CommandType.Text, @"INSERT INTO profile_attribute_claim (Id, ProfileAttributeId, ClaimId) VALUES (@Id, @ProfileAttributeId, @ClaimId)", parameters);
            }
        }

        public void Update(string profileAttributeId, string claimId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ProfileAttributeId", profileAttributeId },
                    { "@ClaimId", claimId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"UPDATE profile_attribute_claim SET ClaimId = @ClaimId WHERE ProfileAttributeId = @ProfileAttributeId", parameters);
            }
        }

        public void Delete(string profileAttributeId, string claimId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ProfileAttributeId", profileAttributeId },
                    { "@ClaimId", claimId}
                };
                MySqlHelper.ExecuteNonQuery(conn, @"Delete FROM profile_attribute_claim WHERE ProfileAttributeId = @ProfileAttributeId AND ClaimId = @ClaimId", parameters);
            }
        }

        public void DeleteByProfileAttributeId(string profileAttributeId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ProfileAttributeId", profileAttributeId }
                };
                MySqlHelper.ExecuteNonQuery(conn, @"Delete FROM profile_attribute_claim WHERE ProfileAttributeId = @ProfileAttributeId", parameters);
            }
        }

        public IQueryable<ProfileAttributeClaim> PopulateUserProfileAttributeClaim(string userId)
        {
            List<ProfileAttributeClaim> profileAttributeClaims = new List<ProfileAttributeClaim>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@UserId", userId }
                };
                IDataReader reader = MySqlHelper.ExecuteReader(conn,
                    CommandType.Text,
                    @"SELECT
                        pac.Id, pac.ProfileAttributeId, pa.Name, pac.ClaimId, COALESCE(uc.ClaimValue, '') AS ClaimValue
                      FROM profile_attribute_claim pac
                        JOIN profile_attributes pa
                        ON pa.Id = pac.ProfileAttributeId
                      LEFT JOIN user_claims uc
                        ON uc.ClaimId = pac.ClaimId
                        AND uc.UserId = @UserId",
                    parameters);
                while (reader.Read())
                {
                    ProfileAttributeClaim profileAttributeClaim = (ProfileAttributeClaim)Activator.CreateInstance(typeof(ProfileAttributeClaim));
                    profileAttributeClaim.Id = reader[0].ToString();
                    profileAttributeClaim.ProfileAttributeId = reader[1].ToString();
                    profileAttributeClaim.ProfileAttributeName = reader[2].ToString();
                    profileAttributeClaim.ClaimId = reader[3].ToString();
                    profileAttributeClaim.ClaimValue = reader[4].ToString();
                    profileAttributeClaims.Add(profileAttributeClaim);
                }
            }

            return profileAttributeClaims.AsQueryable();
        }

        public ProfileAttributeClaim GetByProfileAttributeId(string profileAttributeId)
        {
            ProfileAttributeClaim profileAttributeClaim = null;

            if (!string.IsNullOrEmpty(profileAttributeId))
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "@ProfileAttributeId", profileAttributeId }
                    };

                    IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, ProfileAttributeId, ClaimId FROM profile_attribute_claim WHERE ProfileAttributeId = @ProfileAttributeId LIMIT 1", parameters);
                    while (reader.Read())
                    {
                        profileAttributeClaim = (ProfileAttributeClaim)Activator.CreateInstance(typeof(ProfileAttributeClaim));

                        profileAttributeClaim.Id = reader[0].ToString();
                        profileAttributeClaim.ProfileAttributeId = reader[1].ToString();
                        profileAttributeClaim.ClaimId = reader[2].ToString();
                    }
                }
            }

            return profileAttributeClaim;
        }
    }
}
