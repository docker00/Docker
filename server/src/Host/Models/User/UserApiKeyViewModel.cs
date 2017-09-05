using System;

namespace IdentityServer4.Quickstart.UI
{
    public class UserApiKeyViewModel
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ClientId { get; set; }
        public string ApiKey { get; set; }
        public string ExperienceTime { get; set; }

        public UserApiKeyViewModel()
        {
        }

        public UserApiKeyViewModel(string id, string userId, string clientId, string apiKey, DateTime experienceTime)
        {
            Id = id;
            UserId = userId;
            ClientId = clientId;
            ApiKey = apiKey;
            ExperienceTime = experienceTime.ToString("dd.MM.yyyy HH:mm");
        }
    }
}
