using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class MethodViewModel
    {
        public string Id { get; set; }
        [Display(Name = "Метод")]
        public string Method { get; set; }
        [Display(Name = "Название метода")]
        public string Name { get; set; }
        [Display(Name = "Контроллер")]
        public string Controller { get; set; }
        [Display(Name = "Описание")]
        public string Description { get; set; }
        [Display(Name = "Положение")]
        public int Weight { get; set; }
        [Display(Name = "Выбран?")]
        public bool Checked { get; set; }

        public MethodViewModel()
        {

        }

        public MethodViewModel(string method, string name, string description, string controller, int weight)
        {
            Method = method;
            Name = name;
            Description = description;
            Controller = controller;
            Weight = weight;
        }
    }
}
