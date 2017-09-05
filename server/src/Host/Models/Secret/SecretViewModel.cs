using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer4.Quickstart.UI
{
    public class SecretViewModel : SecretInputModel
    {
        public SecretViewModel()
        {
        }

        public SecretViewModel(string id, string value)
        {
            Id = id;
            Value = value;
        }
    }
}
