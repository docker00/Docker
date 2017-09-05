using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MySql.AspNet.Identity.Repositories
{
    public class EmailSmtpServerRepository<TEmailSmtpServer> where TEmailSmtpServer : EmailSmtpServer
    {
        private readonly string _connectionString;

        public EmailSmtpServerRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public TEmailSmtpServer Get()
        {
            TEmailSmtpServer emailSmtpServer = null;
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                IDataReader reader = MySqlHelper.ExecuteReader(conn,
                    CommandType.Text,
                    @"SELECT Id, SmtpAdress, SmtpPort, SslRequred, AuthenticateName, AuthenticateLogin, AuthenticatePassword, Enabled
                        FROM email_smtp_server LIMIT 1",
                    null);
                while (reader.Read())
                {
                    emailSmtpServer = (TEmailSmtpServer)Activator.CreateInstance(typeof(TEmailSmtpServer));
                    emailSmtpServer.Id = reader[0].ToString();
                    emailSmtpServer.SmtpAdress = reader[1].ToString();
                    emailSmtpServer.SmtpPort = Convert.ToInt32(reader[2]);
                    emailSmtpServer.SslRequred = (bool)reader[3];
                    emailSmtpServer.AuthenticateName = reader[4].ToString();
                    emailSmtpServer.AuthenticateLogin = reader[5].ToString();
                    emailSmtpServer.AuthenticatePassword = reader[6].ToString();
                    emailSmtpServer.Enabled = (bool)reader[7];
                }
            }

            return emailSmtpServer;
        }

        public void Insert(TEmailSmtpServer emailSmtpServer)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", emailSmtpServer.Id },
                    { "@SmtpAdress", emailSmtpServer.SmtpAdress },
                    { "@SmtpPort", emailSmtpServer.SmtpPort },
                    { "@SslRequred", emailSmtpServer.SslRequred },
                    { "@AuthenticateName", emailSmtpServer.AuthenticateName ?? string.Empty },
                    { "@AuthenticateLogin", emailSmtpServer.AuthenticateLogin ?? string.Empty },
                    { "@AuthenticatePassword", emailSmtpServer.AuthenticatePassword ?? string.Empty },
                    { "@Enabled", emailSmtpServer.Enabled }
                };
                MySqlHelper.ExecuteNonQuery(conn,
                    CommandType.Text,
                    @"INSERT INTO email_smtp_server (Id, SmtpAdress, SmtpPort, SslRequred, AuthenticateName, AuthenticateLogin, AuthenticatePassword, Enabled)
                        VALUES (@Id, @SmtpAdress, @SmtpPort, @SslRequred, @AuthenticateName, @AuthenticateLogin, @AuthenticatePassword, @Enabled)",
                    parameters);
            }
        }

        public void Update(TEmailSmtpServer emailSmtpServer)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", emailSmtpServer.Id },
                    { "@SmtpAdress", emailSmtpServer.SmtpAdress },
                    { "@SmtpPort", emailSmtpServer.SmtpPort },
                    { "@SslRequred", emailSmtpServer.SslRequred },
                    { "@AuthenticateName", emailSmtpServer.AuthenticateName },
                    { "@AuthenticateLogin", emailSmtpServer.AuthenticateLogin },
                    { "@AuthenticatePassword", emailSmtpServer.AuthenticatePassword },
                    { "@Enabled", emailSmtpServer.Enabled }
                };
                MySqlHelper.ExecuteNonQuery(conn,
                    CommandType.Text,
                    @"UPDATE email_smtp_server SET SmtpAdress = @SmtpAdress, SmtpPort = @SmtpPort, SslRequred = @SslRequred, AuthenticateName = @AuthenticateName, 
                        AuthenticateLogin = @AuthenticateLogin, AuthenticatePassword = @AuthenticatePassword, Enabled = @Enabled
                        WHERE Id = @Id LIMIT 1",
                    parameters);
            }
        }

        public void Delete(string Id)
        {
            using (MySqlConnection conn = new MySqlConnection(_connectionString))
            {
                Dictionary<string, object> parameters = new Dictionary<string, object>
                {
                    { "@Id", Id }
                };
                MySqlHelper.ExecuteNonQuery(conn,
                    CommandType.Text,
                    @"DELETE FROM email_smtp_server WHERE Id = @Id",
                    parameters);
            }
        }
    }
}