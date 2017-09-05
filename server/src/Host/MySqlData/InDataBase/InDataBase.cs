using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;

namespace Host.MySqlData.InDataBase
{
    public static class IdentityServerBuilderExtensionsInDataBase
    {
        public static IIdentityServerBuilder AddInDataBaseIdentityResources(this IIdentityServerBuilder builder, string connectionString)
        {
            builder.Services.AddSingleton(connectionString);
            builder.AddResourceStore<InDataBaseResourcesStore>();

            return builder;
        }

        public static IIdentityServerBuilder AddInDataBaseApiResources(this IIdentityServerBuilder builder, string connectionString)
        {
            builder.Services.AddSingleton(connectionString);
            builder.AddResourceStore<InDataBaseResourcesStore>();

            return builder;
        }

        public static IIdentityServerBuilder AddInDataBaseClients(this IIdentityServerBuilder builder, string connectionString)
        {
            builder.Services.AddSingleton(connectionString);

            builder.AddClientStore<InDataBaseClientStore>();
            builder.AddCorsPolicyService<InDataBaseCorsPolicyService>();

            return builder;
        }
    }
}
