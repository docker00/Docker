using IdentityServer4.Models;
using System;
using System.Collections.Generic;

namespace MySql.AspNet.Identity
{
    public class RedirectUri
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public RedirectUri()
        {
            Id = Guid.NewGuid().ToString();
        }

        public RedirectUri(string name)
            : this()
        {
            Name = name;
        }
    }
}
