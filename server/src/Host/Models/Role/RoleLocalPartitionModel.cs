using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class RoleLocalPartitionModel
    {
        public string Id { get; set; }
        public string RoleId { get; set; }
        public string LocalPartitionId { get; set; }
    }
}
