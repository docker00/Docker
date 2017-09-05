using IdentityServer4.Quickstart.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySql.AspNet.Identity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityServer4.Quickstart
{
    [Route("api/[controller]/[action]")]
    public class UserApiController : Controller
    {
        private readonly UserStore<User> _users;
        private readonly ProfileAttributeStore<ProfileAttribute> _profileAttributes;

        public UserApiController(UserStore<User> users = null, ProfileAttributeStore<ProfileAttribute> profileAttributes = null)
        {
            _users = users ?? new UserStore<User>(Host.Startup.ConnectionString);
            _profileAttributes = profileAttributes ?? new ProfileAttributeStore<ProfileAttribute>(Host.Startup.ConnectionString);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            User user = await _users.FindByIdAsync(id, CancellationToken.None);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            return Json(user);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            IQueryable<ProfileAttributeClaim> profileAttributesClaims = _profileAttributes.ProfileAttributesClaims.Where(p => p.RequiredRegister).OrderBy(p => p.Weight);
            RegisterViewModel registerView = new RegisterViewModel() { };
            foreach (ProfileAttributeClaim profileAttributeClaim in profileAttributesClaims)
            {
                registerView.RegisterClaimsInputModel.Add(new RegisterClaimsInputModel()
                {
                    AttributeName = profileAttributeClaim.ProfileAttributeName,
                    ClaimId = profileAttributeClaim.ClaimId,
                    ClaimValue = profileAttributeClaim.ClaimValue
                });
            }
            return Json(registerView);
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Register([FromBody]RegisterViewModel model, string returnUrl = null)
        {
            User user = await _users.FindByEmailAsync(model.Email, CancellationToken.None);
            if (user != null)
            {
                ModelState.AddModelError("Email", "Пользователь с таким email уже существует");
            }
            //TODO: переделать проверку после показа
            for (int i = 0; i < model.RegisterClaimsInputModel.Count; i++)
            {
                if (string.IsNullOrEmpty(model.RegisterClaimsInputModel[i].ClaimValue))
                    ModelState.AddModelError("RegisterClaimsInputModel[" + i + "].ClaimValue", "Поле " + model.RegisterClaimsInputModel[i].AttributeName + " дожно быть заполнено");
            }
            if (ModelState.IsValid && model.Password.Equals(model.ConfirmPassword))
            {
                user = new User()
                {
                    Email = model.Email,
                    UserName = model.Email,
                    PasswordHash = model.Password
                };
                IdentityResult result = await _users.CreateAsync(user, CancellationToken.None);
                if (result != null && result.Succeeded)
                {
                    return Ok("Create success");
                }
            }
            return BadRequest(ModelState);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string id, [FromBody]UserPasswordInputModel model)
        {
            User user = await _users.FindByIdAsync(id, CancellationToken.None);
            if (user == null)
            {
                ModelState.AddModelError("", "Такой пользователь не существует");
                return BadRequest(ModelState);
            }
            if (ModelState.IsValid)
            {
                IdentityResult result = await _users.UpdatePasswordAsync(user, model.Password);
                if (result != null && result.Succeeded)
                {
                    return Ok("Password changed");
                }
                AddErrors(result);
            }
            return BadRequest(ModelState);
        }

        private void AddErrors(IdentityResult result)
        {
            if (result != null)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }
    }
}
