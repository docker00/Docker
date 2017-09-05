using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MySql.AspNet.Identity;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using Host.Extensions;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace IdentityServer4.Quickstart.UI
{
    [Menu(GroupName = "User", ShowInMenu = true)]
    public class ProfileAttributeController : Controller
    {
        private readonly ProfileAttributeStore<ProfileAttribute> _profileAttributes;
        private readonly ClaimCustomStore _claims;
        private readonly UserStore<User> _users;

        public ProfileAttributeController(
            ProfileAttributeStore<ProfileAttribute> profileAttributes = null,
            ClaimCustomStore claims = null,
            UserStore<User> users = null
            )
        {
            _profileAttributes = profileAttributes ?? new ProfileAttributeStore<ProfileAttribute>(Host.Startup.ConnectionString);
            _claims = claims ?? new ClaimCustomStore(Host.Startup.ConnectionString);
            _users = users ?? new UserStore<User>(Host.Startup.ConnectionString);
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

        [Menu(Name = "Атрибуты профилей", Description = "Атрибуты профилей", GroupName = "User", ShowInMenu = true, Weight = 3)]
        public IActionResult List()
        {
            return View();
        }

        [Menu(Relation = "List")]
        public JsonResult Data(string order, int limit, int offset, string sort, string search)
        {
            IQueryable profileAttributes = _profileAttributes.ProfileAttributes;
            List<ProfileAttribute> profileAttributeView = new List<ProfileAttribute>();

            foreach (ProfileAttribute profileAttribute in profileAttributes)
            {
                profileAttributeView.Add(new ProfileAttribute()
                {
                    Id = profileAttribute.Id,
                    Name = profileAttribute.Name,
                    RequiredRegister = profileAttribute.RequiredRegister,
                    RequiredAdditional = profileAttribute.RequiredAdditional,
                    Disabled = profileAttribute.Disabled,
                    Weight = profileAttribute.Weight,
                    Deleted = profileAttribute.Deleted
                });
            }

            return Json(profileAttributeView);
        }

        [Menu(Name = "Добавить атрибут", Description = "Добавление атрибута", GroupName = "User")]
        [HttpGet]
        public IActionResult Add(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Claims"] = _claims.ClaimCustoms.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(ProfileAttributesInputModel model)
        {
            if (ModelState.IsValid)
            {
                ProfileAttribute profileAttributeByName = await _profileAttributes.FindByNameAsync(model.Name, CancellationToken.None);
                if (profileAttributeByName == null)
                {
                    ProfileAttribute profileAttribute = new ProfileAttribute()
                    {
                        Id = model.Id,
                        Name = model.Name,
                        RequiredRegister = model.RequiredRegister,
                        RequiredAdditional = model.RequiredAdditional,
                        Disabled = model.Disabled,
                        Weight = model.Weight,
                        Deleted = model.Deleted
                    };
                    IdentityResult result = await _profileAttributes.CreateAsync(profileAttribute, CancellationToken.None);
                    if (result.Succeeded)
                    {
                        result = await _profileAttributes.CreateClaimAsync(profileAttribute.Id, model.ClaimId, CancellationToken.None);
                        if (model.RequiredRegister)
                        {
                            result = await _users.UpdateAttributesValidatedAsync(null, false, CancellationToken.None);
                        }
                        return RedirectToAction("List");
                    }
                    AddErrors(result);
                }
                else
                {
                    ModelState.AddModelError("Name", "Такие атрибуты профиля уже существуют");
                }
            }
            ViewData["Claims"] = _claims.ClaimCustoms.ToList();
            return View(model);
        }

        [Menu(Name = "Удалить атрибут", Description = "Удаление атрибута", GroupName = "User")]
        [HttpPost]
        public async Task<IActionResult> Delete(ProfileAttribute model)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _profileAttributes.DeleteAsync(model, CancellationToken.None);
                if (result == null || !result.Succeeded)
                {
                    AddErrors(result);
                    return View(model);
                }
            }

            return RedirectToAction("List");
        }

        [Menu(Name = "Редактировать атрибут", Description = "Редактирование атрибута", GroupName = "User")]
        [HttpGet]
        public async Task<IActionResult> Edit(string profileAttributeId)
        {
            if (string.IsNullOrEmpty(profileAttributeId))
            {
                return RedirectToAction("List");
            }

            ProfileAttribute profileAttribute = await _profileAttributes.FindByIdAsync(profileAttributeId, CancellationToken.None);
            ProfileAttributeClaim profileAttributeClaim = await _profileAttributes.GetClaimAsync(profileAttributeId, CancellationToken.None);
            ProfileAttributesInputModel profileAttributeView = new ProfileAttributesInputModel()
            {
                Id = profileAttribute.Id,
                Name = profileAttribute.Name,
                RequiredRegister = profileAttribute.RequiredRegister,
                RequiredAdditional = profileAttribute.RequiredAdditional,
                Disabled = profileAttribute.Disabled,
                Weight = profileAttribute.Weight,
                Deleted = profileAttribute.Deleted,
                ClaimId = (profileAttributeClaim == null) ? "" : profileAttributeClaim.ClaimId,
                ClaimType = (profileAttributeClaim == null) ? "" : (await _claims.FindByIdAsync(profileAttributeClaim.ClaimId, CancellationToken.None)).Type
            };

            IQueryable<ProfileAttributeClaim> profileAttributeClaims = _profileAttributes.ProfileAttributesClaims;
            List<ClaimViewModel> claimView = new List<ClaimViewModel>();
            foreach (ClaimCustom claim in _claims.ClaimCustoms)
            {
                if (profileAttributeClaims.FirstOrDefault(c => c.ClaimId.Contains(claim.Id)) == null)
                {
                    claimView.Add(new ClaimViewModel(claim.Id, claim.Type));
                }
            }
            ViewData["Claims"] = claimView;
            return View(profileAttributeView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileAttributesInputModel model)
        {
            if (ModelState.IsValid)
            {
                ProfileAttribute profileAttribute = new ProfileAttribute()
                {
                    Id = model.Id,
                    Name = model.Name,
                    RequiredRegister = model.RequiredRegister,
                    RequiredAdditional = model.RequiredAdditional,
                    Disabled = model.Disabled,
                    Weight = model.Weight,
                    Deleted = model.Deleted
                };

                IdentityResult result = await _profileAttributes.UpdateAsync(profileAttribute, CancellationToken.None);
                if (result != null && result.Succeeded)
                {
                    ProfileAttributeClaim profileAttributeClaim = await _profileAttributes.GetClaimAsync(profileAttribute.Id, CancellationToken.None);
                    if (!string.IsNullOrEmpty(model.ClaimId))
                    {
                        if (profileAttributeClaim != null)
                        {
                            result = await _profileAttributes.UpdateClaimAsync(profileAttribute.Id, model.ClaimId, CancellationToken.None);
                        }
                        else
                        {
                            result = await _profileAttributes.CreateClaimAsync(profileAttribute.Id, model.ClaimId, CancellationToken.None);
                        }
                    }
                    else
                    {
                        if (profileAttributeClaim != null)
                        {
                            result = await _profileAttributes.DeleteClaimByProfileAttributeIdAsync(profileAttribute.Id, CancellationToken.None);
                        }
                    }

                    if (model.RequiredRegister)
                    {
                        result = await _users.UpdateAttributesValidatedAsync(null, false, CancellationToken.None);
                    }

                    return RedirectToAction("List");
                }
                AddErrors(result);
            }

            IQueryable<ProfileAttributeClaim> profileAttributeClaims = _profileAttributes.ProfileAttributesClaims;
            List<ClaimViewModel> claimView = new List<ClaimViewModel>();
            foreach (ClaimCustom claim in _claims.ClaimCustoms)
            {
                if (profileAttributeClaims.FirstOrDefault(c => c.ClaimId.Contains(claim.Id)) == null)
                {
                    claimView.Add(new ClaimViewModel(claim.Id, claim.Type));
                }
            }
            ViewData["Claims"] = claimView;

            return View(model);
        }
    }
}
