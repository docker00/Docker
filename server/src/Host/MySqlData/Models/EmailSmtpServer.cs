using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySql.AspNet.Identity
{
    public class EmailSmtpServer
    {
        public string Id { get; set; }
        public string SmtpAdress { get; set; }
        public int SmtpPort { get; set; }
        public bool SslRequred { get; set; }
        public string AuthenticateName { get; set; }
        public string AuthenticateLogin { get; set; }
        public string AuthenticatePassword { get; set; }
        public bool Enabled { get; set; }

        public EmailSmtpServer()
        {
            Id = Guid.NewGuid().ToString();
        }

        public EmailSmtpServer(string smtpAdress, int smtpPort, bool sslRequred,
            string authenticateName, string authenticateLogin, string authenticatePassword, bool enabled)
            : this()
        {
            SmtpAdress = smtpAdress;
            SmtpPort = smtpPort;
            SslRequred = sslRequred;
            AuthenticateName = string.IsNullOrEmpty(authenticateName) ? authenticateLogin : authenticateName;
            AuthenticateLogin = authenticateLogin;
            AuthenticatePassword = authenticatePassword;
            Enabled = enabled;
        }

        public EmailSmtpServer(string id, string smtpAdress, int smtpPort, bool sslRequred,
           string authenticateName, string authenticateLogin, string authenticatePassword, bool enabled)
           : this(smtpAdress, smtpPort, sslRequred, authenticateName, authenticateLogin, authenticatePassword, enabled)
        {
            Id = id;
        }
    }
}
