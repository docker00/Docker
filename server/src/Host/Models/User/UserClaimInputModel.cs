using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class UserClaimInputModel
    {
        [HiddenInput]
        public string UserId { get; set; }
        [HiddenInput]
        public string ClaimId { get; set; }
        [Display(Name = "Значение")]
        public string ClaimValue { get; set; }

        public UserClaimInputModel()
        {

        }

        public UserClaimInputModel(string userId, string claimId)
        {
            UserId = userId;
            ClaimId = claimId;
        }
    }
}
