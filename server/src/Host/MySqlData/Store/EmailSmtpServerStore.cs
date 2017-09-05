using Microsoft.AspNetCore.Identity;
using MySql.AspNet.Identity.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace MySql.AspNet.Identity
{
    public class EmailSmtpServerStore<TEmailSmtpServer> where TEmailSmtpServer : EmailSmtpServer
    {
        private readonly string _connectionString;
        private readonly EmailSmtpServerRepository<TEmailSmtpServer> _emailSmtpServerRepository;
        public EmailSmtpServerStore()
            : this("DefaultConnection")
        {

        }

        public EmailSmtpServerStore(string connectionString)
        {
            _connectionString = connectionString;
            _emailSmtpServerRepository = new EmailSmtpServerRepository<TEmailSmtpServer>(connectionString);
        }

        public TEmailSmtpServer EmailSmtpServer
        {
            get
            {
                return _emailSmtpServerRepository.Get();
            }
        }

        public Task<IdentityResult> CreateAsync(TEmailSmtpServer emailSmtpServer, CancellationToken cancellationToken)
        {
            if (emailSmtpServer == null)
            {
                throw new ArgumentException("emailSmtpServer");
            }
            if (_emailSmtpServerRepository.Get() != null)
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError() { Description = "Настройки уже созданы" }));
            }

            _emailSmtpServerRepository.Insert(emailSmtpServer);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(TEmailSmtpServer emailSmtpServer, CancellationToken cancellationToken)
        {
            if (emailSmtpServer == null)
            {
                throw new ArgumentException("emailSmtpServer");
            }
            _emailSmtpServerRepository.Update(emailSmtpServer);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(TEmailSmtpServer emailSmtpServer, CancellationToken cancellationToken)
        {
            if (emailSmtpServer == null)
            {
                throw new ArgumentException("emailSmtpServer");
            }
            _emailSmtpServerRepository.Delete(emailSmtpServer.Id);
            return Task.FromResult(IdentityResult.Success);
        }
    }
}
