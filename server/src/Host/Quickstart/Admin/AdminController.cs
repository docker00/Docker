using Host.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySql.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityServer4.Quickstart.UI
{
    [SecurityHeaders]
    public class AdminController : Controller
    {

        private IHostingEnvironment _env;

        public AdminController(IHostingEnvironment env)
        {
            _env = env;
        }

        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> ViewLog()
        {
            var webRoot = _env.ContentRootPath;
            var file = System.IO.Path.Combine(webRoot, "identityserver4_log.txt");
            FileReader fr = new FileReader(file);
            var result = new List<string>();
            result = await fr.LogView();
            return View(result);
        }
    }
}
