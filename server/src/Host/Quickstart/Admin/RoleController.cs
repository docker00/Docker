using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MySql.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using Host.Extensions.GenerateMenu.Models;
using Host.Extensions.GenerateMenu;
using Host.Extensions;
using Microsoft.AspNetCore.Authorization;

namespace IdentityServer4.Quickstart.UI
{
    [Menu(Name = "Роли", Description = "Управление ролями", GroupName = "Role", ShowInMenu = true, Weight = 1)]
    public class RoleController : Controller
    {
        private readonly RoleStore<Role> _roles;
        private readonly PermissionStore _permissions;
        private readonly ObjectStore _objects;
        private readonly ObjectEndpointStore _objectEndpoints;
        private readonly LocalPartitionStore<LocalPartition> _localPartitions;
        private readonly GroupStore<Group> _groups;
        private readonly UserStore<User> _users;

        public RoleController(
            RoleStore<Role> roles = null,
            PermissionStore permissions = null,
            ObjectStore objects = null,
            ObjectEndpointStore objectEndpoints = null,
            LocalPartitionStore<LocalPartition> localPratitions = null,
            GroupStore<Group> groups = null,
            UserStore<User> users = null
            )
        {
            _roles = roles ?? new RoleStore<Role>(Host.Startup.ConnectionString);
            _permissions = permissions ?? new PermissionStore(Host.Startup.ConnectionString);
            _objects = objects ?? new ObjectStore(Host.Startup.ConnectionString);
            _objectEndpoints = objectEndpoints ?? new ObjectEndpointStore(Host.Startup.ConnectionString);
            _localPartitions = localPratitions ?? new LocalPartitionStore<LocalPartition>(Host.Startup.ConnectionString);
            _groups = groups ?? new GroupStore<Group>(Host.Startup.ConnectionString);
            _users = users ?? new UserStore<User>(Host.Startup.ConnectionString);
        }

        public IActionResult Index()
        {
            return View();
        }

        #region Main
        [Menu(Name = "Список ролей", Description = "Список ролей", GroupName = "Role", ShowInMenu = true, Weight = 1)]
        public IActionResult List()
        {
            return View();
        }

        [Menu(Relation = "List")]
        [HttpGet]
        public async Task<JsonResult> Data(string order, int limit, int offset, string sort, string search, string groupId = null, bool inGroup = false)
        {
            KeyValuePair<int, IQueryable<Role>> roles = await _roles.GetBootsrapTableData(order, limit, offset, sort, search, groupId, inGroup);
            List<RoleViewModel> roleView = new List<RoleViewModel>();
            foreach (Role role in roles.Value)
            {
                roleView.Add(new RoleViewModel { Id = role.Id, Name = role.Name });
            }

            return Json(new
            {
                total = roles.Key,
                rows = roleView
            });
        }

