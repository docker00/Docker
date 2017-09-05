using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class UserMergeModel
    {
        [Required(ErrorMessage = "{0} должен быть заполнен")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        public List<string> Roles { get; set; }
        public List<GroupViewModel> Groups { get; set; }
        public List<UserClaimInputModel> Claims { get; set; }

        public List<string> EmailSelect { get; set; }
        public List<string> UserNameSelect { get; set; }

        public List<UserClaimViewModel> ClaimSelect { get; set; }

        public UserMergeModel()
        {
            Roles = new List<string>();
            Groups = new List<GroupViewModel>();
            Claims = new List<UserClaimInputModel>();

            EmailSelect = new List<string>();
            UserNameSelect = new List<string>();
            ClaimSelect = new List<UserClaimViewModel>();
        }
    }
}
