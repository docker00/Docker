using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.Quickstart.UI
{
    public class AllowedClaimsOriginViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "Название")]
        public string Name { get; set; }

        public AllowedClaimsOriginViewModel()
        {
        }

        public AllowedClaimsOriginViewModel(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
