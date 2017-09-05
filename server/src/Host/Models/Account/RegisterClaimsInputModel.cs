using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class RegisterClaimsInputModel
    {
        public string AttributeName { get; set; }
        public string ClaimId { get; set; }

        public string ClaimValue { get; set; }
        public bool ShowError {get; set;}

        public RegisterClaimsInputModel()
        {
            ShowError = false;
        }
    }
}
