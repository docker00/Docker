using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.Net;

namespace Host.Extensions
{
    public class MenuAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Отображаемое имя
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Описание
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Имя группы, необходимо для группировки методов из разных контроллеров
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// Параметр отвечающий за отображение данного действия в меню
        /// </summary>
        public bool ShowInMenu { get; set; }
        /// <summary>
        /// Расположение элемента - вес
        /// </summary>
        public int Weight { get; set; }
        /// <summary>
        /// Данная связь используется для наследования доспупа.
        /// Нужно указывать имя действия, от которого необходимо пронаследовать доступы.
        /// </summary>
        public string Relation { get; set; }

        public MenuAttribute()
        {
            Name = "";
            Description = "";
            GroupName = "";
            ShowInMenu = false;
            Weight = 0;
            Relation = "";
        }

        
        /// <summary>
        /// Проверяет, имеет ли доступ пользователь, при переходе по ссылке
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string requestPath = context?.HttpContext?.Request?.Path.Value;

            if (context != null)
            {
                if (string.IsNullOrEmpty(context?.HttpContext?.User?.Identity?.Name))
                {
                    context.HttpContext.Response.Redirect("/Account/Login?returnUrl=" + requestPath);
                    context.Result = new ContentResult { Content = HttpStatusCode.Unauthorized.ToString() };
                }
                else if (!string.IsNullOrEmpty(context?.HttpContext?.User?.Identity?.Name))
                {
                    string[] requestPaths = requestPath.Trim('/').Split('/');
                    if (requestPaths.Length == 1)
                    {
                        requestPaths = new string[] { requestPaths[0], "Index" };
                    }

                    string userId = context.HttpContext.User?.FindFirst("sub")?.Value;
                    bool checkUrl = new GenerateMenu.GenerateMenu().CheckUrl(userId, requestPaths[0], requestPaths[1]);
                    if (!checkUrl)
                    {
                        context.HttpContext.Response.Redirect("/Home/Forbidden");
                        context.Result = new ContentResult { Content = HttpStatusCode.Forbidden.ToString() };
                    }
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
