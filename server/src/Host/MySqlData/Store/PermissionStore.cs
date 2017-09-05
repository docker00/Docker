using Microsoft.AspNetCore.Identity;
using MySql.AspNet.Identity.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace MySql.AspNet.Identity
{
    public class PermissionStore
    {
        private readonly string _connectionString;
        private readonly PermissionRepository _permissionRepository;
        private readonly RolePermissionRepository _rolePermissionRepository;
        private readonly ObjectEndpointPermissionsRepository _objectEndpointPermissionsRepository;

        public PermissionStore()
            : this("DefaultConnection")
        {

        }

        public PermissionStore(string connectionString)
        {
            _connectionString = connectionString;
            _permissionRepository = new PermissionRepository(_connectionString);
            _rolePermissionRepository = new RolePermissionRepository(_connectionString);
            _objectEndpointPermissionsRepository = new ObjectEndpointPermissionsRepository(_connectionString);
        }

        public IQueryable<Permission> Permissions
        {
            get
            {
                return _permissionRepository.GetPermissions();
            }
        }

        public void Dispose()
        {
            // connection is automatically disposed
        }

        public Task<IdentityResult> CreateAsync(Permission permission, CancellationToken cancellationToken)
        {
            if (permission == null)
            {
                throw new ArgumentNullException("permission");
            }

            _permissionRepository.Insert(permission);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(Permission permission, CancellationToken cancellationToken)
        {
            if (permission == null)
            {
                throw new ArgumentNullException("permission");
            }

            _permissionRepository.Update(permission);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(Permission permission, CancellationToken cancellationToken)
        {
            if (permission == null)
            {
                throw new ArgumentNullException("permission");
            }

            _permissionRepository.Delete(permission.Id);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<Permission> GetByIdAsync(Permission permission, CancellationToken cancellationToken)
        {
            if (permission == null)
            {
                return Task.FromResult<Permission>(null);
            }

            Permission result = _permissionRepository.GetPermissionById(permission.Id);

            return Task.FromResult<Permission>(result);
        }

        public Task<IQueryable<Permission>> GetPermissionsByRoleAsync(Role role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                return Task.FromResult<IQueryable<Permission>>(null);
            }

            IList<Permission> result = _rolePermissionRepository.PopulateRolePermissionsOnRoleId(role.Id);

            return Task.FromResult(result.AsQueryable());
        }

        public Task<IQueryable<Permission>> FindPermissionsByRoleIdAsync(string roleId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return Task.FromResult<IQueryable<Permission>>(null);
            }

            IList<Permission> result = _rolePermissionRepository.PopulateRolePermissionsOnRoleId(roleId);

            return Task.FromResult(result.AsQueryable());
        }

        public Task<IQueryable<Permission>> GetEndpointPermissionsIdByObjectAsync(ObjectEndpoint _object, CancellationToken cancellationToken)
        {
            if (_object == null)
            {
                return Task.FromResult<IQueryable<Permission>>(null);
            }

            IList<Permission> result = _objectEndpointPermissionsRepository.PopulateObjectEndpointPermissions(_object.Id);

            return Task.FromResult(result.AsQueryable());
        }

        public Task<Permission> GetPermissionNameAsync(Permission permission, CancellationToken cancellationToken)
        {
            if (permission == null)
            {
                return Task.FromResult<Permission>(null);
            }

            Permission result = _permissionRepository.GetPermissionByName(permission.Name);
            return Task.FromResult(result);
        }

        public Task<Permission> FindByIdAsync(string permissionId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(permissionId))
            {
                return Task.FromResult<Permission>(null);
            }

            Permission result = _permissionRepository.GetPermissionById(permissionId);

            return Task.FromResult(result);
        }

        public Task<Permission> FindByNameAsync(string permissionName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(permissionName))
            {
                return Task.FromResult<Permission>(null);
            }

            Permission result = _permissionRepository.GetPermissionByName(permissionName);
            return Task.FromResult(result);
        }
    }
}
