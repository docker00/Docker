using IdentityServer4.Quickstart.UI;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MySql.AspNet.Identity.Repositories
{
    public class RoleLocalPartitionRepository
    {
        private readonly string _connectionString;

        public RoleLocalPartitionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void Insert(string roleId, string localPartitionId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                string id = Guid.NewGuid().ToString();
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", id },
                    { "@RoleId", roleId },
                    { "@LocalPartitionId", localPartitionId }
                };
                MySqlHelper.ExecuteNonQuery(conn,
                    @"INSERT INTO role_local_partitions (Id, RoleId, LocalPartitionId) VALUES(@Id, @RoleId, @LocalPartitionId)",
                    parameters);
            }
        }

        public void Delete(string roleId, string localPartitionId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                string id = Guid.NewGuid().ToString();
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@RoleId", roleId },
                    { "@LocalPartitionId", localPartitionId }
                };
                MySqlHelper.ExecuteNonQuery(conn,
                    @"DELETE FROM role_local_partitions WHERE RoleId = @RoleId AND @LocalPartitionId = LocalPartitionId LIMIT 1",
                    parameters);
            }
        }

        public IQueryable<LocalPartition> PopulateRoleLocalPartitions(string roleId)
        {
            List<LocalPartition> localPartitions = new List<LocalPartition>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                string id = Guid.NewGuid().ToString();
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@RoleId", roleId }
                };
                IDataReader reader = MySqlHelper.ExecuteReader(conn,
                    CommandType.Text,
                    @"SELECT lp.Id, lp.ControllerName, lp.ActionName FROM local_partitions lp 
                        JOIN role_local_partitions rlp ON rlp.RoleId = @RoleId AND lp.Id = rlp.LocalPartitionId",
                    parameters);
                while (reader.Read())
                {
                    LocalPartition localPartition = (LocalPartition)Activator.CreateInstance(typeof(LocalPartition));

                    localPartition.Id = reader[0].ToString();
                    localPartition.ControllerName = reader[1].ToString();
                    localPartition.ActionName = reader[2].ToString();
                    localPartitions.Add(localPartition);

                }
            }
            return localPartitions.AsQueryable();
        }

        public IQueryable<LocalPartition> PopulateRoleLocalPartitionsByUserId(string userId)
        {
            List<LocalPartition> localPartitions = new List<LocalPartition>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@UserId", userId }
                };
                IDataReader reader = MySqlHelper.ExecuteReader(conn,
                    CommandType.Text,
                    @"SELECT
                          DISTINCT
                          lp.Id,
                          lp.ControllerName,
                          lp.ActionName
                        FROM local_partitions lp
                          JOIN user_roles ur ON ur.UserId = @UserId
                          JOIN role_local_partitions rlp
                            ON rlp.RoleId = ur.RoleId
                            AND lp.Id = rlp.LocalPartitionId",
                    parameters);
                while (reader.Read())
                {
                    LocalPartition localPartition = (LocalPartition)Activator.CreateInstance(typeof(LocalPartition));

                    localPartition.Id = reader[0].ToString();
                    localPartition.ControllerName = reader[1].ToString();
                    localPartition.ActionName = reader[2].ToString();

                    localPartitions.Add(localPartition);
                }
            }
            return localPartitions.AsQueryable();
        }

        public IQueryable<LocalPartition> PopulateRoleLocalPartitionsByRoleId(string roleId)
        {
            List<LocalPartition> localPartitions = new List<LocalPartition>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@RoleId", roleId }
                };
                IDataReader reader = MySqlHelper.ExecuteReader(conn,
                    CommandType.Text,
                    @"SELECT
                        DISTINCT
                          lp.Id,
                          lp.ControllerName,
                          lp.ActionName
                        FROM local_partitions lp
                          JOIN role_local_partitions rlp
                            ON rlp.RoleId = @RoleId
                            AND lp.Id = rlp.LocalPartitionId",
                    parameters);
                while (reader.Read())
                {
                    LocalPartition localPartition = (LocalPartition)Activator.CreateInstance(typeof(LocalPartition));

                    localPartition.Id = reader[0].ToString();
                    localPartition.ControllerName = reader[1].ToString();
                    localPartition.ActionName = reader[2].ToString();

                    localPartitions.Add(localPartition);
                }
            }
            return localPartitions.AsQueryable();
        }

        public bool CheckControllerNameAndActionNameByUserId(string userId, string controllerName, string actionName)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@UserId", userId },
                    { "@ControllerName", controllerName },
                    { "@ActionName", actionName }
                };
                IDataReader reader = MySqlHelper.ExecuteReader(conn,
                    CommandType.Text,
                    @"SELECT
                      COUNT(*)
                    FROM local_partitions lp
                      JOIN user_roles ur
                        ON ur.UserId = @UserId
                      JOIN role_local_partitions rlp
                        ON rlp.RoleId = ur.RoleId
                        AND lp.Id = rlp.LocalPartitionId 
                        AND lp.ControllerName = @ControllerName 
                        AND lp.ActionName = @ActionName",
                    parameters);
                while (reader.Read())
                {
                    return Convert.ToInt32(reader[0]) > 0;
                }
            }
            return false;
        }
        public RoleLocalPartitionModel Get(string roleId, string localPartitionId)
        {
            RoleLocalPartitionModel roleLocalPartition = null;
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@RoleId", roleId },
                    { "@LocalPartitionId", localPartitionId }
                };
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text,
                    @"SELECT Id, RoleId, LocalPartitionId FROM role_local_partitions WHERE RoleId = @RoleId AND LocalPartitionId = @LocalPartitionId LIMIT 1",
                    parameters);
                while (reader.Read())
                {
                    roleLocalPartition = (RoleLocalPartitionModel)Activator.CreateInstance(typeof(RoleLocalPartitionModel));

                    roleLocalPartition.Id = reader[0].ToString();
                    roleLocalPartition.RoleId = reader[1].ToString();
                    roleLocalPartition.LocalPartitionId = reader[2].ToString();
                }
            }
            return roleLocalPartition;
        }
    }
}
