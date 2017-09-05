using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class ClientAllowedGrantTypeInputModel
    {
        public string ClientId { get; set; }
        [Required]
        public string AllowedGrantTypeId { get; set; }

        public ClientAllowedGrantTypeInputModel()
        {
        }

        public ClientAllowedGrantTypeInputModel(string clientId)
        {
            ClientId = clientId;
        }

        public ClientAllowedGrantTypeInputModel(string clientId, string allowedGrantTypeId)
        {
            ClientId = clientId;
            AllowedGrantTypeId = allowedGrantTypeId;
        }
    }
}
