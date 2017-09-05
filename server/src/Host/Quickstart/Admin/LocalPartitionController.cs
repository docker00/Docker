using Host.Extensions;
using Host.Extensions.GenerateMenu;
using Host.Extensions.GenerateMenu.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MySql.AspNet.Identity;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityServer4.Quickstart.UI
{
    [Menu(Name = "Настройки", Description = "Управление настройками", GroupName = "Settings", ShowInMenu = true, Weight = 7)]
    public class LocalPartitionController : Controller
    {
        private readonly LocalPartitionStore<LocalPartition> _localPartitions;

        public LocalPartitionController(LocalPartitionStore<LocalPartition> localPartitions = null)
        {
            _localPartitions = localPartitions ?? new LocalPartitionStore<LocalPartition>(Host.Startup.ConnectionString);
        }

        public IActionResult Index()
        {
            return View();
        }

        [Menu(Name = "Список разделов подсистемы", Description = "Управление списоком разделов подсистемы", GroupName = "Settings", ShowInMenu = true, Weight = 0)]
        [HttpGet]
        public IActionResult List()
        {
            return View();
        }

        [Menu(Relation = "List")]
        [HttpGet]
        public IActionResult Data()
        {
            List<ControllerModel> controllers = new GenerateMenu().GetControllers();
            List<ControllerViewModel> controllersView = new List<ControllerViewModel>();
            IQueryable<LocalPartition> partitions = _localPartitions.LocalPartitions;
            foreach (ControllerModel controller in controllers)
            {
                ControllerViewModel controllerView = new ControllerViewModel(controller.Controller, controller.Name, controller.Description, controller.Weight);
                int methods_count = 0;
                foreach (MethodModel method in controller.Methods)
                {
                    if (partitions.FirstOrDefault(p => p.ControllerName.ToLower().Equals(method.Controller.ToLower())
                    && p.ActionName.ToLower().Equals(method.Method.ToLower())) != null)
                    {
                        methods_count++;
                    }
                }
                controllerView.State = methods_count == controller.Methods.Count;
                controllersView.Add(controllerView);
            }
            controllersView = controllersView.OrderBy(c => c.Weight).ToList();
            return Json(controllersView);
        }

        [Menu(Relation = "List")]
        public IActionResult _MethodListPartial(string roleId, string _controller)
        {
            ViewData["RoleId"] = roleId;
            List<MethodViewModel> Methods = new List<MethodViewModel>();
            List<ControllerModel> controllers = new GenerateMenu().GetControllers();
            IQueryable<LocalPartition> partitions = _localPartitions.LocalPartitions;

            foreach (ControllerModel controller in controllers.Where(c => c.Controller == _controller))
            {
                foreach (MethodModel method in controller.Methods)
                {
                    Methods.Add(new MethodViewModel(method.Method, method.Name, method.Description, method.Controller, method.Weight)
                    {
                        Checked = !(partitions.FirstOrDefault(p => p.ControllerName == method.Controller && p.ActionName == method.Method) == null)
                    });
                }
            }
            Methods = Methods.OrderBy(m => m.Weight).ToList(); ;
            return PartialView(Methods);
        }

        [Menu(Name = "Добавить раздел", Description = "Добавить раздел", GroupName = "Settings")]
        [HttpPost]
        public async Task<IActionResult> Add(LocalPartitionInputModel model)
        {
            if (ModelState.IsValid)
            {
                LocalPartition localPartition = new LocalPartition(model.ControllerName, model.ActionName);
                IdentityResult result = await _localPartitions.CreateAsync(localPartition, CancellationToken.None);
                if (result != null)
                {
                    return Json(result.Succeeded);
                }
            }
            return Json(false);
        }

        //[Menu(Name = "Добавить секцию", Description = "Добавить все разделы по контроллеру", GroupName = "Settings")]
        [Menu(Relation = "Add")]
        [HttpGet]
        public async Task<IActionResult> AddActionsController(string _controller)
        {
            List<ControllerModel> controllers = new GenerateMenu().GetControllers();
            LocalPartition localPartition;
            IdentityResult result;
            foreach (ControllerModel controller in controllers.Where(c => c.Controller == _controller))
            {
                foreach (MethodModel method in controller.Methods)
                {
                    localPartition = new LocalPartition(method.Controller, method.Method);
                    result = await _localPartitions.CreateAsync(localPartition, CancellationToken.None);
                    if (!result.Succeeded)
                    {
                        return Json(false);
                    }
                }
            }
            return Json(true);
        }

        //[Menu(Name = "Удалить ссылку на раздел подсистемы", Description = "Удалить ссылку на раздел подсистемы по контроллеру", GroupName = "Settings")]
        [Menu(Relation = "Delete")]
        [HttpGet]
        public async Task<IActionResult> DeleteActionsController(string _controller)
        {
            List<ControllerModel> controllers = new GenerateMenu().GetControllers();
            LocalPartition localPartition;
            IdentityResult result;
            foreach (ControllerModel controller in controllers.Where(c => c.Controller == _controller))
            {
                ControllerViewModel controllerView = new ControllerViewModel(controller.Controller, controller.Name, controller.Description, controller.Weight);
                foreach (MethodModel method in controller.Methods)
                {
                    localPartition = await _localPartitions.FindAsync(method.Controller, method.Method, CancellationToken.None);
                    if (localPartition != null)
                    {
                        result = await _localPartitions.DeleteAsync(localPartition, CancellationToken.None);
                        if (!result.Succeeded)
                        {
                            return Json(false);
                        }
                    }
                }
            }
            return Json(true);
        }

        [Menu(Relation = "List")]
        public async Task<IActionResult> GetRoleLocalPartitionCount(string controllerName, string actionName)
        {
            int count = await _localPartitions.GetRoleLocalPartitionCountAsync(controllerName, actionName, CancellationToken.None);
            return Json(count);
        }


        [Menu(Name = "Удалить ссылку на раздел подсистемы", Description = "Удалить ссылку на раздел подсистемы", GroupName = "Settings")]
        [HttpGet]
        public async Task<IActionResult> Delete(string controllerName, string actionName)
        {
            LocalPartition localPartition = await _localPartitions.FindAsync(controllerName, actionName, CancellationToken.None);
            if (localPartition != null)
            {
                IdentityResult result = await _localPartitions.DeleteAsync(localPartition, CancellationToken.None);
                return Json(result.Succeeded);
            }
            return Json(false);
        }
    }
}
