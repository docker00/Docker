using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class EmailSmtpServerInputModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "{0} должен быть указан")]
        [Display(Name = "Адрес Smtp")]
        public string SmtpAdress { get; set; }
        [Required(ErrorMessage = "{0} должен быть указан")]
        [Display(Name = "Порт Smtp")]
        public int SmtpPort { get; set; }
        [Display(Name = "Безопасная связть (SSL)")]
        public bool SslRequred { get; set; }
        [Display(Name = "Имя отправителя")]
        public string AuthenticateName { get; set; }
        [Required(ErrorMessage = "{0} должен быть указан")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public string AuthenticateLogin { get; set; }
        [Required(ErrorMessage = "{0} должен быть заполнен")]
        [Display(Name = "Пароль")]
        public string AuthenticatePassword { get; set; }
        [Display(Name = "Включен?")]
        public bool Enabled { get; set; }

        public EmailSmtpServerInputModel()
        {
        }

        public EmailSmtpServerInputModel(string smtpAdress, int smtpPort, bool sslRequred,
           string authenticateName, string authenticateLogin, string authenticatePassword, bool enabled)
        {
            SmtpAdress = smtpAdress;
            SmtpPort = smtpPort;
            SslRequred = sslRequred;
            AuthenticateName = string.IsNullOrEmpty(authenticateName) ? authenticateLogin : authenticateName;
            AuthenticateLogin = authenticateLogin;
            AuthenticatePassword = authenticatePassword;
            Enabled = enabled;
        }

        public EmailSmtpServerInputModel(string id, string smtpAdress, int smtpPort, bool sslRequred,
           string authenticateName, string authenticateLogin, string authenticatePassword, bool enabled)
            : this(smtpAdress, smtpPort, sslRequred,
           authenticateName, authenticateLogin, authenticatePassword, enabled)
        {
            Id = id;
        }
    }
}
