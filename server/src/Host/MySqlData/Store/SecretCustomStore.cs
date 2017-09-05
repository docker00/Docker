using Microsoft.AspNetCore.Identity;
using MySql.AspNet.Identity.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using IdentityServer4.Models;

namespace MySql.AspNet.Identity
{
    public class SecretCustomStore
    {
        private readonly string _connectionString;
        private readonly SecretCustomRepository _secretCustomRepository;
        public SecretCustomStore()
            : this("DefaultConnection")
        {

        }

        public SecretCustomStore(string connectionString)
        {
            _connectionString = connectionString;
            _secretCustomRepository = new SecretCustomRepository(_connectionString);
        }

        public IQueryable<SecretCustom> SecretCustoms
        {
            get
            {
                return _secretCustomRepository.GetSecretCustoms();
            }
        }

        public void Dispose()
        {
            // connection is automatically disposed
        }

        public Task<IdentityResult> CreateAsync(SecretCustom secretCustom, CancellationToken cancellationToken)
        {
            if (secretCustom == null)
            {
                throw new ArgumentNullException("secretCustom");
            }
            if (Constants.SecretTypes.Where(s => s.Value == secretCustom.Type).Select(s => new { Key = s.Key, Value = s.Value })
                .FirstOrDefault() == null)
            {
                throw new ArgumentNullException("Type");
            }
            if (secretCustom.Type.ToLower().Equals("SharedSecret".ToLower()))
            {
                secretCustom.Value = secretCustom.Value.Sha256();
            }
            _secretCustomRepository.Insert(secretCustom);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(SecretCustom secretCustom, CancellationToken cancellationToken)
        {
            if (secretCustom == null)
            {
                throw new ArgumentNullException("secretCustom");
            }
            if (Constants.SecretTypes.Where(s => s.Value == secretCustom.Type).Select(s => new { Key = s.Key, Value = s.Value })
              .FirstOrDefault() == null)
            {
                throw new ArgumentNullException("Type");
            }
            if (secretCustom.Type.ToLower().Equals("SharedSecret".ToLower()))
            {
                secretCustom.Value = secretCustom.Value.Sha256();
            }
            _secretCustomRepository.Update(secretCustom);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(SecretCustom secretCustom, CancellationToken cancellationToken)
        {
            if (secretCustom == null)
            {
                throw new ArgumentNullException("secretCustom");
            }

            _secretCustomRepository.Delete(secretCustom.Id);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<SecretCustom> FindByIdAsync(string secretCustomId, CancellationToken cancellationToken)
        {
            SecretCustom result = _secretCustomRepository.GetSecretCustomById(secretCustomId);

            return Task.FromResult(result);
        }

        public Task<SecretCustom> FindByNameAsync(string secretCustomValue, CancellationToken cancellationToken)
        {
            SecretCustom result = _secretCustomRepository.GetSecretCustomByValue(secretCustomValue);
            return Task.FromResult(result);
        }

        public Task<KeyValuePair<int, IQueryable<SecretCustom>>> GetSecretsQueryFormatterAsync(string order, int limit, int offset, string sort, string search)
        {
            KeyValuePair<int, IQueryable<SecretCustom>> secrets = _secretCustomRepository.GetSecretQueryFormatter(order, limit, offset, sort, search);
            return Task.FromResult(secrets);
        }
    }
}
