using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "{0} должен быть заполнен")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
