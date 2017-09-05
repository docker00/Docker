// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using MySql.AspNet.Identity.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Host.MySqlData.InDataBase
{
    public class InDataBaseCorsPolicyService : ICorsPolicyService
    {
        private readonly ILogger<InMemoryCorsPolicyService> _logger;
        private readonly ClientCustomRepository ClientCustom;

        public InDataBaseCorsPolicyService(ILogger<InMemoryCorsPolicyService> logger, string connectionString)
        {
            _logger = logger;
            ClientCustom = new ClientCustomRepository(connectionString);
        }

        public Task<bool> IsOriginAllowedAsync(string origin)
        {
            //TODO: переделать
            var query =
                from client in ClientCustom.GetClientCustoms()
                from url in client.AllowedCorsOrigins
                select url.GetOrigin();

            var result = query.Contains(origin, StringComparer.OrdinalIgnoreCase);

            if (result)
            {
                _logger.LogDebug("Client list checked and origin: {0} is allowed", origin);
            }
            else
            {
                _logger.LogDebug("Client list checked and origin: {0} is not allowed", origin);
            }
            
            return Task.FromResult(result);
        }
    }
}
