using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MySql.AspNet.Identity.Repositories
{
    public class LocalPartitionRepository<TLocalPartition> where TLocalPartition : LocalPartition
    {
        private readonly string _connectionString;

        public LocalPartitionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IQueryable<TLocalPartition> Get()
        {
            List<TLocalPartition> localPartitions = (List<TLocalPartition>)Activator.CreateInstance(typeof(List<TLocalPartition>));
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                IDataReader reader = MySqlHelper.ExecuteReader(conn,
                    CommandType.Text,
                    @"SELECT Id, ControllerName, ActionName FROM local_partitions",
                    null);
                while (reader.Read())
                {
                    TLocalPartition localPartition = (TLocalPartition)Activator.CreateInstance(typeof(TLocalPartition));
                    localPartition.Id = reader[0].ToString();
                    localPartition.ControllerName = reader[1].ToString();
                    localPartition.ActionName = reader[2].ToString();
                    localPartitions.Add(localPartition);
                }
            }
            return localPartitions.AsQueryable();
        }

        public TLocalPartition Get(string localPartitionId)
        {
            TLocalPartition localPartition = null;
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", localPartitionId },
                };
                IDataReader reader = MySqlHelper.ExecuteReader(conn,
                    CommandType.Text,
                    @"SELECT Id, ControllerName, ActionName FROM local_partitions WHERE Id = @Id LIMIT 1",
                    parameters);
                while (reader.Read())
                {
                    localPartition = (TLocalPartition)Activator.CreateInstance(typeof(TLocalPartition));
                    localPartition.Id = reader[0].ToString();
                    localPartition.ControllerName = reader[1].ToString();
                    localPartition.ActionName = reader[2].ToString();
                }
            }
            return localPartition;
        }

        public TLocalPartition Get(string controllerName, string actionName)
        {
            TLocalPartition localPartition = null;
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ControllerName", controllerName },
                    { "@ActionName", actionName },
                };
                IDataReader reader = MySqlHelper.ExecuteReader(conn,
                    CommandType.Text,
                    @"SELECT Id, ControllerName, ActionName FROM local_partitions
                        WHERE ControllerName = @ControllerName AND ActionName = @ActionName LIMIT 1",
                    parameters);
                while (reader.Read())
                {
                    localPartition = (TLocalPartition)Activator.CreateInstance(typeof(TLocalPartition));
                    localPartition.Id = reader[0].ToString();
                    localPartition.ControllerName = reader[1].ToString();
                    localPartition.ActionName = reader[2].ToString();
                }
            }
            return localPartition;
        }

        public void Insert(LocalPartition localPartition)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", localPartition.Id },
                    { "@ControllerName", localPartition.ControllerName },
                    { "@ActionName", localPartition.ActionName },
                };
                MySqlHelper.ExecuteNonQuery(conn,
                    @"INSERT INTO local_partitions (Id, ControllerName, ActionName) 
                        VALUES(@Id, @ControllerName, @ActionName)",
                    parameters);
            }
        }

        public void Update(LocalPartition localPartition)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", localPartition.Id },
                    { "@ControllerName", localPartition.ControllerName },
                    { "@ActionName", localPartition.ActionName },
                };
                MySqlHelper.ExecuteNonQuery(conn,
                    @"UPDATE local_partitions 
                        SET ControllerName = @ControllerName, ActionName = @ActionName
                        WHERE Id = @Id LIMIT 1",
                    parameters);
            }
        }

        public void Delete(string localPartitionId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", localPartitionId },
                };
                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM local_partitions WHERE Id = @Id LIMIT 1", parameters);
            }
        }

        public void Delete(string controllerName, string actionName)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ControllerName", controllerName },
                    { "@ActionName", actionName }
                };
                MySqlHelper.ExecuteNonQuery(conn,
                    @"DELETE FROM local_partitions WHERE ControllerName = @ControllerName AND ActionName = @ActionName LIMIT 1",
                    parameters);
            }
        }

        public int GetRoleLocalPartitionCount(string controllerName, string actionName)
        {
            if (string.IsNullOrEmpty(controllerName) || string.IsNullOrEmpty(actionName))
            {
                return 0;
            }

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ControllerName", controllerName },
                    { "@ActionName", actionName }
                };
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT
                              COUNT(*)
                            FROM local_partitions lp
                              JOIN role_local_partitions rlp
                                ON rlp.LocalPartitionId = lp.Id
                            WHERE lp.ControllerName = @ControllerName
                            AND lp.ActionName = @ActionName", parameters);

                while (reader.Read())
                {
                    return Convert.ToInt32(reader[0]);
                }
            }
            return 0;
        }
    }
}
