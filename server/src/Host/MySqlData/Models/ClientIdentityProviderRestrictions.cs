using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySql.AspNet.Identity
{
    public class ClientIdentityProviderRestriction
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string IdentityProviderRestrictionId { get; set; }

        public ClientIdentityProviderRestriction()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
