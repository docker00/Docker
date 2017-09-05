using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class ClaimViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Тип")]
        [Required]
        public string Type { get; set; }

        public ClaimViewModel()
        {
        }

        public ClaimViewModel(string id, string type)
        {
            Id = id;
            Type = type;
        }
    }
}
