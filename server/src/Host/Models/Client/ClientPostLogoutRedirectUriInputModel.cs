using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class ClientPostLogoutRedirectUriInputModel
    {
        public string ClientId { get; set; }
        public string PostLogoutRedirectUriId { get; set; }
        [Required]
        [DataType(DataType.Url)]
        public string Name { get; set; }

        public ClientPostLogoutRedirectUriInputModel()
        {
            PostLogoutRedirectUriId = Guid.NewGuid().ToString();
        }

        public ClientPostLogoutRedirectUriInputModel(string clientId)
        {
            PostLogoutRedirectUriId = Guid.NewGuid().ToString();
            ClientId = clientId;
        }

        public ClientPostLogoutRedirectUriInputModel(string clientId, string postLogoutRedirectUriId)
        {
            ClientId = clientId;
            PostLogoutRedirectUriId = postLogoutRedirectUriId;
        }
    }
}
