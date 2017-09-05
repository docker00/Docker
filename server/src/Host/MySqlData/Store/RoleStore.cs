using Microsoft.AspNetCore.Identity;
using MySql.AspNet.Identity.Repositories;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using IdentityServer4.Quickstart.UI;

namespace MySql.AspNet.Identity
{
    public class RoleStore<TRole> : IQueryableRoleStore<TRole> where TRole : Role
    {
        private readonly string _connectionString;
        private readonly RoleRepository<TRole> _roleRepository;
        private readonly RolePermissionRepository _rolePermissionRepository;
        private readonly UserRoleRepository<User> _userRoleRepository;
        private readonly RoleLocalPartitionRepository _roleLocalPartitionRepository;

        public RoleStore()
            : this("DefaultConnection")
        {

        }

        public RoleStore(string connectionString)
        {
            _connectionString = connectionString;
            _roleRepository = new RoleRepository<TRole>(_connectionString);
            _rolePermissionRepository = new RolePermissionRepository(_connectionString);
            _userRoleRepository = new UserRoleRepository<User>(_connectionString);
            _roleLocalPartitionRepository = new RoleLocalPartitionRepository(_connectionString);
        }

        public IQueryable<TRole> Roles
        {
            get
            {
                return _roleRepository.GetRoles();
            }
        }

        public void Dispose()
        {
            // connection is automatically disposed
        }

        public Task<IdentityResult> CreateAsync(TRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException("role");
            }

            _roleRepository.Insert(role);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(TRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException("user");
            }

            _roleRepository.Update(role);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateUserRolesAsync(string userId, List<string> roles_ids, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("userId");
            }
            if (roles_ids == null)
            {
                roles_ids = new List<string>();
            }
            IQueryable<Role> old_roles = _userRoleRepository.GetUserRoles(userId);
            foreach (Role role in old_roles)
            {
                if (!roles_ids.Contains(role.Id))
                {
                    _userRoleRepository.Delete(userId, role.Id);
                }
                else
                {
                    roles_ids.Remove(role.Id);
                }
            }
            foreach (string role_id in roles_ids)
            {
                _userRoleRepository.Insert(userId, role_id);
            }
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateRoleLocalPartitionsAsync(TRole role, List<string> localPartitionsIds, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentException("role");
            }
            if (localPartitionsIds == null)
            {
                localPartitionsIds = new List<string>();
            }
            IQueryable<LocalPartition> oldLocalPartitions = _roleLocalPartitionRepository.PopulateRoleLocalPartitions(role.Id);
            foreach (LocalPartition localPartition in oldLocalPartitions)
            {
                if (!localPartitionsIds.Contains(localPartition.Id))
                {
                    _roleLocalPartitionRepository.Delete(role.Id, localPartition.Id);
                }
                else
                {
                    localPartitionsIds.Remove(localPartition.Id);
                }
            }
            foreach (string localPartitionId in localPartitionsIds)
            {
                _roleLocalPartitionRepository.Insert(role.Id, localPartitionId);
            }

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> CreateRolePermissionAsync(string roleId, string permissionId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                throw new ArgumentNullException("roleId");
            }

            if (string.IsNullOrEmpty(permissionId))
            {
                throw new ArgumentNullException("permissionId");
            }

            RolePermissionModel getRolePermission = _rolePermissionRepository.Get(roleId, permissionId);
            if (getRolePermission != null)
            {
                return Task.FromResult(IdentityResult.Success);
            }

            _rolePermissionRepository.Insert(roleId, permissionId);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> CreateRoleLocalPartitionAsync(string roleId, string localPartitionId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                throw new ArgumentNullException("roleId");
            }

            if (string.IsNullOrEmpty(localPartitionId))
            {
                throw new ArgumentNullException("localPartitionId");
            }

            RoleLocalPartitionModel getRoleLocalPartition = _roleLocalPartitionRepository.Get(roleId, localPartitionId);
            if (getRoleLocalPartition != null)
            {
                return Task.FromResult(IdentityResult.Success);
            }

            _roleLocalPartitionRepository.Insert(roleId, localPartitionId);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(TRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentNullException("user");
            }

            _roleRepository.Delete(role.Id);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteRolePermissionAsync(string roleId, string permissionId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                throw new ArgumentNullException("roleId");
            }

            if (string.IsNullOrEmpty(permissionId))
            {
                throw new ArgumentNullException("permissionId");
            }

            _rolePermissionRepository.Delete(roleId, permissionId);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteRoleLocalPartitionAsync(string roleId, string localPartitionId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                throw new ArgumentNullException("roleId");
            }

            if (string.IsNullOrEmpty(localPartitionId))
            {
                throw new ArgumentNullException("localPartitionId");
            }

