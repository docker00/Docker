using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class GroupUserInputModel
    {
        [Required]
        public string GroupId { get; set; }
        [Required]
        public string UserId { get; set; }

        public GroupUserInputModel()
        {

        }

        public GroupUserInputModel(string userId)
        {
            UserId = userId;
        }

        public GroupUserInputModel(string userId, string groupId)
        {
            UserId = userId;
            GroupId = groupId;
        }
    }
}
