using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;

namespace MySql.AspNet.Identity.Repositories
{
    public class ClaimCustomRepository
    {
        private readonly string _connectionString;
        public ClaimCustomRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IQueryable<ClaimCustom> GetClaimCustoms()
        {
            List<ClaimCustom> claimCustoms = new List<ClaimCustom>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Type FROM claims", null);

                while (reader.Read())
                {
                    ClaimCustom _claimCustom = new ClaimCustom(reader[1].ToString(), "");
                    _claimCustom.Id = reader[0].ToString();
                    claimCustoms.Add(_claimCustom);
                }

            }
            return claimCustoms.AsQueryable();
        }

        public string Insert(ClaimCustom claimCustom)
        {
            string id = Guid.NewGuid().ToString();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", id },
                    { "@Type", claimCustom.Type }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO claims (Id, Type) VALUES (@Id, @Type)", parameters);
            }

            return id;
        }

        public void Update(ClaimCustom claimCustom)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", claimCustom.Id },
                    { "@Type", claimCustom.Type }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"UPDATE claims SET Type = @Type WHERE Id = @Id", parameters);
            }
        }

        public void Delete(string claimCustomId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@id", claimCustomId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM claims WHERE Id = @id", parameters);
            }
        }

        public ClaimCustom GetClaimCustomById(string claimCustomId)
        {
            ClaimCustom claimCustom = null;

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", claimCustomId }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Type FROM claims WHERE Id = @Id", parameters);

                while (reader.Read())
                {
                    ClaimCustom _claimCustom = new ClaimCustom(reader[1].ToString(), "");
                    _claimCustom.Id = reader[0].ToString();

                    claimCustom = _claimCustom;
                }
            }

            return claimCustom;
        }

        public ClaimCustom GetClaimCustomByType(string claimCustomType, string claimCustomValue = "")
        {
            ClaimCustom claimCustom = null;

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Type", claimCustomType }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Type FROM claims WHERE Type = @Type", parameters);

                while (reader.Read())
                {
                    ClaimCustom _claimCustom = new ClaimCustom(reader[1].ToString(), claimCustomValue);
                    _claimCustom.Id = reader[0].ToString();

                    claimCustom = _claimCustom;
                }
            }

            return claimCustom;
        }

    }
}
