using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.Quickstart.UI
{
    public class ObjectPermissionViewModel
    {
        public string Id { get; set; }
        public string ObjectId { get; set; }
        public string PermissionId { get; set; }

        public IList<string> SelectedPermissions { get; set; }
    }
}
