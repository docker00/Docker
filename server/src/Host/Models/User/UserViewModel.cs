using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class UserViewModel
    {
        public string Id { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }
        [Display(Name = "Номер телефона")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        [Display(Name = "Активирован?")]
        public bool Activated { get; set; }
        //TODO : сделать коллекцией RoleView
        [Display]
        public IEnumerable<string> Roles { get; set; }

        public List<GroupViewModel> Groups { get; set; }

        public UserViewModel()
        {
            Groups = new List<GroupViewModel>();
        }
    }
}
