using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.Quickstart.UI
{
    public class ObjectEndpointsViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Значение")]
        public string Value { get; set; }
    }
}
