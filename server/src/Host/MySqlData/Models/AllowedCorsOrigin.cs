using IdentityServer4.Models;
using System;
using System.Collections.Generic;

namespace MySql.AspNet.Identity
{
    public class AllowedCorsOrigin
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public AllowedCorsOrigin()
        {
            Id = Guid.NewGuid().ToString();
        }

        public AllowedCorsOrigin(string name)
            : this()
        {
            Name = name;
        }
    }
}
