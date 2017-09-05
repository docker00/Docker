using IdentityServer4.Models;
using System;
using System.Collections.Generic;

namespace MySql.AspNet.Identity
{
    public class IdentityProviderRestriction
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public IdentityProviderRestriction()
        {
            Id = Guid.NewGuid().ToString();
        }

        public IdentityProviderRestriction(string name)
            : this()
        {
            Name = name;
        }
    }
}
