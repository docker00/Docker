using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class RoleViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "{0} должно быть заполнено")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "{0} должно быть не меньше {2} и не больши {1} символов." )]
        [Display(Name = "Название")]
        public string Name { get; set; }

        public List<GroupViewModel> Groups { get; set; }
        public List<UserViewModel> Users { get; set; }
        public List<ControllerViewModel> Controllers = new List<ControllerViewModel>();
        

        public RoleViewModel()
        {
            Groups = new List<GroupViewModel>();
            Users = new List<UserViewModel>();
            Controllers = new List<ControllerViewModel>();
        }

        public RoleViewModel(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
