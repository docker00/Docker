using System;

namespace MySql.AspNet.Identity
{
    public class GroupParent
    {
        public string Id { get; set; }
        public string GroupId { get; set; }
        public string ParentGroupId { get; set; }

        public GroupParent()
        {
            Id = Guid.NewGuid().ToString();
        }

        public GroupParent(string groupId, string parentGroupId)
            : this()
        {
            GroupId = groupId;
            ParentGroupId = parentGroupId;
        }

        public GroupParent(string groupId, string parentGroupId, string id)
        {
            GroupId = groupId;
            ParentGroupId = parentGroupId;
            Id = id;
        }
    }
}