        [Menu(Name = "Добавление роли", Description = "Добавление роли", GroupName = "Role", ShowInMenu = true, Weight = 0)]
        [HttpGet]
        public IActionResult Add(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                Role roleByName = await _roles.FindByNameAsync(model.Name, CancellationToken.None);
                if (roleByName == null)
                {
                    Role role = new Role() { Name = model.Name };
                    IdentityResult result = await _roles.CreateAsync(role, CancellationToken.None);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("Details", new { roleId = role.Id });
                    }
                    AddErrors(result);
                }
                else
                {
                    ModelState.AddModelError("Name", "Такое название уже существует");
                }
            }
            return View(model);
        }

        [Menu(Name = "Редактирование роли", Description = "Редактирование роли", GroupName = "Role")]
        [HttpGet]
        public async Task<IActionResult> Edit(string roleId, string roleName)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return RedirectToAction("List");
            }

            if (string.IsNullOrEmpty(roleName))
            {
                Role roleNameById = await _roles.FindByIdAsync(roleId, CancellationToken.None);
                roleName = roleNameById.Name;
            }

            RoleViewModel role = new RoleViewModel() { Id = roleId, Name = roleName };
            return View(role);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                Role role = new Role() { Id = model.Id, Name = model.Name };
                IdentityResult result = await _roles.UpdateAsync(role, CancellationToken.None);
                if (result != null && result.Succeeded)
                {
                    return RedirectToAction("Details", new { roleId = role.Id });
                }
                AddErrors(result);
            }
            return View(model);
        }

        [Menu(Name = "Удаление роли", Description = "Удаление роли", GroupName = "Role")]
        [HttpPost]
        public async Task<IActionResult> Delete(Role model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _roles.DeleteAsync(model, CancellationToken.None);
                if (result == null || !result.Succeeded)
                {
                    AddErrors(result);
                    return View(model);
                }
            }

            return RedirectToAction("List");
        }
        #endregion

        #region Details Main
        [Menu(Name = "Подробнее", Description = "Детальная информация о роле", GroupName = "Role")]
        [HttpGet]
        public async Task<IActionResult> Details(string roleId)
        {
            Role role = await _roles.FindByIdAsync(roleId, CancellationToken.None);
            if (role != null)
            {
                RoleViewModel roleView = new RoleViewModel()
                {
                    Id = role.Id,
                    Name = role.Name
                };

                IQueryable<Group> groups = await _groups.FindGroupsByRoleIdAsync(roleId, CancellationToken.None);
                foreach (Group group in groups)
                {
                    roleView.Groups.Add(new GroupViewModel()
                    {
                        Id = group.Id,
                        Name = group.Name
                    });
                }
                return View(roleView);
            }
            return RedirectToAction("List");
        }

        [Menu(Relation = "Details")]
        [HttpGet]
        public async Task<IActionResult> _DetailsPartial(string roleId)
        {
            Role resource = await _roles.FindByIdAsync(roleId, CancellationToken.None);
            if (resource != null)
            {
                RoleViewModel roleView = new RoleViewModel()
                {
                    Id = resource.Id,
                    Name = resource.Name
                };
                return PartialView(roleView);
            }
            return RedirectToAction("List");
        }

        [Menu(Relation = "Details")]
        [HttpGet]
        public async Task<IActionResult> _EditPartial(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return RedirectToAction("List");
            }

            Role roleNameById = await _roles.FindByIdAsync(roleId, CancellationToken.None);

            RoleViewModel roleViewModel = new RoleViewModel()
            {
                Id = roleId,
                Name = roleNameById.Name
            };
            return PartialView(roleViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _EditPartial(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                Role role = new Role()
                {
                    Id = model.Id,
                    Name = model.Name
                };
                IdentityResult result = await _roles.UpdateAsync(role, CancellationToken.None);
                if (result != null || result.Succeeded)
                {
                    return RedirectToAction("_DetailsPartial", new { roleId = model.Id });
                }
                AddErrors(result);
            }
            return PartialView(model);
        }
        #endregion

        #region Details Groups
        [Menu(Relation = "Details")]
        [HttpGet]
        public async Task<IActionResult> _AddGroupPartial(string roleId)
        {
            IQueryable<Group> groups = await _groups.FindGroupsByRoleIdAsync(roleId, CancellationToken.None);
            List<GroupViewModel> groupView = new List<GroupViewModel>();
            foreach (Group group in _groups.Groups)
            {
                if (groups.FirstOrDefault(c => c.Id.Contains(group.Id)) == null)
                {
                    groupView.Add(new GroupViewModel(group.Id, group.Name));
                }
            }
            ViewData["Groups"] = groupView;
            return PartialView(new GroupRoleInputModel() { RoleId = roleId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _AddGroupPartial(GroupRoleInputModel model)
        {

            if (ModelState.IsValid)
            {
                IdentityResult result = await _groups.CreateGroupRoleAsync(new GroupRole() { GroupId = model.GroupId, RoleId = model.RoleId }, CancellationToken.None);
                return Json(result.Succeeded);
            }

            IQueryable<Group> groups = await _groups.FindGroupsByRoleIdAsync(model.RoleId, CancellationToken.None);
            List<GroupViewModel> groupView = new List<GroupViewModel>();
            foreach (Group group in _groups.Groups)
            {
                if (groups.FirstOrDefault(c => c.Id.Contains(group.Id)) == null)
                {
                    groupView.Add(new GroupViewModel(group.Id, group.Name));
                }
            }
            ViewData["Groups"] = groupView;

            return PartialView(model);
        }

        [Menu(Relation = "Details")]
        public async Task<IActionResult> DeleteGroup(string groupId, string roleId)
        {
            Group group = await _groups.FindByIdAsync(groupId, CancellationToken.None);
            if (group == null)
            {
                return Json(false);
            }
            Role role = await _roles.FindByIdAsync(roleId, CancellationToken.None);
            if (role == null)
            {
                return Json(false);
            }
            IdentityResult result = await _groups.DeleteGroupRoleAsync(group, role, CancellationToken.None);
            return Json(result.Succeeded);
        }
        #endregion

        #region Details Users
        [Menu(Relation = "List")]
        [HttpGet]
        public JsonResult UsersData(string order, int limit, int offset, string sort, string search, string roleId = null, bool inRole = false)
        {
            IList<UserViewModel> rows = _users.GetUsersQueryFormatterByRoleIdAsync(order, limit, offset, sort, search, roleId, inRole);
            int total = _users.GetUsersQueryFormatterCountByRoleIdAsync(search, roleId, inRole);
            return Json(new
            {
                total = total,
                rows = rows.ToList()
            });
        }

        [Menu(Relation = "List")]
        [HttpPost]
        public async Task<IActionResult> UserAdd(string roleId, List<string> users_ids)
        {
            if (users_ids != null && users_ids.Count > 0)
            {
                Role role = await _roles.FindByIdAsync(roleId, CancellationToken.None);
                if (role == null)
                {
                    return Json(false);
                }
                List<User> users = new List<User>();
                foreach (string userId in users_ids)
                {
                    User user = await _users.FindByIdAsync(userId, CancellationToken.None);
                    if (user == null)
                    {
                        return Json(false);
                    }
                    users.Add(user);
                }
                await _roles.AddRoleUsers(role, users, CancellationToken.None);
                return Json(true);
            }

            return Json(false);
        }

        [Menu(Relation = "Details")]
        [HttpGet]
        public async Task<IActionResult> _AddUserPartial(string roleId)
        {
            IQueryable<User> users = await _users.FindUsersByRoleIdAsync(roleId, CancellationToken.None);
            List<UserViewModel> userView = new List<UserViewModel>();
            foreach (User user in _users.Users)
            {
                if (users.FirstOrDefault(c => c.Id.Contains(user.Id)) == null)
                {
                    userView.Add(new UserViewModel() { Id = user.Id, Email = user.Email });
                }
            }
            ViewData["Users"] = userView;
            return PartialView(new UserRoleInputModel() { RoleId = roleId });
        }

        [HttpPost]
        public async Task<IActionResult> _AddUserPartial(UserRoleInputModel model)
        {

            if (ModelState.IsValid)
            {
                Role role = await _roles.FindByIdAsync(model.RoleId, CancellationToken.None);
                await _users.AddToRoleAsync(new User() { Id = model.UserId }, role.Name, CancellationToken.None);
                return Json(true);
            }

            IQueryable<User> users = await _users.FindUsersByRoleIdAsync(model.RoleId, CancellationToken.None);
            List<UserViewModel> userView = new List<UserViewModel>();
            foreach (User user in _users.Users)
            {
                if (users.FirstOrDefault(c => c.Id.Contains(user.Id)) == null)
                {
                    userView.Add(new UserViewModel() { Id = user.Id, Email = user.Email });
                }
            }
            ViewData["Users"] = userView;

            return PartialView(model);
        }

        [Menu(Relation = "Details")]
        public async Task<IActionResult> DeleteUser(string userId, string roleId)
        {
            Role role = await _roles.FindByIdAsync(roleId, CancellationToken.None);
            await _users.RemoveFromRoleAsync(new User() { Id = userId }, role.Name, CancellationToken.None);
            return Json(true);
        }
        #endregion

        #region Details Permissions

        [Menu(Relation = "Details")]
        public async Task<IActionResult> _ObjectEndpointListPartial(string objectId, string roleId)
        {
            List<ObjectEndpoint> objectEndpointList = (await _objectEndpoints.FindObjectEndpointsByObjectIdAsync(objectId, CancellationToken.None)).ToList();

            List<ObjectEndpointPermitionViewModel> objectEndpointPermissionList = new List<ObjectEndpointPermitionViewModel>();
            IQueryable<ObjectEndpointPermition> objectEndpointPermitions;

            foreach (ObjectEndpoint _objectEndpoint in objectEndpointList)
            {
                objectEndpointPermitions = await _objectEndpoints.FindObjectEndpointPermission(_objectEndpoint.Id, CancellationToken.None);
                foreach (ObjectEndpointPermition objectEndpointPermition in objectEndpointPermitions)
                {
                    string permissionName = (await _permissions.FindByIdAsync(objectEndpointPermition.PermissionId, CancellationToken.None)).Name;
                    objectEndpointPermissionList.Add(new ObjectEndpointPermitionViewModel
                    {
                        Id = objectEndpointPermition.Id,
                        ObjectEndpointId = _objectEndpoint.Id,
                        PermissionId = objectEndpointPermition.PermissionId,
                        PermissionName = permissionName
                    });
                }
            }

            ViewData["objectEndpointPermissions"] = objectEndpointPermissionList.ToList();

            IQueryable<ObjectEndpointPermition> permissionsIdByRole = await _objectEndpoints.FindObjectEndpointPermissionByRoleAsync(roleId, CancellationToken.None);
            ViewData["permissionsByRole"] = permissionsIdByRole.ToList();
            ViewData["RoleId"] = roleId;
            return PartialView(objectEndpointList);
        }

        [Menu(Name = "Выбор разрешений для роли", Description = "Выбор разрешений для роли", GroupName = "Role")]
        [HttpGet]
        public async Task<IActionResult> RolePermissionList(string roleId)
        {
            if (string.IsNullOrEmpty(roleId))
            {
                return RedirectToAction("List");
            }

            IQueryable<MySql.AspNet.Identity.Object> objectList = _objects.Objects;
            ViewData["Objects"] = objectList.ToList();

            IQueryable<ObjectEndpoint> objectEndpointList = _objectEndpoints.ObjectEndpoints;
            ViewData["objectEndpoints"] = objectEndpointList.ToList();

            List<ObjectEndpointPermitionViewModel> objectEndpointPermissionList = new List<ObjectEndpointPermitionViewModel>();
            IQueryable<ObjectEndpointPermition> objectEndpointPermitions;

            foreach (ObjectEndpoint _objectEndpoint in objectEndpointList)
            {
                objectEndpointPermitions = await _objectEndpoints.FindObjectEndpointPermission(_objectEndpoint.Id, CancellationToken.None);
                foreach (ObjectEndpointPermition objectEndpointPermition in objectEndpointPermitions)
                {
                    string permissionName = (await _permissions.FindByIdAsync(objectEndpointPermition.PermissionId, CancellationToken.None)).Name;
                    objectEndpointPermissionList.Add(new ObjectEndpointPermitionViewModel
                    {
                        Id = objectEndpointPermition.Id,
                        ObjectEndpointId = _objectEndpoint.Id,
                        PermissionId = objectEndpointPermition.PermissionId,
                        PermissionName = permissionName
                    });
                }
            }

            ViewData["objectEndpointPermissions"] = objectEndpointPermissionList.ToList();

            IQueryable<ObjectEndpointPermition> permissionsIdByRole = await _objectEndpoints.FindObjectEndpointPermissionByRoleAsync(roleId, CancellationToken.None);
            ViewData["permissionsByRole"] = permissionsIdByRole.ToList();

            ViewData["roleId"] = roleId;

            return View();
        }

        [Menu(Relation = "Details")]
        [HttpGet]
        public JsonResult RolePermissionData(string order, int limit, int offset, string sort, string search, string roleId)
        {
            IQueryable objects = _objects.Objects;
            List<ObjectViewModel> objectView = new List<ObjectViewModel>();

            foreach (MySql.AspNet.Identity.Object _object in objects)
            {
                objectView.Add(new ObjectViewModel { Id = _object.Id, Name = _object.Name, Description = _object.Description, RoleId = roleId });
            }

            return Json(objectView);
        }

        [Menu(Relation = "Details")]
        [HttpPost]
        public async Task<IActionResult> AddRolePermission(string roleId, string objectEndpointPermissionId)
        {
            if (string.IsNullOrEmpty(roleId) && string.IsNullOrEmpty(objectEndpointPermissionId))
            {
                return Json(false);
            }

            IdentityResult result = await _roles.CreateRolePermissionAsync(roleId, objectEndpointPermissionId, CancellationToken.None);

            if (result != null && result.Succeeded)
            {
                return Json(true);
            }

            AddErrors(result);
            return Json(false);
        }

        [Menu(Relation = "Details")]
        [HttpPost]
        public async Task<IActionResult> DeleteRolePermission(string roleId, string objectEndpointPermissionId)
        {
            if (string.IsNullOrEmpty(roleId) && string.IsNullOrEmpty(objectEndpointPermissionId))
            {
                return Json(false);
            }

            IdentityResult result = await _roles.DeleteRolePermissionAsync(roleId, objectEndpointPermissionId, CancellationToken.None);

            if (result != null && result.Succeeded)
            {
                return Json(true);
            }

            AddErrors(result);
            return Json(false);
        }

        #endregion

        #region LocalPartitions

        [Menu(Relation = "Details")]
        [HttpGet]
        public async Task<JsonResult> RoleLocalPartitionData(string order, int limit, int offset, string sort, string search, string roleId)
        {
            Role role = await _roles.FindByIdAsync(roleId, CancellationToken.None);
            List<LocalPartition> rolePartitions = new List<LocalPartition>();
            if (role != null)
            {
                rolePartitions = (await _roles.GetRoleLocalPartitons(role, CancellationToken.None)).ToList();
            }
            List<ControllerViewModel> controllersView = new List<ControllerViewModel>();
            List<ControllerModel> controllers = new GenerateMenu().GetControllers();
            IQueryable<LocalPartition> partitions = _localPartitions.LocalPartitions;
            foreach (ControllerModel controller in controllers)
            {
                List<LocalPartition> controllerPartition = new List<LocalPartition>();
                int SelectedCounter = 0;
                foreach (MethodModel method in controller.Methods)
                {
                    LocalPartition partition = partitions.FirstOrDefault(p => p.ControllerName.ToLower().Equals(method.Controller.ToLower())
                    && p.ActionName.ToLower().Equals(method.Method.ToLower()));
                    if (partition != null)
                    {
                        controllerPartition.Add(partition);
                        if (rolePartitions.FirstOrDefault(rp => rp.ControllerName.ToLower().Equals(partition.ControllerName.ToLower())
                         && rp.ActionName.ToLower().Equals(partition.ActionName.ToLower())) != null)
                        {
                            SelectedCounter++;
                        }
                    }
                }
                if (controllerPartition.Count > 0)
                {
                    controllersView.Add(new ControllerViewModel(controller.Controller, controller.Name, controller.Description, controller.Weight)
                    {
                        State = controllerPartition.Count == SelectedCounter
                    });
                };
            }
            controllersView = controllersView.OrderBy(c => c.Weight).ToList();
            ViewData["RoleId"] = roleId;
            return Json(controllersView);
        }

        [Menu(Relation = "Details")]
        [HttpGet]
        public async Task<IActionResult> _MethodListPartial(string roleId, string _controller)
        {
            ViewData["RoleId"] = roleId;
            Role role = await _roles.FindByIdAsync(roleId, CancellationToken.None);

            List<MethodViewModel> Methods = new List<MethodViewModel>();
            if (role == null)
            {
                return PartialView(Methods);
            }
            ControllerModel controller = new GenerateMenu().GetControllers().FirstOrDefault(c => c.Controller == _controller);
            if (_controller == null)
            {
                return PartialView(Methods);
            }
            IQueryable<LocalPartition> rolePartitions = await _roles.GetRoleLocalPartitons(role, CancellationToken.None);
            foreach (LocalPartition partition in _localPartitions.LocalPartitions)
            {
                MethodModel method = controller.Methods.FirstOrDefault(m => m.Controller == partition.ControllerName && m.Method == partition.ActionName);
                if (method != null)
                {
                    Methods.Add(new MethodViewModel(method.Method, method.Name, method.Description, method.Controller, method.Weight)
                    {
                        Id = partition.Id,
                        Checked = rolePartitions.FirstOrDefault(rp => rp.ControllerName.ToLower().Equals(method.Controller.ToLower())
                        && rp.ActionName.ToLower().Equals(method.Method.ToLower())) != null
                    });
                }
            }
            Methods = Methods.OrderBy(m => m.Weight).ToList();
            return PartialView(Methods);
        }

        [Menu(Relation = "Details")]
        public async Task<IActionResult> _AddControllerMethodsPartial(string roleId, string _controller)
        {
            Role role = await _roles.FindByIdAsync(roleId, CancellationToken.None);
            if (role == null)
            {
                return Json(false);
            }
            ControllerModel controller = new GenerateMenu().GetControllers().FirstOrDefault(c => c.Controller == _controller);
            if (_controller == null)
            {
                return Json(false);
            }
            IQueryable<LocalPartition> rolePartitions = await _roles.GetRoleLocalPartitons(role, CancellationToken.None);
            foreach (LocalPartition partition in _localPartitions.LocalPartitions)
            {
                MethodModel method = controller.Methods.FirstOrDefault(m => m.Controller == partition.ControllerName && m.Method == partition.ActionName);
                if (method != null)
                {
                    IdentityResult result = await _roles.CreateRoleLocalPartitionAsync(roleId, partition.Id, CancellationToken.None);
                    if (!result.Succeeded)
                    {
                        AddErrors(result);
                        return Json(false);
                    }
                }
            }

            return Json(true);
        }

        [Menu(Relation = "Details")]
        public async Task<IActionResult> _DeleteControllerMethodsPartial(string roleId, string _controller)
        {
            ViewData["RoleId"] = roleId;
            Role role = await _roles.FindByIdAsync(roleId, CancellationToken.None);

            List<MethodViewModel> Methods = new List<MethodViewModel>();
            if (role == null)
            {
                return PartialView(Methods);
            }
            ControllerModel controller = new GenerateMenu().GetControllers().FirstOrDefault(c => c.Controller == _controller);
            if (_controller == null)
            {
                return PartialView(Methods);
            }
            IQueryable<LocalPartition> rolePartitions = await _roles.GetRoleLocalPartitons(role, CancellationToken.None);
            foreach (LocalPartition partition in _localPartitions.LocalPartitions)
            {
                MethodModel method = controller.Methods.FirstOrDefault(m => m.Controller == partition.ControllerName && m.Method == partition.ActionName);
                if (method != null)
                {
                    IdentityResult result = await _roles.DeleteRoleLocalPartitionAsync(roleId, partition.Id, CancellationToken.None);
                    if (!result.Succeeded)
                    {
                        AddErrors(result);
                        return Json(false);
                    }
                }
            }

            return Json(true);
        }

        [Menu(Relation = "Details")]
        [HttpPost]
        public async Task<IActionResult> AddLocalPartition(string roleId, string localPartitionId)
        {
            Role role = await _roles.FindByIdAsync(roleId, CancellationToken.None);
            if (role == null)
            {
                return Json(false);
            }
            LocalPartition partition = await _localPartitions.FindAsync(localPartitionId, CancellationToken.None);
            if (partition == null)
            {
                return Json(false);
            }
            IdentityResult result = await _roles.CreateRoleLocalPartitionAsync(roleId, localPartitionId, CancellationToken.None);
            if (result != null && result.Succeeded)
            {
                return Json(true);
            }
            return Json(false);
        }

        [Menu(Relation = "Details")]
        [HttpPost]
        public async Task<IActionResult> DeleteLocalPartition(string roleId, string localPartitionId)
        {
            if (string.IsNullOrEmpty(roleId) && string.IsNullOrEmpty(localPartitionId))
            {
                return Json(false);
            }

            IdentityResult result = await _roles.DeleteRoleLocalPartitionAsync(roleId, localPartitionId, CancellationToken.None);

            if (result != null && result.Succeeded)
            {
                return Json(true);
            }

            AddErrors(result);
            return Json(false);
        }
        #endregion

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
