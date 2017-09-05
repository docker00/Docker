using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySql.AspNet.Identity
{
    public class UserProfileAttribute
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ProfileAttributeClaimId { get; set; }
        public UserProfileAttribute()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
