using System;
using System.IO;
using System.Security.Claims;

namespace MySql.AspNet.Identity
{
    public class ClaimCustom : Claim
    {
        public string Id { get; set; }
        
        public ClaimCustom(BinaryReader reader)
            : base(reader)
        {
            Id = Guid.NewGuid().ToString();
        }
        public ClaimCustom(BinaryReader reader, ClaimsIdentity subject) 
            : base(reader, subject)
        {
            Id = Guid.NewGuid().ToString();
        }

        public ClaimCustom(string type, string value)
            : base(type, value)
        {
            Id = Guid.NewGuid().ToString();
        }
        public ClaimCustom(string type, string value, string valueType) 
            : base(type, value, valueType)
        {
            Id = Guid.NewGuid().ToString();
        }
        public ClaimCustom(string type, string value, string valueType, string issuer) 
            : base(type, value, valueType, issuer)
        {
            Id = Guid.NewGuid().ToString();
        }
        public ClaimCustom(string type, string value, string valueType, string issuer, string originalIssuer) 
            : base(type, value, valueType, issuer, originalIssuer)
        {
            Id = Guid.NewGuid().ToString();
        }
        public ClaimCustom(string type, string value, string valueType, string issuer, string originalIssuer, ClaimsIdentity subject) 
            : base(type, value, valueType, issuer, originalIssuer, subject)
        {
            Id = Guid.NewGuid().ToString();
        }
        protected ClaimCustom(Claim other)
            : base(other)
        {
            Id = Guid.NewGuid().ToString();
        }
        protected ClaimCustom(Claim other, ClaimsIdentity subject)
            : base(other, subject)
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}
