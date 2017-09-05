using Microsoft.AspNetCore.Identity;
using MySql.AspNet.Identity.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace MySql.AspNet.Identity
{
    public class ObjectEndpointStore
    {
        private readonly string _connectionString;
        private readonly ObjectEndpointRepository _objectEndpointRepository;
        private readonly ObjectEndpointPermissionsRepository _objectEndpointPermissionsRepository;

        public ObjectEndpointStore()
            : this("DefaultConnection")
        {

        }

        public ObjectEndpointStore(string connectionString)
        {
            _connectionString = connectionString;
            _objectEndpointRepository = new ObjectEndpointRepository(_connectionString);
            _objectEndpointPermissionsRepository = new ObjectEndpointPermissionsRepository(_connectionString);
        }

        public IQueryable<ObjectEndpoint> ObjectEndpoints
        {
            get
            {
                return _objectEndpointRepository.GetObjectEndpoints();
            }
        }

        public void Dispose()
        {
            // connection is automatically disposed
        }

        public Task<IdentityResult> CreateAsync(ObjectEndpoint objectEndpoint, CancellationToken cancellationToken)
        {
            if (objectEndpoint == null)
            {
                throw new ArgumentNullException("objectEndpoint");
            }

            _objectEndpointRepository.Insert(objectEndpoint);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdatePermissionAsync(string objectEndpointId, IList<string> permissionsIds, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(objectEndpointId))
            {
                throw new ArgumentNullException("objectEndpoint");
            }
            if(permissionsIds == null)
            {
                permissionsIds = new List<string>();
            }
            List<ObjectEndpointPermition> oldEndpointPermissions = _objectEndpointPermissionsRepository.GetByObjectEndpointId(objectEndpointId);
            foreach (ObjectEndpointPermition oldEndpointPermission in oldEndpointPermissions)
            {
                if (!permissionsIds.Contains(oldEndpointPermission.PermissionId))
                {
                    _objectEndpointPermissionsRepository.Delete(objectEndpointId, oldEndpointPermission.PermissionId);
                }
                else
                {
                    permissionsIds.Remove(oldEndpointPermission.PermissionId);
                }
            }
            foreach (string permissionId in permissionsIds)
            {
                _objectEndpointPermissionsRepository.Insert(objectEndpointId, permissionId);
            }

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeletePermissionAsync(string objectEndpointId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(objectEndpointId))
            {
                throw new ArgumentNullException("objectEndpoint");
            }

            _objectEndpointPermissionsRepository.DeletePermissions(objectEndpointId);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(ObjectEndpoint objectEndpoint, CancellationToken cancellationToken)
        {
            if (objectEndpoint == null)
            {
                throw new ArgumentNullException("objectEndpoint");
            }

            _objectEndpointRepository.Update(objectEndpoint);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(ObjectEndpoint objectEndpoint, CancellationToken cancellationToken)
        {
            if (objectEndpoint == null)
            {
                throw new ArgumentNullException("objectEndpoint");
            }

            _objectEndpointRepository.Delete(objectEndpoint.Id);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IQueryable<ObjectEndpoint>> GetObjectEndpointsByObjectAsync(Object _object, CancellationToken cancellationToken)
        {
            if (_object == null)
            {
                return Task.FromResult<IQueryable<ObjectEndpoint>>(null);
            }

            IList<ObjectEndpoint> result = _objectEndpointRepository.PopulateObjectEndpoints(_object.Id);

            return Task.FromResult(result.AsQueryable());
        }

        public Task<IQueryable<Permission>> GetEndpointPermissionsIdByObjectAsync(ObjectEndpointPermition object_, CancellationToken cancellationToken)
        {
            if (object_ == null)
            {
                return Task.FromResult<IQueryable<Permission>>(null);
            }

            IList<Permission> result = _objectEndpointPermissionsRepository.PopulateObjectEndpointPermissions(object_.Id);

            return Task.FromResult(result.AsQueryable());
        }

        public Task<ObjectEndpoint> FindByIdAsync(string objectEndpointId, CancellationToken cancellationToken)
        {
            ObjectEndpoint result = _objectEndpointRepository.GetObjectEndpointById(objectEndpointId);

            return Task.FromResult<ObjectEndpoint>(result);
        }

        public Task<ObjectEndpoint> FindByObjectIdAsync(string objectId, CancellationToken cancellationToken)
        {
            ObjectEndpoint result = _objectEndpointRepository.GetObjectEndpointByObjectId(objectId);

            return Task.FromResult<ObjectEndpoint>(result);
        }

        public Task<IQueryable<Permission>> FindEndpointPermissionsIdByObjectAsync(string objectId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(objectId))
            {
                return Task.FromResult<IQueryable<Permission>>(null);
            }

            IList<Permission> result = _objectEndpointPermissionsRepository.PopulateObjectEndpointPermissions(objectId);

            return Task.FromResult(result.AsQueryable());
        }

        public Task<IQueryable<ObjectEndpointPermition>> FindObjectEndpointPermission(string objectEndpointId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(objectEndpointId))
            {
                return Task.FromResult<IQueryable<ObjectEndpointPermition>>(null);
            }

            IList<ObjectEndpointPermition> result = _objectEndpointPermissionsRepository.GetByObjectEndpointId(objectEndpointId);

            return Task.FromResult(result.AsQueryable());
        }

        public Task<IQueryable<ObjectEndpointPermition>> FindObjectEndpointPermissionByRoleAsync(string roleId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return Task.FromResult<IQueryable<ObjectEndpointPermition>>(null);
            }

            IList<ObjectEndpointPermition> result = _objectEndpointPermissionsRepository.PopulateObjectEndpointPermissionByRole(roleId);

            return Task.FromResult(result.AsQueryable());
        }

        public Task<int> GetObjectEndpointQueryFormatterCountAsync(string search)
        {
            return Task.FromResult(_objectEndpointRepository.GetQueryFormatterCount(search));
        }


        public Task<IQueryable<ObjectEndpoint>> FindObjectEndpointsByObjectIdAsync(string objectId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(objectId))
            {
                return Task.FromResult<IQueryable<ObjectEndpoint>>(null);
            }

            IList<ObjectEndpoint> result = _objectEndpointRepository.PopulateObjectEndpoints(objectId);

            return Task.FromResult(result.AsQueryable());
        }
    }
}
