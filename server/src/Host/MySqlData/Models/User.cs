using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace MySql.AspNet.Identity
{
    public class User
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public int AccessFailedCount { get; set; }
        public bool LockoutEnabled { get; set; }
        public DateTime? LockoutEndDate { get; set; }
        public bool TwoFactorAuthEnabled { get; set; }
        public bool Activated { get; set; }
        public bool AttributesValidated { get; set; }
        public List<string> Roles { get; set; }
        public List<Claim> Claims { get; set; }
        public List<IdentityUserLogin> Logins { get; set; }

        public User()
        {
            this.Claims = new List<Claim>();
            this.Roles = new List<string>();
            this.Logins = new List<IdentityUserLogin>();
            this.Id = Guid.NewGuid().ToString();
            LockoutEnabled = false;
            Activated = false;
            AttributesValidated = false;
        }

        public User(string userName)
            : this()
        {
            this.UserName = userName;
        }
    }

    public sealed class IdentityUserLogin
    {
        //public string Id { get; set; }
        //public string UserId { get; set; }
        //public string Provider { get; set; }
        //public string ProviderKey { get; set; }
        public IdentityUserLogin()
        {
        }

        public IdentityUserLogin(string loginProvider, string providerKey)
        {
            LoginProvider = loginProvider;
            ProviderKey = providerKey;
        }

        public IdentityUserLogin(string loginProvider, string providerKey, string providerDisplayName)
        {
            LoginProvider = loginProvider;
            ProviderKey = providerKey;
            ProviderDisplayName = providerDisplayName;
        }

        public IdentityUserLogin(string userId, string loginProvider, string providerKey, string providerDisplayName)
        {
            UserId = userId;
            LoginProvider = loginProvider;
            ProviderKey = providerKey;
            ProviderDisplayName = providerDisplayName;
        }

        public string UserId { get; set; }
        public string LoginProvider { get; set; }
        public string ProviderKey { get; set; }
        public string ProviderDisplayName { get; set; }

    }

    public class IdentityUserClaim
    {
        public virtual string ClaimType { get; set; }
        public virtual string ClaimValue { get; set; }
    }
}