            _roleLocalPartitionRepository.Delete(roleId, localPartitionId);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<TRole> GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
        {
            var result = _roleRepository.GetRoleById(role.Id) as TRole;

            return Task.FromResult<TRole>(result);
        }

        public Task<TRole> GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            var result = _roleRepository.GetRoleByName(role.Name) as TRole;
            return Task.FromResult(result);
        }

        public Task<IQueryable<LocalPartition>> GetRoleLocalPartitons(TRole role, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentException("role");
            }
            IQueryable<LocalPartition> localPartitions = _roleLocalPartitionRepository.PopulateRoleLocalPartitions(role.Id);
            return Task.FromResult(localPartitions);
        }

        public Task<KeyValuePair<int, IQueryable<TRole>>> GetBootsrapTableData(string order, int limit, int offset, string sort, string search, string groupId = null, bool inGroup = false)
        {
            KeyValuePair<int, IQueryable<TRole>> roles = _roleRepository.GetQueryFormatter(order, limit, offset, sort, search, groupId, inGroup);
            return Task.FromResult(roles);
        }

        public Task<IQueryable<TRole>> FindRolesByObjectEndpointIdAsync(string objectEndpointId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(objectEndpointId))
            {
                return Task.FromResult<IQueryable<TRole>>(null);
            }

            IList<TRole> result = _roleRepository.GetRolesByObjectEndpointId(objectEndpointId);

            return Task.FromResult(result.AsQueryable());
        }

        public Task SetRoleNameAsync(TRole role, string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedRoleNameAsync(TRole role, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<TRole> FindByIdAsync(string roleId, CancellationToken cancellationToken)
        {
            TRole result = _roleRepository.GetRoleById(roleId);

            return Task.FromResult(result);
        }

        public Task<TRole> FindByNameAsync(string normalizedRoleName, CancellationToken cancellationToken)
        {
            var result = _roleRepository.GetRoleByName(normalizedRoleName) as TRole;
            return Task.FromResult(result);
        }

        public Task<IQueryable<TRole>> FindGroupRoles(string groupId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(groupId))
            {
                throw new ArgumentException("groupId");
            }
            IQueryable<TRole> roles = _roleRepository.GetByGroupId(groupId);
            return Task.FromResult(roles);
        }

        public Task<RoleLocalPartitionModel> FindRoleLocalPartitionAsync(string roleId, string localPartitionId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                throw new ArgumentNullException("roleId");
            }

            if (string.IsNullOrEmpty(localPartitionId))
            {
                throw new ArgumentNullException("localPartitionId");
            }

            RoleLocalPartitionModel roleLocalPartition = _roleLocalPartitionRepository.Get(roleId, localPartitionId);
            
            return Task.FromResult(roleLocalPartition);
        }

        Task<string> IRoleStore<TRole>.GetRoleIdAsync(TRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        Task<string> IRoleStore<TRole>.GetRoleNameAsync(TRole role, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> AddRoleUsers(TRole role, List<User> users, CancellationToken cancellationToken)
        {
            if (role == null)
            {
                throw new ArgumentException("group");
            }
            if (users != null)
            {
                List<User> old_users = _userRoleRepository.PopulateRoleUsers(role.Id);
                //String roleName = (_roleRepository.GetRoleById(role.Id)).Name;
                //List<Role> groupRoles = _groupRoleRepository.PopulateGroupRoles(group.Id);
                //IQueryable<Group> parents = _groupParentRepository.PopulateGroupParents(group.Id, true).Distinct();
                /*foreach (Group parentGroup in parents)
                {
                    groupRoles = groupRoles.Concat(_groupRoleRepository.PopulateGroupRoles(parentGroup.Id)).ToList();
                }
                groupRoles = groupRoles.Distinct().ToList();*/
                foreach (User user in users)
                {
                    //Проверка на наличие группы у пользователя
                    if (old_users.FirstOrDefault(u => u.Id.Equals(user.Id)) == null)
                    {
                        _userRoleRepository.Insert(user.Id, role.Id);
                    }
                    //Проверка начилия ролей у пользователя
                    /*List<string> rolesNames = new List<string>();
                    IEnumerable<string> userRoles = _userRoleRepository.PopulateRoles(user.Id).Select(r => r.Name);
                    foreach (Role groupRole in groupRoles)
                    {
                        if (!userRoles.Contains(groupRole.Name))
                        {
                            _userRoleRepository.Insert(user.Id, groupRole.Id);
                            rolesNames.Add(groupRole.Name);
                        }
                    }
                    string message = "Вы были включены в группу " + group.Name + ".";
                    if (rolesNames.Count > 0)
                    {
                        message += " Вам были назначены роли " + string.Join(", ", rolesNames);
                    }
                    _messageService.SendMessage(user, "Добавление в группу", message);*/
                }
            }
            return Task.FromResult(IdentityResult.Success);
        }
    }
}
