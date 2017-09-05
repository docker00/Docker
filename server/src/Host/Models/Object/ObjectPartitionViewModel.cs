using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class ObjectEndpointViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Значение")]
        public string Value { get; set; }

        [Display(Name = "Объект")]
        public string ObjectId { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }
    }
}
