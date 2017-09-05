using Microsoft.AspNetCore.Identity;
using MySql.AspNet.Identity.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace MySql.AspNet.Identity
{
    public class LocalPartitionStore<TLocalPartition> where TLocalPartition : LocalPartition
    {
        private readonly string _connectionString;
        private readonly LocalPartitionRepository<TLocalPartition> _localPartitionRepository;

        public LocalPartitionStore()
            : this("DefaultConnection")
        {

        }

        public LocalPartitionStore(string connectionString)
        {
            _connectionString = connectionString;
            _localPartitionRepository = new LocalPartitionRepository<TLocalPartition>(_connectionString);
        }

        public IQueryable<TLocalPartition> LocalPartitions
        {
            get
            {
                return _localPartitionRepository.Get();
            }
        }

        public Task<TLocalPartition> FindAsync(string localPartitionId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(localPartitionId))
            {
                throw new ArgumentException("localPartitionId");
            }
            TLocalPartition localPartition = _localPartitionRepository.Get(localPartitionId);
            return Task.FromResult(localPartition);
        }

        public Task<TLocalPartition> FindAsync(string controllerName, string actionName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(controllerName))
            {
                throw new ArgumentException("controllerName");
            }
            if (string.IsNullOrEmpty(actionName))
            {
                throw new ArgumentException("actionName");
            }
            TLocalPartition localPartition = _localPartitionRepository.Get(controllerName, actionName);
            return Task.FromResult(localPartition);
        }

        public Task<int> GetRoleLocalPartitionCountAsync(string controllerName, string actionName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(controllerName))
            {
                throw new ArgumentException("controllerName");
            }
            if (string.IsNullOrEmpty(actionName))
            {
                throw new ArgumentException("actionName");
            }

            return Task.FromResult(_localPartitionRepository.GetRoleLocalPartitionCount(controllerName, actionName));
        }

        public Task<IdentityResult> CreateAsync(TLocalPartition localPartition, CancellationToken cancellationToken)
        {
            if (localPartition == null)
            {
                throw new ArgumentException("localPartition");
            }
            if (string.IsNullOrEmpty(localPartition.ControllerName))
            {
                throw new ArgumentException("ControllerName");
            }
            if (string.IsNullOrEmpty(localPartition.ActionName))
            {
                throw new ArgumentException("ActionName");
            }

            TLocalPartition oldLocalPartition = _localPartitionRepository.Get(localPartition.ControllerName, localPartition.ActionName);
            if (oldLocalPartition == null)
            {
                _localPartitionRepository.Insert(localPartition);
            }

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(TLocalPartition localPartition, CancellationToken cancellationToken)
        {
            if (localPartition == null)
            {
                throw new ArgumentException("localPartition");
            }
            if (string.IsNullOrEmpty(localPartition.ControllerName))
            {
                throw new ArgumentException("ControllerName");
            }
            if (string.IsNullOrEmpty(localPartition.ActionName))
            {
                throw new ArgumentException("ActionName");
            }
            _localPartitionRepository.Update(localPartition);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(TLocalPartition localPartition, CancellationToken cancellationToken)
        {
            if (localPartition == null)
            {
                throw new ArgumentException("localPartition");
            }
            _localPartitionRepository.Delete(localPartition.Id);
            return Task.FromResult(IdentityResult.Success);
        }


    }
}
