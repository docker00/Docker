using System;

namespace MySql.AspNet.Identity
{
    public class GroupUser
    {
        public string Id { get; set; }
        public string GroupId { get; set; }
        public string UserId { get; set; }

        public GroupUser()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
