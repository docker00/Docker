using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class PostLogoutRedirectUriInputModel
    {
        public string Id { get; set; }
        [Required]
        public string Name { get; set; }

        public PostLogoutRedirectUriInputModel()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
