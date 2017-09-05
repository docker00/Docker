using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.Quickstart.UI
{
    public class RedirectUriViewModel: RedirectUriInputModel
    {

        public RedirectUriViewModel()
        {

        }

        public RedirectUriViewModel(string id, string name)
        {
            Id = id;
            Name = name;
        }
    }
}
