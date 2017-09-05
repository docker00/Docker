using IdentityServer4.Models;
using System;
using System.Collections.Generic;

namespace MySql.AspNet.Identity
{
    public class PostLogoutRedirectUri
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public PostLogoutRedirectUri()
        {
            Id = Guid.NewGuid().ToString();
        }

        public PostLogoutRedirectUri(string name)
            : this()
        {
            Name = name;
        }
    }
}
