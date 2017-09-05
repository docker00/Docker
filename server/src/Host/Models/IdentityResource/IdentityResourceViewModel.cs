using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class IdentityResourceViewModel : IdentityResourceInputModel
    {
        public List<ClaimViewModel> Claims { get; set; }

        public IdentityResourceViewModel()
        {
            Claims = new List<ClaimViewModel>();
        }
    }
}
