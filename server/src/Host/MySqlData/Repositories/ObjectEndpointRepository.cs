using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;

namespace MySql.AspNet.Identity.Repositories
{
    public class ObjectEndpointRepository
    {
        private readonly string _connectionString;

        public ObjectEndpointRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IQueryable<ObjectEndpoint> GetObjectEndpoints()
        {
            List<ObjectEndpoint> objectEndpoints = new List<ObjectEndpoint>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Value, ObjectId, Description FROM object_endpoints", null);

                while (reader.Read())
                {
                    ObjectEndpoint objectEndpoint = (ObjectEndpoint)Activator.CreateInstance(typeof(ObjectEndpoint));

                    objectEndpoint.Id = reader[0].ToString();
                    objectEndpoint.Value = reader[1].ToString();
                    objectEndpoint.ObjectId = reader[2].ToString();
                    objectEndpoint.Description = reader[3].ToString();

                    objectEndpoints.Add(objectEndpoint);
                }

            }
            return objectEndpoints.AsQueryable();
        }

        public void Insert(ObjectEndpoint objectEndpoint)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", objectEndpoint.Id },
                    { "@Value", objectEndpoint.Value },
                    { "@ObjectId", objectEndpoint.ObjectId },
                    { "@Description", objectEndpoint.Description }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO object_endpoints (Id, Value, ObjectId) VALUES (@Id, @Value, @ObjectId)", parameters);
            }
        }

        public void Update(ObjectEndpoint objectEndpoint)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", objectEndpoint.Id },
                    { "@Value", objectEndpoint.Value },
                    { "@ObjectId", objectEndpoint.ObjectId },
                    { "@Description", objectEndpoint.Description ?? string.Empty }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"UPDATE object_endpoints SET Value = @Value, ObjectId = @ObjectId, Description = @Description WHERE Id = @Id", parameters);
            }
        }

        public void Delete(string objectEndpointId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@id", objectEndpointId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM object_endpoints WHERE Id = @id", parameters);
            }
        }

        public void DeleteByObjectId(string objectId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ObjectId", objectId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM object_endpoints WHERE ObjectId = @ObjectId", parameters);
            }
        }

        public ObjectEndpoint GetObjectEndpointById(string objectEndpointId)
        {
            ObjectEndpoint objectEndpoint = null;

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", objectEndpointId }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Value, ObjectId, Description FROM object_endpoints WHERE Id = @Id", parameters);

                while (reader.Read())
                {
                    objectEndpoint = (ObjectEndpoint)Activator.CreateInstance(typeof(ObjectEndpoint));

                    objectEndpoint.Id = reader[0].ToString();
                    objectEndpoint.Value = reader[1].ToString();
                    objectEndpoint.ObjectId = reader[2].ToString();
                    objectEndpoint.Description = reader[3].ToString();
                }
            }

            return objectEndpoint;
        }

        public ObjectEndpoint GetObjectEndpointByObjectId(string ObjectId)
        {
            ObjectEndpoint objectEndpoint = null;

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@ObjectId", ObjectId }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Value, ObjectId, Description FROM object_endpoints WHERE ObjectId = @ObjectId", parameters);

                while (reader.Read())
                {
                    objectEndpoint = (ObjectEndpoint)Activator.CreateInstance(typeof(ObjectEndpoint));

                    objectEndpoint.Id = reader[0].ToString();
                    objectEndpoint.Value = reader[1].ToString();
                    objectEndpoint.ObjectId = reader[2].ToString();
                    objectEndpoint.Description = reader[3].ToString();
                }
            }

            return objectEndpoint;
        }

        public int GetQueryFormatterCount(string search)
        {
            string query = @"SELECT COUNT(Id) FROM object_endpoints";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
                if (search.Length > 0)
                {
                    query += " WHERE ('Value' LIKE '%@search%' )";
                    parameters.Add("@search", search);
                }
            }

            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, query, parameters);
                while (reader.Read())
                {
                    return Convert.ToInt32(reader[0]);
                }
            }
            return 0;
        }

        public List<ObjectEndpoint> PopulateObjectEndpoints(string objectId)
        {
            List<ObjectEndpoint> objectEndpoints = new List<ObjectEndpoint>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@objectId", objectId }
                };

                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Value, ObjectId, Description FROM object_endpoints WHERE ObjectId = @objectId", parameters);
                while (reader.Read())
                {
                    ObjectEndpoint objectEndpoint = (ObjectEndpoint)Activator.CreateInstance(typeof(ObjectEndpoint));

                    objectEndpoint.Id = reader[0].ToString();
                    objectEndpoint.Value = reader[1].ToString();
                    objectEndpoint.ObjectId = reader[2].ToString();
                    objectEndpoint.Description = reader[3].ToString();

                    objectEndpoints.Add(objectEndpoint);
                }
            }

            return objectEndpoints;
        }
    }
}
