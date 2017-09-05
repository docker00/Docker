using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySql.AspNet.Identity
{
    public class UserRole
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string RoleId { get; set; }

        public UserRole()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
