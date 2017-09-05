using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class ControllerViewModel
    {
        [Display(Name = "Контроллер")]
        public string Controller { get; set; }
        [Display(Name = "Название контроллера")]
        public string Name { get; set; }
        [Display(Name = "Описание")]
        public string Description { get; set; }
        [Display(Name = "Положение")]
        public int Weight { get; set; }
        public bool State { get; set; }

        public List<MethodViewModel> Methods { get; set; }

        public ControllerViewModel()
        {
            Methods = new List<MethodViewModel>();
        }

        public ControllerViewModel(string controller, string name, string description, int weight)
            : this()
        {
            Controller = controller;
            Name = name;
            Description = description;
            Weight = weight;
        }
    }
}
