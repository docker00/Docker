using System;

namespace MySql.AspNet.Identity
{
    public class Group
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public Group()
        {
            Id = Guid.NewGuid().ToString();
        }

        public Group(string name)
            : this()
        {
            Name = name;
        }

        public Group(string name, string id)
        {
            Name = name;
            Id = id;
        }
    }
}
