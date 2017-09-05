using System;

namespace MySql.AspNet.Identity
{
    public class Permission
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public Permission()
        {
            Id = Guid.NewGuid().ToString();
        }

        public Permission(string name)
            : this()
        {
            Name = name;
        }

        public Permission(string name, string id)
        {
            Name = name;
            Id = id;
        }
    }
}
