using Host.Extensions.GenerateMenu.Models;
using IdentityServer4.Quickstart.UI;
using MySql.AspNet.Identity;
using MySql.AspNet.Identity.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Host.Extensions.GenerateMenu
{
    public class GenerateMenu
    {
        private readonly UserRoleRepository<User> _userRoleRepository;
        private readonly RoleLocalPartitionRepository _roleLocalPartitionRepository;

        public GenerateMenu()
        {
            _userRoleRepository = new UserRoleRepository<User>(Startup.ConnectionString);
            _roleLocalPartitionRepository = new RoleLocalPartitionRepository(Startup.ConnectionString);
        }

        /// <summary>
        /// Получает сгруппированный по GroupName весь список контроллеров и методов
        /// </summary>
        /// <returns></returns>
        public List<ControllerModel> GetControllers()
        {
            List<ControllerModel> controllers = new List<ControllerModel>();
            var assembly = typeof(MenuAttribute).GetTypeInfo().Assembly;
            foreach (Type type in assembly.GetTypes())
            {
                MenuAttribute attribute = type.GetTypeInfo().GetCustomAttribute<MenuAttribute>();
                if (attribute == null) { continue; }

                ControllerModel controllerModel = new ControllerModel()
                {
                    Controller = type.Name.Replace("Controller", ""),
                    Name = attribute.Name,
                    Description = attribute.Description,
                    GroupName = attribute.GroupName,
                    ShowInMenu = attribute.ShowInMenu,
                    Weight = attribute.Weight,
                    Relation = attribute.Relation
                };

                IEnumerable<MethodInfo> methods = type.GetMethods().Where(method => method.GetCustomAttributes<MenuAttribute>().Count() > 0);
                foreach (MethodInfo method in methods)
                {
                    MenuAttribute attributeMethod = method.GetCustomAttribute<MenuAttribute>();
                    if (attributeMethod == null || (attributeMethod != null && !string.IsNullOrEmpty(attributeMethod.Relation))) { continue; }

                    MethodModel methodModel = new MethodModel()
                    {
                        Controller = controllerModel.Controller,
                        Method = method.Name,
                        Name = attributeMethod.Name,
                        Description = attributeMethod.Description,
                        GroupName = attributeMethod.GroupName,
                        ShowInMenu = attributeMethod.ShowInMenu,
                        Weight = attributeMethod.Weight,
                        Relation = attributeMethod.Relation
                    };

                    controllerModel.Methods.Add(methodModel);
                    controllerModel.Methods = controllerModel.Methods.OrderBy(m => m.Weight).ToList();
                }

                controllers.Add(controllerModel);
            }

            List<ControllerModel> controllersGroup = controllers.Where(c => !string.IsNullOrEmpty(c.Name)).OrderBy(c => c.Weight).ToList();
            controllers = controllers.Where(c => string.IsNullOrEmpty(c.Name)).ToList();
            foreach (ControllerModel model in controllers)
            {
                ControllerModel _model = controllersGroup.FirstOrDefault(c => c.GroupName.ToLower().Equals(model.GroupName.ToLower()));
                if (_model != null)
                {
                    _model.Methods.AddRange(model.Methods);
                    _model.Methods = _model.Methods.OrderBy(m => m.Weight).ToList();
                }
                else
                {
                    controllersGroup.Add(model);
                }
            }

            return controllersGroup;
        }

        /// <summary>
        /// Получает сгруппированный по GroupName список контроллеров и методов для пользоваателя
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <returns></returns>
        public List<ControllerModel> GetMenuByUserId(string userId)
        {
            List<ControllerModel> menu = new List<ControllerModel>();

            List<ControllerModel> controllers = GetControllers();
            IQueryable<LocalPartition> localPartition = _roleLocalPartitionRepository.PopulateRoleLocalPartitionsByUserId(userId);

            foreach (ControllerModel controller in controllers)
            {
                if (!controller.ShowInMenu) { continue; }
                IQueryable<LocalPartition> partitions = localPartition.Where(l => l.ControllerName.ToLower().Equals(controller.Controller.ToLower()));
                if (partitions.Count() > 0)
                {
                    ControllerModel controllerUser = new ControllerModel()
                    {
                        Controller = controller.Controller,
                        Name = controller.Name,
                        Description = controller.Description,
                        GroupName = controller.GroupName,
                        Weight = controller.Weight,
                        Relation = controller.Relation
                    };

                    controllerUser.Methods.AddRange(controller.Methods.Where(m => m.ShowInMenu && partitions.Count(p => p.ActionName.ToLower().Equals(m.Method.ToLower())) > 0));
                    controllerUser.Methods = controllerUser.Methods.OrderBy(m => m.Weight).ToList();

                    menu.Add(controllerUser);
                }
            }

            menu = menu.OrderBy(m => m.Weight).ToList();

            return menu;
        }

        /// <summary>
        /// Получает сгруппированный по GroupName список контроллеров и методов для роли
        /// </summary>
        /// <param name="roleId">Id роли</param>
        /// <returns></returns>
        public List<ControllerModel> GetControllerModelByRoleId(string roleId)
        {
            List<ControllerModel> menu = new List<ControllerModel>();

            List<ControllerModel> controllers = GetControllers();
            IQueryable<LocalPartition> localPartition = _roleLocalPartitionRepository.PopulateRoleLocalPartitionsByRoleId(roleId);

            foreach (ControllerModel controller in controllers)
            {
                if (!controller.ShowInMenu) { continue; }
                IQueryable<LocalPartition> partitions = localPartition.Where(l => l.ControllerName.ToLower().Equals(controller.Controller.ToLower()));
                if (partitions.Count() > 0)
                {
                    ControllerModel controllerUser = new ControllerModel()
                    {
                        Controller = controller.Controller,
                        Name = controller.Name,
                        Description = controller.Description,
                        GroupName = controller.GroupName,
                        Weight = controller.Weight,
                        Relation = controller.Relation
                    };

                    controllerUser.Methods.AddRange(controller.Methods.Where(m => partitions.Count(p => p.ActionName.ToLower().Equals(m.Method.ToLower())) > 0));
                    controllerUser.Methods = controllerUser.Methods.OrderBy(m => m.Weight).ToList();

                    menu.Add(controllerUser);
                }
            }

            menu = menu.OrderBy(m => m.Weight).ToList();

            return menu;
        }

        /// <summary>
        /// Проверяет, может ли пользователь пройти по данной ссылке
        /// </summary>
        /// <param name="userId">Id пользователя</param>
        /// <param name="controllerName">Имя контроллера</param>
        /// <param name="actionName">Имя действия</param>
        /// <returns></returns>
        public bool CheckUrl(string userId, string controllerName, string actionName)
        {
            //return true;
            //TODO: раскомментировать после исправления интерфейсов
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(controllerName) || string.IsNullOrEmpty(actionName)) { return false; }
            
            MethodModel methodModel = GetMethodFromMenu(controllerName, actionName);
            if (methodModel != null && !string.IsNullOrEmpty(methodModel.Relation))
            {
                actionName = methodModel.Relation;
            }
            
            return _roleLocalPartitionRepository.CheckControllerNameAndActionNameByUserId(userId, controllerName, actionName);
        }

        /// <summary>
        /// Возвращает модель действия
        /// </summary>
        /// <param name="controllerName">Имя контроллера</param>
        /// <param name="actionName">Имя действия</param>
        /// <returns></returns>
        public MethodModel GetMethodFromMenu(string controllerName, string actionName)
        {
            var assembly = typeof(MenuAttribute).GetTypeInfo().Assembly;
            foreach (Type type in assembly.GetTypes())
            {
                MenuAttribute attribute = type.GetTypeInfo().GetCustomAttribute<MenuAttribute>();
                if (attribute == null || (attribute != null && !type.Name.Replace("Controller", "").ToLower().Equals(controllerName.ToLower()))) { continue; }

                ControllerModel controllerModel = new ControllerModel()
                {
                    Controller = type.Name.Replace("Controller", ""),
                    Name = attribute.Name,
                    Description = attribute.Description,
                    GroupName = attribute.GroupName,
                    Weight = attribute.Weight,
                    Relation = attribute.Relation
                };

                IEnumerable<MethodInfo> methods = type.GetMethods().Where(method => method.GetCustomAttributes<MenuAttribute>().Count() > 0);
                foreach (MethodInfo method in methods)
                {
                    MenuAttribute attributeMethod = method.GetCustomAttribute<MenuAttribute>();
                    if (attributeMethod == null || attributeMethod != null && !method.Name.ToLower().Equals(actionName.ToLower())) { continue; }

                    MethodModel methodModel = new MethodModel()
                    {
                        Controller = controllerModel.Controller,
                        Method = method.Name,
                        Name = attributeMethod.Name,
                        Description = attributeMethod.Description,
                        GroupName = attributeMethod.GroupName,
                        Weight = attributeMethod.Weight,
                        Relation = attributeMethod.Relation
                    };

                    return methodModel;
                }
            }

            return null;
        }

    }
}
