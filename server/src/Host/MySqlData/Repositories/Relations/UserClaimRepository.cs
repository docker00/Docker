using IdentityModel;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;

namespace MySql.AspNet.Identity.Repositories
{
    public class UserClaimRepository<TUser> where TUser : User
    {
        private readonly string _connectionString;
        private readonly ClaimCustomRepository _claimCustomRepository;
        public UserClaimRepository(string connectionString)
        {
            _connectionString = connectionString;
            _claimCustomRepository = new ClaimCustomRepository(_connectionString);
        }

        public ClaimCustom Get(string userId, string claimId)
        {
            ClaimCustom claim = null;
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@UserId", userId },
                    { "@ClaimId", claimId }
                };
                IDataReader reader = MySqlHelper.ExecuteReader(conn,
                    CommandType.Text,
                    @"SELECT c.Id, c.Type, uc.ClaimValue FROM claims c 
                        JOIN user_claims uc ON uc.ClaimId = @ClaimId AND uc.UserId = @UserId AND c.Id = uc.ClaimId",
                    parameters);
                if (reader.Read())
                {
                    claim = new ClaimCustom(reader[1].ToString(), reader[2].ToString());
                    claim.Id = reader[0].ToString();
                }
            }
            return claim;
        }

        public void Insert(string userId, ClaimCustom claim)
        {
            string id = Guid.NewGuid().ToString();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", id },
                    { "@UserId", userId },
                    { "@ClaimId", claim.Id },
                    { "@ClaimValue",claim.Value }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO user_claims (Id, UserId, ClaimId, ClaimValue) VALUES(@Id, @UserId, @ClaimId, @ClaimValue)", parameters);
            }
        }

        public void Update(string userId, ClaimCustom claim)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@UserId", userId },
                    { "@ClaimId", claim.Id },
                    { "@ClaimValue",claim.Value }
                };

                MySqlHelper.ExecuteNonQuery(
                    conn,
                    @"UPDATE user_claims SET ClaimValue = @ClaimValue WHERE UserId=@UserId AND ClaimId=@ClaimId",
                    parameters);
            }
        }

        public void Delete(string userId, ClaimCustom claim)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@UserId", userId },
                    { "@ClaimId", claim.Id },
                    { "@ClaimValue", claim.Value }
                };

                MySqlHelper.ExecuteNonQuery(conn,
                    @"DELETE FROM user_claims WHERE UserId = @UserId AND ClaimId = @ClaimId AND ClaimValue = @ClaimValue", parameters);
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

                MySqlHelper.ExecuteNonQuery(conn,
                    @"DELETE FROM user_claims WHERE UserId = @UserId", parameters);
            }
        }

        public IQueryable<ClaimCustom> PopulateUserClaimsCustom(string userId)
        {
            List<ClaimCustom> claims = new List<ClaimCustom>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@UserId", userId }
                };
                IDataReader reader = MySqlHelper.ExecuteReader(
                    conn,
                    CommandType.Text,
                    @"SELECT c.Id, c.Type, uc.ClaimValue FROM claims c JOIN user_claims uc ON uc.UserId = @UserId AND uc.ClaimId = c.Id",
                    parameters);
                while (reader.Read())
                {
                    ClaimCustom claim = new ClaimCustom(reader[1].ToString(), reader[2].ToString());
                    claim.Id = reader[0].ToString();
                    claims.Add(claim);
                }
            }

            return claims.AsQueryable();
        }

        public List<Claim> PopulateUserClaims(User user)
        {
            List<Claim> claims = new List<Claim>();

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@UserId", user.Id }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                @"SELECT c.Type, uc.ClaimValue
                       FROM claims c
                         JOIN user_claims uc
                           ON uc.UserId = @UserId
                           AND uc.ClaimId = c.Id", parameters);
                while (reader.Read())
                {
                    Claim claim = new Claim(reader[0].ToString(), reader[1].ToString());

                    claims.Add(claim);
                }
            }

            Claim subjectClaim = new Claim(JwtClaimTypes.Subject, user.Id);
            claims.Add(subjectClaim);
            Claim userNameClaim = new Claim(JwtClaimTypes.Name, user.UserName);
            claims.Add(userNameClaim);
            if (!string.IsNullOrEmpty(user.Email))
            {
                Claim emailClaim = new Claim(JwtClaimTypes.Email, user.Email);
                claims.Add(emailClaim);

                Claim EmailVerifiedClaim = new Claim(JwtClaimTypes.EmailVerified, user.EmailConfirmed.ToString());
                claims.Add(EmailVerifiedClaim);
            }
            if (!string.IsNullOrEmpty(user.PhoneNumber))
            {
                Claim phoneNumberClaim = new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber);
                claims.Add(phoneNumberClaim);

                Claim phoneNumberVerifiedClaim = new Claim(JwtClaimTypes.PhoneNumberVerified, user.PhoneNumberConfirmed.ToString());
                claims.Add(phoneNumberVerifiedClaim);
            }

            return claims;
        }
    }
}
