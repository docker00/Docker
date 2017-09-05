using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "{0} должен быть заполнен")]
        [Display(Name = "E-mail")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "{0} должен быть введен")]
        [Display(Name = "Пароль")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "{0} должен быть не меньше {2} и не больше {1} символов в длину.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Повторите пароль")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        public List<RegisterClaimsInputModel> RegisterClaimsInputModel { get; set; }

        public RegisterViewModel()
        {
            RegisterClaimsInputModel = new List<RegisterClaimsInputModel>();
        }
    }
}
