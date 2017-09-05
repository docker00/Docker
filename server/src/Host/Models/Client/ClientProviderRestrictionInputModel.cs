using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class ClientProviderRestrictionInputModel
    {
        public string ClientId { get; set; }
        [Required]
        public string IdentityProviderRestrictionId { get; set; }

        public ClientProviderRestrictionInputModel()
        {

        }

        public ClientProviderRestrictionInputModel(string clientId)
        {
            ClientId = clientId;
        }

        public ClientProviderRestrictionInputModel(string clientId, string identityProviderRestrictionId)
        {
            ClientId = clientId;
            IdentityProviderRestrictionId = identityProviderRestrictionId;
        }
    }
}
