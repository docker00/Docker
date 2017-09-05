using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class ClientAllowedCorsOriginInputModel
    {
        public string ClientId { get; set; }
        [Required]
        public string AllowedCorsOriginId { get; set; }

        [Required]
        public string Name { get; set; }

        public ClientAllowedCorsOriginInputModel()
        {
            AllowedCorsOriginId = Guid.NewGuid().ToString();
        }

        public ClientAllowedCorsOriginInputModel(string clientId)
        {
            AllowedCorsOriginId = Guid.NewGuid().ToString();
            ClientId = clientId;
        }

        public ClientAllowedCorsOriginInputModel(string clientId, string allowedCorsOriginId)
        {
            ClientId = clientId;
            AllowedCorsOriginId = allowedCorsOriginId;
        }
    }
}
