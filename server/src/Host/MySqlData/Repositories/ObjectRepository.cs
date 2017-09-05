using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;

namespace MySql.AspNet.Identity.Repositories
{
    public class ObjectRepository
    {
        private readonly string _connectionString;
        public ObjectRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IQueryable<Object> GetObjects()
        {
            List<Object> objects = new List<Object>();
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Name, Description, Url FROM objects", null);

                while (reader.Read())
                {
                    Object _object = (Object)Activator.CreateInstance(typeof(Object));

                    _object.Id = reader[0].ToString();
                    _object.Name = reader[1].ToString();
                    _object.Description = reader[2].ToString();
                    _object.Url = reader[3].ToString();

                    objects.Add(_object);
                }
            }
            return objects.AsQueryable();
        }

        public void Insert(Object _object)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", _object.Id },
                    { "@Name", _object.Name },
                    { "@Description", _object.Description ?? string.Empty },
                    { "@Url", _object.Url == null ? string.Empty : _object.Url.TrimEnd('/') },
                };

                MySqlHelper.ExecuteNonQuery(conn, @"INSERT INTO objects (Id, Name, Description, Url) VALUES (@Id, @Name, @Description, @Url)", parameters);
            }
        }

        public void Delete(string objectId)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", objectId }
                };

                MySqlHelper.ExecuteNonQuery(conn, @"DELETE FROM objects WHERE Id = @Id", parameters);
            }
        }

        public Object GetObjectById(string objectId)
        {
            Object _object = (Object)Activator.CreateInstance(typeof(Object));

            if (!string.IsNullOrEmpty(objectId))
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "@Id", objectId }
                    };

                    IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Name, Description, Url FROM objects WHERE Id = @Id", parameters);
                    while (reader.Read())
                    {
                        _object.Id = reader[0].ToString();
                        _object.Name = reader[1].ToString();
                        _object.Description = reader[2].ToString();
                        _object.Url = reader[3].ToString();
                    }
                }
            }

            return _object;
        }

        public Object GetObjectByName(string objectName)
        {
            Object _object = null;

            if (!string.IsNullOrEmpty(objectName))
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "@Name", objectName }
                    };

                    IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Name, Description, Url FROM objects WHERE Name = @Name", parameters);
                    while (reader.Read())
                    {
                        _object = (Object)Activator.CreateInstance(typeof(Object));

                        _object.Id = reader[0].ToString();
                        _object.Name = reader[1].ToString();
                        _object.Description = reader[2].ToString();
                        _object.Url = reader[3].ToString();
                    }
                }
            }

            return _object;
        }

        public Object GetObjectByUrl(string objectUrl)
        {
            Object _object = null;

            if (!string.IsNullOrEmpty(objectUrl))
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    Dictionary<string, object> parameters = new Dictionary<string, object>
                    {
                        { "@Url", objectUrl }
                    };

                    IDataReader reader = MySqlHelper.ExecuteReader(conn, CommandType.Text, @"SELECT Id, Name, Description, Url FROM objects WHERE Url = @Url LIMIT 1", parameters);
                    while (reader.Read())
                    {
                        _object = (Object)Activator.CreateInstance(typeof(Object));

                        _object.Id = reader[0].ToString();
                        _object.Name = reader[1].ToString();
                        _object.Description = reader[2].ToString();
                        _object.Url = reader[3].ToString();
                    }
                }
            }

            return _object;
        }

        public int GetQueryFormatterCount(string search)
        {
            string query = @"SELECT COUNT(Id) FROM objects";
            Dictionary<string, object> parameters = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim();
                if (search.Length > 0)
                {
                    query += " WHERE ('Name' LIKE '%@search%' OR 'Description' LIKE '%@search%' OR 'Url' LIKE '%@search%')";
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

        public void Update(Object _object)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", _object.Id },
                    { "@Name", _object.Name },
                    { "@Description", _object.Description ?? string.Empty },
                    { "@Url", _object.Url == null ? string.Empty : _object.Url.TrimEnd('/') },
                };

                MySqlHelper.ExecuteNonQuery(conn, @"UPDATE objects SET Id = @Id, Name = @Name, Description = @Description, Url = @Url WHERE Id = @Id", parameters);
            }
        }
    }
}
