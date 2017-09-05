using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySql.AspNet.Identity
{
    public class GroupRole
    {
        public string Id { get; set; }
        public string GroupId { get; set; }
        public string RoleId { get; set; }

        public GroupRole()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
