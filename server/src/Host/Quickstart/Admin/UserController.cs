using Host.Extensions;
using Host.Extensions.MessageService;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySql.AspNet.Identity;
using MySql.AspNet.Identity.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityServer4.Quickstart.UI
{
    [Menu(Name = "Пользователи", Description = "Управление пользователями", GroupName = "User", ShowInMenu = true, Weight = 0)]
    [SecurityHeaders]
    public class UserController : Controller
    {
        private readonly UserStore<User> _users;
        private readonly ClaimCustomRepository _claimCustomRepository;
        private readonly RoleStore<Role> _roles;
        private readonly ProfileAttributeStore<ProfileAttribute> _attributes;
        private readonly GroupStore<Group> _groups;
        private readonly ClaimCustomStore _claims;
        private readonly ClientCustomStore _clients;
        private readonly MessageService _message_service;

        public UserController(
          UserStore<User> users = null,
          RoleStore<Role> roles = null,
          ProfileAttributeStore<ProfileAttribute> attributes = null,
          GroupStore<Group> groups = null,
          ClaimCustomStore claims = null,
          ClientCustomStore clients = null,
          MessageService message_service = null
          )
        {
            _users = users ?? new UserStore<User>(Host.Startup.ConnectionString);
            _claimCustomRepository = new ClaimCustomRepository(Host.Startup.ConnectionString);
            _roles = roles ?? new RoleStore<Role>(Host.Startup.ConnectionString);
            _attributes = attributes ?? new ProfileAttributeStore<ProfileAttribute>(Host.Startup.ConnectionString);
            _groups = groups ?? new GroupStore<Group>(Host.Startup.ConnectionString);
            _claims = claims ?? new ClaimCustomStore(Host.Startup.ConnectionString);
            _clients = clients ?? new ClientCustomStore(Host.Startup.ConnectionString);
            _message_service = message_service ?? new MessageService(Host.Startup.ConnectionString);
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        #region Main
        [Menu(Name = "Список учетных записей", Description = "Список учетных записей пользователей", GroupName = "User", ShowInMenu = true, Weight = 1)]
        [HttpGet]
        public IActionResult List()
        {
            return View();
        }

        [Menu(Relation = "List")]
        [HttpGet]
        public JsonResult UsersData(string order, int limit, int offset, string sort, string search, string groupId = null, bool inGroup = false)
        {
            IList<UserViewModel> rows = _users.GetUsersQueryFormatterAsync(order, limit, offset, sort, search, groupId, inGroup);
            int total = _users.GetUsersQueryFormatterCountAsync(search, groupId, inGroup);
            return Json(new
            {
                total = total,
                rows = rows.ToList()
            });
        }

        [Menu(Relation = "List")]
        [HttpGet]
        public async Task<IActionResult> ActivatedEdit(string userId, bool activated)
        {
            IdentityResult result = await _users.UpdateActivatedAsync(userId, activated, CancellationToken.None);
            if (result.Succeeded)
            {
                return Json(true);
            }
            return Json(false);
        }

        [Menu(Name = "Детальная информация об учетной записи", Description = "Детальная информация об учетное записи пользователя", GroupName = "User")]
        [HttpGet]
        public async Task<IActionResult> Details(string userId)
        {
            User user = await _users.FindByIdAsync(userId, CancellationToken.None);

            UserViewModel user_view = new UserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Roles = user.Roles,
                Activated = user.Activated
            };
            IQueryable<Group> groups = await _users.GetGroupsAsync(user, CancellationToken.None);
            foreach (Group group in groups)
            {
                user_view.Groups.Add(new GroupViewModel()
                {
                    Id = group.Id,
                    Name = group.Name
                });
            }
            return View(user_view);
        }

        [Menu(Name = "Добавление учетной записи", Description = "Добавить пользователя", GroupName = "User", ShowInMenu = true, Weight = 0)]
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(RegisterViewModel model)
        {
            if (ModelState.IsValid && model.Password.Equals(model.ConfirmPassword))
            {
                User userByName = await _users.FindByNameAsync(model.Email, CancellationToken.None);
                if (userByName == null)
                {
                    User user = new User { UserName = model.Email, Email = model.Email, PasswordHash = model.Password };
                    IdentityResult result = await _users.CreateAsync(user, CancellationToken.None);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Details", new { userId = user.Id });
                    }
                    AddErrors(result);
                }
                else
                {
                    ModelState.AddModelError("Email", "Такой Email уже существует");
                }
            }
            return View(model);
        }

        [Menu(Relation = "Details")]
        [HttpGet]
        public async Task<IActionResult> _DetailsPartial(string userId)
        {
            User user = await _users.FindByIdAsync(userId, CancellationToken.None);
            UserViewModel user_view = new UserViewModel()
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Roles = user.Roles,
                Activated = user.Activated
            };
            return PartialView(user_view);
        }

        [Menu(Name = "Редактирование учетной записи", Description = "Редактирование учетной записи пользователя пользователя", GroupName = "User")]
        [HttpGet]
        public async Task<IActionResult> _EditPartial(string userId)
        {
            User user = await _users.FindByIdAsync(userId, CancellationToken.None);
            UserEditModel userEdit = new UserEditModel() { Id = user.Id, Email = user.Email, UserName = user.UserName };
            return PartialView(userEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _EditPartial(UserEditModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _users.FindByIdAsync(model.Id, CancellationToken.None);
                user.Email = model.Email;
                user.PasswordHash = model.Password;
                IdentityResult result = await _users.UpdateAsync(user, CancellationToken.None);
                if (result != null && result.Succeeded)
                {
                    return PartialView("_DetailsPartial", new UserViewModel()
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Roles = user.Roles,
                        UserName = user.UserName,
                        PhoneNumber = user.PhoneNumber
                    });
                }
                AddErrors(result);
            }
            return PartialView(model);
        }

        [Menu(Relation = "_EditPartial")]
        [HttpGet]
        public async Task<IActionResult> _UserNameEditPartial(string userId)
        {
            User user = await _users.FindByIdAsync(userId, CancellationToken.None);
            UserEditModel userEdit = new UserEditModel();
            if (user != null)
            {
                userEdit.Id = user.Id;
                userEdit.Email = user.Email;
                userEdit.UserName = user.UserName;
            }
            return PartialView(userEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _UserNameEditPartial(UserEditModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _users.FindByIdAsync(model.Id, CancellationToken.None);
                user.UserName = model.UserName;
                user.PasswordHash = model.Password;
                IdentityResult result = await _users.UpdateAsync(user, CancellationToken.None);
                if (result != null && result.Succeeded)
                {
                    return PartialView("_DetailsPartial", new UserViewModel()
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Roles = user.Roles,
                        UserName = user.UserName,
                        PhoneNumber = user.PhoneNumber
                    });
                }
                AddErrors(result);
            }
            return PartialView(model);
        }

        [Menu(Relation = "_EditPartial")]
        [HttpGet]
        public IActionResult _PasswordEditPartial(string userId)
        {
            ViewBag.userId = userId;
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _PasswordEditPartial(string userId, UserPasswordInputModel model)
        {
            User user = await _users.FindByIdAsync(userId, CancellationToken.None);
            ViewBag.userId = userId;
            if (user == null)
            {
                ModelState.AddModelError("", "Пользователь не существует");
                return Json(false);
            }
            if (ModelState.IsValid && model.Password.Equals(model.ConfirmPassword))
            {
                IdentityResult result = await _users.UpdatePasswordAsync(user, model.Password);
                if (result.Succeeded)
                {
                    return Json(true);
                }
                AddErrors(result);
            }
            return Json(false);
        }

        [Menu(Name = "Удаление учетной записи", Description = "Удаление пользователя", GroupName = "User")]
        [HttpGet]
        public async Task<IActionResult> Delete(string userId)
        {
            User _user = await _users.FindByIdAsync(userId, CancellationToken.None);
            if (_user != null)
            {
                IdentityResult result = await _users.DeleteAsync(_user, CancellationToken.None);
                if (result.Succeeded)
                {
                    await _message_service.SendMessage(_user, "Учетная запись", "Ваша учетная запись " + _user.Email + " была удалена.");
                    return Json(true);
                }
            }
            return Json(false);
        }
        #endregion

        #region Roles
        [Menu(Relation = "_EditPartial")]
        [HttpGet]
        public async Task<IActionResult> _AddRolePartial(string userId)
        {
            User user = await _users.FindByIdAsync(userId, CancellationToken.None);
            List<RoleViewModel> roleView = new List<RoleViewModel>();

            foreach (Role role in _roles.Roles)
            {
                if (!user.Roles.Contains(role.Name))
                {
                    roleView.Add(new RoleViewModel(role.Id, role.Name));
                }
            }
            ViewData["Roles"] = roleView;

            return PartialView(new UserRoleInputModel(userId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _AddRolePartial(UserRoleInputModel model)
        {
            User user = await _users.FindByIdAsync(model.UserId, CancellationToken.None);
            if (user != null && ModelState.IsValid)
            {
                Role role = await _roles.FindByNameAsync(model.RoleId, CancellationToken.None);
                if (role != null)
                {
                    await _users.AddToRoleAsync(user, role.Name, CancellationToken.None);
                    _message_service.SendMessage(user, "Новая роль", "Вам добавили права " + role.Name);
                    return Json(true);
                }
            }
            List<RoleViewModel> roleView = new List<RoleViewModel>();
            foreach (Role role in _roles.Roles)
            {
                if (!user.Roles.Contains(role.Name))
                {
                    roleView.Add(new RoleViewModel(role.Id, role.Name));
                }
            }
            ViewData["Roles"] = roleView;

            return PartialView(model);
        }

        [Menu(Relation = "_EditPartial")]
        [HttpGet]
        public async Task<IActionResult> DeleteRole(string userId, string roleId)
        {
            User user = await _users.FindByIdAsync(userId, CancellationToken.None);
            if (user == null)
            {
                return Json(false);
            }
            Role role = await _roles.FindByNameAsync(roleId, CancellationToken.None);
            if (role == null)
            {
                return Json(false);
            }
            await _users.RemoveFromRoleAsync(user, roleId, CancellationToken.None);
            _message_service.SendMessage(user, "Роль", "Вы были исключены из роли " + roleId);
            return Json(true);
        }
        #endregion

        #region Groups
        [Menu(Relation = "_EditPartial")]
        [HttpGet]
        public async Task<IActionResult> _AddGroupPartial(string userId, string groupId)
        {
            User user = await _users.FindByIdAsync(userId, CancellationToken.None);
            List<GroupViewModel> groupView = new List<GroupViewModel>();
            if (user != null)
            {
                IQueryable<Group> groups = await _users.GetGroupsAsync(user, CancellationToken.None);
                foreach (Group group in _groups.Groups)
                {
                    if (groups.FirstOrDefault(g => g.Id == group.Id) == null)
                    {
                        groupView.Add(new GroupViewModel() { Id = group.Id, Name = group.Name });
                    }
                }
            }
            ViewBag.Groups = groupView;
            return PartialView(new GroupUserInputModel(userId));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _AddGroupPartial(GroupUserInputModel model)
        {
            User user = await _users.FindByIdAsync(model.UserId, CancellationToken.None);
            if (user == null)
            {
                ModelState.AddModelError("UserId", "user not found or has been deleted");
            }
            Group group = await _groups.FindByIdAsync(model.GroupId, CancellationToken.None);
            if (group == null)
            {
                ModelState.AddModelError("Group", "Group not found or has been deleted");
            }
            if (ModelState.IsValid)
            {
                IdentityResult result = await _users.AddToGroupAsync(user, group, CancellationToken.None);
                return Json(result.Succeeded);
            }
            List<GroupViewModel> groupView = new List<GroupViewModel>();
            if (user != null)
            {
                IQueryable<Group> groups = await _users.GetGroupsAsync(user, CancellationToken.None);
                foreach (Group _group in _groups.Groups)
                {
                    if (groups.FirstOrDefault(g => g.Id == group.Id) == null)
                    {
                        groupView.Add(new GroupViewModel() { Id = group.Id, Name = group.Name });
                    }
                }
            }
            ViewBag.Groups = groupView;

            return PartialView(model);
        }

        [Menu(Relation = "_EditPartial")]
        [HttpGet]
        public async Task<IActionResult> DeleteGroup(string userId, string groupId)
        {
            User user = await _users.FindByIdAsync(userId, CancellationToken.None);
            if (user == null)
            {
                return Json(false);
            }
            Group group = await _groups.FindByIdAsync(groupId, CancellationToken.None);
            if (group == null)
            {
                return Json(false);
            }
            IdentityResult result = await _users.RemoveFromGroupAsync(user, group, CancellationToken.None);
            return Json(result.Succeeded);
        }
        #endregion

        #region ProfileAttributes
        [Menu(Relation = "_EditPartial")]
        [HttpGet]
        public async Task<IActionResult> _ProfileAttributeListPartial(string userId)
        {
            IQueryable<ProfileAttributeClaim> userClaims = await _attributes.GetUserProfileAttributeClaim(userId, CancellationToken.None);
            List<UserClaimViewModel> claimsView = new List<UserClaimViewModel>();
            foreach (ProfileAttributeClaim userClaim in userClaims)
            {
                //TODO: переделать
                ClaimCustom _claimCheckCustom = _claimCustomRepository.GetClaimCustomById(userClaim.ClaimId);
                if (_claimCheckCustom != null && (_claimCheckCustom.Type.ToLower().Equals(JwtClaimTypes.Subject) || _claimCheckCustom.Type.ToLower().Equals(JwtClaimTypes.Name) ||
                    _claimCheckCustom.Type.ToLower().Equals(JwtClaimTypes.Email) || _claimCheckCustom.Type.ToLower().Equals(JwtClaimTypes.EmailVerified) ||
                    _claimCheckCustom.Type.ToLower().Equals(JwtClaimTypes.PhoneNumber) || _claimCheckCustom.Type.ToLower().Equals(JwtClaimTypes.PhoneNumberVerified)))
                {
                    continue;
                }

                claimsView.Add(new UserClaimViewModel(userId, userClaim.ClaimId)
                {
                    ProfileAttributeName = userClaim.ProfileAttributeName,
                    ClaimValue = userClaim.ClaimValue
                });
            }
            return PartialView(claimsView.AsEnumerable());
        }

        [Menu(Relation = "_EditPartial")]
        [HttpGet]
        public async Task<IActionResult> ProfileAttributeList(string userId)
        {
            IQueryable<ProfileAttributeClaim> userClaims = await _attributes.GetUserProfileAttributeClaim(userId, CancellationToken.None);
            List<UserClaimViewModel> claimsView = new List<UserClaimViewModel>();
            foreach (ProfileAttributeClaim userClaim in userClaims)
            {
                //TODO: переделать
                ClaimCustom _claimCheckCustom = _claimCustomRepository.GetClaimCustomById(userClaim.ClaimId);
                if (_claimCheckCustom != null && (_claimCheckCustom.Type.ToLower().Equals(JwtClaimTypes.Subject) || _claimCheckCustom.Type.ToLower().Equals(JwtClaimTypes.Name) ||
                    _claimCheckCustom.Type.ToLower().Equals(JwtClaimTypes.Email) || _claimCheckCustom.Type.ToLower().Equals(JwtClaimTypes.EmailVerified) ||
                    _claimCheckCustom.Type.ToLower().Equals(JwtClaimTypes.PhoneNumber) || _claimCheckCustom.Type.ToLower().Equals(JwtClaimTypes.PhoneNumberVerified)))
                {
                    continue;
                }

                claimsView.Add(new UserClaimViewModel(userId, userClaim.ClaimId)
                {
                    ProfileAttributeName = userClaim.ProfileAttributeName,
                    ClaimValue = userClaim.ClaimValue
                });
            }

            if (!_users.GetRequiredAttributesOnUser(userId))
            {
                IdentityResult result = await _users.UpdateAttributesValidatedAsync(userId, true, CancellationToken.None);
            }

            return View(claimsView.AsEnumerable());
        }

        [Menu(Relation = "Details")]
        [HttpGet]
        public async Task<IActionResult> _ProfileAttributeDetailsPartial(string userId, string claimId)
        {
            UserClaimViewModel userClaim = new UserClaimViewModel(userId, claimId);
            User user = await _users.FindByIdAsync(userId, CancellationToken.None);
            if (user != null)
            {
                ClaimCustom claim = await _users.GetUserClaimAsync(user, claimId, CancellationToken.None);
                if (claim != null)
                {
                    userClaim.ClaimValue = claim.Value;
                }
            }

            if (!_users.GetRequiredAttributesOnUser(userId))
            {
                IdentityResult result = await _users.UpdateAttributesValidatedAsync(userId, true, CancellationToken.None);
            }
            else
            {
                IdentityResult result = await _users.UpdateAttributesValidatedAsync(userId, false, CancellationToken.None);
            }

            return PartialView(userClaim);
        }

        [Menu(Relation = "_EditPartial")]
        [HttpGet]
        public async Task<IActionResult> _ProfileAttributeEditPartial(string userId, string claimId)
        {

            UserClaimInputModel userClaim = new UserClaimInputModel(userId, claimId);
            User user = await _users.FindByIdAsync(userId, CancellationToken.None);
            if (user != null)
            {
                ClaimCustom claim = await _users.GetUserClaimAsync(user, claimId, CancellationToken.None);
                if (claim != null)
                {
                    userClaim.ClaimValue = claim.Value;
                }
            }
            return PartialView(userClaim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _ProfileAttributeEditPartial(UserClaimInputModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _users.FindByIdAsync(model.UserId, CancellationToken.None);
                if (user != null)
                {
                    ClaimCustom claim = new ClaimCustom("", model.ClaimValue ?? string.Empty)
                    {
                        Id = model.ClaimId
                    };
                    IdentityResult result = await _users.CreateOrUpdateClaimAsync(user, claim, CancellationToken.None);
                    return RedirectToAction("_ProfileAttributeDetailsPartial", new { userId = model.UserId, claimId = model.ClaimId });
                }
            }
            return PartialView(model);
        }
        #endregion

        #region ApiKey
        [Menu(Relation = "_ApiKeysJoinPartial")]
        public async Task<IActionResult> ApiKeyData(string userId)
        {
            List<UserApiKeyViewModel> apiKeysViewList = new List<UserApiKeyViewModel>();
            User user = await _users.FindByIdAsync(userId, CancellationToken.None);
            if (user != null)
            {
                IQueryable<UserApiKey> apiKeys = await _users.GetUserApiKeys(user, CancellationToken.None);
                foreach (UserApiKey key in apiKeys)
                {
                    ClientCustom client = await _clients.FindByIdAsync(key.ClientId, CancellationToken.None);
                    if (client != null)
                    {
                        apiKeysViewList.Add(new UserApiKeyViewModel(key.Id, key.UserId, client.ClientName, key.ApiKey, key.ExperienceTime));
                    }
                }
            }
            return Json(apiKeysViewList);
        }

        [Menu(Name = "Добавление ключей API", Description = "Добавление ключей API", GroupName = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _ApiKeyAddPartial(UserApiKeyInputModel model)
        {
            User user = await _users.FindByIdAsync(model.UserId, CancellationToken.None);
            if (user == null)
            {
                ModelState.AddModelError("UserId", "Пользователь не найден");
                return PartialView(model);
            }
            ClientCustom client = await _clients.FindByIdAsync(model.ClientId, CancellationToken.None);
            if (client == null)
            {
                ModelState.AddModelError("ClientId", "Клиент не найден");
                return PartialView(model);
            }
            if (model.ExperienceTime <= DateTime.Now.AddMinutes(1))
            {
                ModelState.AddModelError("ExperienceTime", "Дата окончания должна быть позже текущей даты");
            }
            if (ModelState.IsValid)
            {
                UserApiKey apiKey = new UserApiKey(model.UserId, model.ClientId, model.ExperienceTime);
                IdentityResult result = await _users.CreateApiKey(apiKey, CancellationToken.None);
                if (result != null && result.Succeeded)
                {
                    model.ClientId = string.Empty;
                    return PartialView(model);
                }
                AddErrors(result);
            }
            return PartialView(model);
        }

        [Menu(Name = "Обновление ключей API", Description = "Обновление ключей API", GroupName = "User")]
        [HttpGet]
        public async Task<IActionResult> ApiKeyUpdate(string apiKeyId)
        {
            UserApiKey key = await _users.FindApiKeyAsync(apiKeyId, CancellationToken.None);
            if (key == null)
            {
                return Json(false);
            }
            IdentityResult result = await _users.UpdateApiKeyAsync(
                new UserApiKey(key.Id, key.UserId, key.ClientId, key.ExperienceTime),
                CancellationToken.None);
            if (result.Succeeded)
            {
                return Json(true);
            }
            return Json(false);
        }

        [Menu(Name = "Удаление ключей API", Description = "Удаление ключей API", GroupName = "User")]
        [HttpGet]
        public async Task<IActionResult> DeleteApiKey(string apiKeyId)
        {
            UserApiKey key = await _users.FindApiKeyAsync(apiKeyId, CancellationToken.None);
            if (key == null)
            {
                return Json(false);
            }
            IdentityResult result = await _users.DeleteApiKeyAsync(key, CancellationToken.None);
            if (result.Succeeded)
            {
                return Json(true);
            }
            return Json(false);
        }

        [Menu(Relation = "_ApiKeysJoinPartial")]
        public async Task<IActionResult> ClientListData(string userId)
        {
            List<ClientViewModel> clientViewList = new List<ClientViewModel>();
            List<UserApiKey> apiKeys = new List<UserApiKey>();
            User user = await _users.FindByIdAsync(userId, CancellationToken.None);
            if (user != null)
            {
                apiKeys = (await _users.GetUserApiKeys(user, CancellationToken.None)).ToList();
            }
            foreach (ClientCustom client in _clients.ClientCustoms)
            {
                if (apiKeys.FirstOrDefault(ak => ak.ClientId.ToLower().Equals(client.Id)) == null)
                {
                    clientViewList.Add(new ClientViewModel(client));
                }
            }
            return Json(clientViewList);
        }

        [Menu(Name = "Список ключей API", Description = "Список ключей API", GroupName = "User")]
        [HttpGet]
        public IActionResult _ApiKeysJoinPartial(string userId)
        {
            ViewData["UserId"] = userId;
            return PartialView();
        }
        #endregion

        [Menu(Name = "Объединение учетных записей", Description = "Объединение дублей учетных учетных записей", GroupName = "User")]
        [HttpGet]
        public async Task<IActionResult> Merge(List<string> usersIds)
        {
            if (usersIds == null || usersIds.Count == 0)
            {
                return RedirectToAction("List");
            }
            UserMergeModel userMerge = new UserMergeModel();
            List<User> users = new List<User>();
            foreach (string userId in usersIds)
            {
                User user = await _users.FindByIdAsync(userId, CancellationToken.None);
                if (user == null)
                {
                    return RedirectToAction("List");
                }
                userMerge.EmailSelect.Add(user.Email);
                if (!string.IsNullOrEmpty(user.UserName) && !userMerge.UserNameSelect.Contains(user.UserName))
                {
                    userMerge.UserNameSelect.Add(user.UserName);

                }
                IList<string> userRoles = await _users.GetRolesAsync(user, CancellationToken.None);
                userMerge.Roles.AddRange(userRoles);

                IQueryable<Group> userGroups = await _users.GetGroupsAsync(user, CancellationToken.None);
                foreach (Group group in userGroups)
                {
                    if (userMerge.Groups.FirstOrDefault(g => g.Id == group.Id) == null)
                    {
                        userMerge.Groups.Add(new GroupViewModel(group.Id, group.Name));
                    }
                }
                IQueryable<ProfileAttributeClaim> claims = await _attributes.GetUserProfileAttributeClaim(user.Id, CancellationToken.None);
                foreach (ProfileAttributeClaim claim in claims)
                {
                    //TODO: переделать
                    ClaimCustom _claimCheckCustom = _claimCustomRepository.GetClaimCustomById(claim.ClaimId);
                    if (_claimCheckCustom != null && (_claimCheckCustom.Type.ToLower().Equals(JwtClaimTypes.Subject) || _claimCheckCustom.Type.ToLower().Equals(JwtClaimTypes.Name) ||
                    _claimCheckCustom.Type.ToLower().Equals(JwtClaimTypes.Email) || _claimCheckCustom.Type.ToLower().Equals(JwtClaimTypes.EmailVerified) ||
                    _claimCheckCustom.Type.ToLower().Equals(JwtClaimTypes.PhoneNumber) || _claimCheckCustom.Type.ToLower().Equals(JwtClaimTypes.PhoneNumberVerified)))
                    {
                        continue;
                    }
                    if (userMerge.ClaimSelect.FirstOrDefault(c => c.ClaimId == claim.ClaimId && c.ClaimValue == claim.ClaimValue) == null)
                    {
                        userMerge.ClaimSelect.Add(new UserClaimViewModel(user.Id, claim.ClaimId)
                        {
                            ProfileAttributeName = claim.ProfileAttributeName,
                            ClaimValue = claim.ClaimValue
                        });
                    }
                }

            }
            userMerge.Roles = userMerge.Roles.Distinct().ToList();

            return View(userMerge);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Merge(UserMergeModel model)
        {

            List<User> usersMerge = new List<User>();
            foreach (string email in model.EmailSelect)
            {
                if (email != model.Email)
                {
                    User mergeUser = await _users.FindByEmailAsync(email, CancellationToken.None);
                    if (mergeUser == null)
                    {
                        return RedirectToAction("List");
                    }
                    usersMerge.Add(mergeUser);
                }
            }

            if (ModelState.IsValid)
            {
                User user = await _users.FindByEmailAsync(model.Email, CancellationToken.None);
                if (user == null)
                {
                    return RedirectToAction("List");
                }

                user.UserName = model.UserName;
                user.PasswordHash = null;
                IdentityResult result = await _users.UpdateAsync(user, CancellationToken.None);
                if (result != null && result.Succeeded)
                {
                    //Проверка ролей
                    IList<string> userRoles = await _users.GetRolesAsync(user, CancellationToken.None);
                    foreach (string roleName in userRoles)
                    {
                        if (!model.Roles.Contains(roleName))
                        {
                            await _users.RemoveFromRoleAsync(user, roleName, CancellationToken.None);
                        }
                        else
                        {
                            model.Roles.Remove(roleName);
                        }
                    }
                    foreach (string roleName in model.Roles)
                    {
                        await _users.AddToRoleAsync(user, roleName, CancellationToken.None);
                    }

                    //Проверка групп
                    IQueryable<Group> userGroups = await _users.GetGroupsAsync(user, CancellationToken.None);
                    foreach (Group group in userGroups)
                    {
                        GroupViewModel userGroup = model.Groups.FirstOrDefault(g => g.Id == group.Id);
                        if (userGroup == null)
                        {
                            await _users.RemoveFromGroupAsync(user, group, CancellationToken.None);
                        }
                        else
                        {
                            model.Groups.Remove(userGroup);
                        }
                    }
                    foreach (GroupViewModel groupView in model.Groups)
                    {
                        Group group = new Group(groupView.Name, groupView.Name);
                        await _users.AddToGroupAsync(user, group, CancellationToken.None);
                    }

                    //Проверка атрибутов
                    foreach (UserClaimInputModel userClaim in model.Claims)
                    {
                        ClaimCustom claim = new ClaimCustom("", userClaim.ClaimValue ?? string.Empty)
                        {
                            Id = userClaim.ClaimId
                        };
                        await _users.CreateOrUpdateClaimAsync(user, claim, CancellationToken.None);
                    }

                    string message = @"Ваши учетные записи " + string.Join(", ", model.EmailSelect) + " были объеденены в учетную запись <b>"
                        + model.Email + "</b> и будут недоступны для авторизации. Используйте учетную запись <b>" + model.Email + "</b> для авторизации."
                        + " Если вы не помните пароля, вы всегда можете воспользоваться восстановлением пароля.";
                    if (user.EmailConfirmed)
                    {
                        await _message_service.SendMessage(user, "Объединение учетных записей", message);
                    }

                    await _message_service.SendMessageMany(usersMerge.Where(u => u.EmailConfirmed).ToList(), "Объединение учетных записей", message);
                    foreach (User userMerge in usersMerge)
                    {
                        await _users.DeleteAsync(userMerge, CancellationToken.None);
                    }

                    return RedirectToAction("Details", new { userId = user.Id });
                }
            }

            foreach (User _user in usersMerge)
            {
                if (model.UserNameSelect == null)
                {
                    model.UserNameSelect = new List<string>();
                }
                if (!string.IsNullOrEmpty(_user.UserName) && !model.UserNameSelect.Contains(_user.UserName))
                {
                    model.UserNameSelect.Add(_user.UserName);
                }
                if (model.ClaimSelect == null)
                {
                    model.ClaimSelect = new List<UserClaimViewModel>();
                }
                IQueryable<ProfileAttributeClaim> claims = await _attributes.GetUserProfileAttributeClaim(_user.Id, CancellationToken.None);
                foreach (ProfileAttributeClaim claim in claims)
                {
                    //TODO: переделать
                    ClaimCustom _claimCheckCustom = _claimCustomRepository.GetClaimCustomById(claim.ClaimId);
                    if (_claimCheckCustom != null && (_claimCheckCustom.Type.ToLower().Equals(JwtClaimTypes.Subject) || _claimCheckCustom.Type.ToLower().Equals(JwtClaimTypes.Name) ||
                    _claimCheckCustom.Type.ToLower().Equals(JwtClaimTypes.Email) || _claimCheckCustom.Type.ToLower().Equals(JwtClaimTypes.EmailVerified) ||
                    _claimCheckCustom.Type.ToLower().Equals(JwtClaimTypes.PhoneNumber) || _claimCheckCustom.Type.ToLower().Equals(JwtClaimTypes.PhoneNumberVerified)))
                    {
                        continue;
                    }

                    if (model.ClaimSelect.FirstOrDefault(c => c.ClaimId == claim.ClaimId && c.ClaimValue == claim.ClaimValue) == null)
                    {
                        model.ClaimSelect.Add(new UserClaimViewModel(_user.Id, claim.ClaimId)
                        {
                            ProfileAttributeName = claim.ProfileAttributeName,
                            ClaimValue = claim.ClaimValue
                        });
                    }
                }
            }
            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
