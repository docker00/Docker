using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class RolePermissionModel
    {
        public string Id { get; set; }
        public string RoleId { get; set; }
        public string PermissionId { get; set; }

        public IList<string> SelectedPermissions { get; set; }
    }
}
