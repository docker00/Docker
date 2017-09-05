
using IdentityServer4.Models;
using System;
using System.Collections.Generic;

namespace MySql.AspNet.Identity
{
    public class SecretCustom : Secret
    {
        public string Id { get; set; }

        public SecretCustom()
            : base()
        {
            Id = Guid.NewGuid().ToString();
        }

        public SecretCustom(string value, DateTime? expiration = null)
            : base(value, expiration)
        {
            Id = Guid.NewGuid().ToString();
        }

        public SecretCustom(string value, string description, DateTime? expiration = null)
            : base(value, description, expiration)
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
