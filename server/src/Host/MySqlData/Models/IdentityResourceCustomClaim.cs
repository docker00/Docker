using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace MySql.AspNet.Identity
{
    public class IdentityResourceCustomClaim
    {
        public string Id { get; set; }
        public string IdentityResourceId { get; set; }
        public string ClaimId { get; set; }

        public IdentityResourceCustomClaim()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
