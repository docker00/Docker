using System;
using IdentityServer4.Models;

namespace MySql.AspNet.Identity
{
    public class ClientCustom : Client
    {
        public string Id { get; set; }

        public ClientCustom()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}