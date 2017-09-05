using System;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer4.Quickstart.UI
{
    public class UserApiKeyInputModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string ClientId { get; set; }

        [Required(ErrorMessage = "{0} должна быть выбрана")]
        [Display(Name = "Дата действия")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
        public DateTime ExperienceTime { get; set; }

        public UserApiKeyInputModel()
        {
        }

        public UserApiKeyInputModel(string userId)
        {
            UserId = userId;
        }

        public UserApiKeyInputModel(string userId, string clientId)
            : this(userId)
        {
            ClientId = clientId;
        }
    }
}
