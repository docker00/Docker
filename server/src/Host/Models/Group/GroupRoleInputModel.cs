namespace IdentityServer4.Quickstart.UI
{
    public class GroupRoleInputModel
    {
        public string RoleId { get; set; }
        public string GroupId { get; set; }

        public GroupRoleInputModel()
        {

        }

        public GroupRoleInputModel(string groupId)
        {
            GroupId = groupId;
        }
    }
}
