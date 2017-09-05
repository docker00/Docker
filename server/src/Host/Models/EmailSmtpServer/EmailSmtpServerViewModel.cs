using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.Quickstart.UI
{
    public class EmailSmtpServerViewModel : EmailSmtpServerInputModel
    {
        public EmailSmtpServerViewModel()
        {
        }

        public EmailSmtpServerViewModel(string id, string smtpAdress, int smtpPort, bool sslRequred,
           string authenticateName, string authenticateLogin, string authenticatePassword, bool enabled)
            : base(id, smtpAdress, smtpPort, sslRequred, authenticateName, authenticateLogin, authenticatePassword, enabled)
        {
        }
    }
}
