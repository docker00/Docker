using Microsoft.AspNetCore.Identity;
using MySql.AspNet.Identity.Repositories;
using System;
using Host.Extensions.MessageService;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;

namespace MySql.AspNet.Identity
{
    public class GroupStore<TGroup> where TGroup : Group
    {
        private readonly string _connectionString;
        private readonly GroupRepository<TGroup> _groupRepository;
        private readonly GroupParentRepository<GroupParent> _groupParentRepository;
        private readonly GroupRoleRepository<Role> _groupRoleRepository;
        private readonly GroupUserRepository _groupUserRepository;
        private readonly UserRoleRepository<User> _userRoleRepository;
        private readonly MessageService _messageService;
        public GroupStore()
            : this("DefaultConnection")
        {

        }

        public GroupStore(string connectionString)
        {
            _connectionString = connectionString;
            _groupRepository = new GroupRepository<TGroup>(_connectionString);
            _groupParentRepository = new GroupParentRepository<GroupParent>(_connectionString);
            _groupRoleRepository = new GroupRoleRepository<Role>(_connectionString);
            _groupUserRepository = new GroupUserRepository(_connectionString);
            _userRoleRepository = new UserRoleRepository<User>(_connectionString);
            _messageService = new MessageService(_connectionString);
        }

        public IQueryable<Group> Groups
        {
            get
            {
                return _groupRepository.GetGroups();
            }
        }

        public void Dispose()
        {
            // connection is automatically disposed
        }

