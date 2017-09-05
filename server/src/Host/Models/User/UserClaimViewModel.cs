using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class UserClaimViewModel: UserClaimInputModel
    {
        [Display(Name = "Название атрибута")]
        public string ProfileAttributeName { get; set; }

        public UserClaimViewModel()
        {

        }

        public UserClaimViewModel(string userId, string claimId)
        {
            UserId = userId;
            ClaimId = claimId;
        }
    }
}
