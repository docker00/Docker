// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Host.Extensions.MessageService;
using IdentityModel;
using IdentityServer4.Events;
using IdentityServer4.Services;
using IdentityServer4.Extensions;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.AspNet.Identity;
using PaulMiami.AspNetCore.Mvc.Recaptcha;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityServer4.Quickstart.UI
{
    [SecurityHeaders]
    public class AccountController : Controller
    {
        private readonly UserStore<User> _users;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEventService _events;
        private readonly AccountService _account;
        private readonly MessageService _messageSender;
        private readonly ProfileAttributeStore<ProfileAttribute> _profileAttributes;

        public AccountController(
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IHttpContextAccessor httpContextAccessor,
            IEventService events,
            UserStore<User> users = null,
            MessageService messageSender = null,
            ProfileAttributeStore<ProfileAttribute> profileAttributes = null)
        {
            _users = users ?? new UserStore<User>(Host.Startup.ConnectionString);

            _interaction = interaction;
            _events = events;
            _account = new AccountService(interaction, httpContextAccessor, clientStore);
            _messageSender = messageSender ?? new MessageService(Host.Startup.ConnectionString);
            _profileAttributes = profileAttributes ?? new ProfileAttributeStore<ProfileAttribute>(Host.Startup.ConnectionString);
        }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            LoginViewModel vm = await _account.BuildLoginViewModelAsync(returnUrl);

            if (vm.IsExternalLoginOnly)
            {
                // only one option for logging in
                return ExternalLogin(vm.ExternalLoginScheme, returnUrl);
            }

            return View(vm);
        }

        [ValidateRecaptcha]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            if (ModelState.IsValid)
            {
                // validate username/password against in-memory store
                if (_users.ValidateCredentials(model.UserName, model.Password))
                {
                    AuthenticationProperties props = null;
                    // only set explicit expiration here if persistent. 
                    // otherwise we reply upon expiration configured in cookie middleware.
                    if (AccountOptions.AllowRememberLogin && model.RememberLogin)
                    {
                        props = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.Add(AccountOptions.RememberMeLoginDuration)
                        };
                    };

                    // issue authentication cookie with subject ID and username
                    User user = await _users.GetUserEmailAsync(model.UserName);
                    if (!user.Activated)
                    {
                        LoginViewModel loginView = await _account.BuildLoginViewModelAsync(model);
                        ModelState.AddModelError("", "Проблемы с авторизацией. Обратитесь в службу поддержки.");
                        return View(loginView);
                    }
                    if (!user.EmailConfirmed)
                    {
                        string code = await _users.GenerateConfirmationCodeAsync(user, CancellationToken.None);
                        string callbackUrl = Url.Action("ConfirmEmail", "Account", new
                        {
                            userId = user.Id,
                            code = code,
                            returnUrl = model.ReturnUrl
                        }, protocol: HttpContext.Request.Scheme);

                        bool send_success = await _messageSender.SendMessage(user, "Подтверждение аккаунта",
                            $"Пожалуйста, подтвержите свой аккаунт по этой ссылке: <a href='{callbackUrl}'>{callbackUrl}</a>");
                        if (!send_success)
                        {
                            ModelState.AddModelError("Email", "Не удалось отправить письмо с подтверждением на почту. Пожалуйста, попробуйте снова");
                            return View(await _account.BuildLoginViewModelAsync(model));
                        }
                        ViewData["ReturnUrl"] = model.ReturnUrl;
                        return View("ConfirmEmail");
                    }

                    if (user.TwoFactorAuthEnabled)
                    {
                        SendCodeViewModel sendCodeViewModel = new SendCodeViewModel
                        {
                            ReturnUrl = model.ReturnUrl,
                            RememberMe = model.RememberLogin,
                            UserId = user.Id,
                            UserName = user.UserName
                        };
                        return RedirectToAction(nameof(SendCode), sendCodeViewModel);
                    }

                    await SignInUser(user.Id, user.UserName, model.RememberLogin);

                    if (_interaction.IsValidReturnUrl(model.ReturnUrl) || Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    //if (_users.GetRequiredAttributesOnUser(user.Id))
                    if (!user.AttributesValidated)
                    {
                        //IdentityResult result = await _users.UpdateActivatedAsync(user.Id, false, CancellationToken.None);
                        return RedirectToAction("ProfileAttributeList", "User", new { userId = user.Id });
                    }
                    else
                    {
                        IdentityResult result = await _users.UpdateActivatedAsync(user.Id, true, CancellationToken.None);
                    }

                    return Redirect("~/");
                }

                await _events.RaiseAsync(new UserLoginFailureEvent(model.UserName, "invalid credentials"));

                ModelState.AddModelError("", AccountOptions.InvalidCredentialsErrorMessage);
            }

            // something went wrong, show form with error
            LoginViewModel vm = await _account.BuildLoginViewModelAsync(model);
            return View(vm);
        }

        [HttpGet]
        public IActionResult ExternalLogin(string provider, string returnUrl)
        {
            returnUrl = Url.Action("ExternalLoginCallback", new { returnUrl = returnUrl });

            // start challenge and roundtrip the return URL
            AuthenticationProperties props = new AuthenticationProperties
            {
                RedirectUri = returnUrl,
                Items = { { "scheme", provider } }
            };
            return new ChallengeResult(provider, props);
        }

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl)
        {
            AuthenticateInfo info = await HttpContext.Authentication.GetAuthenticateInfoAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            ClaimsPrincipal tempUser = info?.Principal;
            if (tempUser == null)
            {
                throw new Exception("Ошибка внешней аутентификации");
            }

            List<Claim> claims = tempUser.Claims.ToList();

            Claim userIdClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject);
            if (userIdClaim == null)
            {
                userIdClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            }
            if (userIdClaim == null)
            {
                throw new Exception("Не найден userid");
            }

            claims.Remove(userIdClaim);
            string provider = info.Properties.Items["scheme"];
            string userId = userIdClaim.Value;

            Task<User> userTask = _users.FindByLoginAsync(provider, userId, CancellationToken.None);
            User user = userTask != null ? userTask.Result : null;
            if (user == null)
            {
                user = await _users.AutoProvisionUser(provider, userId, claims);
                if (user == null)
                {
                    return Redirect("/Account/Login/?returnUrl=" + returnUrl);
                }
            }

            string sessionId = user.Claims.FirstOrDefault(x => x.Type == JwtClaimTypes.SessionId)?.Value;
            string id_token = info.Properties.GetTokenValue("id_token");

            if (user.TwoFactorAuthEnabled)
            {
                SendCodeViewModel sendCodeViewModel = new SendCodeViewModel
                {
                    ReturnUrl = returnUrl,
                    RememberMe = false,
                    UserId = user.Id,
                    UserName = !string.IsNullOrEmpty(user.UserName) ? user.UserName : user.Email,
                    ExternalProvider = provider,
                    IdToken = id_token,
                    ExternalUserId = userId,
                    SessionId = sessionId
                };
                return RedirectToAction(nameof(SendCode), sendCodeViewModel);
            }
            
            await SignInUser(user.Id, !string.IsNullOrEmpty(user.UserName) ? user.UserName : user.Email, false, provider, id_token, userId, sessionId);

            if (_interaction.IsValidReturnUrl(returnUrl) || Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect("~/");
        }

        private async Task SignInUser(string userId, string userName, bool rememberLogin, string provider = "", string idToken = "", string externalUserId = "", string sessionId = "")
        {
            AuthenticationProperties props = null;
            if (string.IsNullOrEmpty(provider))
            {
                if (AccountOptions.AllowRememberLogin && rememberLogin)
                {
                    props = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.Add(AccountOptions.RememberMeLoginDuration)
                    };
                };

                await _events.RaiseAsync(new UserLoginSuccessEvent(userName, userId, userName));
                await HttpContext.Authentication.SignInAsync(userId, userName, props);
            }
            else
            {
                List<Claim> additionalClaims = new List<Claim>();
                if (!string.IsNullOrEmpty(sessionId))
                {
                    additionalClaims.Add(new Claim(JwtClaimTypes.SessionId, sessionId));
                }

                if (!string.IsNullOrEmpty(idToken))
                {
                    props = new AuthenticationProperties();
                    props.StoreTokens(new[] { new AuthenticationToken { Name = "id_token", Value = idToken } });
                }

                await _events.RaiseAsync(new UserLoginSuccessEvent(provider, externalUserId, userId, userName));
                await HttpContext.Authentication.SignInAsync(userId, userName, provider, props, additionalClaims.ToArray());

                // delete temporary cookie used during external authentication
                await HttpContext.Authentication.SignOutAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            }
        }
        
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

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
            return View(registerView);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [ValidateRecaptcha]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            for (int i = 0; i < model.RegisterClaimsInputModel.Count; i++)
            {
                if (string.IsNullOrEmpty(model.RegisterClaimsInputModel[i].ClaimValue))
                    ModelState.AddModelError("RegisterClaimsInputModel[" + i + "].ClaimValue", "Поле " + model.RegisterClaimsInputModel[i].AttributeName + " дожно быть заполнено");
            }

            if (ModelState.IsValid && model.RegisterClaimsInputModel != null && model.RegisterClaimsInputModel.Count > 0 && model.Password.Equals(model.ConfirmPassword))
            {
                User user = new User { UserName = model.Email, Email = model.Email, PasswordHash = model.Password };
                IdentityResult result = await _users.CreateAsync(user, CancellationToken.None);
                if (result != null && result.Succeeded)
                {
                    foreach (RegisterClaimsInputModel registerClaimsInputModel in model.RegisterClaimsInputModel)
                    {
                        ClaimCustom claim = new ClaimCustom("", registerClaimsInputModel.ClaimValue ?? string.Empty)
                        {
                            Id = registerClaimsInputModel.ClaimId
                        };
                        result = await _users.CreateOrUpdateClaimAsync(user, claim, CancellationToken.None);
                    }

                    string code = await _users.GenerateConfirmationCodeAsync(user, CancellationToken.None);
                    string callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code, returnUrl = returnUrl }, protocol: HttpContext.Request.Scheme);
                    bool send_success = await _messageSender.SendMessage(user, "Подтверждение аккаунта",
                        $"Пожалуйста, подтвержите свой аккаунт по этой ссылке: <a href='{callbackUrl}'>{callbackUrl}</a>");
                    if (!send_success)
                    {
                        ModelState.AddModelError("Email", "Не удалось отправить письмо с подтверждением на почту. Пожалуйста, попробуйте снова");
                        return View(model);
                    }
                    return View("ConfirmEmail");
                }
                AddErrors(result);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout(string logoutId)
        {
            LogoutViewModel vm = await _account.BuildLogoutViewModelAsync(logoutId);

            if (vm.ShowLogoutPrompt == false)
            {
                // no need to show prompt
                return await Logout(vm);
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutInputModel model)
        {
            LoggedOutViewModel vm = await _account.BuildLoggedOutViewModelAsync(model.LogoutId);
            if (vm.TriggerExternalSignout)
            {
                string url = Url.Action("Logout", new { logoutId = vm.LogoutId });
                try
                {
                    // hack: try/catch to handle social providers that throw
                    await HttpContext.Authentication.SignOutAsync(vm.ExternalAuthenticationScheme,
                        new AuthenticationProperties { RedirectUri = url });
                }
                catch (NotSupportedException) // this is for the external providers that don't have signout
                {
                }
                catch (InvalidOperationException) // this is for Windows/Negotiate
                {
                }
            }

            // delete local authentication cookie
            await HttpContext.Authentication.SignOutAsync();

            ClaimsPrincipal user = await HttpContext.GetIdentityServerUserAsync();
            if (user != null)
            {
                await _events.RaiseAsync(new UserLogoutSuccessEvent(user.GetSubjectId(), user.GetName()));
            }

            return View("LoggedOut", vm);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code, string returnUrl = null)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            User user = await _users.FindByIdAsync(userId, CancellationToken.None);
            if (user == null)
            {
                return View("Error");
            }
            IdentityResult result = await _users.ConfirmEmailAsync(user, code, CancellationToken.None);
            if (result.Succeeded)
            {
                await _events.RaiseAsync(new UserLoginSuccessEvent(user.UserName, user.Id, user.UserName));
                await HttpContext.Authentication.SignInAsync(user.Id, user.UserName);

                if (_interaction.IsValidReturnUrl(returnUrl) || Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                ViewBag.returnUrl = returnUrl;
                return RedirectToAction("ConfirmedEmail");
            }
            AddErrors(result);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!_messageSender.isEmailSenderEnabled)
            {
                ModelState.AddModelError("Email", "Восстановление пароля по email временно не доступно");
            }
            if (ModelState.IsValid)
            {
                User user = await _users.FindByNameAsync(model.Email, CancellationToken.None);
                if (user == null || !(user.EmailConfirmed))
                {
                    ModelState.AddModelError("Email", "Email не подтвержден либо пользователь не найден");
                    return View(model);
                }
                string code = await _users.GenerateConfirmationCodeAsync(user, CancellationToken.None);
                string callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: HttpContext.Request.Scheme);

                bool result = await _messageSender.SendMessage(user, "Восстановление пароля",
                   $"Пожалуйста, восстановите свой пароль по этой ссылке: <a href='{callbackUrl}'>{callbackUrl}</a>");
                if (result)
                {
                    return View("ForgotPasswordConfirmation");
                }
                ModelState.AddModelError("Email", "Ошибка при отправке письма, повторите снова");
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null ? View("Error") : View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            User user = await _users.FindByNameAsync(model.Email, CancellationToken.None);
            if (user == null)
            {
                return RedirectToAction(nameof(AccountController.ResetPassword), "Account");
            }
            IdentityResult result = await _users.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(AccountController.ResetPasswordConfirmation), "Account");
            }
            AddErrors(result);
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl = null, bool rememberMe = false,
                string userId = "", string userName = "", string externalProvider = "", string idToken = "", string externalUserId = "", string sessionId = "")
        {
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction(nameof(Login), new { ReturnUrl = returnUrl });
            }

            User user = await _users.FindByIdAsync(userId, CancellationToken.None);
            if (user == null)
            {
                return View("Error");
            }

            List<string> userFactors = new List<string>();
            if (user.EmailConfirmed)
            {
                userFactors.Add("Email");
            }
            if (user.PhoneNumberConfirmed)
            {
                userFactors.Add("Phone");
            }

            List<SelectListItem> factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            SendCodeViewModel sendCodeViewModel = new SendCodeViewModel
            {
                Providers = factorOptions,
                ReturnUrl = returnUrl,
                RememberMe = rememberMe,
                UserId = userId,
                UserName = userName,
                ExternalProvider = externalProvider,
                IdToken = idToken,
                ExternalUserId = externalUserId
            };
            return View(sendCodeViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (string.IsNullOrEmpty(model.UserId))
            {
                return RedirectToAction(nameof(Login), new { ReturnUrl = model.ReturnUrl });
            }

            User user = _users.FindById(model.UserId);
            if (user == null)
            {
                return View("Error");
            }

            string code = await _users.GenerateConfirmationCodeAsync(user, CancellationToken.None);
            if (string.IsNullOrWhiteSpace(code))
            {
                return View("Error");
            }

            string message = "Код подтверждения: " + code;
            if (model.SelectedProvider == "Email")
            {
                await _messageSender.SendMessage(user, "Код подтверждения", message);
            }
            else if (model.SelectedProvider == "Phone")
            {
                //await _smsSender.SendSmsAsync(await _userManager.GetPhoneNumberAsync(user), message);
            }

            VerifyCodeViewModel verifyCodeViewModel = new VerifyCodeViewModel
            {
                Provider = model.SelectedProvider,
                ReturnUrl = model.ReturnUrl,
                RememberMe = model.RememberMe,
                UserId = model.UserId,
                UserName = model.UserName,
                ExternalProvider = model.ExternalProvider,
                IdToken = model.IdToken,
                ExternalUserId = model.ExternalUserId,
                SessionId = model.SessionId
            };

            return RedirectToAction(nameof(VerifyCode), verifyCodeViewModel);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string returnUrl = null,
                string userId = "", string userName = "", string externalProvider = "", string idToken = "", string externalUserId = "", string sessionId = "")
        {
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction(nameof(Login), new { ReturnUrl = returnUrl });
            }

            User user = await _users.FindByIdAsync(userId, CancellationToken.None);
            if (user == null)
            {
                return View("Error");
            }
            VerifyCodeViewModel verifyCodeViewModel = new VerifyCodeViewModel
            {
                Provider = provider,
                ReturnUrl = returnUrl,
                RememberMe = rememberMe,
                UserId = userId,
                UserName = userName,
                ExternalProvider = externalProvider,
                IdToken = idToken,
                ExternalUserId = externalUserId,
                SessionId = sessionId
            };
            return View(verifyCodeViewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (string.IsNullOrEmpty(model.UserId))
            {
                return RedirectToAction(nameof(Login), new { ReturnUrl = model.ReturnUrl });
            }

            User user = await _users.FindByIdAsync(model.UserId, CancellationToken.None);
            if (user == null)
            {
                return View("Error");
            }

            bool result = await _users.TwoFactorVerifyCodeAsync(model.UserId, model.Code);
            if (result)
            {
                await SignInUser(model.UserId, model.UserName, model.RememberMe, model.ExternalProvider, model.IdToken, model.ExternalUserId, model.SessionId);
                return RedirectToLocal(model.ReturnUrl);
            }
            //if (result)
            //{
            //    return View("Lockout");
            //}
            else
            {
                ModelState.AddModelError(string.Empty, "Неверный код.");
                return View(model);
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }
    }
}