using System;

namespace MySql.AspNet.Identity
{
    public class UserDevice
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool Confirmed { get; set; }

        public UserDevice()
        {
            Id = Guid.NewGuid().ToString();
        }

        public UserDevice(string userId, string name, string type)
            : this()
        {
            UserId = userId;
            Name = name;
            Type = type;
        }
    }
}