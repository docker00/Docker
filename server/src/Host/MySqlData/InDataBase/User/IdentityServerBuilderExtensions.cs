// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Microsoft.Extensions.DependencyInjection;
using MySql.AspNet.Identity;

namespace Host.MySqlData.InDataBase
{
    /// <summary>
    /// Extension methods for the IdentityServer builder
    /// </summary>
    public static class IdentityServerBuilderExtensions
    {
        /// <summary>
        /// Adds test users.
        /// </summary>
        /// <param name="builder">The builder.</param>
        /// <param name="users">The users.</param>
        /// <returns></returns>
        public static IIdentityServerBuilder AddUsers(this IIdentityServerBuilder builder, string connectionString)
        {
            builder.Services.AddSingleton(new UserStore<User>(connectionString));
            builder.AddProfileService<CustomUserProfileService>();
            builder.AddResourceOwnerValidator<CustomUserResourceOwnerPasswordValidator>();

            return builder;
        }
    }
}