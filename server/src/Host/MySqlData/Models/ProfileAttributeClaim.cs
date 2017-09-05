using System;

namespace MySql.AspNet.Identity
{
    public class ProfileAttributeClaim
    {
        public string Id { get; set; }
        public string ProfileAttributeId { get; set; }
        public string ProfileAttributeName { get; set; }
        public string ClaimId { get; set; }
        public string ClaimValue { get; set; }

        public bool RequiredRegister { get; set; }
        public bool RequiredAdditional { get; set; }
        public bool Disabled { get; set; }
        public int Weight { get; set; }
        public bool Deleted { get; set; }

        public ProfileAttributeClaim()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
