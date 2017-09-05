using IdentityServer4.Models;
using System;
using System.Collections.Generic;

namespace MySql.AspNet.Identity
{
    public class IdentityResourceCustom : IdentityResource
    {
        public string Id { get; set; }

        public IdentityResourceCustom()
            : base()
        {
            Id = Guid.NewGuid().ToString();
        }

        public IdentityResourceCustom(string name, IEnumerable<string> claimTypes)
            : base(name, name, claimTypes)
        {
            Id = Guid.NewGuid().ToString();
        }

        public IdentityResourceCustom(string name, string displayName, IEnumerable<string> claimTypes) 
            : base(name, displayName, claimTypes)
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
