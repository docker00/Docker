using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class UserPasswordInputModel
    {
        [Required(ErrorMessage = "{0} должен быть введен")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "{0} должен быть не меньше {2} и не больше {1} символов." )]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
    }
}
