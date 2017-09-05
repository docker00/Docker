// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using IdentityServer4.Stores;
using MySql.AspNet.Identity.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Host.MySqlData.InDataBase
{
    public class InDataBaseResourcesStore : IResourceStore
    {
        private readonly IdentityResourceCustomRepository _identityResourceCustom;
        public InDataBaseResourcesStore(string connectionString)
        {
            _identityResourceCustom = new IdentityResourceCustomRepository(connectionString);
        }

        public Task<Resources> GetAllResources()
        {
            IEnumerable<IdentityResource> _identityResources = _identityResourceCustom.GetIdentityResources();
            //TODO: сlient to client
            IEnumerable<ApiResource> _apiResources = new List<ApiResource>() { new ApiResource("access_token_api") };

            var result = new Resources(_identityResources, _apiResources);
            return Task.FromResult(result);
        }

        public Task<ApiResource> FindApiResourceAsync(string name)
        {
            //TODO: сlient to client
            ApiResource api = new ApiResource("access_token_api");

            return Task.FromResult(api);
        }

        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> names)
        {
            if (names == null) throw new ArgumentNullException(nameof(names));

            //TODO: переделать
            IEnumerable<IdentityResource> identity = from i in _identityResourceCustom.GetIdentityResources()
                           where names.Contains(i.Name)
                           select i;

            return Task.FromResult(identity);
        }

        public Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> names)
        {
            if (names == null) throw new ArgumentNullException(nameof(names));

            //TODO: сlient to client
            IEnumerable<ApiResource> api = new List<ApiResource>() { new ApiResource("access_token_api") };

            return Task.FromResult(api);
        }
    }
}
