// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using MySql.AspNet.Identity;
using System.Linq;
using System.Threading.Tasks;

namespace Host.MySqlData.InDataBase
{
    public class CustomUserProfileService : IProfileService
    {
        protected readonly ILogger Logger;

        private readonly UserStore<User> _userStore;

        public CustomUserProfileService(string connectionString, ILogger<CustomUserProfileService> logger)
        {
            _userStore = new UserStore<User>(connectionString);
            Logger = logger;
        }

        public virtual Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            Logger.LogDebug("Get profile called for subject {subject} from client {client} with claim types {claimTypes} via {caller}",
                context.Subject.GetSubjectId(),
                context.Client.ClientName ?? context.Client.ClientId,
                context.RequestedClaimTypes,
                context.Caller);

            if (context.RequestedClaimTypes.Any())
            {
                var user = _userStore.FindById(context.Subject.GetSubjectId());
                if (user != null)
                {
                    context.AddFilteredClaims(user.Claims);
                }
            }

            return Task.FromResult(0);
        }

        public virtual Task IsActiveAsync(IsActiveContext context)
        {
            var user = _userStore.FindById(context.Subject.GetSubjectId());
            context.IsActive = user != null;

            return Task.FromResult(0);
        }
    }
}