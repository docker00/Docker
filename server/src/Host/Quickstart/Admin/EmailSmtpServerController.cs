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
    [Menu(GroupName = "Settings", ShowInMenu = true)]
    public class EmailSmtpServerController : Controller
    {
        private readonly EmailSmtpServerStore<EmailSmtpServer> _emailSmtpServers;

        public EmailSmtpServerController(EmailSmtpServerStore<EmailSmtpServer> emailSmtpServers = null)
        {
            _emailSmtpServers = emailSmtpServers ?? new EmailSmtpServerStore<EmailSmtpServer>(Host.Startup.ConnectionString);
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        //[Menu(Name = "Добавить SMTP", Description = "Добавить настройки SMTP сервера", GroupName = "EmailSmtpServer", ShowInMenu = true, Weight = 0)]
        [Menu(Relation = "Details")]
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(EmailSmtpServerInputModel model)
        {
            if (ModelState.IsValid)
            {
                EmailSmtpServer emailServer = new EmailSmtpServer(model.SmtpAdress, model.SmtpPort, model.SslRequred,
                model.AuthenticateName, model.AuthenticateLogin, model.AuthenticatePassword, model.Enabled);
                IdentityResult result = await _emailSmtpServers.CreateAsync(emailServer, CancellationToken.None);
                if (result != null && result.Succeeded)
                {
                    return RedirectToAction("Details");
                }
                AddErrors(result);
            }
            return View(model);
        }

        //[Menu(Name = "Редактировать SMTP", Description = "Редактировать настройки SMTP сервера", GroupName = "EmailSmtpServer")]
        [Menu(Relation = "Details")]
        [HttpGet]
        public IActionResult Edit()
        {
            EmailSmtpServer emailServer = _emailSmtpServers.EmailSmtpServer;
            if (emailServer != null)
            {
                EmailSmtpServerInputModel emailServerView = new EmailSmtpServerInputModel(emailServer.Id,
                    emailServer.SmtpAdress, emailServer.SmtpPort, emailServer.SslRequred,
                    emailServer.AuthenticateName, emailServer.AuthenticateLogin, emailServer.AuthenticatePassword, emailServer.Enabled);
                return View(emailServerView);
            }
            return RedirectToAction("Details");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EmailSmtpServerInputModel model)
        {
            if (ModelState.IsValid)
            {
                EmailSmtpServer emailServer = new EmailSmtpServer(model.Id, model.SmtpAdress, model.SmtpPort, model.SslRequred,
                    model.AuthenticateName, model.AuthenticateLogin, model.AuthenticatePassword, model.Enabled);

                IdentityResult result = await _emailSmtpServers.UpdateAsync(emailServer, CancellationToken.None);
                if (result != null && result.Succeeded)
                {
                    return RedirectToAction("Details");
                }
                AddErrors(result);
            }
            return View(model);
        }

        [HttpGet]
        [Menu(Name = "Настройки SMTP", Description = "Настройка Smtp сервера", GroupName = "EmailSmtpServer", ShowInMenu = true, Weight = 1)]
        public IActionResult Details()
        {
            EmailSmtpServer emailServer = _emailSmtpServers.EmailSmtpServer;
            EmailSmtpServerViewModel emailServerView = null;
            if (emailServer != null)
            {
                emailServerView = new EmailSmtpServerViewModel(emailServer.Id,
                    emailServer.SmtpAdress, emailServer.SmtpPort, emailServer.SslRequred,
                    emailServer.AuthenticateName, emailServer.AuthenticateLogin, emailServer.AuthenticatePassword, emailServer.Enabled);
            }
            return View(emailServerView);
        }

        //[Menu(Name = "Удалить SMTP", Description = "Удалить SMTP сервер", GroupName = "EmailSmtpServer")]
        [Menu(Relation = "Details")]
        [HttpPost]
        public async Task<IActionResult> Delete(string emailStmpServerId)
        {
            EmailSmtpServer emailSmtpServer = new EmailSmtpServer { Id = emailStmpServerId };

            IdentityResult result = await _emailSmtpServers.DeleteAsync(emailSmtpServer, CancellationToken.None);
            if (result.Succeeded)
            {
                return Json(true);
            }
            AddErrors(result);

            return Json(false);
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
