using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class PermissionViewModel
    {
        public string Id { get; set; }

        [Required]
        [StringLength(255, ErrorMessage = "{0} должен быть не менее {2}, а при максимальном {1} символе.", MinimumLength = 3)]
        [Display(Name = "Название")]
        public string Name { get; set; }
    }
}
