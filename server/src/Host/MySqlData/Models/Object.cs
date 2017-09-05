using IdentityServer4.Models;
using System;
using System.Collections.Generic;

namespace MySql.AspNet.Identity
{
    public class Object
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }

        public Object()
        {
            Id = Guid.NewGuid().ToString();
        }

        public Object(string name)
            : this()
        {
            Name = name;
        }
    }
}
