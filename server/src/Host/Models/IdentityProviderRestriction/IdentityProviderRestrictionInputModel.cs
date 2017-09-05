using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class IdentityProviderRestrictionInputModel
    {
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }

        public IdentityProviderRestrictionInputModel()
        {
            Id = Guid.NewGuid().ToString();
        }

        public IdentityProviderRestrictionInputModel(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
