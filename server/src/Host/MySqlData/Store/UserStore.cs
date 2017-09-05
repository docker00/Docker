using Host.Extensions.MessageService;
using IdentityModel;
using IdentityServer4.Quickstart.UI;
using Microsoft.AspNetCore.Identity;
using MySql.AspNet.Identity.Repositories;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MySql.AspNet.Identity
{
    public class UserStore<TUser> :
        IUserStore<TUser>,
        IUserLoginStore<TUser>,
        IUserClaimStore<TUser>,
        IUserRoleStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IUserEmailStore<TUser>,
        IUserLockoutStore<TUser>,
        IUserTwoFactorStore<TUser>,
        IUserPhoneNumberStore<TUser>,
        IQueryableUserStore<TUser>
    where TUser : User
    {
        private readonly string _connectionString;
        private readonly UserRepository<TUser> _userRepository;
        private readonly UserLoginRepository _userLoginRepository;
        private readonly UserClaimRepository<TUser> _userClaimRepository;
        private readonly UserRoleRepository<TUser> _userRoleRepository;
        private readonly GroupUserRepository _groupUserRepository;
        private readonly GroupRoleRepository<Role> _groupRoleRepository;
        private readonly ClaimCustomRepository _claimCustomRepository;
        private readonly GroupParentRepository<GroupParent> _groupParentRepository;
        private readonly UserConfirmCodeRepository<UserConfirmCode> _userConfirmCodeRepository;
        private readonly UserApiKeyRepository<UserApiKey> _userApiKeyRepository;
        private readonly MessageService _messageService;
        public UserStore()
            : this("DefaultConnection")
        {

        }

        public UserStore(string connectionString)
        {
            _connectionString = connectionString;
            _userRepository = new UserRepository<TUser>(_connectionString);
            _userLoginRepository = new UserLoginRepository(_connectionString);
            _userClaimRepository = new UserClaimRepository<TUser>(_connectionString);
            _userRoleRepository = new UserRoleRepository<TUser>(_connectionString);
            _groupUserRepository = new GroupUserRepository(_connectionString);
            _groupRoleRepository = new GroupRoleRepository<Role>(_connectionString);
            _claimCustomRepository = new ClaimCustomRepository(_connectionString);
            _groupParentRepository = new GroupParentRepository<GroupParent>(_connectionString);
            _userConfirmCodeRepository = new UserConfirmCodeRepository<UserConfirmCode>(_connectionString);
            _userApiKeyRepository = new UserApiKeyRepository<UserApiKey>(_connectionString);
            _messageService = new MessageService(_connectionString);
        }

        #region Other Methods
        public bool ValidateCredentials(string username, string password)
        {
            TUser user = _userRepository.GetByEmail(username);
            if (user != null && !string.IsNullOrEmpty(user.PasswordHash) && !string.IsNullOrEmpty(password))
            {
                return user.PasswordHash.Equals(CalculateMD5Hash(password));
            }

            return false;
        }
        public Task<TUser> GetUserEmailAsync(string email)
        {
            if (string.IsNullOrEmpty(email))
                throw new ArgumentNullException("user");


            TUser user = _userRepository.GetByEmail(email);
            if (user != null)
            {
                return Task.FromResult(user);
            }

            return Task.FromResult<TUser>(null);
        }

        public async Task<TUser> AutoProvisionUser(string provider, string userId, List<Claim> claims)
        {
            List<Claim> filtered = new List<Claim>();

            #region Filtered
            foreach (Claim claim in claims)
            {
                if (claim.Type == ClaimTypes.Name)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, claim.Value));
                }
                else if (JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.ContainsKey(claim.Type))
                {
                    filtered.Add(new Claim(JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap[claim.Type], claim.Value));
                }
                else
                {
                    filtered.Add(claim);
                }
            }

            if (!filtered.Any(x => x.Type == JwtClaimTypes.Name))
            {
                var first = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.GivenName)?.Value;
                var last = filtered.FirstOrDefault(x => x.Type == JwtClaimTypes.FamilyName)?.Value;
                if (first != null && last != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first + " " + last));
                }
                else if (first != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, first));
                }
                else if (last != null)
                {
                    filtered.Add(new Claim(JwtClaimTypes.Name, last));
                }
            }
            #endregion Filtered

            User user = new User();

            string sub = user.Id;
            

            #region Remove Claims
            Claim userNameClaim = filtered.FirstOrDefault(c => c.Type == JwtClaimTypes.Name);
            Claim subjectClaim = filtered.FirstOrDefault(c => c.Type == JwtClaimTypes.Subject);
            Claim emailClaim = filtered.FirstOrDefault(c => c.Type == JwtClaimTypes.Email);
            Claim emailVerifiedClaim = filtered.FirstOrDefault(c => c.Type == JwtClaimTypes.EmailVerified);
            Claim phoneNumberClaim = filtered.FirstOrDefault(c => c.Type == JwtClaimTypes.PhoneNumber);
            Claim phoneNumberVerifiedClaim = filtered.FirstOrDefault(c => c.Type == JwtClaimTypes.PhoneNumberVerified);

            if (userNameClaim != null)
            {
                filtered.Remove(userNameClaim);
            }
            if (subjectClaim != null)
            {
                filtered.Remove(subjectClaim);
            }
            if (emailClaim != null)
            {
                filtered.Remove(emailClaim);
            }
            if (emailVerifiedClaim != null)
            {
                filtered.Remove(emailVerifiedClaim);
            }
            if (phoneNumberClaim != null)
            {
                filtered.Remove(phoneNumberClaim);
            }
            if (phoneNumberVerifiedClaim != null)
            {
                filtered.Remove(phoneNumberVerifiedClaim);
            }
            #endregion Remove Claims

            string email = emailClaim?.Value ?? "";
            string name = userNameClaim?.Value ?? sub;

            user.UserName = name;
            user.Email = email;
            user.Claims = filtered;
            user.EmailConfirmed = true;
            user.Activated = true;

            if (string.IsNullOrEmpty(email))
            {
                return null;
            }

            IdentityResult result = await CreateAsync((TUser)user, CancellationToken.None);
            if (result == null || (result != null && !result.Succeeded))
            {
                return null;
            }

            UserLoginInfo login = new UserLoginInfo(provider, userId, "");
            await AddClaimsAsync((TUser)user, filtered, CancellationToken.None);
            await AddLoginAsync((TUser)user, login, CancellationToken.None);
            user = FindByLoginAsync(provider, userId, CancellationToken.None).Result;

            return (TUser)user;
        }

        public string CalculateMD5Hash(string input)
        {
            StringBuilder sb = new StringBuilder();

            input += "...---...";

            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }

        public Task<string> GenerateConfirmationCodeAsync(TUser user, CancellationToken cancellationToken)
        {
            //TODO: К коду нужно еще привязать тип
            if (user == null)
            {
                throw new ArgumentException("user");
            }
            UserConfirmCode confirmCode = new UserConfirmCode(user.Id);
            _userConfirmCodeRepository.Insert(confirmCode);

            return Task.FromResult(confirmCode.Code);

        }

        public bool ValidatePermissionOnUrl(string userId, string objectUrl, string endpointValue, string permissionName)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(objectUrl) || string.IsNullOrEmpty(endpointValue) || string.IsNullOrEmpty(permissionName))
            {
                return false;
            }

            return _userRepository.ValidatePermissionOnUrl(userId, objectUrl, endpointValue, permissionName);
        }

        public TUser FindByEmail(string email)
        {
            if (email == null)
                throw new ArgumentNullException("email");

            TUser user = _userRepository.GetByEmail(email);
            if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                //user.Roles = _userRoleRepository.PopulateRoles(user.Id);
                //user.Claims = _userClaimRepository.PopulateUserClaims(user.Id);
                //user.Logins = _userLoginRepository.PopulateUserLogins(user.Id);

                return user;
            }

            return null;
        }

        public TUser FindById(string userId)
        {
            TUser user = _userRepository.GetById(userId);
            if (user != null)
            {
                //user.Roles = _userRoleRepository.PopulateRoles(user.Id).ToList();
                //user.Claims = _userClaimRepository.PopulateUserClaims(user.Id);
                //user.Logins = _userLoginRepository.PopulateUserLogins(user.Id);

                return user;
            }

            return null;
        }

        public Task<bool> TwoFactorVerifyCodeAsync(string userId, string code)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("userId");
            }

            bool verifyed = _userConfirmCodeRepository.Get(userId, code) != null ? true : false;
            return Task.FromResult(verifyed);
        }
        #endregion Other Methods

        public IQueryable<TUser> Users
        {
            get
            {
                return _userRepository.GetAll();
            }
        }

        public Task AddClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            foreach (Claim claim in claims)
            {
                ClaimCustom claimCustom = _claimCustomRepository.GetClaimCustomByType(claim.Type, claim.Value);

                if (claimCustom == null) { continue; }

                _userClaimRepository.Insert(user.Id, claimCustom);
            }

            return Task.FromResult<int>(0);
        }

        public Task AddLoginAsync(TUser user, UserLoginInfo login, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (login == null)
            {
                throw new ArgumentNullException("login");
            }

            IdentityUserLogin identityUserLogin = new IdentityUserLogin(login.LoginProvider, login.ProviderKey, login.ProviderDisplayName);
            user.Logins.Add(identityUserLogin);

            _userLoginRepository.Insert(user.Id, identityUserLogin);

            return Task.FromResult<int>(0);
        }

        public Task AddToRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (!user.Roles.Contains(roleName, StringComparer.OrdinalIgnoreCase))
            {
                user.Roles.Add(roleName);
            }

            _userRoleRepository.Insert(user, roleName);

            return Task.FromResult(0);
        }

        public Task<IdentityResult> AddToGroupAsync(TUser user, Group group, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentException("user");
            }
            if (group == null)
            {
                throw new ArgumentException("group");
            }
            _groupUserRepository.Insert(group.Id, user.Id);

            List<string> userRoles = _userRoleRepository.PopulateRoles(user.Id).Select(r => r.Name).ToList();
            List<Role> groupRoles = _groupRoleRepository.PopulateGroupRoles(group.Id);
            IQueryable<Group> parents = _groupParentRepository.PopulateGroupParents(group.Id, true).Distinct();
            foreach (Group groupParent in parents)
            {
                groupRoles = groupRoles.Concat(_groupRoleRepository.PopulateGroupRoles(groupParent.Id)).ToList();
            }
            groupRoles = groupRoles.Distinct().ToList();

            List<string> roleNames = new List<string>();
            foreach (Role groupRole in groupRoles)
            {
                if (!userRoles.Contains(groupRole.Name))
                {
                    _userRoleRepository.Insert(user.Id, groupRole.Id);
                    roleNames.Add(groupRole.Name);
                }
            }
            string message = "Вас включили в группу " + group.Name + ".";
            if (roleNames.Count > 0)
            {
                message += " В связи с этим вам добавились права: " + string.Join(", ", roleNames);
            }
            _messageService.SendMessage(user, "Добавление в группу", message);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> CreateAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (string.IsNullOrEmpty(user.Id))
                throw new InvalidOperationException("user.Id property must be specified before calling CreateAsync");

            if (string.IsNullOrEmpty(user.Email))
                throw new InvalidOperationException("user.Id property must be specified before calling CreateAsync");

            TUser _user = _userRepository.GetByEmail(user.Email);
            if (!string.IsNullOrEmpty(_user.Email))
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError()
                {
                    Description = "Пользователь с таким email уже существует"
                }));
                //throw new InvalidOperationException("Пользователь с email уже существует!");
            }

            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                user.PasswordHash = CalculateMD5Hash(user.PasswordHash);
            }

            _userRepository.Insert(user);

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> CreateOrUpdateClaimAsync(TUser user, ClaimCustom claim, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentException("user");
            }
            if (claim == null)
            {
                throw new ArgumentException("claim");
            }
            ClaimCustom claimCustom = _userClaimRepository.Get(user.Id, claim.Id);
            //TODO: Если переделается ClaimCustom, то нужно будет подумать над этой проверкой
            ClaimCustom _claimCheckCustom = _claimCustomRepository.GetClaimCustomById(claim.Id);
            if (_claimCheckCustom.Type.ToLower().Equals(JwtClaimTypes.Subject) || _claimCheckCustom.Type.ToLower().Equals(JwtClaimTypes.Name) ||
                _claimCheckCustom.Type.ToLower().Equals(JwtClaimTypes.Email) || _claimCheckCustom.Type.ToLower().Equals(JwtClaimTypes.EmailVerified) ||
                _claimCheckCustom.Type.ToLower().Equals(JwtClaimTypes.PhoneNumber) || _claimCheckCustom.Type.ToLower().Equals(JwtClaimTypes.PhoneNumberVerified))
            {
                return Task.FromResult(IdentityResult.Success);
            }

            if (claimCustom == null)
            {
                _userClaimRepository.Insert(user.Id, claim);
            }
            else
            {
                _userClaimRepository.Update(user.Id, claim);
            }
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> CreateApiKey(UserApiKey apiKey, CancellationToken cancellationToken)
        {
            if (apiKey == null)
            {
                throw new ArgumentException("apiKey");
            }
            if(apiKey.ExperienceTime <= DateTime.Now)
            {
                throw new ArgumentException("ExperienceTime");
            }
            UserApiKey oldApiKey = _userApiKeyRepository.Get(apiKey.UserId, apiKey.ClientId);
            if (oldApiKey != null)
            {
                throw new ArgumentException("apiKey");
            }
            _userApiKeyRepository.Create(apiKey);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            _userRoleRepository.Delete(user.Id);
            _groupUserRepository.Delete(user.Id);
            _userClaimRepository.Delete(user.Id);
            _userRepository.Delete(user);

            return Task.FromResult(IdentityResult.Success);
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            if (normalizedEmail == null)
                throw new ArgumentNullException("email");

            TUser user = _userRepository.GetByEmail(normalizedEmail);
            if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                //user.Roles = _userRoleRepository.PopulateRoles(user.Id);
                //user.Claims = _userClaimRepository.PopulateUserClaims(user.Id);
                //user.Logins = _userLoginRepository.PopulateUserLogins(user.Id);

                return Task.FromResult(user);
            }

            return Task.FromResult<TUser>(null);
        }

        public Task<TUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            TUser user = _userRepository.GetById(userId);
            if (user != null)
            {
                //user.Roles = _userRoleRepository.PopulateRoles(user.Id).ToList();
                //user.Claims = _userClaimRepository.PopulateUserClaims(user.Id);
                //user.Logins = _userLoginRepository.PopulateUserLogins(user.Id);

                return Task.FromResult(user);
            }

            return Task.FromResult<TUser>(null);
        }

        public Task<TUser> FindByLoginAsync(string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            IdentityUserLogin login = new IdentityUserLogin()
            {
                LoginProvider = loginProvider,
                ProviderKey = providerKey
            };

            string userId = _userLoginRepository.GetByUserLoginInfo(login);
            if (string.IsNullOrEmpty(userId))
            {
                return Task.FromResult<TUser>(null);
            }

            TUser _tUser = _userRepository.GetById(userId);

            return Task.FromResult(_tUser);
            ///return _users.FirstOrDefault(x => x.ProviderName == provider && x.ProviderSubjectId == userId);
        }

        public Task<TUser> FindByNameAsync(string normalizedEmail, CancellationToken cancellationToken)
        {
            if (normalizedEmail == null)
            {
                return Task.FromResult<TUser>(null);
            }

            TUser user = _userRepository.GetByEmail(normalizedEmail);
            if (user != null && !string.IsNullOrEmpty(user.Email))
            {
                //user.Roles = _userRoleRepository.PopulateRoles(user.Id).ToList();
                //user.Claims = _userClaimRepository.PopulateUserClaims(user.Id);
                //user.Logins = _userLoginRepository.PopulateUserLogins(user.Id);

                return Task.FromResult(user);
            }

            return Task.FromResult<TUser>(null);
        }

        public Task<IQueryable<User>> FindUsersByRoleIdAsync(string roleId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return Task.FromResult<IQueryable<User>>(null);
            }

            IList<User> result = _userRoleRepository.PopulateRoleUsers(roleId);

            return Task.FromResult(result.AsQueryable());
        }

        public Task<UserApiKey> FindApiKeyAsync(string apiKeyId, CancellationToken cancellationToken)
        {
            UserApiKey apiKey = _userApiKeyRepository.Get(apiKeyId);
            return Task.FromResult(apiKey);
        }

        public Task<int> GetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<IList<Claim>> GetClaimsAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            IList<Claim> result = user.Claims.Select(c => new Claim(c.Type, c.Value)).ToList();
            return Task.FromResult(result);
        }

        public Task<string> GetEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.Email);
        }

        public Task<bool> GetEmailConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.EmailConfirmed);
        }

        public Task<bool> GetLockoutEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.LockoutEnabled);
        }

        public Task<DateTimeOffset?> GetLockoutEndDateAsync(TUser user, CancellationToken cancellationToken)
        {
            DateTimeOffset? dateTimeOffset = null;
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (user.LockoutEndDate.HasValue)
            {
                DateTime? lockoutEndDateUtc = user.LockoutEndDate;
                dateTimeOffset = new DateTimeOffset(DateTime.SpecifyKind(lockoutEndDateUtc.Value, DateTimeKind.Utc));
            }
            else
            {
                dateTimeOffset = new DateTimeOffset();
            }
            return Task.FromResult(dateTimeOffset);
        }

        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            List<UserLoginInfo> _userLoginInfo = new List<UserLoginInfo>();
            foreach (IdentityUserLogin _login in user.Logins)
            {
                UserLoginInfo _userLogin = new UserLoginInfo(_login.LoginProvider, _login.ProviderKey, _login.ProviderDisplayName);
                _userLoginInfo.Add(_userLogin);
            }

            return Task.FromResult<IList<UserLoginInfo>>(_userLoginInfo);
        }

        public Task<string> GetNormalizedEmailAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            if (string.IsNullOrEmpty(user.PasswordHash))
                throw new ArgumentNullException("user");

            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetPhoneNumberAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.PhoneNumber);
        }

        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.PhoneNumberConfirmed);
        }

        public Task<IList<string>> GetRolesAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult<IList<string>>(user.Roles.ToList());
        }

        public Task<IQueryable<Group>> GetGroupsAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentException("user");
            }
            List<Group> groups = _groupUserRepository.PopulateUserGroups(user.Id);
            return Task.FromResult(groups.AsQueryable());
        }

        public Task<string> GetSecurityStampAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.SecurityStamp);
        }

        public Task<bool> GetTwoFactorEnabledAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.TwoFactorAuthEnabled);
        }

        public Task<string> GetUserIdAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            if (string.IsNullOrEmpty(user.Id))
                throw new ArgumentNullException("user");

            return Task.FromResult(user.Id);
        }

        public Task<string> GetUserNameAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            if (string.IsNullOrEmpty(user.UserName))
                throw new ArgumentNullException("user");

            return Task.FromResult(user.UserName);
        }

        public Task<IQueryable<User>> FindUsersByObjectEndpointIdAsync(string objectEndpointId, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(objectEndpointId))
            {
                return Task.FromResult<IQueryable<User>>(null);
            }

            IList<User> result = _userRepository.GetUsersByObjectEndpointId(objectEndpointId);

            return Task.FromResult(result.AsQueryable());
        }

        public Task<IList<TUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IList<TUser>> GetUsersInRoleAsync(string roleName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public IList<UserViewModel> GetUsersQueryFormatterAsync(string order, int limit, int offset, string sort, string search, string groupId, bool inGroup)
        {
            List<UserViewModel> _users = new List<UserViewModel>();
            foreach (TUser user in _userRepository.GetQueryFormatter(order, limit, offset, sort, search, groupId, inGroup))
            {
                UserViewModel _user = new UserViewModel()
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    Activated = user.Activated,
                    Roles = _userRoleRepository.PopulateRoles(user.Id).Select(r => r.Name).ToList()
                };
                List<Group> groups = _groupUserRepository.PopulateUserGroups(user.Id);
                foreach (Group group in groups)
                {
                    _user.Groups.Add(new GroupViewModel()
                    {
                        Id = group.Id,
                        Name = group.Name
                    });
                }
                _users.Add(_user);
            }
            return _users;
        }

        public IList<UserViewModel> GetUsersQueryFormatterByRoleIdAsync(string order, int limit, int offset, string sort, string search, string roleId, bool inRole)
        {
            List<UserViewModel> _users = new List<UserViewModel>();
            foreach (TUser user in _userRepository.GetQueryFormatterByRoleId(order, limit, offset, sort, search, roleId, inRole))
            {
                UserViewModel _user = new UserViewModel()
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    PhoneNumber = user.PhoneNumber,
                    Activated = user.Activated,
                    Roles = _userRoleRepository.PopulateRoles(user.Id).Select(r => r.Name).ToList()
                };
                List<Group> groups = _groupUserRepository.PopulateUserGroups(user.Id);
                foreach (Group group in groups)
                {
                    _user.Groups.Add(new GroupViewModel()
                    {
                        Id = group.Id,
                        Name = group.Name
                    });
                }
                _users.Add(_user);
            }
            return _users;
        }

        public int GetUsersQueryFormatterCountAsync(string search, string groupId, bool inGroup)
        {
            return _userRepository.GetQueryFormatterCount(search, groupId, inGroup);
        }

        public int GetUsersQueryFormatterCountByRoleIdAsync(string search, string roleId, bool inGroup)
        {
            return _userRepository.GetQueryFormatterCountByRoleId(search, roleId, inGroup);
        }

        public Task<IQueryable<User>> GetUsersInGroup(string groupId, CancellationToken cancellationToken)
        {
            IQueryable<User> users = _groupUserRepository.PopulateGroupUsers(groupId).AsQueryable();
            return Task.FromResult(users);
        }

        public Task<ClaimCustom> GetUserClaimAsync(TUser user, string claimId, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentException("user");
            }
            if (string.IsNullOrEmpty(claimId))
            {
                throw new ArgumentException("claimId");
            }
            ClaimCustom userClaim = _userClaimRepository.Get(user.Id, claimId);
            return Task.FromResult(userClaim);
        }

        public Task<IQueryable<UserApiKey>> GetUserApiKeys(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentException("user");
            }
            IQueryable<UserApiKey> apiKeys = _userApiKeyRepository.PopulateUserApiKeys(user.Id);
            return Task.FromResult(apiKeys);
        }

        public Task<bool> HasPasswordAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(string.IsNullOrEmpty(user.PasswordHash));
        }

        public Task<int> IncrementAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            user.AccessFailedCount++;
            return Task.FromResult(user.AccessFailedCount);
        }

        public Task<bool> IsInRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            return Task.FromResult(user.Roles.Contains(roleName, StringComparer.OrdinalIgnoreCase));
        }

        public Task RemoveClaimsAsync(TUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task RemoveFromRoleAsync(TUser user, string roleName, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.Roles.RemoveAll(r => String.Equals(roleName, roleName, StringComparison.OrdinalIgnoreCase));

            _userRoleRepository.Delete(user, roleName);

            return Task.FromResult(0);
        }

        public Task<IdentityResult> RemoveFromGroupAsync(TUser user, Group group, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException("user");
            if (group == null)
                throw new ArgumentNullException("group");
            _groupUserRepository.Delete(group.Id, user.Id);
            _messageService.SendMessage(user, "Исключение из группы", "Вас исключили из группы " + group.Name);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task RemoveLoginAsync(TUser user, string loginProvider, string providerKey, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (string.IsNullOrEmpty(loginProvider))
            {
                throw new ArgumentNullException("login");
            }
            IdentityUserLogin tUserLogin = user.Logins.SingleOrDefault(l =>
            {
                if (l.LoginProvider != loginProvider)
                {
                    return false;
                }
                return l.ProviderKey == providerKey;
            });
            if (tUserLogin != null)
            {
                user.Logins.Remove(tUserLogin);

                IdentityUserLogin login = new IdentityUserLogin(loginProvider, providerKey, tUserLogin.ProviderDisplayName);
                _userLoginRepository.Delete(user.Id, login);

            }
            return Task.FromResult<int>(0);
        }

        public Task ReplaceClaimAsync(TUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteApiKeyAsync(UserApiKey apiKey, CancellationToken cancellationToken)
        {
            if (apiKey == null)
            {
                throw new ArgumentException("apiKey");
            }
            _userApiKeyRepository.Delete(apiKey);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task ResetAccessFailedCountAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.AccessFailedCount = 0;
            return Task.FromResult(0);
        }

        public Task SetEmailAsync(TUser user, string email, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.Email = email;

            return Task.FromResult(0);
        }

        public Task SetEmailConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.EmailConfirmed = confirmed;

            return Task.FromResult(0);
        }

        public Task SetLockoutEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.LockoutEnabled = enabled;

            return Task.FromResult(0);
        }

        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken)
        {
            DateTime? nullable;
            if (user == null)
            {
                throw new ArgumentNullException("user");
            }
            if (lockoutEnd == DateTimeOffset.MinValue)
            {
                nullable = null;
            }
            else
            {
                nullable = new DateTime?(lockoutEnd.Value.UtcDateTime);
            }
            user.LockoutEndDate = nullable;
            return Task.FromResult<int>(0);
        }

        public Task SetNormalizedEmailAsync(TUser user, string normalizedEmail, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedUserNameAsync(TUser user, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetPasswordHashAsync(TUser user, string passwordHash, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.PasswordHash = CalculateMD5Hash(passwordHash);
            return Task.FromResult(0);
        }

        public Task SetPhoneNumberAsync(TUser user, string phoneNumber, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.PhoneNumber = phoneNumber;

            return Task.FromResult(0);
        }

        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.PhoneNumberConfirmed = confirmed;

            return Task.FromResult(0);
        }

        public Task SetSecurityStampAsync(TUser user, string stamp, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.SecurityStamp = stamp;
            return Task.FromResult(0);
        }

        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            user.TwoFactorAuthEnabled = enabled;

            return Task.FromResult(0);
        }

        public Task SetUserNameAsync(TUser user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(TUser user, CancellationToken cancellationToken)
        {
            if (user == null)
                throw new ArgumentNullException("user");

            if (string.IsNullOrEmpty(user.Id))
                throw new InvalidOperationException("user.Id property must be specified before calling CreateAsync");

            if (!string.IsNullOrEmpty(user.PasswordHash))
            {
                user.PasswordHash = CalculateMD5Hash(user.PasswordHash);
            }
            _userRepository.Update(user);
            _messageService.SendMessage(user, "Учетная запись", "Данные учетной записи вашего профиля были изменены");
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdatePasswordAsync(TUser user, string newPassword)
        {
            if (user == null)
            {
                throw new InvalidOperationException("user property must be specified before calling UpdatePassword");
            }
            if (string.IsNullOrEmpty(newPassword))
            {
                throw new ArgumentNullException("password property must be specified before calling UpdatePassword");
            }
            newPassword = CalculateMD5Hash(newPassword);
            _userRepository.UpdatePassword(user.Id, newPassword);
            _messageService.SendMessage(user, "Смена пароля", "В ваше учетное записи сменился пароль");
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateActivatedAsync(string userId, bool activated, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException("userId");
            }
            _userRepository.UpdateActivated(userId, activated);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateAttributesValidatedAsync(string userId, bool attributesValidated, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userId))
            {
                _userRepository.UpdateAttributesValidated(attributesValidated);
            }
            else
            {
                _userRepository.UpdateAttributesValidated(userId, attributesValidated);
            }

            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> UpdateApiKeyAsync(UserApiKey apiKey, CancellationToken cancellationToken)
        {
            if (apiKey == null)
            {
                throw new ArgumentException("apiKey");
            }
            _userApiKeyRepository.Update(apiKey);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> ConfirmEmailAsync(TUser user, string code, CancellationToken cancellationToken)
        {
            if (user == null)
            {
                throw new ArgumentException("user");
            }
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentException("code");
            }
            UserConfirmCode confirmCode = _userConfirmCodeRepository.Get(user.Id, code);
            if (confirmCode == null)
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError()
                {
                    Description = "Неверный код или время на подтверждение вышло"
                }));
            }
            _userRepository.UpdateEmailConfirmed(user.Id, true);
            _userConfirmCodeRepository.Delete(user.Id, code);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> ResetPasswordAsync(TUser user, string code, string password)
        {
            if (user == null)
            {
                throw new ArgumentException("user");
            }
            if (string.IsNullOrEmpty(code))
            {
                throw new ArgumentException("code");
            }
            UserConfirmCode confirmCode = _userConfirmCodeRepository.Get(user.Id, code);
            if (confirmCode == null)
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError() { Description = "Неверный код или вышло время" }));
            }

            password = CalculateMD5Hash(password);
            _userRepository.UpdatePassword(user.Id, password);
            _userConfirmCodeRepository.Delete(user.Id, code);
            return Task.FromResult(IdentityResult.Success);
        }

        public bool GetRequiredAttributesOnUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return false;
            }

            return _userRepository.GetRequiredAttributesOnUser(userId);
        }
    }
}
