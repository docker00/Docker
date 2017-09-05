using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.Quickstart.UI
{
    public class IdentityProviderRestrictionViewModel : IdentityProviderRestrictionInputModel
    {
        public IdentityProviderRestrictionViewModel(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
