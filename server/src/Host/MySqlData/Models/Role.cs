using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;

namespace MySql.AspNet.Identity
{
    public class Role
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public Role()
        {
            Id = Guid.NewGuid().ToString();
        }

        public Role(string name)
            : this()
        {
            Name = name;
        }

        public Role(string name, string id)
        {
            Name = name;
            Id = id;
        }
    }
}
