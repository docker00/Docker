using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MySql.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using IdentityServer4.Quickstart.UI;
using Host.Extensions;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Host.Quickstart.Admin
{
    [Menu(GroupName = "IdentityResource", ShowInMenu = true)]
    public class AllowedGrantTypesController : Controller
    {
        private readonly AllowedGrantTypeStore _allowedGrantTypes;

        public AllowedGrantTypesController(
            AllowedGrantTypeStore allowedGrantTypes = null
            )
        {
            _allowedGrantTypes = allowedGrantTypes ?? new AllowedGrantTypeStore(Host.Startup.ConnectionString);
        }

        public IActionResult Index()
        {
            return View();
        }

        [Menu(Name = "Список способов взаимодействия", Description = "Список способов взаимодействия", GroupName = "IdentityResource", ShowInMenu = true, Weight = 5)]
        public IActionResult List()
        {
            return View();
        }

        [Menu(Relation = "List")]
        public JsonResult Data(string order, int limit, int offset, string sort, string search)
        {
            IQueryable allowedGrantTypes = _allowedGrantTypes.AllowedGrantTypes;
            List<AllowedGrantType> allowedGrantTypesView = new List<AllowedGrantType>();

            foreach (AllowedGrantType allowedGrantType in allowedGrantTypes)
            {
                allowedGrantTypesView.Add(new AllowedGrantType() { Id = allowedGrantType.Id, Name = allowedGrantType.Name });
            }

            return Json(allowedGrantTypesView);
        }

        [Menu(Name = "Добавить способ взаимодействия", Description = "Добавить способ взаимодействия", GroupName = "IdentityResource", ShowInMenu = true, Weight = 4)]
        [HttpGet]
        public IActionResult Add(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AllowedGrantTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                AllowedGrantType allowedCorsOrigin = new AllowedGrantType() { Name = model.Name };
                IdentityResult result = await _allowedGrantTypes.CreateAsync(allowedCorsOrigin, CancellationToken.None);
                if (result != null && result.Succeeded)
                {
                    AddErrors(result);
                    return RedirectToAction("List");
                }
            }

            return View(model);
        }

        [Menu(Name = "Редактирование способа взаимодействия", Description = "Редактирование способа взаимодействия", GroupName = "IdentityResource")]
        [HttpGet]
        public async Task<IActionResult> Edit(string allowedGrantTypeId, string allowedGrantTypeName)
        {
            if (string.IsNullOrEmpty(allowedGrantTypeId))
            {
                return RedirectToAction("List");
            }

            if (string.IsNullOrEmpty(allowedGrantTypeName))
            {
                AllowedGrantType allowedGrantTypeNameById = await _allowedGrantTypes.FindByIdAsync(allowedGrantTypeId, CancellationToken.None);
                allowedGrantTypeName = allowedGrantTypeNameById.Name;
            }

            AllowedGrantTypeViewModel allowedGrantType = new AllowedGrantTypeViewModel() { Id = allowedGrantTypeId, Name = allowedGrantTypeName };
            return View(allowedGrantType);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AllowedGrantTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                AllowedGrantType allowedCorsOrigin = new AllowedGrantType() { Id = model.Id, Name = model.Name };
                IdentityResult result = await _allowedGrantTypes.UpdateAsync(allowedCorsOrigin, CancellationToken.None);
                if (result != null && result.Succeeded)
                {
                    AddErrors(result);
                    return RedirectToAction("List");
                }
            }

            return View(model);
        }

        [Menu(Name = "Удаление способа взаимодействия", Description = "Удаление способа взаимодействия", GroupName = "IdentityResource")]
        [HttpPost]
        public async Task<IActionResult> Delete(AllowedGrantType model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _allowedGrantTypes.DeleteAsync(model, CancellationToken.None);
                if (result == null || !result.Succeeded)
                {
                    AddErrors(result);
                    return View(model);
                }
            }

            return RedirectToAction("List");
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
