using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MySql.AspNet.Identity;
using IdentityServer4.Quickstart.UI;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using Host.Extensions;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Host.Quickstart.Admin
{
    [Menu(Name = "Разрешения", Description = "Управление разрешениями", GroupName = "Permission", ShowInMenu = true, Weight = 3)]
    public class PermissionController : Controller
    {
        private readonly PermissionStore _permissions;

        public PermissionController(
            PermissionStore permissions = null
            )
        {
            _permissions = permissions ?? new PermissionStore(Host.Startup.ConnectionString);
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

        [Menu(Name = "Список разрешений", Description = "Список разрешений", GroupName = "Permission", ShowInMenu = true, Weight = 1)]
        public IActionResult List()
        {
            return View();
        }

        [Menu(Relation = "List")]
        public JsonResult Data(string order, int limit, int offset, string sort, string search)
        {
            IQueryable permissions = _permissions.Permissions;
            List<PermissionViewModel> permissionView = new List<PermissionViewModel>();

            foreach (Permission permission in permissions)
            {
                permissionView.Add(new PermissionViewModel { Id = permission.Id, Name = permission.Name });
            }

            return Json(permissionView);
        }

        [Menu(Name = "Добавить разрешение", Description = "Добавить разрешение", GroupName = "Permission", ShowInMenu = true, Weight = 0)]
        [HttpGet]
        public IActionResult Add(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(PermissionViewModel model)
        {
            if (ModelState.IsValid)
            {
                Permission permissionByName = await _permissions.FindByNameAsync(model.Name, CancellationToken.None);
                if (permissionByName == null)
                {
                    Permission permission = new Permission() { Name = model.Name };
                    IdentityResult result = await _permissions.CreateAsync(permission, CancellationToken.None);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("List");
                    }
                    AddErrors(result);
                }
                else
                {
                    ModelState.AddModelError("Name", "Такое название уже существует");
                }
            }

            IQueryable getpermissionView = _permissions.Permissions;

            List<PermissionViewModel> permissionView = new List<PermissionViewModel>();

            foreach (Permission _permission in getpermissionView)
            {
                permissionView.Add(new PermissionViewModel { Id = _permission.Id, Name = _permission.Name });
            }

            ViewData["permissionView"] = permissionView;

            return View(model);
        }

        [Menu(Name = "Редактирование разрешения", Description = "Редактирование разрешения", GroupName = "Permission")]
        [HttpGet]
        public async Task<IActionResult> Edit(string permissionId, string permissionName)
        {

            if (string.IsNullOrEmpty(permissionId))
            {
                return RedirectToAction("List");
            }


            if (string.IsNullOrEmpty(permissionName))
            {
                Permission permissionNameById = await _permissions.FindByIdAsync( permissionId, CancellationToken.None);
                permissionName = permissionNameById.Name;
            }

            PermissionViewModel permission = new PermissionViewModel() { Id = permissionId, Name = permissionName };

            return View(permission);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PermissionViewModel model)
        {
            if (ModelState.IsValid)
            {
                Permission permission = new Permission() { Id = model.Id, Name = model.Name };
                IdentityResult result = await _permissions.UpdateAsync(permission, CancellationToken.None);
                if (result != null && result.Succeeded)
                {
                    AddErrors(result);
                    return RedirectToAction("List");
                }
            }

            IQueryable getpermissionView = _permissions.Permissions;

            List<PermissionViewModel> permissionView = new List<PermissionViewModel>();

            foreach (Permission _permission in getpermissionView)
            {
                permissionView.Add(new PermissionViewModel { Id = _permission.Id, Name = _permission.Name });
            }

            ViewData["permissionView"] = permissionView;

            return View(model);
        }

        [Menu(Name = "Удаление разрешения", Description = "Удаление разрешения", GroupName = "Permission")]
        [HttpPost]
        public async Task<IActionResult> Delete(Permission model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _permissions.DeleteAsync(model, CancellationToken.None);
                if (result == null || !result.Succeeded)
                {
                    AddErrors(result);
                    return View(model);
                }
            }

            return RedirectToAction("List");
        }
    }
}
