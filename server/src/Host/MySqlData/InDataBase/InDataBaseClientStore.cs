using IdentityServer4.Models;
using IdentityServer4.Stores;
using MySql.AspNet.Identity.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Host.MySqlData.InDataBase
{
    public class InDataBaseClientStore : IClientStore
    {
        private readonly ClientCustomRepository ClientCustom;

        public InDataBaseClientStore(string connectionString)
        {
            ClientCustom = new ClientCustomRepository(connectionString);
        }

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            Client client = ClientCustom.GetClientCustomByClientId(clientId);

            return Task.FromResult(client);
        }
    }
}
