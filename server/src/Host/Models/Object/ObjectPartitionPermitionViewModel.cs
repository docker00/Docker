using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class ObjectEndpointPermitionViewModel
    {
        public string Id { get; set; }
        [Required]
        public string ObjectEndpointId { get; set; }
        public string PermissionId { get; set; }
        public string PermissionName { get; set; }

        public IList<string> SelectedPermissions { get; set; }

        public ObjectEndpointPermitionViewModel()
        {
            SelectedPermissions = new List<string>();
        }
    }
}
