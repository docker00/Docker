using Host.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySql.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IdentityServer4.Quickstart.UI
{
    [Menu(Name = "Группы", Description = "Управление группами", GroupName = "Group", ShowInMenu = true, Weight = 2)]
    [SecurityHeaders]
    public class GroupController : Controller
    {
        private readonly GroupStore<Group> _groups;
        private readonly RoleStore<Role> _roles;
        private readonly UserStore<User> _users;

        public GroupController(
            GroupStore<Group> groups = null,
            RoleStore<Role> roles = null,
            UserStore<User> users = null)
        {
            _groups = groups ?? new GroupStore<Group>(Host.Startup.ConnectionString);
            _roles = roles ?? new RoleStore<Role>(Host.Startup.ConnectionString);
            _users = users ?? new UserStore<User>(Host.Startup.ConnectionString);
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        [Menu(Name = "Список групп", Description = "Список групп", GroupName = "Group", ShowInMenu = true, Weight = 0)]
        [HttpGet]
        public async Task<IActionResult> List()
        {
            List<GroupViewModel> GroupsList = new List<GroupViewModel>();
            foreach (Group group in _groups.Groups)
            {
                if ((await _groups.GetParentsAsync(group, false, CancellationToken.None)).Count() == 0)
                {
                    GroupsList.Add(new GroupViewModel()
                    {
                        Id = group.Id,
                        ParentId = string.Empty,
                        Name = group.Name
                    });
                }
            }
            return View(GroupsList.AsEnumerable());
        }

        [Menu(Relation = "List")]
        [HttpGet]
        public async Task<IActionResult> GetData(string order, int limit, int offset, string sort, string search)
        {
            KeyValuePair<int, IQueryable<Group>> groups = await _groups.GetQueryFromatterAsync(order, limit, offset, sort, search);
            List<GroupViewModel> groupView = new List<GroupViewModel>();
            foreach (Group group in groups.Value)
            {
                groupView.Add(new GroupViewModel(group.Id, group.Name));
            }
            return Json(new
            {
                total = groups.Key,
                rows = groupView
            });
        }

        [Menu(Relation = "List")]
        [HttpGet]
        public async Task<IActionResult> _ChildrenPartial(string parentId)
        {
            GroupViewModel groupView = new GroupViewModel()
            {
                Id = parentId,
                Name = string.Empty,
                ParentId = string.Empty,
            };
            Group group = await _groups.FindByIdAsync(parentId, CancellationToken.None);
            if (group != null)
            {
                IQueryable<Group> childrenGroups = await _groups.GetChildrenAsync(group, false, CancellationToken.None);
                foreach (Group childGroup in childrenGroups)
                {
                    groupView.ChildrenGroups.Add(new GroupViewModel()
                    {
                        Id = childGroup.Id,
                        Name = childGroup.Name,
                        ParentId = parentId
                    });
                }
                IQueryable<Role> roles = await _roles.FindGroupRoles(parentId, CancellationToken.None);
                foreach (Role role in roles)
                {
                    groupView.Roles.Add(new GroupRoleViewModel()
                    {
                        Id = role.Id,
                        Name = role.Name,
                        GroupId = parentId
                    });
                }
                IQueryable<User> users = await _users.GetUsersInGroup(parentId, CancellationToken.None);
                foreach (User user in users)
                {
                    groupView.Users.Add(new UserViewModel()
                    {
                        Id = user.Id,
                        Email = user.Email,
                        UserName = user.UserName,
                        PhoneNumber = user.PhoneNumber,
                        Roles = user.Roles,
                        Activated = user.Activated,
                    });
                }
            }
            return PartialView(groupView);
        }

        [Menu(Relation = "List")]
        [HttpGet]
        public async Task<IActionResult> _DetailsPartial(string groupId, string groupParentId)
        {
            Group group = await _groups.FindByIdAsync(groupId, CancellationToken.None);

            return PartialView(new GroupViewModel()
            {
                Id = group.Id,
                Name = group.Name,
                ParentId = groupParentId
            });
        }

        [Menu(Name = "Добавление группы", Description = "Добавление групп", GroupName = "Group")]
        [HttpGet]
        public async Task<IActionResult> _AddPartial(string parentId = null)
        {
            GroupInputModel model = new GroupInputModel(parentId);
            List<Group> unavailableGroups = new List<Group>();
            if (!string.IsNullOrEmpty(parentId))
            {
                Group group = await _groups.FindByIdAsync(parentId, CancellationToken.None);
                if (group != null)
                {
                    unavailableGroups = (await _groups.GetChildrenAsync(group, false, CancellationToken.None)).ToList();
                    unavailableGroups = unavailableGroups.Concat(await _groups.GetParentsAsync(group, true, CancellationToken.None)).ToList();
                    unavailableGroups = unavailableGroups.Distinct().ToList();
                }
            }
            List<GroupViewModel> avalibleGroups = new List<GroupViewModel>();
            foreach (Group group in _groups.Groups)
            {
                if (unavailableGroups.FirstOrDefault(g => g.Id == group.Id) == null)
                {
                    avalibleGroups.Add(new GroupViewModel(group.Id, group.Name));
                }
            }
            ViewData["Groups"] = avalibleGroups.AsEnumerable();
            return PartialView(model);
        }

        [HttpPost]
        public async Task<IActionResult> _AddPartial(GroupInputModel model)
        {
            List<Group> unavailableGroups = new List<Group>();
            Group parentGroup = null;
            //TODO: если возможно, надо будет оптимизировать
            if (ModelState.IsValid)
            {
                Group group = await _groups.FindByNameAsync(model.Name.Trim(), CancellationToken.None);
                if (group == null)
                {
                    group = new Group()
                    {
                        Name = model.Name
                    };
                    IdentityResult result = await _groups.CreateAsync(group, CancellationToken.None);
                    if (result != null && result.Succeeded && !string.IsNullOrEmpty(model.ParentId))
                    {
                        result = await _groups.AddParent(group, model.ParentId, CancellationToken.None);
                    }
                    if (result != null && result.Succeeded)
                    {
                        return PartialView("_DetailsPartial", new GroupViewModel(group.Id, group.Name) { ParentId = model.ParentId });
                    }
                    AddErrors(result);
                }
                else
                {
                    if (string.IsNullOrEmpty(model.ParentId))
                    {
                        ModelState.AddModelError("Name", "Такая группа уже существует");
                    }
                    else if (group.Id == model.ParentId)
                    {
                        ModelState.AddModelError("Name", "Нельзя в группу вложить саму себя");
                    }
                    else
                    {
                        parentGroup = await _groups.FindByIdAsync(model.ParentId, CancellationToken.None);
                        if (parentGroup != null)
                        {
                            unavailableGroups = (await _groups.GetChildrenAsync(parentGroup, false, CancellationToken.None)).ToList();
                            unavailableGroups = unavailableGroups.Concat(await _groups.GetParentsAsync(parentGroup, true, CancellationToken.None)).ToList();
                            unavailableGroups = unavailableGroups.Distinct().ToList();
                        }
                        if (unavailableGroups.FirstOrDefault(g => g.Id == group.Id) == null)
                        {
                            IdentityResult result = await _groups.AddParent(group, parentGroup.Id, CancellationToken.None);
                            if (result != null && result.Succeeded)
                            {
                                return PartialView("_DetailsPartial", new GroupViewModel(group.Id, group.Name) { ParentId = parentGroup.Id });
                            }
                            AddErrors(result);
                        }
                        else
                        {
                            ModelState.AddModelError("Name", "Эта группа уже привязана к этому родителю либо недоступна как дочерняя");
                        }
                    }
                }
            }
            if (parentGroup == null && !string.IsNullOrEmpty(model.ParentId))
            {
                parentGroup = await _groups.FindByIdAsync(model.ParentId, CancellationToken.None);
                if (parentGroup != null)
                {
                    unavailableGroups = (await _groups.GetChildrenAsync(parentGroup, false, CancellationToken.None)).ToList();
                    unavailableGroups = unavailableGroups.Concat(await _groups.GetParentsAsync(parentGroup, true, CancellationToken.None)).ToList();
                    unavailableGroups = unavailableGroups.Distinct().ToList();
                }
            }
            List<GroupViewModel> avalibleGroups = new List<GroupViewModel>();
            foreach (Group group in _groups.Groups)
            {
                if (unavailableGroups.FirstOrDefault(g => g.Id == group.Id) == null)
                {
                    avalibleGroups.Add(new GroupViewModel(group.Id, group.Name));
                }
            }
            ViewData["Groups"] = avalibleGroups.AsEnumerable();
            return PartialView(model);
        }

        [Menu(Name = "Редактирование группы", Description = "Редактирование группы", GroupName = "Group")]
        [HttpGet]
        public async Task<IActionResult> _EditPartial(string groupId, string groupParentId)
        {
            Group group = await _groups.FindByIdAsync(groupId, CancellationToken.None);
            return PartialView(new GroupInputModel(group.Id, group.Name) { ParentId = groupParentId });
        }

        [HttpPost]
        public async Task<IActionResult> _EditPartial(GroupInputModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _groups.UpdateAsync(new Group(model.Name, model.Id), CancellationToken.None);
                if (result.Succeeded)
                {
                    return PartialView("_DetailsPartial", new GroupViewModel(model.Id, model.Name) { ParentId = model.ParentId });
                }
                AddErrors(result);
            }
            return PartialView(model);
        }

        [Menu(Name = "Удаление группы", Description = "Удаление группы", GroupName = "Group")]
        [HttpGet]
        public async Task<IActionResult> Delete(string groupId, bool deleteChilden)
        {
            Group group = await _groups.FindByIdAsync(groupId, CancellationToken.None);
            if (group != null)
            {
                if (deleteChilden)
                {
                    IQueryable<Group> childrenGroups = await _groups.GetChildrenAsync(group, true, CancellationToken.None);
                    foreach (Group childGroup in childrenGroups)
                    {
                        await _groups.DeleteAsync(childGroup, CancellationToken.None);
                    }
                }
                IdentityResult result = await _groups.DeleteAsync(group, CancellationToken.None);
                if (result.Succeeded)
                {
                    return Json(result.Succeeded);
                }
                AddErrors(result);
            }

            return Json(false);
        }

        [Menu(Relation = "_EditPartial")]
        [HttpGet]
        public async Task<IActionResult> ChildDelete(string groupId, string parentId)
        {
            IdentityResult result = await _groups.DeleteGroupChild(groupId, parentId);
            if (result.Succeeded)
            {
                return Json(parentId);
            }
            return Json(false);
        }

        [Menu(Relation = "_EditPartial")]
        [HttpPost]
        public async Task<IActionResult> RoleAdd(string groupId, List<string> rolesIds)
        {
            Group group = await _groups.FindByIdAsync(groupId, CancellationToken.None);
            if (group == null)
            {
                return Json(false);
            }
            List<Role> roles = new List<Role>();
            foreach (string roleId in rolesIds)
            {
                Role role = await _roles.FindByIdAsync(roleId, CancellationToken.None);
                if (role == null)
                {
                    return Json(false);
                }
                roles.Add(role);
            }

            IdentityResult result = await _groups.UpdateRolesAsync(group, roles, CancellationToken.None);
            if (result != null && result.Succeeded)
            {
                return Json(result.Succeeded);
            }
            return Json(false);
        }

        [Menu(Relation = "_EditPartial")]
        [HttpGet]
        public async Task<IActionResult> RoleDelete(string roleId, string groupId, bool fromUsersDelete)
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
            IdentityResult result = await _groups.DeleteGroupRoleAsync(group, role, CancellationToken.None, fromUsersDelete);
            if (result.Succeeded)
            {
                return Json(true);
            }
            return Json(false);
        }

        [Menu(Relation = "_EditPartial")]
        [HttpPost]
        public async Task<IActionResult> UserAdd(string groupId, List<string> users_ids)
        {
            if (users_ids != null && users_ids.Count > 0)
            {
                Group group = await _groups.FindByIdAsync(groupId, CancellationToken.None);
                if (group == null)
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
                await _groups.AddGroupUsers(group, users, CancellationToken.None);
                return Json(true);
            }

            return Json(false);
        }

        [Menu(Relation = "_EditPartial")]
        [HttpGet]
        public async Task<IActionResult> RemoveUser(string groupId, string userId)
        {
            Group group = await _groups.FindByIdAsync(groupId, CancellationToken.None);
            if (group == null)
            {
                return Json(false);
            }
            User user = await _users.FindByIdAsync(userId, CancellationToken.None);
            if (user == null)
            {
                return Json(false);
            }
            IdentityResult result = await _users.RemoveFromGroupAsync(user, group, CancellationToken.None);
            return Json(result.Succeeded);
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
