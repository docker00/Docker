using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.Quickstart.UI
{
    public class SecretInputModel
    {
        public string Id { get; set; }
        [Required]
        public string Value { get; set; }
        [Required]
        public string Type { get; set; } = "SharedSecret";

        public SecretInputModel()
        {
            Id = Guid.NewGuid().ToString();
        }

        public SecretInputModel(string value)
        {
            Value = value;
        }
    }
}
