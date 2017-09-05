using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class IdentityResourceClaimInputModel
    {
        public string IdentityResourceId { get; set; }
        [Required(ErrorMessage = "{0} должно быть выбрано")]
        [Display(Name = "Требование")]
        public string ClaimId { get; set; }

        public IdentityResourceClaimInputModel()
        {

        }

        public IdentityResourceClaimInputModel(string identityResourceId)
        {
            IdentityResourceId = identityResourceId;
        }

        public IdentityResourceClaimInputModel(string identityResourceId, string claimId)
        {
            IdentityResourceId = identityResourceId;
            ClaimId = claimId;
        }
    }
}
