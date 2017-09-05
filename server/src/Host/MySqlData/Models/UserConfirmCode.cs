using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MySql.AspNet.Identity
{
    public class UserConfirmCode
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Code { get; set; }
        public DateTime TimeEnd { get; set; }

        public UserConfirmCode()
        {
            Id = Guid.NewGuid().ToString();
            TimeEnd = DateTime.Now.AddMinutes(11);
            Code = new Random().Next(100000, 999999).ToString();
        }

        public UserConfirmCode(string userId)
            : this()
        {
            UserId = userId;
        }
    }
}
