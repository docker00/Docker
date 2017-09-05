using System;

namespace MySql.AspNet.Identity
{
    public class ProfileAttribute
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool RequiredRegister { get; set; }
        public bool RequiredAdditional { get; set; }
        public bool Disabled { get; set; }
        public int Weight { get; set; }
        public bool Deleted { get; set; }

        public ProfileAttribute()
        {
            Id = Guid.NewGuid().ToString();
        }

        public ProfileAttribute(string name)
            : this()
        {
            Name = name;
        }

        public ProfileAttribute(string name, string id)
        {
            Name = name;
            Id = id;
        }
    }
}
