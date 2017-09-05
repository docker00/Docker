using IdentityServer4.Models;
using System;
using System.Collections.Generic;

namespace MySql.AspNet.Identity
{
    public class AllowedGrantType
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public AllowedGrantType()
        {
            Id = Guid.NewGuid().ToString();
        }

        public AllowedGrantType(string name)
            : this()
        {
            Name = name;
        }
    }
}
