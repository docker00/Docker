using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.Quickstart.UI
{
    public class RedirectUriInputModel
    {
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }

        public RedirectUriInputModel()
        {
            Id = Guid.NewGuid().ToString();
        }

        public RedirectUriInputModel(string name)
        {
            Name = name;
        }
    }
}
