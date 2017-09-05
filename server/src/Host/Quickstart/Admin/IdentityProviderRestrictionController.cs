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
    [Menu(GroupName = "IdentityResource", ShowInMenu = true)]
    public class IdentityProviderRestrictionController : Controller
    {
        private readonly IdentityProviderRestrictionStore _identityProviderRestrictions;

        public IdentityProviderRestrictionController(
            IdentityProviderRestrictionStore identityProviderRestrictions = null
            )
        {
            _identityProviderRestrictions = identityProviderRestrictions ?? new IdentityProviderRestrictionStore(Host.Startup.ConnectionString);
        }

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
        [Menu(Name = "Список ограничений", Description = "Список ограничений провайдера", GroupName = "IdentityResource", ShowInMenu = true, Weight = 7)]
        public IActionResult List()
        {
            return View();
        }

        [Menu(Relation = "List")]
        public JsonResult Data(string order, int limit, int offset, string sort, string search)
        {
            IQueryable identityProviderRestrictions = _identityProviderRestrictions.IdentityProviderRestrictions;
            List<IdentityProviderRestriction> identityProviderRestrictionView = new List<IdentityProviderRestriction>();

            foreach (IdentityProviderRestriction identityProviderRestriction in identityProviderRestrictions)
            {
                identityProviderRestrictionView.Add(new IdentityProviderRestriction { Id = identityProviderRestriction.Id, Name = identityProviderRestriction.Name });
            }

            return Json(identityProviderRestrictionView);
        }

        [Menu(Name = "Добавление ограничения", Description = "Добавление ограничения провайдера", GroupName = "IdentityResource", ShowInMenu = true, Weight = 6)]
        [HttpGet]
        public IActionResult Add(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(IdentityProviderRestriction model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _identityProviderRestrictions.CreateAsync(model, CancellationToken.None);
                if (result == null || !result.Succeeded)
                {
                    AddErrors(result);
                    return View(model);
                }
            }

            return RedirectToAction("List");
        }

        [Menu(Name = "Редактирование ограничения", Description = "Список ограничений провайдера")]
        [HttpGet]
        public IActionResult Edit(string identityProviderRestrictionId, string identityProviderRestrictionName)
        {
            if (string.IsNullOrEmpty(identityProviderRestrictionId))
            {
                return RedirectToAction("List");
            }

            IdentityProviderRestriction identityProviderRestriction = new IdentityProviderRestriction() { Id = identityProviderRestrictionId, Name = identityProviderRestrictionName };
            return View(identityProviderRestriction);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(IdentityProviderRestriction model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _identityProviderRestrictions.UpdateAsync(model, CancellationToken.None);
                if (result == null || !result.Succeeded)
                {
                    AddErrors(result);
                    return View(model);
                }
            }

            return RedirectToAction("List");
        }

        [Menu(Name = "Удаление ограничения", Description = "Удаление ограничения провайдера")]
        [HttpGet]
        public IActionResult Delete(string identityProviderRestrictionId, string identityProviderRestrictionName)
        {
            if (string.IsNullOrEmpty(identityProviderRestrictionId))
            {
                return RedirectToAction("List");
            }

            IdentityProviderRestriction identityProviderRestriction = new IdentityProviderRestriction() { Id = identityProviderRestrictionId, Name = identityProviderRestrictionName };
            return View(identityProviderRestriction);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(IdentityProviderRestriction model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _identityProviderRestrictions.DeleteAsync(model, CancellationToken.None);
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
