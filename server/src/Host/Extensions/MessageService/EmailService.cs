using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace Host.Extensions.MessageService
{
    public class EmailService
    {
        /// <summary>
        /// Адрес smtp-сервера, с которого будет отправлятся письмо
        /// </summary>
        public string SmtpAdress { get; private set; }
        /// <summary>
        /// Порт smtp-сервера
        /// </summary>
        public int SmtpPort { get; private set; }
        /// <summary>
        /// Использовать безопасную связь
        /// </summary>
        public bool SslRequred { get; private set; }
        /// <summary>
        /// Имя отправителя. Например, Администрация сайта
        /// </summary>
        public string AuthenticateName { get; private set; }
        /// <summary>
        /// Логин отправителя. Должен соответствовать smtp-серверу
        /// </summary>
        public string AuthenticateLogin { get; private set; }
        /// <summary>
        /// Пароль отпарвителя
        /// </summary>
        public string AuthenticatePassword { get; private set; }

        /// <summary>
        /// По умолчанию устанавливает stmp-сервер smtp.yandex.ru с портом 25
        /// </summary>
        /// <param name="authenticateLogin">Логин отправителя. Должен соответствовать smtp-серверу</param>
        /// <param name="authenticatePassword">Пароль отпарвителя</param>
        public EmailService(string authenticateLogin, string authenticatePassword)
        {
            SmtpAdress = "smtp.yandex.ru";
            SmtpPort = 25;
            SslRequred = false;
            AuthenticateName = authenticateLogin;
            AuthenticateLogin = authenticateLogin;
            AuthenticatePassword = authenticatePassword;
        }

        /// <summary>
        /// Полная настройка отправки сообщений
        /// </summary>
        /// <param name="smtpAdress">Адрес smtp-сервера, с которого будет отправлятся письмо</param>
        /// <param name="smtpPort">Порт smtp-сервера</param>
        /// <param name="sslRequred">Использовать безопасную связь</param>
        /// <param name="authenticateName">Имя отправителя. Например, Администрация сайта</param>
        /// <param name="authenticateLogin">Логин отправителя. Должен соответствовать smtp-серверу</param>
        /// <param name="authenticatePassword">Пароль отпарвителя</param>
        public EmailService(string smtpAdress,
            int smtpPort,
            bool sslRequred,
            string authenticateName,
            string authenticateLogin,
            string authenticatePassword)
        {
            SmtpAdress = smtpAdress;
            SmtpPort = smtpPort;
            SslRequred = sslRequred;
            AuthenticateName = string.IsNullOrEmpty(authenticateName) ? authenticateLogin : authenticateName;
            AuthenticateLogin = authenticateLogin;
            AuthenticatePassword = authenticatePassword;
        }

        /// <summary>
        /// Отправляет сообщение с темой subject и текстом messageText на указанный email
        /// </summary>
        /// <param name="subject">Тема письма</param>
        /// <param name="messageText">Текст письма</param>
        /// <param name="email">Получатель</param>
        /// <returns></returns>
        public async Task<bool> Send(string subject, string messageText, string email)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(AuthenticateName, AuthenticateLogin));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = messageText
            };
            try
            {
                using (SmtpClient client = new SmtpClient())
                {
                    client.SslProtocols = System.Security.Authentication.SslProtocols.Tls11 | System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls;
                    client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => ServerCertificateValidation(sender, certificate, chain, sslPolicyErrors);
                    await client.ConnectAsync(SmtpAdress, SmtpPort, SslRequred);
                    await client.AuthenticateAsync(AuthenticateLogin, AuthenticatePassword);
                    await client.SendAsync(emailMessage);

                    await client.DisconnectAsync(true);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> SendMany(string subject, string messageText, IEnumerable<string> emails)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(AuthenticateName, AuthenticateLogin));
            foreach (string email in emails)
            {
                emailMessage.To.Add(new MailboxAddress("", email));
            }
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = messageText
            };
            try
            {
                using (SmtpClient client = new SmtpClient())
                {
                    client.SslProtocols = System.Security.Authentication.SslProtocols.Tls11 | System.Security.Authentication.SslProtocols.Tls12 | System.Security.Authentication.SslProtocols.Tls;
                    client.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => ServerCertificateValidation(sender, certificate, chain, sslPolicyErrors);
                    await client.ConnectAsync(SmtpAdress, SmtpPort, SslRequred);
                    await client.AuthenticateAsync(AuthenticateLogin, AuthenticatePassword);
                    await client.SendAsync(emailMessage);

                    await client.DisconnectAsync(true);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> CheckSmtpConnection()
        {
            try
            {
                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(SmtpAdress, SmtpPort, SslRequred);
                    await client.AuthenticateAsync(AuthenticateLogin, AuthenticatePassword);
                    await client.DisconnectAsync(true);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool ServerCertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;

            if ((sslPolicyErrors & SslPolicyErrors.RemoteCertificateChainErrors) != 0)
            {
                if (chain != null && chain.ChainStatus != null)
                {
                    foreach (X509ChainStatus status in chain.ChainStatus)
                    {
                        if ((certificate.Subject == certificate.Issuer) && (status.Status == X509ChainStatusFlags.UntrustedRoot))
                        {
                            continue;
                        }
                        else if (status.Status != X509ChainStatusFlags.NoError)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

            return false;
        }

        public bool Equals(EmailService emailService)
        {
            if (this.SmtpAdress == emailService.SmtpAdress &&
                this.SmtpPort == emailService.SmtpPort &&
                this.SslRequred == emailService.SslRequred &&
                this.AuthenticateLogin == emailService.AuthenticateLogin &&
                this.AuthenticateName == emailService.AuthenticateName &&
                this.AuthenticatePassword == emailService.AuthenticatePassword)
            {
                return true;
            }
            else
                return false;
        }
    }
}
