namespace IdentityServer4.Quickstart.UI
{
    public class UserRoleInputModel
    {
        public string RoleId { get; set; }
        public string UserId { get; set; }

        public UserRoleInputModel()
        {

        }

        public UserRoleInputModel(string userId)
        {
            UserId = userId;
        }
    }
}
