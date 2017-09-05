using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MySql.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using Host.Extensions;

namespace IdentityServer4.Quickstart.UI
{
    [Menu(GroupName = "IdentityResource", ShowInMenu = true)]
    public class ClaimController : Controller
    {
        private readonly ClaimCustomStore _claims;

        public ClaimController(ClaimCustomStore claims = null)
        {
            _claims = claims ?? new ClaimCustomStore(Host.Startup.ConnectionString);
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

        [Menu(Name = "Список требований", Description = "Список требований", GroupName = "IdentityResource", ShowInMenu = true, Weight = 3)]
        public IActionResult List()
        {
            return View();
        }

        [Menu(Relation = "List")]
        public JsonResult Data(string order, int limit, int offset, string sort, string search)
        {
            IQueryable claims = _claims.ClaimCustoms;
            List<ClaimCustom> claimView = new List<ClaimCustom>();

            foreach (ClaimCustom claim in claims)
            {
                claimView.Add(new ClaimCustom(claim.Type, " ") { Id = claim.Id });
            }

            return Json(claimView);
        }

        [Menu(Name = "Добавление требования", Description = "Добавить требование", GroupName = "IdentityResource", ShowInMenu = true, Weight = 2)]
        [HttpGet]
        public IActionResult Add(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ClaimViewModel model)
        {

            if (ModelState.IsValid)
            {
                ClaimCustom _claim = new ClaimCustom(model.Type, " ");
                IdentityResult result = await _claims.CreateAsync(_claim, CancellationToken.None);
                if (result.Succeeded)
                {
                    return RedirectToAction("Details", new { claimId = _claim.Id });
                }
                AddErrors(result);
            }
            return View(model);
        }

        [Menu(Name = "Подробно", Description = "Детальная информация по требованию", GroupName = "IdentityResource")]
        [HttpGet]
        public async Task<IActionResult> Details(string claimId)
        {
            ClaimCustom claim = await _claims.FindByIdAsync(claimId, CancellationToken.None);
            if (claim != null)
            {
                ClaimCustom claimView = new ClaimCustom(claim.Type, " ") { Id = claim.Id };
                return View(claimView);
            }
            return RedirectToAction("List");
        }

        [Menu(Name = "Редактирование требования", Description = "Редактирование требования", GroupName = "IdentityResource")]
        [HttpGet]
        public async Task<IActionResult> Edit(string claimId, string claimType)
        {
            if (string.IsNullOrEmpty(claimId))
            {
                return RedirectToAction("List");
            }

            if (string.IsNullOrEmpty(claimType))
            {
                ClaimCustom claimNameById = await _claims.FindByIdAsync(claimId, CancellationToken.None);
                claimType = claimNameById.Type;
            }

            ClaimViewModel claim = new ClaimViewModel() { Id = claimId, Type = claimType };
            return View(claim);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ClaimViewModel model)
        {
            if (ModelState.IsValid)
            {
                ClaimCustom _claim = new ClaimCustom(model.Type, " ") { Id = model.Id };
                IdentityResult result = await _claims.UpdateAsync(_claim, CancellationToken.None);
                if (result != null && result.Succeeded)
                {
                    return RedirectToAction("Details", new { claimId = model.Id });
                }
                AddErrors(result);
            }
            return View(model);
        }

        [Menu(Name = "Удаление требования", Description = "Удаление требования", GroupName = "IdentityResource")]
        [HttpPost]
        public async Task<IActionResult> Delete(ClaimViewModel model)
        {
            if (ModelState.IsValid)
            {
                ClaimCustom _claim = new ClaimCustom(model.Type, " ") { Id = model.Id };
                IdentityResult result = await _claims.DeleteAsync(_claim, CancellationToken.None);
                if (result == null || !result.Succeeded)
                {
                    return View(model);
                }
                AddErrors(result);
            }

            return RedirectToAction("List");
        }
    }
}
