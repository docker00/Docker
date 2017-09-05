using MySql.AspNet.Identity;
using MySql.AspNet.Identity.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Host.Extensions.MessageService
{
    public class MessageService
    {
        private readonly EmailSmtpServerRepository<EmailSmtpServer> _emailSmtpServerRepository;
        public bool isEmailSenderEnabled
        {
            get
            {
                return GetEmailService().CheckSmtpConnection().Result;
            }
        }


        public MessageService(string _connectionString)
        {
            _emailSmtpServerRepository = new EmailSmtpServerRepository<EmailSmtpServer>(_connectionString);

        }

        private EmailService GetEmailService()
        {
            EmailSmtpServer smtpServer = _emailSmtpServerRepository.Get();
            if (smtpServer != null)
            {
                EmailService emailService = new EmailService(smtpServer.SmtpAdress, smtpServer.SmtpPort, smtpServer.SslRequred,
                    smtpServer.AuthenticateName, smtpServer.AuthenticateLogin, smtpServer.AuthenticatePassword);
                return emailService;
            }
            return null;
        }

        public virtual async Task<bool> SendMessage(User user, string message)
        {
            EmailService emailService = GetEmailService();
            if (emailService != null)
            {
                if (user != null)
                {
                    bool result = await emailService.Send("", message, user.Email);
                    return result;
                }
            }
            return false;
        }

        public virtual async Task<bool> SendMessage(User user, string subject, string message)
        {
            EmailService emailService = GetEmailService();
            if (emailService != null)
            {
                if (user != null)
                {
                    bool result = await emailService.Send(subject, message, user.Email);
                    return result;
                }
            }
            return false;
        }

        public virtual async Task<bool> SendMessageMany(List<User> users, string message)
        {
            EmailService emailService = GetEmailService();
            if (emailService != null)
            {
                if (users != null && users.Count > 0)
                {
                    bool result = await emailService.SendMany("", message, users.Select(u => u.Email));
                    return result;
                }
            }
            return false;
        }

        public virtual async Task<bool> SendMessageMany(List<User> users, string subject, string message)
        {
            EmailService emailService = GetEmailService();
            if (emailService != null)
            {
                if (users != null && users.Count > 0)
                {
                    bool result = await emailService.SendMany(subject, message, users.Select(u => u.Email));
                    return result;
                }
            }
            return false;
        }
    }
}
