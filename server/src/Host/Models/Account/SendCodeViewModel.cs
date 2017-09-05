using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class SendCodeViewModel
    {
        [Required]
        [Display(Name = "Способ аудентификации")]
        public string SelectedProvider { get; set; }

        public ICollection<SelectListItem> Providers { get; set; }

        public string ReturnUrl { get; set; }

        public bool RememberMe { get; set; }

        public string UserId { get; set; }
        public string UserName { get; set; }
        public string ExternalProvider { get; set; }
        public string IdToken { get; set; }
        public string ExternalUserId { get; set; }
        public string SessionId { get; set; }
    }
}
