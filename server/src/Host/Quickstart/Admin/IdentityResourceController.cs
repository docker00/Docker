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
    [Menu(Name = "Ресурсы идентификации", Description = "Управление ресурсами идентификации", GroupName = "IdentityResource", ShowInMenu = true, Weight = 6)]
    public class IdentityResourceController : Controller
    {
        private readonly IdentityResourceCustomStore _identityResources;
        private readonly ClaimCustomStore _claimCustomStore;

        public IdentityResourceController(
            IdentityResourceCustomStore identityResources = null,
            ClaimCustomStore claimCustomStore = null
            )
        {
            _identityResources = identityResources ?? new IdentityResourceCustomStore(Host.Startup.ConnectionString);
            _claimCustomStore = claimCustomStore ?? new ClaimCustomStore(Host.Startup.ConnectionString);
        }

        public IActionResult Index()
        {
            return View();
        }

        [Menu(Name = "Список ресурсов", Description = "Список ресурсов идентификации", GroupName = "IdentityResource", ShowInMenu = true, Weight = 1)]
        public IActionResult List()
        {
            return View();
        }

        [Menu(Relation = "List")]
        public JsonResult Data(string order, int limit, int offset, string sort, string search)
        {
            IQueryable identityResources = _identityResources.IdentityResourceCustoms;
            List<IdentityResourceCustom> identityResourceView = new List<IdentityResourceCustom>();

            foreach (IdentityResourceCustom identityResource in identityResources)
            {
                identityResourceView.Add(new IdentityResourceCustom()
                {
                    Id = identityResource.Id,
                    Name = identityResource.Name,
                    DisplayName = identityResource.DisplayName,
                    Description = identityResource.Description,
                    Enabled = identityResource.Enabled,
                    Required = identityResource.Required,
                    Emphasize = identityResource.Emphasize,
                    ShowInDiscoveryDocument = identityResource.ShowInDiscoveryDocument
                });
            }

            return Json(identityResourceView);
        }

        [Menu(Name = "Добавить ресурс", Description = "Добавить ресурс идентификации", GroupName = "IdentityResource", ShowInMenu = true, Weight = 0)]
        [HttpGet]
        public IActionResult Add(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new IdentityResourceInputModel());
        }

        [HttpPost]
        public async Task<IActionResult> Add(IdentityResourceInputModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityResourceCustom checkContain = await _identityResources.FindByNameAsync(model.Name, CancellationToken.None);

                if (checkContain == null)
                {
                    IdentityResourceCustom identityResourceCustom = new IdentityResourceCustom()
                    {
                        Name = model.Name,
                        DisplayName = model.DisplayName,
                        Description = model.Description,
                        Enabled = model.Enabled,
                        Required = model.Required,
                        Emphasize = model.Emphasize,
                        ShowInDiscoveryDocument = model.ShowInDiscoveryDocument
                    };
                    IdentityResult result = await _identityResources.CreateAsync(identityResourceCustom, CancellationToken.None);

                    if (result != null && result.Succeeded)
                    {
                        return RedirectToAction("Details", new { identityResourceId = identityResourceCustom.Id });
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

        [Menu(Relation = "Edit")]
        [HttpGet]
        public async Task<IActionResult> _AddClaimPartial(string identityResourceId)
        {
            IQueryable<ClaimCustom> claims = (await _identityResources.FindClaimsAsync(identityResourceId, CancellationToken.None)).AsQueryable();
            List<ClaimViewModel> claimView = new List<ClaimViewModel>();
            foreach (ClaimCustom claim in _claimCustomStore.ClaimCustoms)
            {
                if (claims.FirstOrDefault(c => c.Id.Contains(claim.Id)) == null)
                {
                    claimView.Add(new ClaimViewModel(claim.Id, claim.Type));
                }
            }
            ViewData["Claims"] = claimView;
            return PartialView(new IdentityResourceClaimInputModel(identityResourceId));
        }

        [HttpPost]
        public async Task<IActionResult> _AddClaimPartial(IdentityResourceCustomClaim model)
        {

            if (ModelState.IsValid)
            {
                IdentityResult result = await _identityResources.CreateClaimAsync(model.IdentityResourceId, model.ClaimId, CancellationToken.None);
                return Json(result.Succeeded);
            }

            IQueryable<ClaimCustom> claims = (await _identityResources.FindClaimsAsync(model.IdentityResourceId, CancellationToken.None)).AsQueryable();
            List<ClaimViewModel> claimView = new List<ClaimViewModel>();
            foreach (ClaimCustom claim in _claimCustomStore.ClaimCustoms)
            {
                if (claims.FirstOrDefault(c => c.Id.Contains(claim.Id)) == null)
                {
                    claimView.Add(new ClaimViewModel(claim.Id, claim.Type));
                }
            }
            ViewData["Claims"] = claimView;

            return PartialView(model);
        }

        [Menu(Name = "Редактирование ресурса идентификации", Description = "Редактирование ресурса идентификации", GroupName = "IdentityResource")]
        [HttpGet]
        public async Task<IActionResult> Edit(string identityResourceId)
        {
            if (string.IsNullOrEmpty(identityResourceId))
            {
                return RedirectToAction("List");
            }

            IdentityResourceCustom identityResourceCustom = await _identityResources.FindByIdAsync(identityResourceId, CancellationToken.None);

            IdentityResourceInputModel identityResourceInput = new IdentityResourceViewModel()
            {
                Id = identityResourceId,
                Name = identityResourceCustom.Name,
                DisplayName = identityResourceCustom.DisplayName,
                Description = identityResourceCustom.Description,
                Enabled = identityResourceCustom.Enabled,
                Required = identityResourceCustom.Required,
                Emphasize = identityResourceCustom.Emphasize,
                ShowInDiscoveryDocument = identityResourceCustom.ShowInDiscoveryDocument
            };
            return View(identityResourceInput);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(IdentityResourceInputModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityResourceCustom identityResourceCustom = new IdentityResourceCustom()
                {
                    Id = model.Id,
                    Name = model.Name,
                    DisplayName = model.DisplayName,
                    Description = model.Description,
                    Enabled = model.Enabled,
                    Required = model.Required,
                    Emphasize = model.Emphasize,
                    ShowInDiscoveryDocument = model.ShowInDiscoveryDocument
                };
                IdentityResult result = await _identityResources.UpdateAsync(identityResourceCustom, CancellationToken.None);
                if (result != null || result.Succeeded)
                {
                    return RedirectToAction("Details", new { identityResourceId = model.Id });
                }
                AddErrors(result);
            }
            return View(model);

        }

        [Menu(Relation = "Edit")]
        [HttpGet]
        public async Task<IActionResult> _EditPartial(string identityResourceId)
        {
            if (string.IsNullOrEmpty(identityResourceId))
            {
                return RedirectToAction("List");
            }

            IdentityResourceCustom identityResourceCustom = await _identityResources.FindByIdAsync(identityResourceId, CancellationToken.None);

            IdentityResourceInputModel identityResourceInput = new IdentityResourceViewModel()
            {
                Id = identityResourceId,
                Name = identityResourceCustom.Name,
                DisplayName = identityResourceCustom.DisplayName,
                Description = identityResourceCustom.Description,
                Enabled = identityResourceCustom.Enabled,
                Required = identityResourceCustom.Required,
                Emphasize = identityResourceCustom.Emphasize,
                ShowInDiscoveryDocument = identityResourceCustom.ShowInDiscoveryDocument
            };
            return PartialView(identityResourceInput);
        }

        [HttpPost]
        public async Task<IActionResult> _EditPartial(IdentityResourceInputModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityResourceCustom identityResourceCustom = new IdentityResourceCustom()
                {
                    Id = model.Id,
                    Name = model.Name,
                    DisplayName = model.DisplayName,
                    Description = model.Description,
                    Enabled = model.Enabled,
                    Required = model.Required,
                    Emphasize = model.Emphasize,
                    ShowInDiscoveryDocument = model.ShowInDiscoveryDocument
                };
                IdentityResult result = await _identityResources.UpdateAsync(identityResourceCustom, CancellationToken.None);
                if (result != null || result.Succeeded)
                {
                    return RedirectToAction("_DetailsPartial", new { identityResourceId = model.Id });
                }
                AddErrors(result);
            }
            return PartialView(model);
        }

        [Menu(Name = "Подробнее об ресурсе", Description = "Детальная информация ресурса", GroupName = "IdentityResource")]
        [HttpGet]
        public async Task<IActionResult> Details(string identityResourceId)
        {
            IdentityResourceCustom resource = await _identityResources.FindByIdAsync(identityResourceId, CancellationToken.None);
            if (resource != null)
            {
                IdentityResourceViewModel resourceView = new IdentityResourceViewModel()
                {
                    Id = resource.Id,
                    Name = resource.Name,
                    DisplayName = resource.DisplayName,
                    Description = resource.Description,
                    Required = resource.Required,
                    Emphasize = resource.Emphasize,
                    ShowInDiscoveryDocument = resource.ShowInDiscoveryDocument,
                    Enabled = resource.Enabled
                };
                IQueryable<ClaimCustom> claims = await _identityResources.FindClaimsAsync(identityResourceId, CancellationToken.None);
                foreach (ClaimCustom claim in claims)
                {
                    resourceView.Claims.Add(new ClaimViewModel()
                    {
                        Id = claim.Id,
                        Type = claim.Type
                    });
                }
                GetResourseViewData();
                return View(resourceView);
            }
            return RedirectToAction("List");
        }

        [Menu(Relation = "Details")]
        [HttpGet]
        public async Task<IActionResult> _DetailsPartial(string identityResourceId)
        {
            IdentityResourceCustom resource = await _identityResources.FindByIdAsync(identityResourceId, CancellationToken.None);
            if (resource != null)
            {
                IdentityResourceViewModel resourceView = new IdentityResourceViewModel()
                {
                    Id = resource.Id,
                    Name = resource.Name,
                    DisplayName = resource.DisplayName,
                    Description = resource.Description,
                    Required = resource.Required,
                    Emphasize = resource.Emphasize,
                    ShowInDiscoveryDocument = resource.ShowInDiscoveryDocument,
                    Enabled = resource.Enabled
                };
                return PartialView(resourceView);
            }
            return RedirectToAction("List");
        }

        [Menu(Name = "Удаление ресурса идентификации", Description = "Удаление ресурса идентификации", GroupName = "IdentityResource")]
        [HttpGet]
        public async Task<IActionResult> Delete(string identityResourceId)
        {
            IdentityResourceCustom resource = await _identityResources.FindByIdAsync(identityResourceId, CancellationToken.None);
            if (resource != null)
            {
                IdentityResult result = await _identityResources.DeleteAsync(resource, CancellationToken.None);
                return Json(result.Succeeded);
            }
            return Json(false);
        }

        [Menu(Relation = "Edit")]
        public async Task<IActionResult> DeleteClaim(string claimId, string identityResourceId)
        {
            IdentityResourceCustomClaim apiResourceCustomClaim = new IdentityResourceCustomClaim() { ClaimId = claimId, IdentityResourceId = identityResourceId };

            IdentityResult result = await _identityResources.DeleteClaimAsync(apiResourceCustomClaim, CancellationToken.None);
            return Json(result.Succeeded);
        }

        private void GetResourseViewData()
        {
            List<ClaimViewModel> claimsView = new List<ClaimViewModel>();
            foreach (ClaimCustom claim in _claimCustomStore.ClaimCustoms)
            {
                claimsView.Add(new ClaimViewModel(claim.Id, claim.Type));
            }
            ViewBag.Claims = claimsView;
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
