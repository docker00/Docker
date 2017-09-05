using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class ClientRedirectUriInputModel
    {
        public string ClientId { get; set; }
        public string RedirectUriId { get; set; }
        [Required]
        [DataType(DataType.Url)]
        public string Name { get; set; }

        public ClientRedirectUriInputModel()
        {
            RedirectUriId = Guid.NewGuid().ToString();
        }

        public ClientRedirectUriInputModel(string clientId)
        {
            RedirectUriId = Guid.NewGuid().ToString();
            ClientId = clientId;
        }

        public ClientRedirectUriInputModel(string clientId, string redirectUriId)
        {
            ClientId = clientId;
            RedirectUriId = redirectUriId;
        }
    }
}
