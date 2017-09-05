using Host.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySql.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityServer4.Quickstart.UI
{
    [Menu(Name = "Объекты", Description = "Управление объектами", GroupName = "Object", ShowInMenu = true, Weight = 4)]
    public class ObjectController : Controller
    {
        private readonly ObjectStore _objects;
        private readonly ObjectEndpointStore _objectEndpoints;
        private readonly PermissionStore _permissions;
        private readonly UserStore<User> _users;
        private readonly RoleStore<Role> _roles;
        private readonly GroupStore<Group> _groups;
        private readonly ClientCustomStore _clients;

        public ObjectController(
            ObjectStore objects = null,
            ObjectEndpointStore objectEndpoints = null,
            PermissionStore permissions = null,
            UserStore<User> users = null,
            RoleStore<Role> roles = null,
            GroupStore<Group> groups = null,
            ClientCustomStore clients = null
            )
        {
            _objects = objects ?? new ObjectStore(Host.Startup.ConnectionString);
            _objectEndpoints = objectEndpoints ?? new ObjectEndpointStore(Host.Startup.ConnectionString);
            _permissions = permissions ?? new PermissionStore(Host.Startup.ConnectionString);
            _users = users ?? new UserStore<User>(Host.Startup.ConnectionString);
            _roles = roles ?? new RoleStore<Role>(Host.Startup.ConnectionString);
            _groups = groups ?? new GroupStore<Group>(Host.Startup.ConnectionString);
            _clients = clients ?? new ClientCustomStore(Host.Startup.ConnectionString);
        }

        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        [Menu(Name = "Список объектов", Description = "Список объектов", GroupName = "Object", ShowInMenu = true, Weight = 1)]
        public IActionResult List()
        {
            return View();
        }

        [Menu(Relation = "List")]
        [HttpGet]
        public async Task<JsonResult> ObjectData(string order, int limit, int offset, string sort, string search)
        {
            IQueryable<Object> objects = _objects.Objects;
            List<ObjectViewModel> rows = new List<ObjectViewModel>();
            foreach (Object _object in objects)
            {
                ClientCustom clientCustom = await _clients.FindByObjectIdAsync(_object.Id, CancellationToken.None);
                rows.Add(new ObjectViewModel()
                {
                    Id = _object.Id,
                    Name = _object.Name,
                    Description = _object.Description,
                    Url = _object.Url,
                    Enabled = (clientCustom != null) ? clientCustom.Enabled : false
                });
            }

            return Json(rows);
        }

        [Menu(Relation = "List")]
        [HttpGet]
        public async Task<JsonResult> ObjectEndpointDataById(string objectId, string order, int limit, int offset, string sort, string search)
        {
            IQueryable objectEndpoints = await _objectEndpoints.FindObjectEndpointsByObjectIdAsync(objectId, CancellationToken.None);
            List<ObjectEndpointViewModel> objectEndpointView = new List<ObjectEndpointViewModel>();

            foreach (ObjectEndpoint objectEndpoint in objectEndpoints)
            {
                objectEndpointView.Add(new ObjectEndpointViewModel
                {
                    Id = objectEndpoint.Id,
                    Value = objectEndpoint.Value,
                    Description = objectEndpoint.Description,
                    ObjectId = objectEndpoint.ObjectId
                });
            }

            return Json(objectEndpointView);
        }

        [Menu(Name = "Добавление объекта", Description = "Добавить объект", GroupName = "Object", ShowInMenu = true, Weight = 0)]
        [HttpGet]
        public async Task<IActionResult> Add(string clientId = null)
        {
            ObjectViewModel objectView = new ObjectViewModel();
            if (!string.IsNullOrEmpty(clientId))
            {
                ClientCustom client = await _clients.FindByIdAsync(clientId, CancellationToken.None);
                if (client != null)
                {
                    objectView.ClientId = clientId;
                    objectView.Name = client.ClientName;
                    objectView.Url = client.ClientUri;
                }
            }
            return View(objectView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ObjectViewModel model)
        {
            if (ModelState.IsValid)
            {
                Object _object = new Object
                {
                    Name = model.Name,
                    Description = model.Description,
                    Url = model.Url
                };
                IdentityResult result = await _objects.CreateAsync(_object, CancellationToken.None);
                if (result != null && result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ClientId))
                    {
                        ClientCustom client = await _clients.FindByIdAsync(model.ClientId, CancellationToken.None);
                        if (client != null)
                        {
                            result = await _clients.AddToObjectAsync(client, _object.Id, CancellationToken.None);
                        }
                    }
                    return RedirectToAction("Details", new { objectId = _object.Id });
                }
                AddErrors(result);
            }
            return View(model);
        }

        [Menu(Name = "Редактирование объекта", Description = "Редактирование объекта", GroupName = "Object")]
        [HttpGet]
        public async Task<IActionResult> Edit(string objectId)
        {
            if (string.IsNullOrEmpty(objectId))
            {
                return RedirectToAction("List");
            }

            MySql.AspNet.Identity.Object _object = await _objects.FindByIdAsync(objectId, CancellationToken.None);
            if (_object == null)
            {
                return RedirectToAction("List");
            }

            ObjectViewModel objectView = new ObjectViewModel() { Id = _object.Id, Name = _object.Name, Description = _object.Description, Url = _object.Url };

            return View(objectView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ObjectViewModel model)
        {
            if (ModelState.IsValid)
            {
                Object _object = new Object()
                {
                    Id = model.Id,
                    Name = model.Name,
                    Description = model.Description,
                    Url = model.Url
                };
                IdentityResult result = await _objects.UpdateAsync(_object, CancellationToken.None);
                if (result != null && result.Succeeded)
                {
                    return RedirectToAction("List");
                }
                AddErrors(result);
            }
            return View(model);
        }

        [Menu(Relation = "Edit")]
        [HttpGet]
        public async Task<IActionResult> _EditPartial(string objectId)
        {
            Object _object = await _objects.FindByIdAsync(objectId, CancellationToken.None);
            if (_object == null)
            {
                return PartialView();
            }

            ObjectViewModel objectView = new ObjectViewModel()
            {
                Id = _object.Id,
                Name = _object.Name,
                Description = _object.Description,
                Url = _object.Url
            };


            return PartialView(objectView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _EditPartial(ObjectViewModel model)
        {
            if (ModelState.IsValid)
            {
                Object _object = new Object()
                {
                    Id = model.Id,
                    Name = model.Name,
                    Description = model.Description,
                    Url = model.Url
                };
                IdentityResult result = await _objects.UpdateAsync(_object, CancellationToken.None);
                if (result != null && result.Succeeded)
                {
                    return RedirectToAction("_DetailsPartial", new { objectId = model.Id });
                }
                AddErrors(result);
            }
            return PartialView(model);
        }

        [Menu(Name = "Подробнее об объекте", Description = "Детальная информация об объекте", GroupName = "Object")]
        [HttpGet]
        public async Task<IActionResult> Details(string objectId)
        {
            Object _object = await _objects.FindByIdAsync(objectId, CancellationToken.None);
            if (_object != null)
            {
                ObjectViewModel objectView = new ObjectViewModel()
                {
                    Id = _object.Id,
                    Name = _object.Name,
                    Description = _object.Description,
                    Url = _object.Url
                };
                ClientCustom client = await _clients.FindByObjectIdAsync(objectId, CancellationToken.None);
                if (client != null)
                {
                    objectView.ClientId = client.Id;
                }

                IQueryable objectEndpoints = await _objectEndpoints.FindObjectEndpointsByObjectIdAsync(objectId, CancellationToken.None);
                List<ObjectEndpoint> objectEndpointsView = new List<ObjectEndpoint>();

                foreach (ObjectEndpoint objectEndpoint in objectEndpoints)
                {
                    objectEndpointsView.Add(new ObjectEndpoint { Id = objectEndpoint.Id, Value = objectEndpoint.Value, ObjectId = objectId });
                }
                ViewBag.objectEndpointsView = objectEndpointsView;
                ViewData["ObjectId"] = objectId;
                return View(objectView);
            }
            return RedirectToAction("List");
        }

        [Menu(Relation = "Details")]
        [HttpGet]
        public async Task<IActionResult> _DetailsPartial(string objectId)
        {
            Object _object = await _objects.FindByIdAsync(objectId, CancellationToken.None);
            if (_object != null)
            {
                ObjectViewModel objectView = new ObjectViewModel()
                {
                    Id = _object.Id,
                    Name = _object.Name,
                    Description = _object.Description,
                    Url = _object.Url
                };
                return PartialView(objectView);
            }
            return PartialView();
        }

        [Menu(Relation = "Details")]
        public async Task<IActionResult> _ObjectEndpointsListPartial(string objectId)
        {
            IQueryable objectEndpoints = await _objectEndpoints.FindObjectEndpointsByObjectIdAsync(objectId, CancellationToken.None);
            List<ObjectEndpoint> objectEndpointsView = new List<ObjectEndpoint>();

            foreach (ObjectEndpoint objectEndpoint in objectEndpoints)
            {
                objectEndpointsView.Add(new ObjectEndpoint { Id = objectEndpoint.Id, Value = objectEndpoint.Value, ObjectId = objectId });
            }
            ViewBag.objectEndpointsView = objectEndpointsView;
            ViewData["ObjectId"] = objectId;
            return PartialView();
        }

        [Menu(Name = "Удаление объекта", Description = "Удаление объекта", GroupName = "Object")]
        [HttpGet]
        public async Task<IActionResult> Delete(string objectId)
        {
            MySql.AspNet.Identity.Object _object = new MySql.AspNet.Identity.Object { Id = objectId };

            IdentityResult result = await _objects.DeleteAsync(_object, CancellationToken.None);
            if (result.Succeeded)
            {
                return Json(true);
            }
            AddErrors(result);

            return Json(false);
        }

        [Menu(Relation = "Details")]
        [HttpGet]
        public async Task<IActionResult> ObjectEndpointList(string objectId)
        {
            if (string.IsNullOrEmpty(objectId))
            {
                return RedirectToAction("ObjectList");
            }

            ViewData["ObjectId"] = objectId;

            IQueryable objectEndpoints = await _objectEndpoints.FindObjectEndpointsByObjectIdAsync(objectId, CancellationToken.None);
            List<ObjectEndpoint> objectEndpointsView = new List<ObjectEndpoint>();

            foreach (ObjectEndpoint objectEndpoint in objectEndpoints)
            {
                objectEndpointsView.Add(new ObjectEndpoint { Id = objectEndpoint.Id, Value = objectEndpoint.Value, ObjectId = objectId });
            }

            return View(objectEndpointsView);
        }

        [Menu(Relation = "Details")]
        [HttpGet]
        public async Task<JsonResult> ObjectEndpointData(string order, int limit, int offset, string sort, string search)
        {
            IQueryable<ObjectEndpoint> rows = _objectEndpoints.ObjectEndpoints;
            int total = await _objectEndpoints.GetObjectEndpointQueryFormatterCountAsync(search);
            return Json(new
            {
                total = total,
                rows = rows.ToList()
            });
        }

        [Menu(Relation = "Edit")]
        [HttpGet]
        public async Task<IActionResult> _ObjectEndpointEditPartial(string objectEndpointId)
        {

            if (string.IsNullOrEmpty(objectEndpointId))
            {
                return RedirectToAction("List");
            }

            ObjectEndpoint objectEndpoint = await _objectEndpoints.FindByIdAsync(objectEndpointId, CancellationToken.None);

            IQueryable<MySql.AspNet.Identity.Object> objectList = _objects.Objects;
            ViewData["Objects"] = objectList.ToList();

            return PartialView(objectEndpoint);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _ObjectEndpointEditPartial(ObjectEndpoint model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _objectEndpoints.UpdateAsync(model, CancellationToken.None);
                if (result != null && result.Succeeded)
                {
                    AddErrors(result);
                    return RedirectToAction("_ObjectEndpointsListPartial", new { objectId = model.ObjectId });
                }
            }

            return PartialView(model);
        }

        [Menu(Relation = "Details")]
        [HttpGet]
        public async Task<IActionResult> _ObjectEndpointPermissionListPartial(string objectEndpointId, string objectId)
        {
            if (string.IsNullOrEmpty(objectEndpointId) || string.IsNullOrEmpty(objectId))
            {
                return Json(false);
            }

            IQueryable permissions = _permissions.Permissions;

            List<PermissionViewModel> permissionView = new List<PermissionViewModel>();


            ViewData["ObjectEndpointId"] = objectEndpointId;
            ViewData["ObjectId"] = objectId;
            foreach (Permission permission in permissions)
            {
                permissionView.Add(new PermissionViewModel { Id = permission.Id, Name = permission.Name });
            }

            IQueryable<Permission> permissionsByObjectEndpoint = await _objectEndpoints.FindEndpointPermissionsIdByObjectAsync(objectEndpointId, CancellationToken.None);
            ViewData["PermissionsByObjectEndpoint"] = permissionsByObjectEndpoint.ToList();

            return PartialView(permissionView);
        }

        [HttpPost]
        public async Task<IActionResult> _ObjectEndpointPermissionListPartial(ObjectEndpointPermitionViewModel model, string objectId)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _objectEndpoints.UpdatePermissionAsync(model.ObjectEndpointId, model.SelectedPermissions, CancellationToken.None);
                if (result != null && result.Succeeded)
                {
                    return RedirectToAction("_ObjectEndpointsListPartial", new { objectId = objectId });
                }
                AddErrors(result);
            }
            return PartialView(model);
        }

        [Menu(Relation = "Details")]
        [HttpGet]
        public async Task<IActionResult> ObjectEndpointPermissionList(string objectEndpointId, string objectId)
        {
            if (string.IsNullOrEmpty(objectEndpointId) || string.IsNullOrEmpty(objectId))
            {
                return View("List");
            }

            IQueryable permissions = _permissions.Permissions;

            List<PermissionViewModel> permissionView = new List<PermissionViewModel>();


            ViewData["ObjectEndpointId"] = objectEndpointId;
            ViewData["ObjectId"] = objectId;
            foreach (Permission permission in permissions)
            {
                permissionView.Add(new PermissionViewModel { Id = permission.Id, Name = permission.Name });
            }

            IQueryable<Permission> permissionsByObjectEndpoint = await _objectEndpoints.FindEndpointPermissionsIdByObjectAsync(objectEndpointId, CancellationToken.None);
            ViewData["PermissionsByObjectEndpoint"] = permissionsByObjectEndpoint.ToList();

            return View(permissionView);
        }

        [HttpPost]
        public async Task<IActionResult> ObjectEndpointPermissionList(ObjectEndpointPermitionViewModel model)
        {
            if (ModelState.IsValid)
            {
                IQueryable permissions = _permissions.Permissions;
                foreach (Permission permission in permissions)
                {
                    await _objectEndpoints.DeletePermissionAsync(model.ObjectEndpointId, CancellationToken.None);
                }

                IdentityResult result = await _objectEndpoints.UpdatePermissionAsync(model.ObjectEndpointId, model.SelectedPermissions, CancellationToken.None);
                if (result == null || !result.Succeeded)
                {
                    return RedirectToAction("List");
                }
                AddErrors(result);
            }
            return View(model);

        }

        [Menu(Relation = "Details")]
        public async Task<IActionResult> _SubjectListPartial(string objectId, string objectEndpointId)
        {
            //TODO: переделать
            ViewData["ObjectId"] = objectId;
            ViewData["ObjectEndpointId"] = objectEndpointId;

            IQueryable roles = await _roles.FindRolesByObjectEndpointIdAsync(objectEndpointId, CancellationToken.None);
            List<Role> RolesByObject = new List<Role>();

            foreach (Role role in roles)
            {
                RolesByObject.Add(new Role { Id = role.Id, Name = role.Name });
            }

            ViewData["RolesByObject"] = RolesByObject.ToList();

            IQueryable users = await _users.FindUsersByObjectEndpointIdAsync(objectEndpointId, CancellationToken.None);
            List<User> usersByObject = new List<User>();

            foreach (User user in users)
            {
                usersByObject.Add(new User { Id = user.Id, UserName = user.UserName });
            }

            ViewData["UsersByObject"] = usersByObject.ToList();

            IQueryable groups = await _groups.FindGroupsByObjectEndpointIdAsync(objectEndpointId, CancellationToken.None);
            List<Group> groupsByObject = new List<Group>();

            foreach (Group group in groups)
            {
                groupsByObject.Add(new Group { Id = group.Id, Name = group.Name });
            }

            ViewData["GroupsByObject"] = groupsByObject.ToList();

            return PartialView();
        }

        [Menu(Relation = "Edit")]
        [HttpGet]
        public async Task<IActionResult> ChangeStatus(string objectId, bool status)
        {
            ClientCustom clientCustom = await _clients.FindByObjectIdAsync(objectId, CancellationToken.None);
            
            if (clientCustom != null)
            {
                clientCustom.Enabled = status;
                IdentityResult result = await _clients.UpdateAsync(clientCustom, CancellationToken.None);
                if (result.Succeeded)
                {
                    return Json(true);
                }
                AddErrors(result);
            }

            return Json(false);
        }

        #region Endpoint
        [Menu(Relation = "Edit")]
        [HttpGet]
        public IActionResult AddEndpoint(string objectId)
        {
            ViewData["ObjectId"] = objectId;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddEndpoint(ObjectEndpointViewModel model)
        {
            if (ModelState.IsValid)
            {
                string ModifyUrl = model.Value;
                Object _object = await _objects.FindByIdAsync(model.ObjectId, CancellationToken.None);
                ModifyUrl = "/" + ModifyUrl.Replace(_object.Url, "").Trim('/');

                ObjectEndpoint objectEndpoint = new ObjectEndpoint();
                objectEndpoint.Value = ModifyUrl;
                objectEndpoint.ObjectId = model.ObjectId;
                objectEndpoint.Description = model.Description;

                IdentityResult result = await _objectEndpoints.CreateAsync(objectEndpoint, CancellationToken.None);
                if (result.Succeeded)
                {
                    return RedirectToAction("ObjectEndpointList", new { objectId = model.ObjectId });
                }
                AddErrors(result);
            }

            ViewData["ObjectId"] = model.ObjectId;
            return View(model);
        }

        [Menu(Relation = "Edit")]
        [HttpGet]
        public IActionResult _AddEndpointPartial(string objectId)
        {
            return PartialView(new ObjectEndpoint() { ObjectId = objectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> _AddEndpointPartial(ObjectEndpoint model)
        {
            if (ModelState.IsValid)
            {
                Object _object = await _objects.FindByIdAsync(model.ObjectId, CancellationToken.None);
                model.Value = "/" + model.Value.Replace(_object.Url, "").Trim('/');

                IdentityResult result = await _objectEndpoints.CreateAsync(model, CancellationToken.None);
                if (result.Succeeded)
                {
                    return Json(result.Succeeded);
                }
            }

            return PartialView(model);
        }
        #endregion Endpoint
    }
}
