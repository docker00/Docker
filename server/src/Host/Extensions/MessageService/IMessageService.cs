using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Host.Extensions.MessageService
{
    interface IMessageService
    {
        Task Send(string subject, string messageText, string email);
    }
}
