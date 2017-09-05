using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class LocalPartitionInputModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "{0} должен быть заполнен")]
        [Display(Name = "Контроллер")]
        public string ControllerName { get; set; }
        [Required(ErrorMessage = "{0} должно быть заполнено")]
        [Display(Name = "Действие")]
        public string ActionName { get; set; }

        public LocalPartitionInputModel()
        {
        }

        public LocalPartitionInputModel(string id, string controllerName, string actionName)
        {
            Id = id;
            ControllerName = controllerName;
            ActionName = actionName;
        }
    }
}
