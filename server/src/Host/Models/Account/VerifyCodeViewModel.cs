using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class VerifyCodeViewModel
    {
        [Required(ErrorMessage = "{0} должен быть выбран")]
        [Display(Name = "Способ аудентификации")]
        public string Provider { get; set; }

        [Required(ErrorMessage = "{0} должен быть заполнен.")]
        [Display(Name = "Код")]
        public string Code { get; set; }

        public string ReturnUrl { get; set; }

        [Display(Name = "Запомнить этот браузер?")]
        public bool RememberBrowser { get; set; }

        [Display(Name = "Запомнить меня?")]
        public bool RememberMe { get; set; }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public string ExternalProvider { get; set; }
        public string IdToken { get; set; }
        public string ExternalUserId { get; set; }
        public string SessionId { get; set; }
    }
}
