using System;

namespace MySql.AspNet.Identity
{
    public class UserApiKey
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ClientId { get; set; }
        public string ApiKey { get; set; }
        public DateTime ExperienceTime { get; set; }

        public UserApiKey()
        {
            Id = Guid.NewGuid().ToString();
            ApiKey = Guid.NewGuid().ToString();
        }

        public UserApiKey(string userId, string clientId, DateTime experienceTime)
            : this()
        {
            UserId = userId;
            ClientId = clientId;
            ExperienceTime = experienceTime;
        }

        public UserApiKey(string id, string userId, string clientId, DateTime experienceTime)
        {
            Id = id;
            UserId = userId;
            ClientId = clientId;
            ExperienceTime = experienceTime;
            ApiKey = Guid.NewGuid().ToString();
        }
    }
}
