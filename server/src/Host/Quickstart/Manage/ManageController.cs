using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySql.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.Extensions;
using System.Security.Claims;
using Host.Extensions;
using MySql.AspNet.Identity.Repositories;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;

namespace IdentityServer4.Quickstart.UI
{
    [Authorize]
    [SecurityHeaders]
    public class ManageController : Controller
    {
        private readonly UserStore<User> _users;
        private readonly ClaimCustomRepository _claimCustomRepository;
        private readonly ProfileAttributeStore<ProfileAttribute> _attributes;
        private readonly ClientCustomStore _clients;

        public ManageController(
          UserStore<User> users = null,
          ProfileAttributeStore<ProfileAttribute> attributes = null,
          ClientCustomStore clients = null
          )
        {
            _users = users ?? new UserStore<User>(Host.Startup.ConnectionString);
            _claimCustomRepository = new ClaimCustomRepository(Host.Startup.ConnectionString);
            _attributes = attributes ?? new ProfileAttributeStore<ProfileAttribute>(Host.Startup.ConnectionString);
            _clients = clients ?? new ClientCustomStore(Host.Startup.ConnectionString);
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            ClaimsPrincipal user = await HttpContext.GetIdentityServerUserAsync();
            string userId = user?.FindFirst("sub")?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Index", "Home");
            }
            User currentUser = await _users.FindByIdAsync(userId, CancellationToken.None);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            UserViewModel user_view = new UserViewModel()
            {
                Id = currentUser.Id,
                Email = currentUser.Email,
                UserName = currentUser.UserName,
                PhoneNumber = currentUser.PhoneNumber,
                Roles = currentUser.Roles,
                Activated = currentUser.Activated
            };
            IQueryable<Group> groups = await _users.GetGroupsAsync(currentUser, CancellationToken.None);
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

        [HttpGet]
        public async Task<IActionResult> _EditPartial(string userId)
        {
            User user = await _users.FindByIdAsync(userId, CancellationToken.None);
            UserEditModel userEdit = new UserEditModel() { Id = user.Id, Email = user.Email, UserName = user.UserName };
            return PartialView(userEdit);
        }

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

        #region PasswordEdit
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
            return PartialView(model);
        }

        #endregion

        #region ProfileAttributes
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
            return PartialView(userClaim);
        }

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

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