        public Task<IdentityResult> AddGroupUsers(TGroup group, List<User> users, CancellationToken cancellationToken)
        {
            if (group == null)
            {
                throw new ArgumentException("group");
            }
            if (users != null)
            {
                List<User> old_users = _groupUserRepository.PopulateGroupUsers(group.Id);
                List<Role> groupRoles = _groupRoleRepository.PopulateGroupRoles(group.Id);
                IQueryable<Group> parents = _groupParentRepository.PopulateGroupParents(group.Id, true).Distinct();
                foreach (Group parentGroup in parents)
                {
                    groupRoles = groupRoles.Concat(_groupRoleRepository.PopulateGroupRoles(parentGroup.Id)).ToList();
                }
                groupRoles = groupRoles.Distinct().ToList();
                foreach (User user in users)
                {
                    //Проверка на наличие группы у пользователя
                    if (old_users.FirstOrDefault(u => u.Id.Equals(user.Id)) == null)
                    {
                        _groupUserRepository.Insert(group.Id, user.Id);
                    }
                    //Проверка начилия ролей у пользователя
                    List<string> rolesNames = new List<string>();
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
                    if(rolesNames.Count > 0)
                    {
                        message += " Вам были назначены роли " + string.Join(", ", rolesNames);
                    }
                    _messageService.SendMessage(user, "Добавление в группу", message);
                }
            }
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> CreateAsync(TGroup group, CancellationToken cancellationToken)
        {
            if (group == null)
            {
                throw new ArgumentNullException("group");
            }
            _groupRepository.Insert(group);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> AddParent(TGroup group, string parentId, CancellationToken cancellationToken)
        {
            if (group == null)
            {
                throw new ArgumentException("group");
            }
            if (string.IsNullOrEmpty(parentId))
            {
                throw new ArgumentException("parentId");
            }

            GroupParent parent = _groupParentRepository.Get(group.Id, parentId);
            if (parent != null)
            {
                throw new ArgumentException("Такая связь уже существует");
            }
            _groupParentRepository.Insert(group.Id, parentId);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> CreateGroupRoleAsync(GroupRole groupRole, CancellationToken cancellationToken)
        {
            if (groupRole == null)
            {
                throw new ArgumentException("groupRole");
            }
            //TODO : сделать проверку на наличе этой роли в группе
            _groupRoleRepository.Insert(groupRole.GroupId, groupRole.RoleId);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAsync(TGroup group, CancellationToken cancellationToken)
        {
            if (group == null)
            {
                throw new ArgumentNullException("group");
            }

            _groupRepository.Update(group);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateRolesAsync(TGroup group, List<Role> roles, CancellationToken cancellationToken)
        {
            if (group == null)
            {
                throw new ArgumentException("groupId");
            }
            if (roles.Count() == 0)
            {
                throw new ArgumentException("roles not selected");
            }
            List<Role> old_roles = _groupRoleRepository.PopulateGroupRoles(group.Id);
            //Проверка на наличе роли
            foreach (Role role in old_roles)
            {
                if (roles.Contains(role))
                {
                    roles.Remove(role);
                }
            }
            foreach (Role role in roles)
            {
                _groupRoleRepository.Insert(group.Id, role.Id);
            }
            old_roles = null;
            //Список пользователей группы
            List<User> users = _groupUserRepository.PopulateGroupUsers(group.Id);
            foreach (User user in users)
            {
                IEnumerable<string> userRoles = _userRoleRepository.GetUserRoles(user.Id).Select(r => r.Id);
                List<string> userRoleNamesAdded = new List<string>();
                foreach (Role role in roles)
                {
                    if (!userRoles.Contains(role.Id))
                    {
                        _userRoleRepository.Insert(user.Id, role.Id);
                        userRoleNamesAdded.Add(role.Name);
                    }
                }
                _messageService.SendMessage(user, "Права", "Вам добавили права: " + string.Join(", ", userRoleNamesAdded));
            }

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(TGroup group, CancellationToken cancellationToken)
        {
            if (group == null)
            {
                throw new ArgumentNullException("group");
            }

            _groupRepository.Delete(group.Id);
            _groupParentRepository.DeleteGroupJoins(group.Id);
            _groupUserRepository.DeleteGroupUsers(group.Id);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteGroupRoleAsync(Group group, Role role, CancellationToken cancellationTokenbool, bool fromUsersDelete = false)
        {
            if (group == null)
            {
                throw new ArgumentException("group");
            }
            if (role == null)
            {
                throw new ArgumentException("role");
            }
            _groupRoleRepository.Delete(group.Id, role.Id);
            if (fromUsersDelete)
            {
                _groupUserRepository.DeleteUserRoles(role.Id, group.Id);
                List<User> groupUsers = _groupUserRepository.PopulateGroupUsers(group.Id);
                if (groupUsers.Count > 0)
                {
                    _messageService.SendMessageMany(groupUsers, "Удаление роли", "У вас убрали право " + role.Name);
                }
            }
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteGroupChild(string groupId, string parentId)
        {
            if (string.IsNullOrEmpty(groupId))
            {
                throw new ArgumentException("groupId");
            }
            if (string.IsNullOrEmpty(parentId))
            {
                throw new ArgumentException("parentId");
            }
            _groupParentRepository.Delete(groupId, parentId);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<Group> GetGroupIdAsync(TGroup group, CancellationToken cancellationToken)
        {
            if (group == null)
            {
                return Task.FromResult<Group>(null);
            }

            Group result = _groupRepository.GetGroupById(group.Id);

            return Task.FromResult(result);
        }

        public Task<Group> GetGroupNameAsync(TGroup group, CancellationToken cancellationToken)
        {
            if (group == null)
            {
                return Task.FromResult<Group>(null);
            }

            Group result = _groupRepository.GetGroupByName(group.Name);
            return Task.FromResult(result);
        }

        public Task<IQueryable<Group>> GetParentsAsync(TGroup group, bool recursive, CancellationToken cancellationToken)
        {
            if (group == null)
            {
                throw new ArgumentException("group");
            }

            IQueryable<Group> parentGroups = _groupParentRepository.PopulateGroupParents(group.Id, recursive);
            return Task.FromResult(parentGroups);
        }

        public Task<IQueryable<Group>> GetChildrenAsync(TGroup group, bool recursive, CancellationToken cancellationToken)
        {
            if (group == null)
            {
                throw new ArgumentException("parentId");
            }
            IQueryable<Group> groups = _groupParentRepository.PoplulateGroupChildren(group.Id, recursive);
            return Task.FromResult(groups);
        }

        public Task<IQueryable<Group>> FindByObjectIdAsync(string objectId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(objectId))
            {
                throw new ArgumentException("objectId");
            }
            IQueryable<Group> groups = _groupParentRepository.PoplulateGroupChildren(objectId);
            return Task.FromResult(groups);
        }

        /// <summary>
        /// Формирование результата для bootstrap_table
        /// </summary>
        /// <param name="order"></param>
        /// <param name="limit"></param>
        /// <param name="offset"></param>
        /// <param name="sort"></param>
        /// <param name="search"></param>
        /// <returns>Возвращает KeyValuePair<int, IQueruable<TGroup>> где int - общее кол-во групп, IQueruable<TGroup> - часть групп</TGoup></returns>
        public Task<KeyValuePair<int, IQueryable<TGroup>>> GetQueryFromatterAsync(string order, int limit, int offset, string sort, string search)
        {
            KeyValuePair<int, IQueryable<TGroup>> groups = _groupRepository.GetQueryFromatter(order, limit, offset, sort, search);
            return Task.FromResult(groups);
        }

        public Task<Group> FindByIdAsync(string groupId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(groupId))
            {
                return Task.FromResult<Group>(null);
            }

            Group result = _groupRepository.GetGroupById(groupId);

            return Task.FromResult(result);
        }

        public Task<Group> FindByNameAsync(string groupName, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(groupName))
            {
                return Task.FromResult<Group>(null);
            }

            Group result = _groupRepository.GetGroupByName(groupName);
            return Task.FromResult(result);
        }

        public Task<IQueryable<Group>> FindGroupsByUserIdAsync(string userId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Task.FromResult<IQueryable<Group>>(null);
            }

            IList<Group> result = _groupUserRepository.PopulateUserGroups(userId);

            return Task.FromResult(result.AsQueryable());
        }

        public Task<IQueryable<Role>> FindRolesByGroupIdAsync(string groupId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(groupId))
            {
                return Task.FromResult<IQueryable<Role>>(null);
            }

            IList<Role> result = _groupRoleRepository.PopulateGroupRoles(groupId);

            return Task.FromResult(result.AsQueryable());
        }

        public Task<IQueryable<Group>> FindGroupsByRoleIdAsync(string roleId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return Task.FromResult<IQueryable<Group>>(null);
            }

            IList<Group> result = _groupRoleRepository.PopulateRoleGroups(roleId);

            return Task.FromResult(result.AsQueryable());
        }

        public Task<IQueryable<Group>> FindGroupsByObjectEndpointIdAsync(string objectEndpointId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(objectEndpointId))
            {
                return Task.FromResult<IQueryable<Group>>(null);
            }

            IList<Group> result = _groupRepository.GetGroupsByObjectEndpointId(objectEndpointId);

            return Task.FromResult(result.AsQueryable());
        }
    }
}
