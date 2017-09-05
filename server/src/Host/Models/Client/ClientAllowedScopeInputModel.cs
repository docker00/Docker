using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class ClientAllowedScopeInputModel
    {

        public string ClientId { get; set; }
        [Required]
        public string ScopeName { get; set; }

        public ClientAllowedScopeInputModel()
        {

        }

        public ClientAllowedScopeInputModel(string clientId)
        {
            ClientId = clientId;
        }

        public ClientAllowedScopeInputModel(string clientId, string scopeName)
        {
            ClientId = clientId;
            ScopeName = scopeName;
        }
    }
}
