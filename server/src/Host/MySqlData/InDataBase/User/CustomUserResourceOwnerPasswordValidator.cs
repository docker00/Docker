// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityModel;
using IdentityServer4.Validation;
using MySql.AspNet.Identity;
using MySql.AspNet.Identity.Repositories;
using System.Threading;
using System.Threading.Tasks;

namespace Host.MySqlData.InDataBase
{
    /// <summary>
    /// Resource owner password validator for test users
    /// </summary>
    /// <seealso cref="IdentityServer4.Validation.IResourceOwnerPasswordValidator" />
    public class CustomUserResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly UserStore<User> _userStore;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestUserResourceOwnerPasswordValidator"/> class.
        /// </summary>
        /// <param name="users">The users.</param>
        public CustomUserResourceOwnerPasswordValidator(string connectionString)
        {
            _userStore = new UserStore<User>(connectionString);
        }

        /// <summary>
        /// Validates the resource owner password credential
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            if (_userStore.ValidateCredentials(context.UserName, context.Password))
            {
                User user = _userStore.FindByEmail(context.UserName);
                context.Result = new GrantValidationResult(user.Id, OidcConstants.AuthenticationMethods.Password, user.Claims);
            }

            return Task.FromResult(0);
        }
    }
}